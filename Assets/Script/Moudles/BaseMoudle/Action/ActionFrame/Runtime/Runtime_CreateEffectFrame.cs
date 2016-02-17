//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Runtime_CreateEffectFrame
//
// Created by : Baoxue at 2015/11/24 20:11:54
//
//
//========================================================================

using ActionEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Runtime_CreateEffectFrame : AbstractActionFrame
{
    private Runtime_CreateEffectFrameConfig m_FrameConfig;

    public Runtime_CreateEffectFrame(ActionPlayer action, ActionFrameData data) : base(action, data)
    {
        m_FrameConfig = m_FrameData.Runtime_CreateEffect;
    }

    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time )
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
       
    }

    public override void Destory()
    {
       
    }

    private void OnTrigger()
    {
        //try get target
        var param = m_ActionPlayer.GetActionParam();
        if (null != param && null != param.Object && param.Object is FuncContext)
        {
            FuncContext context = param.Object as FuncContext;

           

            var userInstance = (EFuncTarget) (m_FrameConfig.TargetType) == EFuncTarget.EFT_Target
                ? (context.Get(FuncContext.ContextKey.Target) as Ilife)
                : (context.Get(FuncContext.ContextKey.User) as Ilife);

            if (null == userInstance)
            {
                //target is null
                return;
            }

            var obj = ((CharTransformData) (((ITransformBehaviour) (userInstance)).GetTransformData())).GetGameObject();
            uint id = uint.Parse(m_FrameConfig.InstanceId);
            GameObject objInstance = EffectContainer.EffectFactory(m_FrameConfig.EffectName,id);
            objInstance.transform.position = obj.transform.position + m_FrameConfig.Pos.GetVector3();
            objInstance.transform.eulerAngles = obj.transform.eulerAngles + m_FrameConfig.Rot.GetVector3();
        }
    }
}

