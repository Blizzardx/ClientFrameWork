using System.Collections.Generic;
using Config;
using UnityEngine;
using System.Collections;

public class LimitMethodDef 
{
    static List<LimitMethodsBase> LimitExec;

    static public void InitLimitMethod()
    {
        //Limit Array
        LimitExec = new List<LimitMethodsBase>
		{
			new Limit_1_Random(),
			new Limit_2_Flag(),
			new Limit_3_ByteCount(),
			new Limit_4_IntCount(),
			new Limit_10_MissionCount(),
			new Limit_11_MissionStepCount(),
			new Limit_14_PlayedAction(),
			new Limit_15_CheckMissionAvailable(),
			new Limit_17_DistanceWithPlayer(),
			new Limit_18_IsPlayer(),
            new Limit_19_NpcIsControlled(),
		};

        LimitMethods.InitLimitMethods(LimitExec);
        LimitExec.Clear();
        LimitExec = null;
    }
}
