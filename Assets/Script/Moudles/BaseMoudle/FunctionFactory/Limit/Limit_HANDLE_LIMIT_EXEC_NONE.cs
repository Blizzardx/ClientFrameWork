using Common.Config;
using UnityEngine;
using System.Collections;

public class Limit_HANDLE_LIMIT_EXEC_NONE : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        return false;
    }
}
