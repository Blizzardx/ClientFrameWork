//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : MissionElement
//
// Created by : Baoxue at 2015/12/9 12:17:23
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MissionElement
{
    private MainMissionConfig       m_CurrentMission;
    private List<MissionStepConfig> m_CurrentMissionStepList;
    private Action<MissionElement>  m_OnMissioinComplete;

    public void InitMissionStep(int missionId, Action<MissionElement> onMissioinComplete)
    {
        m_OnMissioinComplete = onMissioinComplete;
        List<MissionStepConfig> missionStepList = ConfigManager.Instance.GetMissionStepConfigByMissioinId(missionId);

        m_CurrentMission = ConfigManager.Instance.GetMainMissionConfig(missionId);
        if (null == m_CurrentMission)
        {
            Debuger.LogWarning("can't load mission " + missionId);
            return;
        }

        if (null == missionStepList || missionStepList.Count <= 0)
        {
            Debuger.LogWarning("can't load mission step ,mission id : " + missionId);
            return;
        }

        m_CurrentMissionStepList = new List<MissionStepConfig>(missionStepList.Count);
        for (int i = 0; i < missionStepList.Count; ++i)
        {
            MissionStepConfig stepConfig = missionStepList[i];
            if (null == stepConfig)
            {
                Debuger.LogWarning("Can't load missioni step config ,mission id " + missionId);
                continue;
            }
            m_CurrentMissionStepList.Add(stepConfig);
        }
    }
    public void InitMissionStep(int missionId, List<int> missionStepList, Action<MissionElement> onMissioinComplete)
    {
        m_OnMissioinComplete = onMissioinComplete;

        m_CurrentMission = ConfigManager.Instance.GetMainMissionConfig(missionId);
        if (null == m_CurrentMission)
        {
            Debuger.LogWarning("can't load mission " + missionId);
            return;
        }

        if (null == missionStepList || missionStepList.Count <= 0)
        {
            Debuger.LogWarning("can't load mission step ,mission id : " + missionId);
            return;
        }

        m_CurrentMissionStepList = new List<MissionStepConfig>(missionStepList.Count);
        for (int i = 0; i < missionStepList.Count; ++i)
        {
            int stepId = missionStepList[i];
            //try load mission step info
            MissionStepConfig stepConfig = ConfigManager.Instance.GetMissionStepConfigByStepId(stepId);
            if (null == stepConfig)
            {
                Debuger.LogWarning("Can't load missioni step config ,step id " + stepId);
                continue;
            }
            m_CurrentMissionStepList.Add(stepConfig);
        }
    }
    public int GetCurrentMissionId()
    {
        return m_CurrentMission.Id;
    }
    public List<MissionStepConfig> GetMissionStepConfig()
    {
        return m_CurrentMissionStepList;
    }
    public void TriggerSceneState(GameLogicSceneType sceneType)
    {
        for (int i = 0; i < m_CurrentMissionStepList.Count; ++i)
        {
            MissionStepConfig elemStep = m_CurrentMissionStepList[i];

            if (elemStep.SceneId == (int)(sceneType))
            {
                HandleTarget target = HandleTarget.GetHandleTarget(null);
                if (LimitMethods.HandleLimitExec(target, elemStep.SceneLimitId, null))
                {
                    FuncMethods.HandleFuncExec(target, elemStep.SceneFuncId, null);
                }
                HandleTarget.CollectionHandlerTargetInstance(target);
            }
            HandleTarget target1 = HandleTarget.GetHandleTarget(null);
            if (LimitMethods.HandleLimitExec(target1, elemStep.CompleteLimitId, null))
            {
                FuncMethods.HandleFuncExec(target1, elemStep.CompleteFuncId, null);
            }
            HandleTarget.CollectionHandlerTargetInstance(target1);
        }

        bool isMissionComplete = false;
        HandleTarget target2 = HandleTarget.GetHandleTarget(null);
        if (LimitMethods.HandleLimitExec(target2, m_CurrentMission.CompleteLimitId, null))
        {
            isMissionComplete = true;
            FuncMethods.HandleFuncExec(target2, m_CurrentMission.CompleteFuncId, null);
        }
        HandleTarget.CollectionHandlerTargetInstance(target2);

        if (isMissionComplete)
        {
            m_OnMissioinComplete(this);
        }
    }
}
