//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : MissionManager
//
// Created by : Baoxue at 2015/11/19 17:48:02
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MissionManager
{
    private MainMissionConfig m_CurrentMission;

    public void InitMissionMgr()
    {
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_ACCEPT_MISSION, TriggerAcceptMission);

    }
    public void Destuctor()
    {
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACCEPT_MISSION, TriggerAcceptMission);
    }
    private void TryAcceptMission(int id)
    {
           
    }
    private void TriggerAcceptMission(MessageObject obj)
    {
        int missinId = (int) (obj.msgValue);

        //try accept mission
        TryAcceptMission(missinId);
    }
}

