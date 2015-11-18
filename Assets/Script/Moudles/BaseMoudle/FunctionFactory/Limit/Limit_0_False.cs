using Config;
using UnityEngine;
using System.Collections;

public class Limit_0_False : LimitMethodsBase
{
    public Limit_0_False(int id) : base(id)
    {
    }

    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        return false;
    }
}
