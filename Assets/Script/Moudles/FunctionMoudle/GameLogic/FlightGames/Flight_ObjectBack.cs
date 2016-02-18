using UnityEngine;
using System.Collections;

public class Flight_ObjectBack : MonoBehaviour {

	public Transform target;
	public float speed;
	
	void Awake()
	{
		if (target == null)
			target = transform;
	}
	
	void FixedUpdate() 
	{
		if (transform.localPosition.z >= -700)
			transform.localPosition = target.localPosition - Vector3.forward * speed;
		else
			transform.localPosition = Vector3.zero;
	}
}
