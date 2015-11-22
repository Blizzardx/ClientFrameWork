//========================================================================
// Copyright(C): CYTX
//
// FileName : SetCameraFrame
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/5 17:16:18
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
public class SetCameraFrame : AbstractActionFrame
{

    #region Field
    private SetCameraFrameConfig m_Config;
    #endregion
    public SetCameraFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.SetCameraFrame;
    }

    #region Public Interface
    public override bool IsTrigger(float fRealTime)
    {
        throw new System.NotImplementedException();
    }
    public override bool IsFinish(float fRealTime)
    {
        throw new System.NotImplementedException();
    }
    protected override void Execute()
    {
        throw new System.NotImplementedException();
    }
    public override void Play()
    {
        throw new System.NotImplementedException();
    }
    public override void Pause(float fTime)
    {
        throw new System.NotImplementedException();
    }
    public override void Stop()
    {
        throw new System.NotImplementedException();
    }
    public override void Destory()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region System Functions

    #endregion

}

