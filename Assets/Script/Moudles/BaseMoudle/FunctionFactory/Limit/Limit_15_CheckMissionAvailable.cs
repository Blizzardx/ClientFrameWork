//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Limit_15_CheckMissionAvailable
//
// Created by : Baoxue at 2015/12/16 17:37:03
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Limit_15_CheckMissionAvailable : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        return MissionManager.Instance.CheckMissionAvailable(Limit.ParamIntList[0]);
    }
}