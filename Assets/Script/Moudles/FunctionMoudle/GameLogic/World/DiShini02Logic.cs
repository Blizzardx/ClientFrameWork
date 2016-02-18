//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : DiShini02Logic
//
// Created by : Baoxue at 2016/1/5 13:36:03
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DiShini02Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 5;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.DiShini02Stage;
    }
    private static DiShini02Logic m_Instance;
    public static DiShini02Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new DiShini02Logic();
            }
            return m_Instance;
        }
    }
}

