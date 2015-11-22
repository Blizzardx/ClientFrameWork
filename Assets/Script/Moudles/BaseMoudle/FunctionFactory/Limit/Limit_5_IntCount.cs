﻿using Config;
using UnityEngine;
using System.Collections.Generic;

public class Limit_5_IntCount : LimitMethodsBase
{
    public Limit_5_IntCount(int id) : base(id)
    {
    }

    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(Limit.Target));
        for (int i = 0; i < target.Count; ++i)
        {
            if (!(target[i] is PlayerCharacter))
            {
                continue;
            }

            int index = Limit.ParamIntList[0];
            int count = ((PlayerCharacter)(target[i])).GetCharCounterData().GetBit32Count(index);
            if (!OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, count, Limit.ParamIntList[1]))
            {
                return false;
            }
        }
        return true;
    }
}