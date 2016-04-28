using Common.Auto;
using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Func_15_ChangeToWorldGame : FuncMethodsBase
{
    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        try
        {
            int id = funcdata.ParamIntList[0];

            MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, id));
        }
        catch (Exception)
        {
            Debuger.LogWarning("error on Change To WorldGame by function ");
        }
        return EFuncRet.Continue;
    }
}