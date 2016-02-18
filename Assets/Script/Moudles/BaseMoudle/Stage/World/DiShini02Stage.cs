//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : DiShini02Stage
//
// Created by : Baoxue at 2016/1/5 13:40:21
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class DiShini02Stage : StageBase
{
    public DiShini02Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        DiShini02Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("DiShini02Scene");
    }

    public override void StartStage()
    {
        DiShini02Logic.Instance.StartLogic();
    }

    public override void EndStage()
    {
        DiShini02Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("DiShini02Scene");
    }
}

