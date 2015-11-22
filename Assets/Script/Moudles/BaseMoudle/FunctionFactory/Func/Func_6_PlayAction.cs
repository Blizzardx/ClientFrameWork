//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : Func_6_PlayAction
//
// Created by : Baoxue at 2015/11/19 16:20:55
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_6_PlayAction : FuncMethodsBase
{
    public Func_6_PlayAction(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_PLAY_ACTION,funcdata.ParamIntList[0]));
        return EFuncRet.Continue;
    }
}