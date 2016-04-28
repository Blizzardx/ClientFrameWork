//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Limit_18_IsPlayer
//
// Created by : Baoxue at 2015/12/15 17:24:55
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Limit_18_IsPlayer : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(Limit.Target));
        if (Limit.Oper == (int) ELimitOperator.ELO_Equal)
        {
            return target[0] is PlayerCharacter;
        }
        else if (Limit.Oper == (int)ELimitOperator.ELO_NotEuqal)
        {
            return (!(target[0] is PlayerCharacter));
        }
        return false;
    }
}