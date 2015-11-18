//========================================================================
// Copyright(C): CYTX
//
// FileName : ActionTickTask
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/12 17:26:35
//
// Purpose : Action Tick 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ActionTickTask : AbstractTickTask
{
    protected override bool FirstRunExecute()
    {
        return true;
    }
    protected override int GetTickTime()
    {
        return TickTaskConstant.TICK_TIME_CORRECT;
    }
    protected override void Beat()
    {
        ActionManager.Instance.Update();
    }
}

