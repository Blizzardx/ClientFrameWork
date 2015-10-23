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
			{0,new Target_HANDLE_TARGET_EXEC_NONE()},
		};

        TargetMethods.InitTargetMethods(TargetExec);
    }
}
