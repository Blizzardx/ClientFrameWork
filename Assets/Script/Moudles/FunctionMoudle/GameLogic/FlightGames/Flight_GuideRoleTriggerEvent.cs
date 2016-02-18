using UnityEngine;
using System.Collections;

public class Flight_GuideRoleTriggerEvent : MonoBehaviour {

	public Flight_GameControllerGuide gameControllerGuide;
	public Flight_CombatFlightControllerGuide combatFlightControllerGuide;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			if(gameControllerGuide != null)
				gameControllerGuide.moving = true;
			if(combatFlightControllerGuide != null)
				combatFlightControllerGuide.StopMoving();
			if(Flight_GuideManager.Instance != null)
			{
				Flight_GuideManager.Instance.uiGuide.CloseAll();
			}
		}
	}
}
