using UnityEngine;
using System.Collections;

public class Flight_CombatBackController : MonoBehaviour {

	public Flight_CombatFlightController combatFlightController;
	public float backSpeed =2;

	void Start()
	{
		if(combatFlightController == null)
			combatFlightController = transform.GetComponent<Flight_CombatFlightController>();
	}

	void FixedUpdate ()
	{
		if (combatFlightController.isFlicker.Equals (false))
			return;

		GetComponent<Rigidbody>().velocity = backSpeed*Vector3.forward;
		GetComponent<Rigidbody>().position = new Vector3
		(
			0f,
			0f,
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, -2, 0)
		);
	}
}
