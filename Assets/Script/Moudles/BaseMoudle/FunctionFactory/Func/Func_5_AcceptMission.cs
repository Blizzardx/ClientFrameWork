//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : Func_5_AcceptMission
//
// Created by : Baoxue at 2015/11/19 17:44:03
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_5_AcceptMission : FuncMethodsBase
{
    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        int missionId = funcdata.ParamIntList[0];
        MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_ACCEPT_MISSION, missionId));
        return EFuncRet.Continue;
    }
}