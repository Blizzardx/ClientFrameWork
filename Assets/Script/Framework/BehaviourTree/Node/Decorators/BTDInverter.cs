using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 时间间隔装饰节点.
	/// </summary>
	public class BTDInverter : BTDecorator
	{
		private float	m_fInverter = 0.1f;
		private float	m_fEndTime;

		//
		public BTDInverter( int iInverter )
		{
			m_fInverter = iInverter * 0.001f;
		}

		//
	    public override EBTState Tick()
	    {
	        EBTState res;
	        do
	        {
	            if (Time.time >= m_fEndTime)
	            {
	                OnEnd();
	                res = (null != m_ChildrenLst && m_ChildrenLst.Count > 0 && m_ChildrenLst[0] != null)
	                    ? m_ChildrenLst[0].Tick()
	                    : EBTState.False;
	                break;
	            }

	            res = EBTState.False;
	        } while (false);

	        //mark status
	        CurrentStatus = res;

	        return res;
	    }
	    //
		public override void OnEnd ()
		{
			m_fEndTime = Time.time + m_fInverter;
		}
	}
}