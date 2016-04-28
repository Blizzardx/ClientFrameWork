using System.Collections.Generic;
using Config;
using UnityEngine;
using System.Collections;

public class FuncMethodDef
{
    static List<FuncMethodsBase> FuncExec;
    static public void InitFuncMethod()
    {
        //Func Array
        FuncExec = new List<FuncMethodsBase>
		{
	        new Func_1_Flag(),
	        new Func_2_Bit8(),
	        new Func_3_Bit32(),
	        new Func_5_AcceptMission(),
	        new Func_6_PlayAction(),
	        new Func_7_MissionCount(),
	        new Func_8_MissionStepCount(),
	        new Func_9_PlayBackgroundMusic(),
	        new Func_10_PlayEffect(),
	        new Func_11_CreateNpc(),
	        new Func_12_DestroyNpc(),
            new Func_13_SetDictateGame(),
            new Func_14_ChangeToNodeGame(),
            new Func_15_ChangeToWorldGame(),
            new Func_16_UnlockStage(),
        };

        FuncMethods.InitFuncMethods(FuncExec);

        FuncExec.Clear();
        FuncExec = null;
    }
}
