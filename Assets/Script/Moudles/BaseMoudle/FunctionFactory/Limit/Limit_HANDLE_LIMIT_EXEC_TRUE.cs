using Config;
using UnityEngine;
using System.Collections;

public class Limit_HANDLE_LIMIT_EXEC_TRUE : LimitMethodsBase
{
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit)
    {
        return true;
    }
}