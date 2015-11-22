using UnityEngine;
using System.Collections;

[System.Serializable]
public class Done_Boundary 
{
	public float xMin, xMax, yMin, yMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour
{
	public float speed;
	public float tilt;
	public Done_Boundary boundary;

	float x=0,z=0;
	Vector3 movement = Vector3.zero;

	public static Done_PlayerController instance;

	public float flickerTime;
	float lastFlickTime;
	public bool isFlicker = false;
	bool flicking = false;

	public GameObject shipObject;
	public int life;
	private Done_GameController gameController;
	float acceleration = 0f;

	public float waitControlTime;
	bool canControl = true;

	public GameObject buttonObject;
    float moveVertical = 0f;
	bool startAcceleration = false;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <Done_GameController>();
			gameController.ShowPlayerLife(life);
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
		acceleration = boundary.zMax;
		if (buttonObject != null)
			UIEventListener.Get (buttonObject).onPress = OnPress;
	}

	void OnPress (GameObject go, bool isPressed)
	{
		if (isPressed) 
		{
			startAcceleration = true;
			moveVertical = 1f;
		} else {
			startAcceleration = false;
			moveVertical = 0f;
		}
	}
	
	public void ResetPos()
	{
		transform.localPosition = Vector3.zero;
	}

	public void Flicker()
	{
		isFlicker = true;
		lastFlickTime = flickerTime;
		transform.localPosition = Vector3.zero;
		life--;
		gameController.ShowPlayerLife(life);
		if(life <=0)
		{
			if(gameController != null)
			{
				gameController.GameOver();
				gameController.Land();
			}
			DestoryShip();
		}
	}

	void DestoryShip()
	{
		if (shipObject != null)
			shipObject.gameObject.SetActive (false);
	}

	void CanControl()
	{
		canControl = true;
	}

	public void SetGameEnd()
	{
		buttonObject.SetActive (false);
		startAcceleration = false;
		moveVertical = 0f;
	}

	void FixedUpdate ()
	{
		if(!canControl) return;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Input.acceleration.x <-0.1f || Input.acceleration.x > 0.1f)
				x = Input.acceleration.x;
			movement = new Vector3 (x, 0, 0);
		} else {
			float moveHorizontal = Input.GetAxis ("Horizontal");
			movement = new Vector3(moveHorizontal,0.0f,moveVertical);
		}

		GetComponent<Rigidbody>().velocity = movement * speed;

		if (moveVertical.Equals (0f)) 
		{
			if(acceleration<=0)
			{
				acceleration = 0;
				GetComponent<Rigidbody>().position = new Vector3
				(
					Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 		
					0.0f,
					Mathf.Clamp (0, 0, 0)
				);
			}else{
				GetComponent<Rigidbody>().position = new Vector3
				(
					Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 		
					0.0f,
					Mathf.Clamp (GetComponent<Rigidbody>().position.z, 0, acceleration-=0.5f)
				);
			}
		}else{
			if(!acceleration.Equals(boundary.zMax))
			{
				acceleration = GetComponent<Rigidbody>().position.z;
			}
			GetComponent<Rigidbody>().position = new Vector3
			(
				Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 		
				0.0f,
				Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
			);
		}

		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);

		if (isFlicker && life >0) 
		{
			if(shipObject != null)
				shipObject.SetActive(flicking);
			flicking = !flicking;
			lastFlickTime -= 0.02f;
			if(lastFlickTime <=0)
			{
				isFlicker = false;
				if(shipObject != null)
					shipObject.SetActive(true);
			}
		}
	}
}
