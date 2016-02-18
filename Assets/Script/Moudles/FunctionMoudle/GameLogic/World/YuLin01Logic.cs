//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : YuLin01Logic
//
// Created by : Baoxue at 2015/12/29 14:57:12
//
//
//========================================================================

using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class YuLin01Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 7;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.YuLin01Stage;
    }
    private static YuLin01Logic m_Instance;
    public static YuLin01Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new YuLin01Logic();
            }
            return m_Instance;
        }
    }
}





