using UnityEngine;
using System.Collections;

public class Flight_DestroyByBoundary : MonoBehaviour
{
	[HideInInspector]
	public float dodgeCount=0;
	[HideInInspector]
	public float difficultyDodgeCount=0;
	void OnTriggerExit (Collider other) 
	{
		if(other.tag == "Player") return;
		if(other.tag == "Grenade") return;

		if(other.transform.parent != null)
		{
			Destroy(other.transform.parent.gameObject);
			//1111111111
			if(Flight_CombatFlightController.instance != null)
			{
				if(Flight_CombatFlightController.instance.isFlicker.Equals(false))
				{
					Flight_CombatFlightController.instance.hitCount=0;
					Flight_CombatFlightController.instance.difficultyHitCount = 0;
					dodgeCount++;
					if(dodgeCount.Equals(1))
					{
						if(AdaptiveDifficultyManager.Instance != null)
						{
							AdaptiveDifficultyManager.Instance.SetUserTalent("OverObs",40);
						}
					}
					if(dodgeCount >=2)
					{
						if(AdaptiveDifficultyManager.Instance != null)
						{
							AdaptiveDifficultyManager.Instance.SetUserTalent("OverObs2",40);
						}
					}
					difficultyDodgeCount++;
					if(difficultyDodgeCount >=2)
					{
						if(AdaptiveDifficultyManager.Instance != null)
						{
							GameDifficulty result = AdaptiveDifficultyManager.Instance.GetGameDifficulty("ObsFreq",40);
							if(Flight_StageController.Instance != null)
							{
								Flight_StageController.Instance.SetDifficultyFrequency(result);
							}
						}
						difficultyDodgeCount=0;
					}
				}
			}
		}
	}
}