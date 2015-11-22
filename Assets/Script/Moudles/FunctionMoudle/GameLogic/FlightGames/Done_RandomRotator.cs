using UnityEngine;
using System.Collections;

public class Done_RandomRotator : MonoBehaviour 
{
	public float tumble;
	public float speed;
	
	void Start ()
	{
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
}