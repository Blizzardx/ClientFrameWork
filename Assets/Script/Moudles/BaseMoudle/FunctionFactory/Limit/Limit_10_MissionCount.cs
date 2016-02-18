//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Limit_10_MissionCount
//
// Created by : Baoxue at 2015/12/15 15:17:35
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Limit_10_MissionCount : LimitMethodsBase
{
    public Limit_10_MissionCount(int id)
        : base(id)
    {
    }

    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        int missionCount = MissionManager.Instance.GetMissionCounter(Limit.ParamIntList[0]);
        return OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, missionCount, Limit.ParamIntList[1]);
    }
}
