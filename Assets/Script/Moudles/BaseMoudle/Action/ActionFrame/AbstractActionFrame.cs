//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCameraFrame
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/2 19:17:12
//
// Purpose : 
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionEditor;
abstract public class AbstractActionFrame
{
    public enum EActionFrameState
    {
        None,
        Excute,
        Finish,
        Max,
    }

    #region Field
    protected EActionFrameState m_FrameState = EActionFrameState.None;
    protected ActionFrameData m_FrameData;
    //protected GameObject m_ObjNpcRoot;
    protected List<GameObject> m_lstTargetObjects;
    protected ActionPlayer m_ActionPlayer;
    #endregion

    public AbstractActionFrame(ActionPlayer action, ActionFrameData data)
    {
        m_ActionPlayer = action;
        m_FrameData = data;
        m_lstTargetObjects = new List<GameObject>();

        if (data.TargetIDs == null)
        {
            Debuger.LogWarning("No Target ID in ActionFrameData at time: " + data.Time.ToString());
            return;
        }

        Dictionary<int, GameObject> allObjects = m_ActionPlayer.GetTargetObjects();
        foreach (int index in data.TargetIDs)
        {
            if (allObjects.ContainsKey(index))
            {
                m_lstTargetObjects.Add(allObjects[index]);
            }
            else if (index > 10000010)
            {
                Debuger.LogWarning(("<color=orange>" + (EActionFrameType)data.Type).ToString() + "</color> Affected GameObject Not Found, ID: " + index.ToString());
            }
            else if (index > 10000000 && index <= 10000010)
            {
                if (!allObjects.ContainsKey(10000001) && !allObjects.ContainsKey(10000002))
                {
                    PlayerCharacter player = PlayerManager.Instance.GetPlayerInstance();
                    if (player != null)
                    {
                        CharTransformData charData = (CharTransformData)(player.GetTransformData());
                        GameObject charObject = charData.GetGameObject();
                        m_lstTargetObjects.Add(charObject);
                    }
                }
            }
            else
            {
                Debuger.Log("Need Generated Object, ID: : " + index.ToString());
            }
        }
    }

    #region Get & Set
    public List<GameObject> TargetObjects { get { return m_lstTargetObjects; } }
    public ActionFrameData FrameData
    {
        get { return m_FrameData; }
    }
    public EActionFrameState GetFrameState()
    {
        return m_FrameState;
    }
    public void SetFrameState(EActionFrameState eState)
    {
        m_FrameState = eState;
    }
    #endregion

    #region Public Interface
    public void ExecuteBase()
    {
        UpdateGeneratedObjects();
        DisableAI(); 
        StopMove();
        Execute();
    }
    public void DestoryBase()
    {
        EnableAI();
        Destory();
    }
    abstract public bool IsTrigger(float fRealTime);
    abstract public bool IsFinish(float fRealTime);
    abstract public void Play();
    abstract public void Pause(float fTime);
    abstract public void Stop();
    abstract public void Destory();
    virtual public void Update(float fRealTime) { }
    #endregion

    #region System Functions
    virtual protected void Execute() { }
    protected void UpdateGeneratedObjects()
    {
        if (m_FrameData.TargetIDs == null)
        {
            return;
        }
        Dictionary<int, GameObject> allObjects = m_ActionPlayer.GetTargetObjects();
        foreach (int index in m_FrameData.TargetIDs)
        {
            if (index <= 10000000)
            {
                if (allObjects.ContainsKey(index))
                {
                    m_lstTargetObjects.Add(allObjects[index]);
                }
                else
                {
                    Debuger.LogWarning(("<color=orange>" + (EActionFrameType)m_FrameData.Type).ToString() + "</color> Generated GameObject Not Found, ID: " + index.ToString());
                }
            }
        }
    }
    private void StopMove ()
    {
        // stop npcs
        if (m_lstTargetObjects == null || m_lstTargetObjects.Count <= 0)
            return;
        foreach (GameObject obj in m_lstTargetObjects)
        {
            CharTransformContainer container = obj.GetComponent<CharTransformContainer>();
            if (container != null)
            {
                if (container.GetData() is Npc)
                {
                    Npc npc = (Npc)container.GetData();
                    npc.StopMove();
                }
            }
        }
    }
    private void DisableAI()
    {
        if (m_lstTargetObjects == null || m_lstTargetObjects.Count <= 0)
        {
            return;
        }
        foreach (GameObject charObject in m_lstTargetObjects)
        {
            CharTransformContainer container = charObject.GetComponent<CharTransformContainer>();
            if (container == null)
            {
                Debuger.LogError("No Container in " + charObject.ToString());
                return;
            }
            Npc npc = null;
            if (container.GetData() is Npc)
            {
                npc = (Npc)container.GetData();
            }
            if (null != npc)
            {
                npc.SetAIStatus(false);
            }

        }
    }
    private void EnableAI()
    {
        if (m_lstTargetObjects == null || m_lstTargetObjects.Count <= 0)
        {
            return;
        }
        foreach (GameObject charObject in m_lstTargetObjects)
        {
            CharTransformContainer container = charObject.GetComponent<CharTransformContainer>();
            if (container == null)
            {
                Debuger.LogError("No Container in " + charObject.ToString());
                return;
            }
            Npc npc = null;
            if (container.GetData() is Npc)
            {
                npc = (Npc)container.GetData();
            }
            if (null != npc)
            {
                npc.SetAIStatus(true);
                npc.IsPlayerControlled = false;
                npc.ResetGroup();
            }
        }
    }
    #endregion

}

