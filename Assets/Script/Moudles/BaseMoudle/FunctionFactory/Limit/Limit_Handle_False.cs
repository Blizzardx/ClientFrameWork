using Config;
using UnityEngine;
using System.Collections;

public class Limit_Handle_False : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        return false;
    }
}
