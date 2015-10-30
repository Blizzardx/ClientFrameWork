using System.Collections.Generic;
using Config;
using UnityEngine;
using System.Collections;

public class LimitMethodDef 
{
    static Dictionary<int, LimitMethodsBase> LimitExec;

    static public void InitLimitMethod()
    {
        //Limit Array
        LimitExec = new Dictionary<int, LimitMethodsBase>
		{
			{0,new Limit_HANDLE_LIMIT_EXEC_NONE()},
			{1,new Limit_HANDLE_LIMIT_EXEC_TRUE()},
		};

        LimitMethods.InitLimitMethods(LimitExec);
    }
}
