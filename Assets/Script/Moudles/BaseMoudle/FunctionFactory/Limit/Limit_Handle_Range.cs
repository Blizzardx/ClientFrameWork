using Config;
using UnityEngine;
using System.Collections;


public class Limit_Handle_Range : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        int random = Random.Range(0, 100);
        return OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, random, Limit.ParamIntList[0]);
    }
}