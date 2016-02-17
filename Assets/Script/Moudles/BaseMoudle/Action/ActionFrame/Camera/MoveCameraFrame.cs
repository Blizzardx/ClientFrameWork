//========================================================================
// Copyright(C): CYTX
//
// FileName : MoveCameraFrame
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/4 17:28:00
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using System;
public class MoveCameraFrame : AbstractActionFrame
{
    private MoveCameraFrameConfig m_Config;
    //private float m_fTickTime;

    public MoveCameraFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.MoveCameraFrame;
    }

    #region Public Interface
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
    protected override void Execute()
    {
        bool isImmediate = m_Config.IsImmediate;
        switch (m_Config.MoveType)
        {
            case EMoveCameraType.UnLock:
                GlobalScripts.Instance.mGameCamera.LockCam = false;
                GlobalScripts.Instance.mGameCamera.SetCameraPos(m_Config.EndPos.GetVector3(), isImmediate);
                GlobalScripts.Instance.mGameCamera.SetCameraRot(m_Config.EndRot.GetVector3(), isImmediate);
                break;
            case EMoveCameraType.Lock:
                if (m_Config.FollowType == ECameraFollowType.Player)
                {
                    PlayerCharacter player = PlayerManager.Instance.GetPlayerInstance();
                    if (player == null)
                    {
                        return;
                    }
                    CharTransformData charData = (CharTransformData)player.GetTransformData();
                    if (charData.GetGameObject() != null)
                    {
                        GlobalScripts.Instance.mGameCamera.SetTarget(charData.GetGameObject().transform, isImmediate);
                    }
                }
                else if (m_Config.FollowType == ECameraFollowType.Npc)
                {
                    if (m_lstTargetObjects == null || m_lstTargetObjects.Count <= 0)
                    {
                        return;
                    }
                    GameObject target = m_lstTargetObjects[0];
                    GlobalScripts.Instance.mGameCamera.SetTarget(target.transform, isImmediate);
                }
                GlobalScripts.Instance.mGameCamera.LockCam = true;
                GlobalScripts.Instance.mGameCamera.Distance = (float)m_Config.Distance;
                GlobalScripts.Instance.mGameCamera.Height = (float)m_Config.Height;
                GlobalScripts.Instance.mGameCamera.OffsetHeight = (float)m_Config.OffseHeight;
                if (!GlobalScripts.Instance.mGameCamera.CheckRoating())
                    GlobalScripts.Instance.mGameCamera.Rotation = (float)m_Config.Rotation;
                break;
        }
        GlobalScripts.Instance.mGameCamera.PositonDamping = (float)m_Config.PosDamping;
        GlobalScripts.Instance.mGameCamera.RotationDamping = (float)m_Config.RotDamping;
        GlobalScripts.Instance.mGameCamera.IsOverObstacle = m_Config.IsOverObstacle;
    }
    public override void Play()
    {
        throw new System.NotImplementedException();
    }
    public override void Pause(float fTime)
    {
        throw new System.NotImplementedException();
    }
    public override void Stop()
    {

    }
    public override void Destory()
    {
        GlobalScripts.Instance.mGameCamera.ResetCam();
    }
    public override void Update(float fRealTime)
    {
        base.Update(fRealTime);

    }
    #endregion

    #region System Functions

    #endregion

}

