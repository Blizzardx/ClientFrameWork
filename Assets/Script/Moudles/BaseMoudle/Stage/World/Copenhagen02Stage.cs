//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Copenhagen02Stage
//
// Created by : Baoxue at 2015/12/29 14:26:46
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Copenhagen02Stage : StageBase
{
    public Copenhagen02Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        Debuger.Log("start logic");

        Copenhagen02Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("Copenhagen02Scene");
    }

    public override void StartStage()
    {
        Copenhagen02Logic.Instance.StartLogic();
    }
    public override void EndStage()
    {
        Copenhagen02Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("Copenhagen02Scene");
    }
}

