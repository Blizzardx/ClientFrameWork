//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : DaBaoJiao01Logic
//
// Created by : Baoxue at 2015/12/29 14:47:04
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DaBaoJiao01Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 6;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.DaBaoJiao01Stage;
    }
    private static DaBaoJiao01Logic m_Instance;
    public static DaBaoJiao01Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new DaBaoJiao01Logic();
            }
            return m_Instance;
        }
    }
}


