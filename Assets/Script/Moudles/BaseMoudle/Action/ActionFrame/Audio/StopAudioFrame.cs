using ActionEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StopAudioFrame : AbstractActionFrame
{
    private StopAudioFrameConfig m_FrameConfig;
    private bool m_bIsTrigger;

    public StopAudioFrame(ActionPlayer action, ActionFrameData data) : base(action, data)
    {
        m_FrameConfig = m_FrameData.StopAudioFrame;
    }
    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time /*&& fRealTime <= m_FrameData.Time + 0.1*/ && !m_bIsTrigger)
        {
            return true;
        }

        return false;
    }
    public override bool IsFinish(float fRealTime)
    {
        return true;
    }
    protected override void Execute()
    {
        AudioPlayer.Instance.StopAudio(m_FrameConfig.AudioSource);
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
    public override void Update(float fRealTime)
    {
        base.Update(fRealTime);
    }
}