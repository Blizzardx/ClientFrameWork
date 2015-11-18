//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCameraFrame
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/4 17:28:00
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;
public class MoveCameraFrame : AbstractActionFrame
{
    public enum EMoveCameraType
    {
        None = -1,
        MoveSelf,
    }
    public enum EMoveCameraState
    {
        None,
        MoveTo,
        Stay,
        MoveBack, 
    }

    #region Field
    private MoveCameraFrameConfig m_Config;
    private float m_fTickTime;
    private EMoveCameraState m_eMoveCameraState;
    #endregion

    public MoveCameraFrame(ActionPlayer action, ActionFrameData data) : base(action, data)
	{
		m_Config = m_FrameData.MoveCameraFrame;
	}

    #region Public Interface
    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time && fRealTime <= m_FrameData.Time + 0.1)
        {
            return true;
        }

        return false;
    }
    public override bool IsFinish(float fRealTime)
    {
        return false;
    }
    public override void Execute()
    {
        float fDetalDistance = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fDistance - (float)m_Config.Distance);
        float fDistanceDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalDistance / (float)m_Config.MoveToTime;
        GlobalScripts.Instance.mGameCamera.m_fDistance = (float)m_Config.Distance;
        GlobalScripts.Instance.mGameCamera.m_fDistanceDamping = fDistanceDamping;

        float fDetalOffsetHeight = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fOffsetHeight - (float)m_Config.OffseHeight);
        float fOffsetHeightDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalOffsetHeight / (float)m_Config.MoveToTime;
        GlobalScripts.Instance.mGameCamera.m_fOffsetHeight = (float)m_Config.OffseHeight;
        GlobalScripts.Instance.mGameCamera.m_fOffsetHeightDamping = fOffsetHeightDamping;

        float fDetalHeight = Math.Abs(GlobalScripts.Instance.mGameCamera.m_fHeight - (float)m_Config.Height);
        float fHeightDamping = 0 == (float)m_Config.MoveToTime ? float.MaxValue : fDetalHeight / (float)m_Config.MoveToTime;
        GlobalScripts.Instance.mGameCamera.m_fHeight = (float)m_Config.Height;
        GlobalScripts.Instance.mGameCamera.m_fHeightDamping = fHeightDamping;

        m_fTickTime = TimeManager.Instance.GetTime() + (float)m_Config.MoveToTime;
        m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.MoveTo;
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
        
    }
    public override void Destory()
    {
        GlobalScripts.Instance.mGameCamera.ResetCam();
    }
    public override void Update(float fRealTime)
    {
        base.Update(fRealTime);

        float fCurrtime = TimeManager.Instance.GetTime();
        switch (m_eMoveCameraState)
        {
            case MoveCameraFrame.EMoveCameraState.MoveTo:
                if (fCurrtime >= m_fTickTime)
                {
                    m_fTickTime = fCurrtime + (float)m_Config.StayTime;
                    m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.Stay;
                }
                break;
            case MoveCameraFrame.EMoveCameraState.Stay:
                if (fCurrtime >= m_fTickTime)
                {
                    m_fTickTime = fCurrtime + (float)m_Config.MoveBackTime;
                    m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.MoveBack;

                    GlobalScripts.Instance.mGameCamera.ResetCam();
                }
                break;
            case MoveCameraFrame.EMoveCameraState.MoveBack:
                if (fCurrtime >= m_fTickTime)
                {
                    m_fTickTime = 0f;
                    m_eMoveCameraState = MoveCameraFrame.EMoveCameraState.None;
                }
                break;
        }
    }
    #endregion

    #region System Functions

    #endregion

}

