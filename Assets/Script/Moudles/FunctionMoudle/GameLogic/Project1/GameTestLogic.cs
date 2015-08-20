using UnityEngine;
using System.Collections;

public class GameTestLogic: LogicBase<GameTestLogic>
{
	
	public override void StartLogic()
	{		
		// load map
		GameObject origin = GameObject.Instantiate(MapManager.Instance.LoadMap(MapId.TestProject1Map)) as GameObject;	
		ComponentTool.Attach(WindowManager.Instance.UIWindowCameraRoot.transform,origin.transform);
		AudioManager.Instance.PlayBackgroundSound(AudioId.LogIn);
	}
	public override void EndLogic()
	{
		
	}
}
