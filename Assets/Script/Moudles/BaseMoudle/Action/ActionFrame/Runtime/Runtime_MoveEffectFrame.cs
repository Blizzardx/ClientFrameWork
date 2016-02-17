//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Runtime_MoveEffectFrame
//
// Created by : Baoxue at 2015/11/24 20:12:51
//
//
//========================================================================

using ActionEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Runtime_MoveEffectFrame : AbstractActionFrame
{
    private GameObject m_EffectObj;
    private bool m_bIsTrigger;
    private Runtime_MoveEffectFrameConfig m_FrameConfig;
    private bool m_bIsRunning;
    private Vector3 m_vTargetPos;
    private float m_fLeftTime;
    private float m_fHigh;
    private float m_fLastVx;
    private float m_fLastVy;
    private float m_fDeltaTime;
    private float m_fLastTime;
    private Vector3 m_vDeltaSpace;
    private Vector3 m_vInitPos;
    private Vector3 m_vDir;
    private float m_fInitTime;
    private GameObject m_TargetObj;

    public Runtime_MoveEffectFrame(ActionPlayer action, ActionFrameData data) : base(action, data)
    {
        m_bIsTrigger = false;
        m_FrameConfig = m_FrameData.Runtime_MoveEffect;
    }
    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time && !m_bIsTrigger)
        {
            m_bIsTrigger = true;
            return true;
        }

        return false;
    }
    public override bool IsFinish(float fRealTime)
    {
        if (m_bIsTrigger)
        {
            return !m_bIsRunning;
        }
        else
        {
            return false;
        }
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

            Ilife userInstance = context.Get(FuncContext.ContextKey.Target) as Ilife;
            if (null == userInstance)
            {
                return;
            }
            m_TargetObj = ((CharTransformData)(((ITransformBehaviour)(userInstance)).GetTransformData())).GetGameObject();
            m_EffectObj = EffectContainer.GetInstance(uint.Parse(m_FrameConfig.InstanceId));
            m_vTargetPos = m_TargetObj.transform.position;
            m_fLeftTime = (float) (m_FrameConfig.Time);
            m_fHigh = (float) (m_FrameConfig.High);

            m_fInitTime = TimeManager.Instance.GetTime();
            m_fLastTime = m_fInitTime;
            m_fLastVx = Vector3.Distance(m_EffectObj.transform.position, m_vTargetPos) / m_fLeftTime;
            m_fLastVy = (float)( 0.5f * 9.8 * m_fLeftTime);
            m_vInitPos = m_EffectObj.transform.position;
            m_vDir = m_vTargetPos - m_vInitPos;
            
            m_vDeltaSpace.Normalize();

            m_bIsRunning = true;
        }
    }
    public override void Update(float fRealTime)
    {
        base.Update(fRealTime);
        if (!EffectContainer.CheckEffectavailable(uint.Parse(m_FrameConfig.InstanceId)))
        {
            m_bIsRunning = false;
        }

        if (m_bIsRunning)
        {
            float currentTime = TimeManager.Instance.GetTime();
            float yDeltaTime = currentTime - m_fInitTime;

            m_fDeltaTime = currentTime - m_fLastTime;
            m_fLastTime = currentTime;
            m_fLeftTime -= m_fDeltaTime;
            
            m_vDeltaSpace = m_fDeltaTime * m_fLastVx * m_vDir.normalized;
            m_EffectObj.transform.position += m_vDeltaSpace;

            m_vDeltaSpace = m_EffectObj.transform.position;
            m_vDeltaSpace.y = yDeltaTime * m_fLastVy - 0.5f * 9.8f * yDeltaTime * yDeltaTime;
            m_EffectObj.transform.position = m_vDeltaSpace;

            m_vTargetPos = m_TargetObj.transform.position;
            m_fLastVx = Vector3.Distance(m_EffectObj.transform.position, m_vTargetPos) / m_fLeftTime;


            Debuger.Log("vy " + m_EffectObj.transform.position.y);
            if (m_fLeftTime <= 0.0f)
            {
                m_bIsRunning = false;
            }
        }
    }
}

