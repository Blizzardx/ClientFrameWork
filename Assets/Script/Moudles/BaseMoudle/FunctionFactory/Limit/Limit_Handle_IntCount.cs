using Config;
using UnityEngine;
using System.Collections.Generic;

public class Limit_Handle_IntCount : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(Limit.Target));
        for (int i = 0; i < target.Count; ++i)
        {
            int count = ((ICountBehaviour)(target[i])).GetCountData().GetIntCount(Limit.ParamIntList[0]);
            if (!OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, count, Limit.ParamIntList[1]))
            {
                return false;
            }
        }
        return true;
    }
}