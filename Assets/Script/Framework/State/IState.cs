using UnityEngine;
using System.Collections;
public enum ELifeState
{
    None = 0,
    Idle,
    Walk,
    Run,
    Attack,
    Attacked,
    Death,
    Disapper,

    Max,
}
public abstract class IState 
{
    protected Ilife unit;
    protected ELifeState state;
    public IState(Ilife unit,ELifeState state)
	{
		this.unit = unit;
        this.state = state;
	}

    public ELifeState GetState()
    {
        return state;
    }
	public abstract bool CanEnter();
	public abstract void DoEnter();
	public abstract bool CanExit();
    public abstract void DoExit();
    public virtual void DoAction()
    {

    }
}
