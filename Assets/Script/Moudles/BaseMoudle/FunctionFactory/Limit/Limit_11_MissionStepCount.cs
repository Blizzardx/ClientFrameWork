//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Limit_11_MissionStepCount
//
// Created by : Baoxue at 2015/12/15 15:17:45
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Limit_11_MissionStepCount : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        int missionStepCount = MissionManager.Instance.GetMissionStepCounter(Limit.ParamIntList[0]);
        return OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, missionStepCount, Limit.ParamIntList[1]);
    }
}

