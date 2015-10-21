using System.Collections.Generic;
using Common.Config;
using UnityEngine;
using System.Collections;

public class FuncMethodDef
{
    static Dictionary<int, FuncMethods.FuncExecHandler> FuncExec;
    static public void InitFuncMethod()
    {
        //Func Array
        FuncExec = new Dictionary<int, FuncMethods.FuncExecHandler>
		{
	        {0,HANDLE_FUNC_EXEC_NONE},
		};

        FuncMethods.InitFuncMethods(FuncExec);
    }
    //-------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>  //
    /// 		   //	
    /// </summary> //
    static private EFuncRet HANDLE_FUNC_EXEC_NONE(HandleTarget Target, FuncData funcdata)
    {
        return EFuncRet.Continue;
    }

}
