using Config;
using UnityEngine;
using System.Collections.Generic;

public class Limit_Handle_ByteCount : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(Limit.Target));
        for (int i = 0; i < target.Count; ++i)
        {
            byte count = ((ICountBehaviour)(target[i])).GetCountData().GetByteCount(Limit.ParamIntList[0]);
            if (!OperationFunc.LimitOperatorValue((ELimitOperator) Limit.Oper, count, Limit.ParamIntList[1]))
            {
                return false;
            }
        }
        return true;
    }
}