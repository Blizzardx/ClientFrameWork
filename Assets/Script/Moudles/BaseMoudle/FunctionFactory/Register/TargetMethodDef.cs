using System.Collections.Generic;
using UnityEngine;
public class TargetMethodDef 
{
    static List<TargetMethodBase> TargetExec;

    static public void InitTargetMethod()
    {
        //Limit Array
        TargetExec = new List<TargetMethodBase>
		{
			new Target_0_NONE(),
            new Target_1_INCIRCLERANGE(),
		};

        TargetMethods.InitTargetMethods(TargetExec);
        TargetExec.Clear();
        TargetExec = null;
    }
}
