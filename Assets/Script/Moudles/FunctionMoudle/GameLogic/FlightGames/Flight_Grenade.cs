using UnityEngine;
using System.Collections;

public class Flight_Grenade : MonoBehaviour {

	public Vector3 Origin,target;
	public float speed;

	void FixedUpdate()
	{
		if(Origin != null && target != null)
			transform.position = Vector3.Lerp(transform.position,target,Time.deltaTime*speed);
	}
}
