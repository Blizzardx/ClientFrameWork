using System.Collections.Generic;
using Config;
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
	        {0,new Func_0_NONE(0)},
		};

        FuncMethods.InitFuncMethods(FuncExec);
    }
}
