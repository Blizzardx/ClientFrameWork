using Config;
using RunnerGame;
using UnityEngine;
using System.Collections;
using NetWork.Auto;
using System;

public class Copenhagen02Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 2;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.Copenhagen02State;
    }
    private static Copenhagen02Logic m_Instance;
    public static Copenhagen02Logic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new Copenhagen02Logic();
            }
            return m_Instance;
        }
    }
}
