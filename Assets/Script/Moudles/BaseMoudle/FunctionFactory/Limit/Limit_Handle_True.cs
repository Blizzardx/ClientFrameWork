using Config;
using UnityEngine;
using System.Collections;

public class Limit_Handle_True : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        return true;
    }
}