//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : PlayAudioFrame
//
// Created by : Baoxue at 2015/11/12 16:47:03
//
//
//========================================================================

using ActionEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayAudioFrame : AbstractActionFrame
{
    private PlayAudioFrameConfig m_FrameConfig;

    public PlayAudioFrame(ActionPlayer action, ActionFrameData data) : base(action, data)
    {
        m_FrameConfig = m_FrameData.PlayAudioFrame;
    }
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
        return true;
    }
    public override void Execute()
    {
        AudioManager.Instance.PlayAudio(m_FrameConfig.AudioSource, m_FrameConfig.PlayPosition.GetVector3(),
            m_FrameConfig.IsLoop);
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
        AudioManager.Instance.StopAudio(m_FrameConfig.AudioSource);
    }
    public override void Update(float fRealTime)
    {
        base.Update(fRealTime);
    }
}

