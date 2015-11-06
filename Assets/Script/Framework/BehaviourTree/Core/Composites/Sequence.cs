

using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 顺序节点.
	/// </summary>
	public class Sequence : BTComposites
	{
		private int					m_iCurrentChildIndex = 0;
		private EBTState			m_eExecState = EBTState.INACTIVE;
		
		//
		public override EBTState Tick ()
		{
			while( true )
			{
				m_eExecState = m_ChildrenLst[m_iCurrentChildIndex].Tick();
				if( EBTState.SUCCESS != m_eExecState )
				{
					return m_eExecState;
				}

				if( ++m_iCurrentChildIndex >= m_ChildrenLst.Count )
				{
					OnEnd();
					return m_eExecState;
				}
			}
		}
		
		//
		public override void OnEnd ()
		{
			m_iCurrentChildIndex = 0;
			m_eExecState = EBTState.INACTIVE;

			foreach( BTNode node in m_ChildrenLst )
			{
				node.OnEnd();
			}
		}
	}
}