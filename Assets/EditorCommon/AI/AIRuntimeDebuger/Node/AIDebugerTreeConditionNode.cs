using UnityEngine;

public class AIDebugerTreeConditionNode : AIDebugerTreeNode
{
    public int m_iLimitId;
    public AIDebugerTreeConditionNode(string name, GameObject template,int limitId) : base(name, template)
    {
        m_iLimitId = limitId;
    }
}