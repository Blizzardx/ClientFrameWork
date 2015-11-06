using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 条件节点基类.
	/// </summary>
	public class BTCondition : BTNode
	{
		private int		m_iTargetId;
		private int		m_iLimitId;

		public BTCondition( int iTargetId, int iLimitId )
		{
			m_iTargetId = iTargetId;
			m_iLimitId = iLimitId;
		}

		//
		public override EBTState Tick ()
		{
			if( Check() )
			{
				return EBTState.SUCCESS;
			}
			else
			{
				return EBTState.FAILD;
			}
		}
		
		//
		public override void OnEnd ()
		{
			
		}

		//
		public bool Check()
		{
            Ilife Owner = m_Database.GetData<Ilife>(EDataBaseKey.Owner);
			if( null == Owner )
			{
				return false;
			}
			////
			/// 不确定因素//
			/// 
			HandleTarget handleTarget = HandleTarget.GetHandleTarget(Owner );
			return LimitMethods.HandleLimitExec (handleTarget, m_iLimitId);
//			VLimitFuncContext context = VLimitFuncContext.Create ();
//			VHandleTarget handleTarget = null;
//			if(m_iTargetId == 0)
//			{
//				handleTarget = new VHandleTarget(Owner);
//				handleTarget.AddTargetUnit(Owner);
//			}
//			else
//			{
//				handleTarget = VLimitFuncManager.Instance.GetHandleTarget( Owner, m_iTargetId, context );
//			}
//			return VLimitFuncManager.Instance.CheckLimit( handleTarget, m_iLimitId, context ) == 0;

		}
	}
}