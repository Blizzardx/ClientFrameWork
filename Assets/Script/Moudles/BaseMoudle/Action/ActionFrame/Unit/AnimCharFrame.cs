//========================================================================
// Copyright(C): CYTX
//
// FileName : AnimCharFrame
// 
// Created by : LeoLi at 2015/12/17 18:50:53
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;

public class AnimCharFrame : AbstractActionFrame
{
    private AnimCharFrameConfig m_Config;
    private Npc m_Npc;
    private PlayerCharacter m_Player;

    public AnimCharFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.AnimcharFrame;
    }

    #region Public Interface
    protected override void Execute()
    {
        if (m_Config.CharType == ECharType.Npc)
        {
            MoveNpc();
        }
        else if (m_Config.CharType == ECharType.Player)
        {
            MovePlayer();
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
        throw new NotImplementedException();
    }

    public override void Pause(float fTime)
    {
        throw new NotImplementedException();
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    public override void Destory()
    {
        
    }
    #endregion

    #region System Functions
    private void MovePlayer()
    {
        m_Player = PlayerManager.Instance.GetPlayerInstance();
        if (m_Player == null)
        {
            Debuger.LogWarning("No Exist Player !");
            return;
        }
        //CharTransformData charTransfrom = (CharTransformData)m_Player.GetTransformData();
        m_Player.DirectPlayAnimation(m_Config.LstAnimName);
    }

    private void MoveNpc()
    {
        if (m_lstTargetObjects == null || m_lstTargetObjects.Count <= 0)
        {
            Debuger.LogWarning("No Exist Npc !");
            return;
        }
        foreach (GameObject charObject in m_lstTargetObjects)
        {
            CharTransformContainer container = charObject.GetComponent<CharTransformContainer>();
            if (container == null)
            {
                Debuger.LogError("No Container in " + charObject.ToString());
                return;
            }
            m_Npc = (Npc)container.GetData();
            m_Npc.DirectPlayAnimation(m_Config.LstAnimName);
        }
    }

    #endregion
}

