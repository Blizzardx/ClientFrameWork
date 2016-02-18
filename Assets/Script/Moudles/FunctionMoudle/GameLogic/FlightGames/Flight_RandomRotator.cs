using UnityEngine;
using System.Collections;

public class Flight_RandomRotator : MonoBehaviour 
{
	public float tumble;
	public float speed;
	
	void Start()
	{
//		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
//		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}

	void OnEnable()
	{
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
}