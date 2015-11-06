using UnityEngine;
using System.Collections;

public class Npc:ICountBehaviour,IStateMachineBehaviour,ITransformBehaviour
{
    protected TransformData m_TransformData;
    protected StateMachine m_StateMation;
    protected CountData m_CountData;

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
}
