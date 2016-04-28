using Config;
using UnityEngine;
using System.Collections.Generic;

public class Limit_2_Flag : LimitMethodsBase
{
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
            bool flag = PlayerManager.Instance.GetCharCounterData().GetFlag(index);
            if (!OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, flag, (Limit.ParamIntList[1] != 0)))
            {
                return false;
            }
        }
        return true;
    }
}