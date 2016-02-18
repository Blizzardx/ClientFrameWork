//========================================================================
// Copyright(C): CYTX
//
// FileName : Func_13_SetDictateGame
// 
// Created by : LeoLi at 2016/1/25 17:07:22
//
// Purpose : 
//========================================================================

using Common.Auto;
using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class Func_13_SetDictateGame: FuncMethodsBase
{
    public Func_13_SetDictateGame(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        try
        {
           bool newValue = funcdata.ParamIntList[0] != 0;
           GestureManager.Instance.SetDictation(newValue);
        }
        catch (Exception)
        {
            Debuger.LogWarning("error on Change Dictate Game Status by function ");
        }
        return EFuncRet.Continue;
    }
}

