using UnityEngine;
using System.Collections;

public class StateMove : IState 
{
    public StateMove(Ilife unit, ELifeState state)
        : base(unit, state)
    {
    }

    public override bool CanEnter()
    {
        return true;
    }

    public override void DoEnter(object param)
    {
        CharTransformData data = (((ITransformBehaviour)(unit)).GetTransformData()) as CharTransformData;
        //data.DirectPlayAnimation("Walk");
    }

    public override bool CanExit()
    {
        return true;
    }

    public override void DoExit()
    {
        CharTransformData data = (((ITransformBehaviour)(unit)).GetTransformData()) as CharTransformData;
        data.StopMove();
    }
}

