

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
		public override EBTState Tick ()
		{
			if( Time.time >= m_fEndTime )
			{
				OnEnd();
				return null != Child ? Child.Tick() : EBTState.FAILD;
			}

			return EBTState.FAILD;
		}

		//
		public override void OnEnd ()
		{
			m_fEndTime = Time.time + m_fInverter;
		}
	}
}