//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : DrawGameStage
//
// Created by : Baoxue at 2016/1/6 12:29:06
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawGameStage : StageBase
{
    public DrawGameStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        DrawSomething.Instance.GameStart();
        EventReporter.Instance.EnterSceneReport("Draw game scene ");
    }

    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("Draw game scene ");
    }
}
