using UnityEngine;
using System.Collections;

public class GameTestLogic: LogicBase<GameTestLogic>
{
	
	public override void StartLogic()
	{		
		// load map
	    MapManager.Instance.LoadMap(MapId.TestProject1Map);
		AudioManager.Instance.PlayBackgroundSound(AudioId.LogIn);
	}
	public override void EndLogic()
	{
		
	}
}
