using UnityEngine;
using System.Collections;

public class Done_DestroyByBoundary : MonoBehaviour
{
	void OnTriggerExit (Collider other) 
	{
		if(other.transform.parent != null)
			Destroy(other.transform.parent.gameObject);
	}
}