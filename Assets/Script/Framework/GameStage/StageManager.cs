using UnityEngine;
using System.Collections.Generic;


public class StageManager : Singleton<StageManager>
{
    private StageBase                           m_CurrentStage;
    private Dictionary<GameStateType, string>   m_StageSceneMap;
    private Dictionary<GameStateType, StageBase> m_StageHandlerMap;
 
 
    public void Initialize()
    {
        m_StageSceneMap = new Dictionary<GameStateType, string>();
        m_StageHandlerMap = new Dictionary<GameStateType, StageBase>();

        Definer.RegisterStage();
    }
    public void RegisterStage(GameStateType type, string sceneName)
    {
        m_StageSceneMap.Add(type, sceneName);
    }
    public void ChangeState(GameStateType pState)
    {
        if (SceneManager.Instance.IsSceneLoadiing())
        {
            Debuger.Log("System busy");
            return;
        }
        if (null != m_CurrentStage)
        {
            m_CurrentStage.EndStage();
        }
        m_CurrentStage = null;
        if (!m_StageHandlerMap.TryGetValue(pState, out m_CurrentStage))
        {
            m_CurrentStage = Definer.StageHandlerFactory(pState);
            m_StageHandlerMap.Add(pState, m_CurrentStage);
        }

        //load scene
        SceneManager.Instance.LoadScene(m_StageSceneMap[pState], m_CurrentStage.StartStage);
    }
    public GameStateType GetCurrentGameStage()
    {
        if (null != m_CurrentStage)
        {
            return m_CurrentStage.StageType;
        }
        return GameStateType.none;
    }
}
