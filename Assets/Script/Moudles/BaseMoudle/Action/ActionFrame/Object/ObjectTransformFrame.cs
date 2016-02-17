//========================================================================
// Copyright(C): CYTX
//
// FileName : ObjectTransformFrame
// 
// Created by : LeoLi at 2016/1/22 17:52:08
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;
public class ObjectTransformFrame : AbstractActionFrame
{
    private ObjectTransformFrameConfig m_Config;

    public ObjectTransformFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.ObjTransformFrame;
    }

    protected override void Execute()
    {
        if (TargetObjects == null || TargetObjects.Count <= 0)
            return;

        foreach (GameObject obj in TargetObjects)
        {
            obj.transform.position = m_Config.Pos.GetVector3();
            obj.transform.eulerAngles = m_Config.Rot.GetVector3();
            obj.transform.localScale = m_Config.Scale.GetVector3();
        }
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

