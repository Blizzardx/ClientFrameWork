//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : DaBaoJiaoStage
//
// Created by : Baoxue at 2015/12/29 15:43:09
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DaBaoJiaoStage : StageBase
{
    public DaBaoJiaoStage(GameStateType type)
        : base(type)
    {
    }

    public override void InitStage()
    {
        base.InitStage();
        DaBaoJiao01Logic.Instance.InitLogic();
        EventReporter.Instance.EnterSceneReport("DaBaoJiaoScene");
    }

    public override void StartStage()
    {
        DaBaoJiao01Logic.Instance.StartLogic();
    }

    public override void EndStage()
    {
        DaBaoJiao01Logic.Instance.EndLogic();
        EventReporter.Instance.ExitSceneReport("DaBaoJiaoScene");
    }
}