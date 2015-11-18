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
    private ActionPlayer m_ActionPlayer;
    protected List<GameObject> m_lstTargetObjects;
    #endregion

    public AbstractActionFrame(ActionPlayer action, ActionFrameData data)
    {
        m_ActionPlayer = action;
        m_FrameData = data;
    }

    #region Set & Get
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
    public List<GameObject> TargetObjects { get { return m_lstTargetObjects; } }
    #endregion

    #region Public Interface
    abstract public bool IsTrigger(float fRealTime);
    abstract public bool IsFinish(float fRealTime);
    abstract public void Execute();
    abstract public void Play();
    abstract public void Pause(float fTime);
    abstract public void Stop();
    abstract public void Destory();
    virtual public void Update(float fRealTime) { }
    #endregion

    #region System Functions

    #endregion

}

