using UnityEngine;
using System.Collections;

public class Flight_DestroyByContactGuide : MonoBehaviour {

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Grenade")
		{
			if(this.tag.Equals("Enemy"))
			{
				if(other.transform.parent != null)
					Destroy(other.transform.parent.gameObject);
				else
					Destroy(other.gameObject);
				if(Flight_EnemyControllerGuide.instance != null)
				{
					Flight_EnemyControllerGuide.instance.Flicker();
				}
			}
		}

	}
}
