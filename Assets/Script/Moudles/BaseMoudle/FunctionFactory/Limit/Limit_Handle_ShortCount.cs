using Config;
using UnityEngine;
using System.Collections.Generic;

public class Limit_Handle_ShortCount : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(Limit.Target));
        for (int i = 0; i < target.Count; ++i)
        {
            short count = ((ICountBehaviour)(target[i])).GetCountData().GetShortCount(Limit.ParamIntList[0]);
            if (!OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, count, Limit.ParamIntList[1]))
            {
                return false;
            }
        }
        return true;
    }
}