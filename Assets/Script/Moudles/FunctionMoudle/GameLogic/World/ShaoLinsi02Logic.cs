//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ShaoLinsi02Logic
//
// Created by : Baoxue at 2015/12/29 14:59:08
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShaoLinsi02Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 9;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.ShaoLinsi02State;
    }
    private static ShaoLinsi02Logic m_Instance;
    public static ShaoLinsi02Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new ShaoLinsi02Logic();
            }
            return m_Instance;
        }
    }
}



