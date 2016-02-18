

using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
	/// <summary>
	/// 组合节点基类.
	/// </summary>
	public abstract class BTComposites : BTNode
	{
	    protected List<EBTState> m_LastRunningStatus = new List<EBTState>();
	    protected List<EBTState> m_CurrentRunningStatus = new List<EBTState>();

	    protected void CheckStatus()
	    {
	        for (int i = 0; i < m_CurrentRunningStatus.Count; ++i)
	        {
                if (m_CurrentRunningStatus[i] == EBTState.UnReach && m_LastRunningStatus[i] == EBTState.Running)
	            {
	                m_ChildrenLst[i].OnEnd();
	            }
                m_LastRunningStatus[i] = m_CurrentRunningStatus[i];
	        }

	    }
	    protected void SetChildElementStatus(EBTState status,int index)
	    {
	        if (index >= m_ChildrenLst.Count || index < 0)
	        {
	            return;
	        }
	        if (index >= m_CurrentRunningStatus.Count)
	        {
	            while (m_CurrentRunningStatus.Count == index + 1)
	            {
	                m_CurrentRunningStatus.Add(EBTState.UnReach);
	            }
                while (m_LastRunningStatus.Count == index + 1)
                {
                    m_LastRunningStatus.Add(EBTState.UnReach);
                }
	        }
	    }
	    public override void AddChild(BTNode node)
	    {
	        base.AddChild(node);
            m_LastRunningStatus.Add(EBTState.UnReach);
            m_CurrentRunningStatus.Add(EBTState.UnReach);
	    }
	}
}