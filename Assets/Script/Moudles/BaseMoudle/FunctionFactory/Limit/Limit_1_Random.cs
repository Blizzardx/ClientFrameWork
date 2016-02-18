using Config;
using UnityEngine;
using System.Collections;


public class Limit_1_Random : LimitMethodsBase
{
    public Limit_1_Random(int id) : base(id)
    {
    }

    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        int random = Random.Range(0, 100);
        return OperationFunc.LimitOperatorValue((ELimitOperator)Limit.Oper, random, Limit.ParamIntList[0]);
    }
}