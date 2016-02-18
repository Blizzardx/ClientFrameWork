//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ShaoLinsi03Logic
//
// Created by : Baoxue at 2015/12/29 14:59:18
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ShaoLinsi03Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 10;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.ShaoLinsi03State;
    }
    private static ShaoLinsi03Logic m_Instance;
    public static ShaoLinsi03Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new ShaoLinsi03Logic();
            }
            return m_Instance;
        }
    }
}





