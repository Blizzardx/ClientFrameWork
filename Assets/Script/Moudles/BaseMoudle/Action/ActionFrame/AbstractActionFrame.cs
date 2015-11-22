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
    protected GameObject m_ObjNpcRoot;
    protected List<GameObject> m_lstTargetObjects;
    protected ActionPlayer m_ActionPlayer;
    #endregion

    public AbstractActionFrame(ActionPlayer action, ActionFrameData data)
    {
        m_ActionPlayer = action;
        m_FrameData = data;
        m_ObjNpcRoot = m_ActionPlayer.GetNpcRoot();
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
            else if (index > 10000000)
            {
                Debuger.LogWarning("Affected GameObject Not Found, ID: " + index.ToString());
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
    public void ExecuteBase() {
        UpdateGeneratedObjects();
        Execute();
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
                    Debuger.LogWarning("Generated GameObject Not Found, ID: " + index.ToString());
                }
            }
        }

    }
    #endregion

}

