//========================================================================
// Copyright(C): CYTX
//
// FileName : FuncMethodFrame
// 
// Created by : LeoLi at 2016/1/26 15:15:18
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;
public class FuncMethodFrame : AbstractActionFrame
{
    private FuncMethodFrameConfig m_Config;
    public FuncMethodFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.FuncMethodFrame;
    }
    protected override void Execute()
    {
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, m_Config.FuncID, null);
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
        
    }

    public override void Destory()
    {
    }
}

