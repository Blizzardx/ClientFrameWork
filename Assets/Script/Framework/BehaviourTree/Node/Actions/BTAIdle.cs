using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
	/// <summary>
	/// 
	/// </summary>
	public class BTAIdle : BTAction
	{
	    protected override EBTState OnEnter()
        {
            Ilife owner = m_Database.GetData<Ilife>(EDataBaseKey.Owner);
            if (null == owner)
            {
                return EBTState.False;
            }

            IStateMachineBehaviour state = (IStateMachineBehaviour)(owner);
            if (!state.GetStateController().TryEnterState(ELifeState.Idle, false))
            {
                return EBTState.False;
            }

            return EBTState.True;
	    }
	    protected override EBTState OnExit()
        {
            return EBTState.True;
	    }
	    protected override EBTState OnRunning()
        {
            return EBTState.True;
	    }
	}
}