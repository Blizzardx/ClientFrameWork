using UnityEngine;
using System.Collections;

public class Flight_GuideBallTriggerEvent : MonoBehaviour {

	public Flight_GameControllerGuide gameControllerGuide;
	public Flight_CombatFlightControllerGuide combatFlightControllerGuide;

	void OnTriggerEnter(Collider other)
	{
		if(gameControllerGuide != null)
			gameControllerGuide.moving = false;
		if(combatFlightControllerGuide != null)
			combatFlightControllerGuide.StartMoving();
		if(gameControllerGuide.right)
		{
			if(Flight_GuideManager.Instance != null)
			{
				Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.CombatFlightStep5);
			}
			gameControllerGuide.right = false;
		}
	}
}
