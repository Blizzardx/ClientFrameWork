//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Runtime_RemoveEffectFrame
//
// Created by : Baoxue at 2015/11/27 16:31:29
//
//
//========================================================================

using ActionEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Runtime_RemoveEffectFrame : AbstractActionFrame
{
    private Runtime_RemoveEffectFrameConfig m_FrameConfig;

    public Runtime_RemoveEffectFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_FrameConfig = m_FrameData.Runtime_RemoveEffect;
    }

    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time)
        {
            return true;
        }

        return false;
    }

    public override bool IsFinish(float fRealTime)
    {
        return true;
    }

    public override void Play()
    {

    }

    protected override void Execute()
    {
        OnTrigger();
    }

    public override void Pause(float fTime)
    {

    }

    public override void Stop()
    {
        OnTrigger();

    }
    public override void Destory()
    {

    }
    private void OnTrigger()
    {
        GameObject objInstance = EffectContainer.GetInstance(uint.Parse(m_FrameConfig.InstanceId));
        if (null != objInstance)
        {
            GameObject.Destroy(objInstance);
        }
    }
}


