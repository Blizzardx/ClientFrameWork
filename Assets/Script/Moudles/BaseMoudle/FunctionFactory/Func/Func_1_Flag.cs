//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : Func_1_Flag
//
// Created by : Baoxue at 2015/11/19 17:23:13
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_1_Flag : FuncMethodsBase
{
    public Func_1_Flag(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget) (funcdata.Target));
        for (int i = 0; i < target.Count; ++i)
        {
            Ilife elem = target[i];
            if (!(elem is PlayerCharacter))
            {
                continue;
            }
            PlayerCharacter playerChar = elem as PlayerCharacter;
            bool newValue = funcdata.ParamIntList[1] != 0;
            playerChar.GetCharCounterData().SetFlag(funcdata.ParamIntList[0],newValue);
        }
        return EFuncRet.Continue;
    }
}
