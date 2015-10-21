using UnityEngine;
using System.Collections;
public enum ELifeState
{
    None = 0,
    Idle,
    Move,
    Attack,
    Attacked,
    Death,
    Disapper,

    Max,
}
public abstract class IState 
{
    protected Ilife unit;

    public IState(Ilife unit)
	{
		this.unit = unit;
	}

	public abstract ELifeState GetState();
	public abstract bool CanEnter();
	public abstract void DoEnter();
	public abstract bool CanExit();
    public abstract void DoExit();
    public virtual void DoAction()
    {

    }
}
