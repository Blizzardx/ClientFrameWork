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
            case EActionFrameType.ShakeCamera:
                actionFrame = new ShakeCameraFrame(action, data);
                break;
            case EActionFrameType.MoveCamera:
                actionFrame = new MoveCameraFrame(action, data);
                break;
            case EActionFrameType.PlayAudio:
                actionFrame = new PlayAudioFrame(action, data);
                break;
            case EActionFrameType.StopAudio:
                actionFrame = new StopAudioFrame(action, data);
                break;
            case EActionFrameType.AddNpc:
                actionFrame = new AddNpcFrame(action, data);
                break;
            case EActionFrameType.MoveObject:
                actionFrame = new MoveTransformFrame(action, data);
                break;
            case EActionFrameType.EnableObject:
                actionFrame = new EnableObjFrame(action, data);
                break;
            case EActionFrameType.EnableMeshRender:
                actionFrame = new EnableMeshRenderFrame(action, data);
                break;
            case EActionFrameType.ChangeColor:
                actionFrame = new ChangeColorFrame(action, data);
                break;
            case EActionFrameType.Runtime_CreateEffect:
                actionFrame = new Runtime_CreateEffectFrame(action, data);
                break;
            case EActionFrameType.Runtime_MoveEffect:
                actionFrame = new Runtime_MoveEffectFrame(action, data);
                break;
            case EActionFrameType.AddStateEffect:
                actionFrame = new AddStateEffectFrame(action, data);
                break;
            case EActionFrameType.Runtime_RemoveEffect:
                actionFrame = new Runtime_RemoveEffectFrame(action, data);
                break;
            case EActionFrameType.Runtime_AddUI:
                actionFrame = new AddUIFrame(action, data);
                break;
            case EActionFrameType.Runtime_RemoveUI:
                actionFrame = new RemoveUIFrame(action, data);
                break;
            case EActionFrameType.MoveChar:
                actionFrame = new MoveCharFrame(action, data);
                break;
            case EActionFrameType.EntityPlayAnimation:
                actionFrame = new EntityPlayAnimFrame(action, data);
                break;
            case EActionFrameType.AnimChar:
                actionFrame = new AnimCharFrame(action, data);
                break;
            case EActionFrameType.RotateChar:
                actionFrame = new RotateCharFrame(action, data);
                break;
            case EActionFrameType.RotateCamera:
                actionFrame = new RotateCameraFrame(action, data);
                break;
            case EActionFrameType.ObjTransform:
                actionFrame = new ObjectTransformFrame(action, data);
                break;
            case EActionFrameType.FuncMethod:
                actionFrame = new FuncMethodFrame(action, data);
                break;
        }
        return actionFrame;
    }
}

