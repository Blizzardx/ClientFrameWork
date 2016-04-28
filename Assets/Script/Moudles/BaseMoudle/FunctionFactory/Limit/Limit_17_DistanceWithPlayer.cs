//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Limit_17_DistanceWithPlayer
//
// Created by : Baoxue at 2015/12/3 17:41:23
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Limit_17_DistanceWithPlayer : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(Limit.Target));
        if (target.Count <= 0)
        {
            return false;
        }

        Vector3 targetPos = ((ITransformBehaviour) (target[0])).GetTransformData().GetPosition();
        float distance = Vector3.Distance(PlayerManager.Instance.GetPlayerInstance().GetTransformData().GetPosition(), targetPos);
        return OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, (int)(distance), Limit.ParamIntList[0]);
    }
}
