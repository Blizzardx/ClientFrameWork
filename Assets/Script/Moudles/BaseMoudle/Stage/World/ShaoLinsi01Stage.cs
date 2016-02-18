//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ShaoLinsi01Stage
//
// Created by : Baoxue at 2015/12/29 15:40:52
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShaoLinsi01Stage : StageBase
{
    public ShaoLinsi01Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        Debuger.Log("start logic");

        ShaoLinsi01Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("ShaoLinsi01Scene");
    }

    public override void StartStage()
    {
        ShaoLinsi01Logic.Instance.StartLogic();
    }
    public override void EndStage()
    {
        ShaoLinsi01Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("ShaoLinsi01Scene");
    }
}


