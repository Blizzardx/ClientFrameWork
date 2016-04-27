//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : MissionManager
//
// Created by : Baoxue at 2015/11/19 17:48:02
//
//
//========================================================================

using Config;
using NetWork.Auto;
using UnityEngine;
using System;
using System.Collections.Generic;

public class MissionManager : Singleton<MissionManager>
{
    private List<MissionElement>    m_MissionList;
    private List<MissionElement>    m_AddedMissionList; 
    private List<int>               m_RemovingStore;
    private bool                    m_bIsBusy;

    private Dictionary<int, int>    m_MissionCounterMap;
    private Dictionary<int, int>    m_MissionStepCounterMap;
    private List<MissionInfo>       m_MissionInfoList;

    #region public interface
    public void InitMissionMgr(List<MissionInfo> missionInfoList)
    {
        m_MissionInfoList = missionInfoList;
        m_RemovingStore = new List<int>();
        m_MissionList = new List<MissionElement>();
        m_AddedMissionList = new List<MissionElement>();
        m_MissionCounterMap = new Dictionary<int, int>();
        m_MissionStepCounterMap = new Dictionary<int, int>();
        for (int i = 0; i < missionInfoList.Count; ++i)
        {
            MissionInfo info = missionInfoList[i];
            //test code
            if(m_MissionCounterMap.ContainsKey(info.MissionId))
            {
                continue;
            }
            m_MissionCounterMap.Add(info.MissionId,info.Counter);
            foreach (var elem in info.MissionStepInfoList)
            {
                if(m_MissionStepCounterMap.ContainsKey(elem.StepId))
                {
                    continue;
                }
                m_MissionStepCounterMap.Add(elem.StepId, elem.Counter);
            }
        }

        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ACCEPT_MISSION, TriggerAcceptMission);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_GAMELOGIC_SCENE_TRIGGER, TriggerSceneState);
    }
    public void Destuctor()
    {
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACCEPT_MISSION, TriggerAcceptMission);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_GAMELOGIC_SCENE_TRIGGER, TriggerSceneState);
    }
    public void TryAcceptMission(int id)
    {
        for (int i = 0; i < m_MissionList.Count; ++i)
        {
            if (m_MissionList[i].GetCurrentMissionId() == id)
            {
                // already has mission
                return;
            }
        }

        //check is mission exist
        for(int i=0;i<m_MissionInfoList.Count;++i)
        {
            if(m_MissionInfoList[i].MissionId == id)
            {
                return;
            }
        }

        MissionElement elem = new MissionElement();
        elem.InitMissionStep(id, OnMissionComplete);

        OnAcceptNewMission(elem);

        if (!m_bIsBusy)
        {
            m_MissionList.Add(elem);
        }
        else
        {
            m_AddedMissionList.Add(elem);
        }
    }
    public int GetMissionCounter(int id)
    {
        if (m_MissionCounterMap.ContainsKey(id))
        {
            return m_MissionCounterMap[id];
        }
        m_MissionCounterMap.Add(id, 0);
        return 0;
    }
    public void SetMissionCounter(int id,int value)
    {
        if (m_MissionCounterMap.ContainsKey(id))
        {
            m_MissionCounterMap[id] = value;
            for (int i = 0; i < m_MissionInfoList.Count; ++i)
            {
                if (m_MissionInfoList[i].MissionId == id)
                {
                    m_MissionInfoList[i].Counter = value;
                    break;
                }
            }
            RewriteMissionCounter();
        }
        else
        {
            Debuger.LogError("can't find target mission id : " + id);
        }
    }
    public int GetMissionStepCounter(int id)
    {
        if (m_MissionStepCounterMap.ContainsKey(id))
        {
            return m_MissionStepCounterMap[id];
        }
        m_MissionStepCounterMap.Add(id, 0);
        return 0;
    }
    public void SetMissionStepCounter(int id, int value)
    {
        if (m_MissionStepCounterMap.ContainsKey(id))
        {
            m_MissionStepCounterMap[id] = value;
            bool isBreak = false;
            for (int i = 0; i < m_MissionInfoList.Count ; ++i)
            {
                foreach (var elem in m_MissionInfoList[i].MissionStepInfoList)
                {
                    if (elem.StepId == id)
                    {
                        elem.Counter = value;
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }
            RewriteMissionCounter();
        }
        else
        {
            m_MissionStepCounterMap.Add(id, value);
        }
    }
    #endregion

    #region system function
    private void OnMissionComplete(MissionElement elem)
    {
        MainMissionConfig config = ConfigManager.Instance.GetMainMissionConfig(elem.GetCurrentMissionId());
        if (config != null && config.NextMissionId != 0)
        {
            TryAcceptMission(config.NextMissionId);
        }

        for (int i = 0; i < m_RemovingStore.Count; ++i)
        {
            if (m_RemovingStore[i] == elem.GetCurrentMissionId())
            {
                return;
            }
        }
        m_RemovingStore.Add(elem.GetCurrentMissionId());

        for (int i = 0; i < m_MissionInfoList.Count; ++i)
        {
            if (m_MissionInfoList[i].MissionId == elem.GetCurrentMissionId())
            {
                m_MissionInfoList.RemoveAt(i);
                break;
            }
        }
        RewriteMissionCounter();
        elem = null;
    }
    private void TriggerAcceptMission(MessageObject obj)
    {
        int missinId = (int)(obj.msgValue);

        //try accept mission
        TryAcceptMission(missinId);
    }
    //scene listener
    private void TriggerSceneState(MessageObject obj)
    {
        if (!(obj.msgValue is GameLogicSceneType))
        {
            return;
        }
        m_bIsBusy = true;
        var sceneType = (GameLogicSceneType)(obj.msgValue);

        for (int i = 0; i < m_MissionList.Count; ++i)
        {
            m_MissionList[i].TriggerSceneState(sceneType);
        }

        for (int i = 0; i < m_RemovingStore.Count; ++i)
        {
            for (int j = 0; j < m_MissionList.Count; ++j)
            {
                if (m_MissionList[j].GetCurrentMissionId() == m_RemovingStore[i])
                {
                    m_MissionList.RemoveAt(j);
                    break;
                }
            }
        }
        m_RemovingStore.Clear();

        for (int i = 0; i < m_AddedMissionList.Count; ++i)
        {
            m_MissionList.Add(m_AddedMissionList[i]);
        }
        m_AddedMissionList.Clear();
        m_bIsBusy = false;
    }
    private void OnAcceptNewMission(MissionElement mission)
    {
        MissionInfo info = new MissionInfo();
        info.MissionId = mission.GetCurrentMissionId();
        info.Counter = 0;
        info.MissionStepInfoList = new List<MissionStepInfo>();

        //set search map
        if (!m_MissionCounterMap.ContainsKey(mission.GetCurrentMissionId()))
        {
            m_MissionCounterMap.Add(mission.GetCurrentMissionId(), 0);
        }
        else
        {
            m_MissionCounterMap[mission.GetCurrentMissionId()] = 0;
        }

        for (int i = 0; i < mission.GetMissionStepConfig().Count; ++i)
        {
            MissionStepInfo stepInfo = new MissionStepInfo();
            stepInfo.Counter = 0;
            stepInfo.StepId = mission.GetMissionStepConfig()[i].Id;

            //set search map
            if (!m_MissionStepCounterMap.ContainsKey(mission.GetMissionStepConfig()[i].Id))
            {
                m_MissionStepCounterMap.Add(mission.GetMissionStepConfig()[i].Id, 0);
            }
            else
            {
                m_MissionStepCounterMap[mission.GetMissionStepConfig()[i].Id] = 0;
            }
            info.MissionStepInfoList.Add(stepInfo);
        }

        //add element
        m_MissionInfoList.Add(info);

        RewriteMissionCounter();
    }
    private void RewriteMissionCounter()
    {
        //rewrite counter
        PlayerManager.Instance.GetMissionData().MissionList = m_MissionInfoList;

        MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_MISSION_COUNTER_CHANGE,null));
    }
    #endregion

    #region Mission Reporter
    public bool CheckMissionAvailable(int missionId)
    {
        for (int i = 0; i < m_MissionList.Count; ++i)
        {
            if (m_MissionList[i].GetCurrentMissionId() == missionId)
            {
                return true;
            }
        }
        return false;
    }
    public void GetMissionCounterReporter(ref List<KeyValuePair<int,int>> list)
    {
        if (null == list)
        {
            list = new List<KeyValuePair<int, int>>();
        }
        else
        {
            list.Clear();
        }

        foreach (var elem in m_MissionCounterMap)
        {
            list.Add(elem);
        }
    }
    public void GetMissionStepCounterReporter(ref List<KeyValuePair<int, int>> list)
    {
        if (null == list)
        {
            list = new List<KeyValuePair<int, int>>();
        }
        else
        {
            list.Clear();
        }

        foreach (var elem in m_MissionStepCounterMap)
        {
            list.Add(elem);
        }
    }
    #endregion
}