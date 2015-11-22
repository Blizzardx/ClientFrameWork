//========================================================================
// Copyright(C): CYTX
//
// FileName : ActionFrameFactory
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/9 19:04:16
//
// Purpose : ActionFrame工厂
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
public class ActionFrameFactory
{
    public static AbstractActionFrame CreateActionFrame(ActionPlayer action, ActionFrameData data)
    {
        AbstractActionFrame actionFrame = null;

        if (null == data)
        {
            return actionFrame;
        }

        switch ((EActionFrameType)data.Type)
        {
            case EActionFrameType.SetCamera:
                actionFrame = new SetCameraFrame(action, data);
                break;
            case EActionFrameType.MoveCamera:
                actionFrame = new MoveCameraFrame(action, data);
                break;
            case EActionFrameType.PlayAudio:
                actionFrame = new PlayAudioFrame(action, data);
                break;
            case EActionFrameType.AddNpc:
                actionFrame = new AddNpcFrame(action, data);
                break;
        }

        return actionFrame;
    }
}

