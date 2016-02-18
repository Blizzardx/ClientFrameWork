using System.Collections.Generic;
using Config;
using UnityEngine;
using System.Collections;

public class FuncMethodDef
{
    static Dictionary<int, FuncMethodsBase> FuncExec;
    static public void InitFuncMethod()
    {
        //Func Array
        FuncExec = new Dictionary<int, FuncMethodsBase>
		{
	        {1,new Func_1_Flag(1)},
	        {2,new Func_2_Bit8(2)},
	        {3,new Func_3_Bit32(3)},
	        {5,new Func_5_AcceptMission(5)},
	        {6,new Func_6_PlayAction(6)},
	        {7,new Func_7_MissionCount(7)},
	        {8,new Func_8_MissionStepCount(8)},
	        {9,new Func_9_PlayBackgroundMusic(9)},
	        {10,new Func_10_PlayEffect(10)},
	        {11,new Func_11_CreateNpc(11)},
	        {12,new Func_12_DestroyNpc(12)},
            {13,new Func_13_SetDictateGame(13)},
            {14,new Func_14_ChangeToNodeGame(14)},
            {15,new Func_15_ChangeToWorldGame(15)},
            {16,new Func_16_UnlockStage(16)},
        };

        FuncMethods.InitFuncMethods(FuncExec);
    }
}
