using System.Collections.Generic;
using UnityEngine;
public class TargetMethodDef 
{
    static Dictionary<int, TargetMethodBase> TargetExec;

    static public void InitTargetMethod()
    {
        //Limit Array
        TargetExec = new Dictionary<int, TargetMethodBase>
		{
			{0,new Target_0_NONE(0)},
            {1,new Target_1_INCIRCLERANGE(1)}
		};

        TargetMethods.InitTargetMethods(TargetExec);
    }
}
