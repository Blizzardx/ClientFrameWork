using UnityEngine;
using System.Collections;

public class Done_EnemyController : MonoBehaviour
{
	public Done_Boundary boundary;
	public float tilt;
	public float dodge;
	public float smoothing;
	public Vector2 startWait;
	public Vector2 maneuverTime;
	public Vector2 maneuverWait;

	private float currentSpeed;
	private float targetManeuver;

	public bool isFlicker = false;

	public static Done_EnemyController instance;
	private Done_GameController gameController;
	public float flickerTime;
	float lastFlickTime;
	public int life;
	public GameObject shipObject;
	bool flicking = false;

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
			gameController.ShowEnemyLife(life);
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
		currentSpeed = GetComponent<Rigidbody>().velocity.z;
//		StartCoroutine(Evade());
	}

	public void Flicker()
	{
		isFlicker = true;
		lastFlickTime = flickerTime;
		transform.localPosition = new Vector3(0,3,20);
		life--;
		gameController.ShowEnemyLife(life);
		if(life <=0)
		{
			if(gameController != null)
			{
				gameController.Victory();
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

	
	IEnumerator Evade ()
	{
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));
		while (true)
		{
			targetManeuver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
			yield return new WaitForSeconds (Random.Range (maneuverTime.x, maneuverTime.y));
			targetManeuver = 0;
			yield return new WaitForSeconds (Random.Range (maneuverWait.x, maneuverWait.y));
		}
	}
	
	void FixedUpdate ()
	{
		float newManeuver = Mathf.MoveTowards (GetComponent<Rigidbody>().velocity.x, targetManeuver, smoothing * Time.deltaTime);
		GetComponent<Rigidbody>().velocity = new Vector3 (newManeuver, 0, 0);
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			3f, 
			20f
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0, 0, GetComponent<Rigidbody>().velocity.x * -tilt);

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
