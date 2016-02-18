//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ShaoLinsi01Logic
//
// Created by : Baoxue at 2015/12/29 14:56:15
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShaoLinsi01Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 8;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.ShaoLinsi01State;
    }
    private static ShaoLinsi01Logic m_Instance;
    public static ShaoLinsi01Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new ShaoLinsi01Logic();
            }
            return m_Instance;
        }
    }
}
