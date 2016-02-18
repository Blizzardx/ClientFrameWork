//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ShaoLinsi03Stage
//
// Created by : Baoxue at 2015/12/29 15:42:17
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShaoLinsi03Stage : StageBase
{
    public ShaoLinsi03Stage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        Debuger.Log("start logic");

        ShaoLinsi03Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("ShaoLinsi03Scene");
    }

    public override void StartStage()
    {
        ShaoLinsi03Logic.Instance.StartLogic();
    }
    public override void EndStage()
    {
        ShaoLinsi03Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("ShaoLinsi03Scene");
    }
}



