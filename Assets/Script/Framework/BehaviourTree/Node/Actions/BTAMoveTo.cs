//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : BTAMoveTo
//
// Created by : Baoxue at 2015/12/3 12:08:13
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviourTree
{
    /// <summary>
    /// 
    /// </summary>
    public class BTAMoveTo : BTAction
    {
        private int m_iTargetId;
        private int m_iFollowPintId;

        private float m_fDeltaTime;
        private float m_fTriggerTime = 2.0f;
        private Npc m_TargetNpc;

        public BTAMoveTo(int targetId,int followPointId)
        {
            m_iTargetId = targetId;
            m_iFollowPintId = followPointId;
        }
        protected override EBTState OnEnter()
        {
            Debuger.Log("enter move to player");
            Ilife owner = m_Database.GetData<Ilife>(EDataBaseKey.Owner);
            if (null == owner)
            {
                return EBTState.False;
            }

            m_TargetNpc = (Npc)(owner);
            m_TargetNpc.MoveTo(PlayerManager.Instance.GetPlayerInstance().GetFollowPoint(m_iFollowPintId).transform.position);
            m_fDeltaTime = 0.0f;
            return EBTState.Running;
        }
        protected override EBTState OnExit()
        {
            return EBTState.True;
        }
        protected override EBTState OnRunning()
        {
            m_fDeltaTime += TimeManager.Instance.GetDeltaTime();
            if (m_fDeltaTime >= m_fTriggerTime)
            {
                m_fDeltaTime = 0.0f;
                m_TargetNpc.MoveTo(PlayerManager.Instance.GetPlayerInstance().GetFollowPoint(m_iFollowPintId).transform.position);
            }
            return EBTState.Running;
        }
    }
}
