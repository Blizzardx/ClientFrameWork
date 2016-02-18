using Common.Auto;
using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Func_14_ChangeToNodeGame : FuncMethodsBase
{
    public Func_14_ChangeToNodeGame(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        try
        {
            int id = funcdata.ParamIntList[0];

            MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_NODE_GAME, id));

            if(funcdata.ParamIntList.Count > 2)
            {
                WorldSceneDispatchController.Instance.SetExitFunId(funcdata.ParamIntList[1], funcdata.ParamIntList[2]);
            }
            else if(funcdata.ParamIntList.Count > 1)
            {
                WorldSceneDispatchController.Instance.SetExitFunId(funcdata.ParamIntList[1], 0);
            }
        }
        catch (Exception)
        {
            Debuger.LogWarning("error on Change To NodeGame by function ");
        }
        return EFuncRet.Continue;
    }
}