using UnityEngine;
using System.Collections;

public class StateRun : IState
{
    public StateRun(Ilife unit, ELifeState state)
        : base(unit, state)
    {
    }

    public override bool CanEnter()
    {
        return true;
    }

    public override void DoEnter()
    {
        ((ITransformBehaviour) (unit)).GetTransformData().PlayAnimation("Run");
    }

    public override bool CanExit()
    {
        return true;
    }

    public override void DoExit()
    {
    }
}

