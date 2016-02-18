using UnityEngine;
using System.Collections;

public class Flight_ObjectForward : MonoBehaviour {

	public Flight_CombatFlight combatFlight;
	public Flight_CombatFlightGuide combatFlightGuide;
	public Transform target;
	public float speed;
	public float loopPositionEventZ;
	public Vector3 loopPositionAdd;

	void Awake()
	{
		if (target == null)
			target = transform;
	}

	void FixedUpdate() 
	{
		if (transform.localPosition.z <= loopPositionEventZ)
		{
			transform.localPosition = target.localPosition + Vector3.forward * speed;
		}else
		{
			if (combatFlight != null)
				transform.localPosition = combatFlight.terrainCameraPos-loopPositionAdd;
			if (combatFlightGuide != null)
				transform.localPosition = combatFlightGuide.terrainCameraPos-loopPositionAdd;
		}
	}
}
