using BehaviourTree;
using Config;
using UnityEngine;
using System.Collections;

public class Npc : IStateMachineBehaviour, ITransformBehaviour, IAIBehaviour
{
    protected CharTransformData m_CharTransformData;
    protected StateMachine      m_StateMation;
    protected AIAgent           m_AIAgent;
    protected NpcConfig         m_NpcBaseInfo;
    protected int               m_iId;

    public int GetInstanceId()
    {
        return m_iId;
    }
    public AIAgent GetAIAgent()
    {
        return m_AIAgent;
    }
    public TransformDataBase GetTransformData()
    {
        return m_CharTransformData;
    }
    public StateMachine GetStateController()
    {
        return m_StateMation;
    }
    public void Initialize(int id)
    {
        m_iId = id;
        m_NpcBaseInfo = ConfigManager.Instance.GetNpcConfig(id);
        m_StateMation = new StateMachine(0, 0, this);
        m_CharTransformData = new CharTransformData();
        m_CharTransformData.Initialize(this, m_NpcBaseInfo.ModelResource, AssetType.Char);
        m_AIAgent = new AIAgent(m_NpcBaseInfo.AiId);

        m_AIAgent.Active(true, this);
        LifeTickTask.Instance.RegisterToUpdateList(Update);

        LifeManager.RegisterLife(m_iId, this);
    }
    public void Distructor()
    {
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        LifeManager.UnRegisterLife(m_iId, this);
    }
    private void Update()
    {
        m_AIAgent.OnTick();
        m_CharTransformData.Update();
    }
}
