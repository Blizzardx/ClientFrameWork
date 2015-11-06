using UnityEngine;
using System.Collections;

public class StateDefine
{
    static public void RegisterState ()
    {
        StateMachine.RegisterState(ELifeState.Idle, typeof(StateIdle));
        StateMachine.RegisterState(ELifeState.Run, typeof(StateRun));
        StateMachine.RegisterState(ELifeState.Walk, typeof(StateWalk));
    }
	
}
