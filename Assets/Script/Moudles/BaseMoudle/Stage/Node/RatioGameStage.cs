//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : RatioGameStage
//
// Created by : Baoxue at 2016/1/6 13:04:57
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RatioGameStage : StageBase
{
    public RatioGameStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        RatioGame.RatioGameManager.Instance.GameSatrt();
        EventReporter.Instance.EnterSceneReport("Ratio game scene ");
    }

    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("Ratio game scene ");

    }
}