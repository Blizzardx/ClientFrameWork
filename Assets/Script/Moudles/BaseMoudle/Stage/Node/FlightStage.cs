using UnityEngine;
using System.Collections;

public class FlightStage : StageBase {

	public FlightStage(GameStateType type): base(type)
	{
		
	}

	public override void StartStage()
    {
        Flight_StageManager.Instance.Initialize();
        EventReporter.Instance.EnterSceneReport("Flight game scene ");
    }

	public override void EndStage()
    {
        if (Flight_StageController.Instance.gameController != null)
		{
			if(Flight_StageController.Instance.gameController.uiManager != null)
			{
				GameObject.Destroy(Flight_StageController.Instance.gameController.uiManager.gameObject);
			}
		}
		if(Flight_StageController.Instance != null)
		{
			GameObject.Destroy(Flight_StageController.Instance.gameObject);
		}
        EventReporter.Instance.ExitSceneReport("Flight game scene ");
	}
}
