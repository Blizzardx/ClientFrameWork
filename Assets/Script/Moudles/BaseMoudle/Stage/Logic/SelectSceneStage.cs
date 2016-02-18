//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : SelectSceneStage
//
// Created by : Baoxue at 2016/1/5 12:27:12
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SelectSceneStage : StageBase
{
    public SelectSceneStage(GameStateType type)
        : base(type)
    {
    }

    public override void StartStage()
    {
        WindowManager.Instance.CloseAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.SelectScene);
        EventReporter.Instance.EnterSceneReport("SelectScene");
    }
    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("SelectScene");
    }
}

