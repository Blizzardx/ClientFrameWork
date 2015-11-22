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

    public override void DoEnter(object param)
    {
        CharTransformData data = (((ITransformBehaviour)(unit)).GetTransformData()) as CharTransformData;
        data.PlayAnimation("Idle");
    }

    public override bool CanExit()
    {
        return true;
    }

    public override void DoExit()
    {
    }
}
