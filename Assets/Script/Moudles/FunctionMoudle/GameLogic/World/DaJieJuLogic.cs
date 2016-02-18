using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DaJieJuLogic : WorldLogicBase
{
    public override int GetSceneId()
    {
        return 11;
    }

    public override GameStateType GetSceneStageId()
    {
        return GameStateType.DaJieJuStage;
    }
    private static DaJieJuLogic m_Instance;
    public static DaJieJuLogic Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new DaJieJuLogic();
            }
            return m_Instance;
        }
    }
}


