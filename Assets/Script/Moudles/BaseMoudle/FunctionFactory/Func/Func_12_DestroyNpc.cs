//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Func_12_DestroyNpc
//
// Created by : Baoxue at 2016/1/4 13:47:15
//
//
//========================================================================

using Common.Auto;
using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Func_12_DestroyNpc : FuncMethodsBase
{
    public Func_12_DestroyNpc(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        try
        {
            int id = funcdata.ParamIntList[0];
            
            TerrainManager.Instance.DestroyNpcById(id);
        }
        catch (Exception)
        {
            Debuger.LogWarning("error on create npc by function ");
        }
        return EFuncRet.Continue;
    }
}
