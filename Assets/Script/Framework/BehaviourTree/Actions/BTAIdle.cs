

using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 
	/// </summary>
	public class BTAIdle : BTAction
	{
		//
		protected override bool Enter ()
		{
			return true;
		}

		protected override bool Waiting()
		{
			Debug.LogError ("BTAAttack  ------- >>>>>  Waiting");
//			if(m_Owner.ActionState.GetState() == ELifeState.Attack)
//			{
//				return true;
//			}else
//			{
//				return false;
//			}
			return true;
		}
		//
		protected override EBTState Execute ()
        {
            Ilife owner = m_Database.GetData<Ilife>(EDataBaseKey.Owner);
			if( null == owner )
			{
				return EBTState.FAILD;
			}

		    IStateMachineBehaviour state = (IStateMachineBehaviour) (owner);
            if (!state.GetStateController().TryEnterState(ELifeState.Idle,false))
			{
				return EBTState.RUNNING;
			}

			return EBTState.SUCCESS;
		}
		
		//
		protected override void Exit ()
		{
			
		}
	}
}