//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : Func_3_Bit32
//
// Created by : Baoxue at 2015/11/19 17:32:15
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_3_Bit32 : FuncMethodsBase
{
    public Func_3_Bit32(int id)
        : base(id)
    {
    }
    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        List<Ilife> target = Target.GetTarget((EFuncTarget)(funcdata.Target));
        for (int i = 0; i < target.Count; ++i)
        {
            Ilife elem = target[i];
            if (!(elem is PlayerCharacter))
            {
                continue;
            }
            int newValue = 0;
            OperationFunc.FuncOperatorValue((EFuncOperator)(funcdata.Oper), ref newValue, funcdata.ParamIntList[1]);
            PlayerManager.Instance.GetCharCounterData().SetBit32Count(funcdata.ParamIntList[0], newValue);
        }
        return EFuncRet.Continue;
    }
}