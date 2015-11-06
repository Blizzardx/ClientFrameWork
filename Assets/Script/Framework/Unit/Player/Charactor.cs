using Config;
using UnityEngine;
using System.Collections;

public class Charactor : ICountBehaviour,IStateMachineBehaviour,ITransformBehaviour
{
    protected TransformData     m_TransformData;
    protected StateMachine      m_StateMation;
    protected CountData         m_CountData;
    protected CharactorConfig   m_CharBaseInfo;

    public int GetInstanceId()
    {
        return 0;
    }
    public TransformData GetTransformData()
    {
        return m_TransformData;
    }
    public StateMachine GetStateController()
    {
        return m_StateMation;
    }
    public CountData GetCountData()
    {
        return m_CountData;
    }
    public void Initialize(int resourceId)
    {
        m_CharBaseInfo  = ConfigManager.Instance.GetCharactorConfig(resourceId);
        m_StateMation   = new StateMachine(0, 0, this);
        m_CountData     = new CountData();
        m_TransformData = new TransformData();
        m_TransformData.Initialize("Char/Prefab/"+m_CharBaseInfo.ModelResource,AssetType.Model);
    }
    public void Distructor()
    {
        m_TransformData.Distructor();
        m_StateMation.Distructor();
        //m_CountData.Distructor();
    }
    
}
