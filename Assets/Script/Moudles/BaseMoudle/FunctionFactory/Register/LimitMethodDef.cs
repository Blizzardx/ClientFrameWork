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
			{0,new Limit_Handle_False()},
			{1,new Limit_Handle_True()},
			{2,new Limit_Handle_Range()},
		};

        LimitMethods.InitLimitMethods(LimitExec);
    }
}
