//========================================================================
// Copyright(C): CYTX
//
// FileName : RotateCameraFrame
// 
// Created by : LeoLi at 2016/1/22 16:28:06
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;
public class RotateCameraFrame : AbstractActionFrame
{

    private RotateCameraFrameConfig m_Config;

    public RotateCameraFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.RotCameraFrame;
    }

    protected override void Execute()
    {
        GlobalScripts.Instance.mGameCamera.RotateWithTarget((float)m_Config.Rotation, (float)m_Config.Speed);
    }

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

    public override void Play()
    {
        
    }

    public override void Pause(float fTime)
    {
        
    }

    public override void Stop()
    {
        GlobalScripts.Instance.mGameCamera.StopRotate();
    }

    public override void Destory()
    {
        GlobalScripts.Instance.mGameCamera.StopRotate();
    }
}

