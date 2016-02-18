using Config;
using RunnerGame;
using UnityEngine;
using System.Collections;
using NetWork.Auto;
using System;

public class Copenhagen01Logic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 1;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.Copenhagen01State;
    }

    private static Copenhagen01Logic m_Instance;
    public static Copenhagen01Logic Instance
    {
        get
        {
            if(null == m_Instance)
            {
                m_Instance = new Copenhagen01Logic();
            }
            return m_Instance;
        }
    }
}
