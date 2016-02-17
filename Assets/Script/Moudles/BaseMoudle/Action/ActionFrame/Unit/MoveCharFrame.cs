//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCharFrame
// 
// Created by : LeoLi at 2015/11/27 16:27:16
//
// Purpose : 
//========================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;

public class CharMovement
{
    public float Speed;
    public Vector3 Target;
}

public class MoveCharFrame : AbstractActionFrame
{
    private MoveCharFrameConfig m_Config;
    private Npc m_Npc;
    private PlayerCharacter m_Player;
    private bool m_bIsKinematic;
    public MoveCharFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.MovecharFrame;
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
        if (m_Config.CharType == ECharType.Npc)
        {
            if (m_lstTargetObjects == null || m_lstTargetObjects.Count <= 0)
            {
                Debuger.LogWarning("No Exist Npc !");
                return;
            }
            foreach (GameObject charObject in m_lstTargetObjects)
            {
                Rigidbody body = charObject.GetComponent<Rigidbody>();
                if (body)
                {
                    body.isKinematic = m_bIsKinematic;
                }
            }
        }
        else if (m_Config.CharType == ECharType.Player)
        {
            if (m_Player == null)
                return;
            CharTransformData charData = (CharTransformData)(m_Player.GetTransformData());
            GameObject charObject = charData.GetGameObject();
            Rigidbody body = charObject.GetComponent<Rigidbody>();
            if (body)
            {
                body.isKinematic = m_bIsKinematic;
            }
        }
    }
    #endregion

    #region System Functions
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
            List<CharMovement> movePath = new List<CharMovement>();
            foreach (var point in m_Config.LstSpeedMove)
            {
                //Vector3 point = target.Target.GetVector3();
                //movePath.Add(point);
                CharMovement move = new CharMovement();
                move.Target = point.Target.GetVector3();
                move.Speed = (float)point.Speed;
                movePath.Add(move);
            }
            Rigidbody body = charObject.GetComponent<Rigidbody>();
            if (body)
            {
                m_bIsKinematic = body.isKinematic;
                body.isKinematic = true;
            }
            m_Npc.MovePath(movePath);
        }
    }
    private void MovePlayer()
    {
        m_Player = PlayerManager.Instance.GetPlayerInstance();
        if (m_Player == null)
        {
            Debuger.LogWarning("No Exist Player !");
            return;
        }
        List<CharMovement> movePath = new List<CharMovement>();
        foreach (var point in m_Config.LstSpeedMove)
        {
            //Vector3 point = target.Target.GetVector3();
            //movePath.Add(point);
            CharMovement move = new CharMovement();
            move.Target = point.Target.GetVector3();
            move.Speed = (float)point.Speed;
            movePath.Add(move);
        }
        CharTransformData charData = (CharTransformData)(m_Player.GetTransformData());
        GameObject charObject = charData.GetGameObject();
        Rigidbody body = charObject.GetComponent<Rigidbody>();
        if (body)
        {
            m_bIsKinematic = body.isKinematic;
            body.isKinematic = true;
        }
        m_Player.MovePath(movePath);
    }
    #endregion

}

