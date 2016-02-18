//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ShaoLinsi02Stage
//
// Created by : Baoxue at 2015/12/29 15:41:50
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShaoLinsi02Stage : StageBase
{
    public ShaoLinsi02Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        ShaoLinsi02Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("ShaoLinsi02Scene");
    }

    public override void StartStage()
    {
        Debuger.Log("start logic");
        ShaoLinsi02Logic.Instance.StartLogic();

    }
    public override void EndStage()
    {
        ShaoLinsi02Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("ShaoLinsi02Scene");
    }
}


