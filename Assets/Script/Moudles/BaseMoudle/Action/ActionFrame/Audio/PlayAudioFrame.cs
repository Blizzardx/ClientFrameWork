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
    private bool m_bIsTrigger;

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
        if (m_FrameConfig.IsAttach)
        {
            // try get attach obj
            GameObject obj = null;
            switch (m_FrameConfig.EntityType)
            {
                case EntityType.Camera:
                    obj = GlobalScripts.Instance.mGameCamera.transform.parent.gameObject;
                    break;
                case EntityType.Npc:
                    Ilife npc = LifeManager.GetLife(m_FrameConfig.AttachNpcId);
                    if (null == npc || (!(npc is Npc)))
                    {
                        Debuger.LogError("Play audio : can't load npc by id " + m_FrameConfig.AttachNpcId);
                        return;
                    }
                    obj = ((CharTransformData)((Npc)(npc)).GetTransformData()).GetGameObject();
                    break;
                case EntityType.Player:
                    if (null == PlayerManager.Instance.GetPlayerInstance())
                    {
                        Debuger.LogError("Play audio : can't load player");
                        return;
                    }
                    obj =
                        ((CharTransformData)(PlayerManager.Instance.GetPlayerInstance().GetTransformData())).GetGameObject();
                    break;
            } 
            AudioPlayer.Instance.PlayAudio(m_FrameConfig.AudioSource, obj.transform,m_FrameConfig.IsLoop);
        }
        else
        {
            AudioPlayer.Instance.PlayAudio(m_FrameConfig.AudioSource, m_FrameConfig.PlayPosition.GetVector3(),
                   m_FrameConfig.IsLoop);
        }
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
        //AudioPlayer.Instance.StopAudio(m_FrameConfig.AudioSource);
    }
    public override void Update(float fRealTime)
    {
        base.Update(fRealTime);
    }
    public string ChekResource()
    {
        if(AppManager.Instance == null || !m_FrameConfig.IsCareGender)
        {
            return m_FrameConfig.AudioSource;
        }
        return PlayerManager.Instance.GetCharBaseData().Gender == 0 ? m_FrameConfig.AudioSource : m_FrameConfig.ParamAudioSource;

    }
}

