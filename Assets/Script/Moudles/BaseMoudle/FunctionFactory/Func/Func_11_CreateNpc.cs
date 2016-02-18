//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Func_11_CreateNpc
//
// Created by : Baoxue at 2016/1/4 13:38:18
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

public class Func_11_CreateNpc : FuncMethodsBase
{
    public Func_11_CreateNpc(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        try
        {
            int id = funcdata.ParamIntList[0];

            ThriftVector3 pos = new ThriftVector3();
            pos.X = funcdata.ParamIntList[1];
            pos.Y = funcdata.ParamIntList[2];
            pos.Z = funcdata.ParamIntList[3];

            ThriftVector3 rot = new ThriftVector3();
            rot.X = 0;
            rot.Y = funcdata.ParamIntList[4];
            rot.Z = 0;

            ThriftVector3 scal = new ThriftVector3();
            scal.X = funcdata.ParamIntList[5];
            scal.Y = funcdata.ParamIntList[5];
            scal.Z = funcdata.ParamIntList[5];

            TerrainManager.Instance.CreateNpcById(id, pos, rot, scal);
        }
        catch (Exception)
        {
            Debuger.LogWarning("error on create npc by function ");
        }
        return EFuncRet.Continue;
    }
}
