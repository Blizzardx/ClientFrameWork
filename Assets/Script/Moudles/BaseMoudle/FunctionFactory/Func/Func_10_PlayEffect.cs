//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Func_10_PlayEffect
//
// Created by : Baoxue at 2015/12/1 16:33:33
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Auto;

public class Func_10_PlayEffect : FuncMethodsBase
{
    public Func_10_PlayEffect(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        try
        {
            ThriftVector3 pos = new ThriftVector3();
            pos.X = funcdata.ParamIntList[0];
            pos.Y = funcdata.ParamIntList[1];
            pos.Z = funcdata.ParamIntList[2];

            ThriftVector3 rot = new ThriftVector3();
            rot.X = 0;
            rot.Y = funcdata.ParamIntList[3];
            rot.Z = 0;

            ThriftVector3 scal = new ThriftVector3();
            scal.X = funcdata.ParamIntList[4];
            scal.Y = funcdata.ParamIntList[4];
            scal.Z = funcdata.ParamIntList[4];
            GameObject elem = EffectContainer.EffectFactory(funcdata.ParamStringList[0]);
            elem.transform.position = pos.GetVector3();
            elem.transform.eulerAngles = rot.GetVector3();
            elem.transform.localScale = scal.GetVector3();
        }
        catch
        {

        }
        
        return EFuncRet.Continue;
    }
}