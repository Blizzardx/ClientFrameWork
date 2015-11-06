using UnityEngine;
using System.Collections;

public class StateWalk : IState 
{
    public StateWalk(Ilife unit, ELifeState state)
        : base(unit, state)
    {
    }

    public override bool CanEnter()
    {
        return true;
    }

    public override void DoEnter()
    {
        ((ITransformBehaviour) (unit)).GetTransformData().PlayAnimation("Walk");
    }

    public override bool CanExit()
    {
        return true;
    }

    public override void DoExit()
    {
    }
}

