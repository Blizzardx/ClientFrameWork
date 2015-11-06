

using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
	/// <summary>
	/// 选择节点.
	/// </summary>
	public class Selector : BTComposites
	{
		private int					m_iCurrentChildIndex = 0;
		private EBTState			m_eExecState = EBTState.INACTIVE;

		//
		public override EBTState Tick ()
		{
			m_eExecState = m_ChildrenLst[m_iCurrentChildIndex].Tick();
			if( m_eExecState == EBTState.FAILD )
			{
				for( ++m_iCurrentChildIndex; m_iCurrentChildIndex < m_ChildrenLst.Count; ++m_iCurrentChildIndex )
				{
					m_eExecState = m_ChildrenLst[m_iCurrentChildIndex].Tick();
					if( m_eExecState != EBTState.FAILD )
					{
						break;
					}
				}
			}

			if( m_iCurrentChildIndex + 1 >= m_ChildrenLst.Count )
			{
				OnEnd();
			}

			return m_eExecState;
		}
		
		//
		public override void OnEnd ()
		{
			m_iCurrentChildIndex = 0;
			m_eExecState = EBTState.INACTIVE;
		}
	}
}