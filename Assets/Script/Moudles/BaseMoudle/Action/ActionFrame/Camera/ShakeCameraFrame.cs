//========================================================================
// Copyright(C): CYTX
//
// FileName : ShakeCameraFrame
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
public class ShakeCameraFrame : AbstractActionFrame
{

    #region Field
    private ShakeCameraFrameConfig m_Config;
    #endregion
    public ShakeCameraFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.ShakeCameraFrame;
    }

    #region Public Interface
    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time && fRealTime <= m_FrameData.Time + 0.5f)
        {
            return true;
        }

        return false;
    }
    public override bool IsFinish(float fRealTime)
    {
        return false;
    }
    protected override void Execute()
    {
        if (null == m_Config || null == m_Config.Amount)
        {
            return;
        }
        GlobalScripts.Instance.mGameCamera.ShakeCamera((float)m_Config.Time, m_Config.Amount.GetVector3());
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
        
    }
    #endregion

    #region System Functions

    #endregion

}

