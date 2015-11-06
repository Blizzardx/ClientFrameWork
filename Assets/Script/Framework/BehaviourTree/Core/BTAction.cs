

using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 行为节点基类.
	/// </summary>
	public abstract class BTAction : BTNode
	{
		protected EBTActionState m_eActionState = EBTActionState.Ready;

		protected abstract bool Enter();

		protected virtual bool Waiting(){ return true;}

		protected abstract EBTState Execute();

		protected abstract void Exit();

//		public virtual void Resert()
//		{
//			m_eActionState = EBTActionState.Ready;
//		}

		public override EBTState Tick ()
		{
			EBTState rlt = EBTState.FAILD;
			if( EBTActionState.Ready == m_eActionState )
			{
				if( !Enter() )
				{
					return rlt;
				}
				m_eActionState = EBTActionState.Waitting;
			}

			if(EBTActionState.Waitting == m_eActionState)
			{
				if(!Waiting())
				{
					return rlt;
				}
				m_eActionState = EBTActionState.Running;
			}

			if( EBTActionState.Running == m_eActionState )
			{
				rlt = Execute();
				if( rlt != EBTState.RUNNING )
				{
					Exit();
					m_eActionState = EBTActionState.Ready;
				}
			}

			return rlt;
		}

		public override void OnEnd ()
		{
			m_eActionState = EBTActionState.Ready;
		}
	}
}