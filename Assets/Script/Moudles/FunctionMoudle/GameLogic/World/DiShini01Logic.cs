//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : DiShini01Logic
//
// Created by : Baoxue at 2016/1/5 13:35:30
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DiShini01Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 4;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.DiShini01Stage;
    }
    private static DiShini01Logic m_Instance;
    public static DiShini01Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new DiShini01Logic();
            }
            return m_Instance;
        }
    }
}