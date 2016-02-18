//========================================================================
// Copyright(C): CYTX
//
// FileName : Limit_19_NpcIsControlled
// 
// Created by : LeoLi at 2015/12/24 16:51:18
//
// Purpose : 
//========================================================================
using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Limit_19_NpcIsControlled : LimitMethodsBase
{
    public Limit_19_NpcIsControlled(int id)
        : base(id)
    {
    }
    public override bool LimitExecHandler(HandleTarget Target, LimitData Limit, FuncContext context)
    {
        List<Ilife> target = Target.GetTarget(EFuncTarget.EFT_User);
        if (target == null || target.Count <= 0)
            return false;

        if (!(target[0] is Npc))
            return false;

        Npc npc = (Npc)target[0];
        return npc.IsPlayerControlled;

    }
}

