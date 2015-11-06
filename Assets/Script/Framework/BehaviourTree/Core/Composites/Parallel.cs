

using UnityEngine;
using System.Collections.Generic;
using BehaviourTree;

namespace BehaviourTree
{
	/// <summary>
	/// 并行节点.
	/// </summary>
	public class Parallel : BTComposites
	{
		private List<EBTState>		m_ExecStateLst = new List<EBTState>();

		//
		public override EBTState Tick ()
		{
			int iEndChildCount = 0;
			for( int iIndex = 0; iIndex < m_ChildrenLst.Count; ++iIndex )
			{
				if( m_ExecStateLst[iIndex] == EBTState.RUNNING )
				{
					m_ExecStateLst[iIndex] = m_ChildrenLst[iIndex].Tick();
				}
				else
				{
					iEndChildCount++;
				}
			}

			if( iEndChildCount == m_ChildrenLst.Count )
			{
				OnEnd();
				return EBTState.SUCCESS;
			}

			return EBTState.RUNNING;
		}

		//
		public override void OnEnd ()
		{
			for( int iIndex = 0; iIndex < m_ExecStateLst.Count; ++iIndex )
			{
				m_ExecStateLst[iIndex] = EBTState.RUNNING;
			}

			foreach( BTNode node in m_ChildrenLst )
			{
				node.OnEnd();
			}
		}

		//
		public override void AddChild (BTNode node)
		{
			base.AddChild( node );
			m_ExecStateLst.Add( EBTState.RUNNING );
		}

		//
		public override void RemoveChild (BTNode node)
		{
			int iIndex = m_ChildrenLst.IndexOf( node );
			base.RemoveChild( node );
			m_ExecStateLst.RemoveAt( iIndex );
		}
	}
}