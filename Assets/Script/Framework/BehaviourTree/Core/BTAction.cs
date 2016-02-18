

using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 行为节点基类.
	/// </summary>
	public abstract class BTAction : BTNode
	{
	    protected EBTState m_CurrentStatus;

	    public override EBTState Tick()
	    {
            if (m_CurrentStatus != EBTState.Running)
            {
                m_CurrentStatus = OnEnter();
            }
	        if (m_CurrentStatus == EBTState.Running)
	        {
	            m_CurrentStatus = OnRunning();
	        }
	        if (m_CurrentStatus != EBTState.Running)
	        {
                m_CurrentStatus = OnExit();
	        }

            //mark current status
	        CurrentStatus = m_CurrentStatus;

	        return m_CurrentStatus;
	    }

        protected abstract EBTState OnEnter();
        protected abstract EBTState OnExit();
        protected abstract EBTState OnRunning();

        public override void OnEnd()
        {
            m_CurrentStatus = OnExit();
            foreach (var elem in m_ChildrenLst)
            {
                elem.OnEnd();
            }
        }
	}
}