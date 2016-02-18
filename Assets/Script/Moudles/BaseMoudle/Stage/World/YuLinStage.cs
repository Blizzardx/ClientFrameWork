//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : YuLinStage
//
// Created by : Baoxue at 2015/12/29 15:44:42
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class YuLinStage : StageBase
{
    public YuLinStage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        Debuger.Log("start logic");

        YuLin01Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("YuLinScene");
    }

    public override void StartStage()
    {
        YuLin01Logic.Instance.StartLogic();
    }
    public override void EndStage()
    {
        YuLin01Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("YuLinScene");
    }
}