//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Copenhagen03Stage
//
// Created by : Baoxue at 2015/12/29 14:27:08
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Copenhagen03Stage : StageBase
{
    public Copenhagen03Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        Debuger.Log("start logic");

        Copenhagen03Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("Copenhagen03Scene");
    }

    public override void StartStage()
    {

        Copenhagen03Logic.Instance.StartLogic();
    }
    public override void EndStage()
    {
        Copenhagen03Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("Copenhagen03Scene");
    }
}

