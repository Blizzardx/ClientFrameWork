using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 条件节点
	/// </summary>
    public class BTCondition : BTNode
	{
        private int             m_iLimitId;

        public BTCondition(int iLimitId)
		{
			m_iLimitId = iLimitId;
		}

	    public override EBTState Tick()
	    {
	        EBTState res;
	        do
	        {
	            Ilife Owner = m_Database.GetData<Ilife>(EDataBaseKey.Owner);
	            if (null == Owner)
	            {
	                res = EBTState.False;
	                break;
	            }
	            HandleTarget handleTarget = HandleTarget.GetHandleTarget(Owner);
	            if (LimitMethods.HandleLimitExec(handleTarget, m_iLimitId, null))
	            {
	                res = EBTState.True;
	                break;
	            }
	            res = EBTState.False;
	        } while (false);

	        //mark current status
	        CurrentStatus = res;

	        return res;
	    }
	    public override void OnEnd()
	    {
	    }
	}
}