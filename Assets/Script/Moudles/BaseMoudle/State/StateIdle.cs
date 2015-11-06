using UnityEngine;
using System.Collections;

public class StateIdle : IState
{
    public StateIdle(Ilife unit, ELifeState state) : base(unit, state)
    {
    }

    public override bool CanEnter()
    {
        return true;
    }

    public override void DoEnter()
    {
        ((ITransformBehaviour) (unit)).GetTransformData().PlayAnimation("Idle");
    }

    public override bool CanExit()
    {
        return true;
    }

    public override void DoExit()
    {
    }
}
