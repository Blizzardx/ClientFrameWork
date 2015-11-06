

using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 装饰节点基类.
	/// </summary>
	public abstract class BTDecorator : BTNode
	{
		protected BTNode Child
		{
			get
			{
				if( m_ChildrenLst.Count > 0 )
				{
					return m_ChildrenLst[0];
				}

				return null;
			}
		}
	}
}