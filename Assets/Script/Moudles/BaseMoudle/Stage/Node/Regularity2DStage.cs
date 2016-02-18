//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Regularity2DStage
//
// Created by : Baoxue at 2015/12/23 15:58:46
//
//
//========================================================================

using RegularityGame;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Regularity2DStage : StageBase
{
    public Regularity2DStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        Regularity2DGameLogic.Instance.Initialize();
    }

    public override void EndStage()
    {

    }
}

