//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Func_8_MissionStepCount
//
// Created by : Baoxue at 2015/12/15 15:20:27
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_8_MissionStepCount : FuncMethodsBase
{
    public Func_8_MissionStepCount(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        int missionStep = MissionManager.Instance.GetMissionStepCounter(funcdata.ParamIntList[0]);
        OperationFunc.FuncOperatorValue((EFuncOperator)funcdata.Oper, ref missionStep, funcdata.ParamIntList[1]);
        MissionManager.Instance.SetMissionStepCounter(funcdata.ParamIntList[0], missionStep);
        return EFuncRet.Continue;
    }
}