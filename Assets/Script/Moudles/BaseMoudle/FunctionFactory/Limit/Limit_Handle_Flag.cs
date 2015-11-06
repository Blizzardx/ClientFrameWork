using Config;
using UnityEngine;
using System.Collections.Generic;

public class Limit_Handle_Flag : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(Limit.Target));
        for (int i = 0; i < target.Count; ++i)
        {
            bool flag = ((ICountBehaviour)(target[i])).GetCountData().GetFlag(Limit.ParamIntList[0]);
            if (!OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, flag, (Limit.ParamIntList[1] != 0)))
            {
                return false;
            }
        }
        return true;
    }
}