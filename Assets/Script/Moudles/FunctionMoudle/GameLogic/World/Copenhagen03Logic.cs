//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : Copenhagen03Logic
//
// Created by : Baoxue at 2015/12/29 14:28:31
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Copenhagen03Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 3;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.Copenhagen03State;
    }
    private static Copenhagen03Logic m_Instance;
    public static Copenhagen03Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new Copenhagen03Logic();
            }
            return m_Instance;
        }
    }
}

