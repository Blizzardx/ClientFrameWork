//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Limit_14_PlayedAction
//
// Created by : Baoxue at 2015/12/15 14:59:11
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Limit_14_PlayedAction : LimitMethodsBase
{
    public Limit_14_PlayedAction(int id)
        : base(id)
    {
    }

    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        return ActionManager.Instance.CheckActionIsPlayed(Limit.ParamIntList[0]);
    }
}
