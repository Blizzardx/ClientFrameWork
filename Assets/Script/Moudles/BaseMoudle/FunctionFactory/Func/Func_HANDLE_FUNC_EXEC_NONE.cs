using UnityEngine;
using System.Collections;
using Common.Config;

public class Func_HANDLE_FUNC_EXEC_NONE : FuncMethodsBase
{
    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata)
    {
        return EFuncRet.Continue;
    }
}
