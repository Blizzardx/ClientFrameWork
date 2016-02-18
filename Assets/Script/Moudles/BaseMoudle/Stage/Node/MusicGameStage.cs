//========================================================================
// Copyright(C): CYTX
//
// FileName : MusicGameStage
// 
// Created by : LeoLi at 2015/12/30 18:08:19
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicGame;

public class MusicGameStage: StageBase
{
    public MusicGameStage(GameStateType type)
        : base(type)
    {
    }
    public override void StartStage()
    {
        WindowManager.Instance.HideAllWindow();
        MusicGameManager.Instance.Initialize();
        EventReporter.Instance.EnterSceneReport("music game scene ");
    }

    public override void EndStage()
    {
        EventReporter.Instance.ExitSceneReport("music game scene ");

    }
}

