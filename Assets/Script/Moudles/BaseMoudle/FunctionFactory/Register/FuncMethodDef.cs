using System.Collections.Generic;
using Common.Config;
using UnityEngine;
using System.Collections;

public class FuncMethodDef
{
    static Dictionary<int, FuncMethodsBase> FuncExec;
    static public void InitFuncMethod()
    {
        //Func Array
        FuncExec = new Dictionary<int, FuncMethodsBase>
		{
	        {0,new Func_HANDLE_FUNC_EXEC_NONE()},
		};

        FuncMethods.InitFuncMethods(FuncExec);
    }
}
