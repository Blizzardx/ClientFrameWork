using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_16_UnlockStage : FuncMethodsBase
{
    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        try
        {
            int id = funcdata.ParamIntList[0];

            if (id > 9 && id < 15)
            {
                PlayerManager.Instance.GetCharCounterData().SetFlag(id, true);
            }
            else
            {
                Debuger.LogWarning("error on unlock stage");
            }
        }
        catch (Exception)
        {
            Debuger.LogWarning("error on unlock stage ");
        }
        return EFuncRet.Continue;
    }
}