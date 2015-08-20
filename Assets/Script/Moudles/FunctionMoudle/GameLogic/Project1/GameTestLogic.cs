using UnityEngine;
using System.Collections;

public class GameTestLogic: LogicBase<GameTestLogic>
{
	
	public override void StartLogic()
	{
	    WindowManager.Instance.OpenWindow(WindowID.WindowProject1);
	}
	public override void EndLogic()
	{
		
	}
    /// ... to do : 
}
