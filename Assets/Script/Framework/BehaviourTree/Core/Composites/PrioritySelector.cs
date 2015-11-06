

using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 优先级选择节点.
	/// </summary>
	public class PropilySelector : BTComposites
	{
		private EBTState			m_eExecState = EBTState.INACTIVE;

		//
		public override EBTState Tick ()
		{
			foreach( BTNode node in m_ChildrenLst )
			{
				m_eExecState = node.Tick();
				if( m_eExecState != EBTState.FAILD )
				{
					return m_eExecState;
				}
			}
			
			return EBTState.FAILD;
		}
		
		//
		public override void OnEnd ()
		{
			m_eExecState = EBTState.INACTIVE;
		}
	}
}