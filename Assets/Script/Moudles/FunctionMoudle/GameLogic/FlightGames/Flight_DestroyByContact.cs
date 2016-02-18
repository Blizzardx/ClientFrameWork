using UnityEngine;
using System.Collections;

public class Flight_DestroyByContact : MonoBehaviour
{
	private Flight_GameController gameController;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <Flight_GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag == "Enemy")
		{
			return;
		}

		if (other.tag == "Grenade")
		{
			if(this.tag.Equals("Boundary"))
			{
				return;
			}
			if(this.tag.Equals("Enemy"))
			{
				if(other.transform.parent != null)
					Destroy(other.transform.parent.gameObject);
				else
					Destroy(other.gameObject);
				if(Flight_EnemyController.instance != null)
				{
					Flight_EnemyController.instance.Flicker();
				}
				if(Flight_CombatFlightController.instance != null)
				{
					Flight_CombatFlightController.instance.useGrenade = false;
				}
			}
		}

		if(other.tag.Equals("Player"))
		{
			if(this.tag.Equals("Boundary"))
			{
				if(Flight_CombatFlightController.instance != null)
				{
					if(Flight_CombatFlightController.instance.isFlicker.Equals(false))
					{
						Flight_CombatFlightController.instance.Flicker();
						Destroy (gameObject);
					}
				}
			}
		}
	}

	public void DestoryBySelf()
	{
		Destroy (gameObject);
	}
}