using UnityEngine;
using System.Collections;

public class StateDefine
{
    static public void RegisterState ()
    {
        StateMachine.RegisterState(ELifeState.Idle, typeof(StateIdle));
        StateMachine.RegisterState(ELifeState.Move, typeof(StateMove));
    }
	
}
