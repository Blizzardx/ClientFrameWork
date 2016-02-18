//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : DiShini01Stage
//
// Created by : Baoxue at 2016/1/5 13:39:20
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DiShini01Stage : StageBase
{
    public DiShini01Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        DiShini01Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("DiShini01Scene");
    }

    public override void StartStage()
    {
        DiShini01Logic.Instance.StartLogic();
    }

    public override void EndStage()
    {
        DiShini01Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("DiShini01Scene");
    }
}
