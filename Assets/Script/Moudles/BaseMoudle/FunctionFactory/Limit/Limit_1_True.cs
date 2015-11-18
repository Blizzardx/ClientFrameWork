using Config;
using UnityEngine;
using System.Collections;

public class Limit_1_True : LimitMethodsBase
{
    public Limit_1_True(int id) : base(id)
    {
    }

    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        return true;
    }
}