//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Func_7_MissionCount
//
// Created by : Baoxue at 2015/12/15 15:20:13
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_7_MissionCount : FuncMethodsBase
{
    public Func_7_MissionCount(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        int missioncount = MissionManager.Instance.GetMissionCounter(funcdata.ParamIntList[0]);
        OperationFunc.FuncOperatorValue((EFuncOperator) funcdata.Oper, ref missioncount, funcdata.ParamIntList[1]);
        MissionManager.Instance.SetMissionCounter(funcdata.ParamIntList[0], missioncount);
        return EFuncRet.Continue;
    }
}
