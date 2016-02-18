using UnityEngine;
using System.Collections;

public class FireworkGuideStage : StageBase {

	public FireworkGuideStage(GameStateType type)
		: base(type)
	{
	}

	public override void StartStage()
	{
		WindowManager.Instance.HideAllWindow();
		WindowManager.Instance.OpenWindow(WindowID.FireworkGuide);
		FireworkGameLogicGuide.Instance.Initialize();
		EventReporter.Instance.EnterSceneReport("Firework game scene ");
	}
	
	public override void EndStage()
	{
		EventReporter.Instance.ExitSceneReport("Firework game scene ");
	}
}
