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
			{1,new Limit_1_Random(1)},
			{2,new Limit_2_Flag(2)},
			{3,new Limit_3_ByteCount(3)},
			{4,new Limit_4_IntCount(4)},
			{10,new Limit_10_MissionCount(10)},
			{11,new Limit_11_MissionStepCount(11)},
			{14,new Limit_14_PlayedAction(14)},
			{15,new Limit_15_CheckMissionAvailable(15)},
			{17,new Limit_17_DistanceWithPlayer(17)},
			{18,new Limit_18_IsPlayer(18)},
            {19,new Limit_19_NpcIsControlled(19)},
		};

        LimitMethods.InitLimitMethods(LimitExec);
    }
}
