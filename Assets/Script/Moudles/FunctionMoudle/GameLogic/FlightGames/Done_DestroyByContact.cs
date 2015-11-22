using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour
{
	private Done_GameController gameController;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <Done_GameController>();
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

		if(other.tag.Equals("Player"))
		{
			if(this.tag.Equals("Enemy"))
			{
				if(Done_EnemyController.instance != null)
				{
					if(Done_EnemyController.instance.isFlicker.Equals(false))
					{
						Done_EnemyController.instance.Flicker();
					}
				}
				if(Done_PlayerController.instance != null)
				{
					if(Done_PlayerController.instance.isFlicker.Equals(false))
					{
						Done_PlayerController.instance.ResetPos();
					}
				}
			}
			if(this.tag.Equals("Boundary"))
			{
				if(Done_PlayerController.instance != null)
				{
					if(Done_PlayerController.instance.isFlicker.Equals(false))
					{
						Done_PlayerController.instance.Flicker();
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