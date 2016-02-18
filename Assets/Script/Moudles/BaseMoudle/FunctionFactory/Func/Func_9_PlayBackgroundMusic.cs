//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Func_9_PlayBackgroundMusic
//
// Created by : Baoxue at 2015/12/1 16:33:14
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Func_9_PlayBackgroundMusic : FuncMethodsBase
{
    public Func_9_PlayBackgroundMusic(int id)
        : base(id)
    {
    }

    public override EFuncRet FuncExecHandler(HandleTarget Target, FuncData funcdata, FuncContext context)
    {
        AudioPlayer.Instance.PlayAudio(funcdata.ParamStringList[0], Vector3.zero,false);
        return EFuncRet.Continue;
    }
}
