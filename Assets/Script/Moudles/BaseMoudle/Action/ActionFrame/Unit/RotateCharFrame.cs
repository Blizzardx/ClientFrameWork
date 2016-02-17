//========================================================================
// Copyright(C): CYTX
//
// FileName : RotateCharFrame
// 
// Created by : LeoLi at 2016/1/16 15:30:46
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;

public class RotateCharFrame : AbstractActionFrame
{
    private RotateCharFrameConfig m_Config;
    private Npc m_Npc;
    private PlayerCharacter m_Player;

    public RotateCharFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.RotcharFrame;
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
        m_Player.Rotate((float)m_Config.Rotation);
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
            m_Npc.Rotate((float)m_Config.Rotation);
        }
    }
    #endregion

}

