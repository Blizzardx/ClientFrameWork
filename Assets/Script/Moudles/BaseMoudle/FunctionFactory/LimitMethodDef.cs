using System.Collections.Generic;
using Common.Config;
using UnityEngine;
using System.Collections;

public class LimitMethodDef 
{
    static Dictionary<int, LimitMethods.LimitExecHandler> LimitExec;

    static public void InitLimitMethod()
    {
        //Limit Array
        LimitExec = new Dictionary<int, LimitMethods.LimitExecHandler>
		{
			{0,HANDLE_LIMIT_EXEC_NONE},
		};

        LimitMethods.InitLimitMethods(LimitExec);
    }
    /// All of the Limit Methods
    /// <summary>  //
    /// 		   //	
    /// </summary> //
    static private bool HANDLE_LIMIT_EXEC_NONE(HandleTarget Target, LimitData Limit)
    {
        return false;
    }
}
