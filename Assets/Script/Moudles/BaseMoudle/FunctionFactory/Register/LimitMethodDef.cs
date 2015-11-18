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
			{0,new Limit_0_False(0)},
			{1,new Limit_1_True(1)},
			{2,new Limit_2_Range(2)},
			{3,new Limit_3_ByteCount(3)},
			{4,new Limit_4_Flag(4)},
			{5,new Limit_5_IntCount(5)},
		};

        LimitMethods.InitLimitMethods(LimitExec);
    }
}
