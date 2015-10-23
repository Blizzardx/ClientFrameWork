using System.Collections.Generic;
using Common.Config;
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
		};

        LimitMethods.InitLimitMethods(LimitExec);
    }
}
