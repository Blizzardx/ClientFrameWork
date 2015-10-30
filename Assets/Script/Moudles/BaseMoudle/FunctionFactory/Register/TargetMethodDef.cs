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
            {1,new Target_HANDLE_TARGET_INCIRCLERANGE()}
		};

        TargetMethods.InitTargetMethods(TargetExec);
    }
}
