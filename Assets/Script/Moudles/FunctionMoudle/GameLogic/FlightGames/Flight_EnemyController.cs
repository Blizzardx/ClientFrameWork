using UnityEngine;
using System.Collections;

public class Flight_EnemyController : MonoBehaviour
{
	public bool isFlicker = false;

	public static Flight_EnemyController instance;
	private Flight_GameController gameController;
	public float flickerTime;
	float lastFlickTime;
	public int life;
	public GameObject meshObject;
	bool flicking = false;
	
	public Vector3 shakeAmount;
	public Vector3 originPos;
	public Animator animator;
	public GameObject originExplodeObject;
	GameObject explodeObject;

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
			gameController = gameControllerObject.GetComponent <Flight_GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	public void SetThrowAnimator(bool play)
	{
		if(animator != null)
			animator.SetBool("Throw",play);
	}

	public void SetFireAnimator()
	{
		if(animator != null)
			animator.SetBool("Fire",true);
	}

	public void SetExplodeAnimator()
	{
		animator.Play("beizha");
	}

	public void Flicker()
	{
		SetExplodeAnimator();
		isFlicker = true;
		lastFlickTime = flickerTime;
		transform.localPosition = originPos;
		life--;
		gameController.ShowEnemyLife(life);
		if (life <= 0) 
		{
			if (gameController != null)
			{
				gameController.Victory ();
			}
			HideMesh();
		} else {
			ShakeMesh(1);
		}
		SetExplode();
	}

	void SetExplode()
	{
		SetExplodeAnimator();
		CreateExplodeObject();
		CancelInvoke ("DestoryExplodeObject");
		Invoke("DestoryExplodeObject",1f);
	}

	void CreateExplodeObject()
	{
		if(originExplodeObject == null) return;

		explodeObject = (GameObject)Instantiate(originExplodeObject);
		explodeObject.transform.localPosition =transform.localPosition;
	}

	void DestoryExplodeObject()
	{
		if(explodeObject != null)
		{
			GameObject.Destroy(explodeObject);
			explodeObject = null;
		}
	}

	public void SetDefault()
	{
		isFlicker = false;
		Hashtable hash = new Hashtable();
		hash.Add("time", 0);
		hash.Add("amount", shakeAmount);
		hash.Add("islocal", true);
		iTween.ShakePosition(meshObject.gameObject, hash);
	}

	void ShakeMesh(float fTime)
	{
		Hashtable hash = new Hashtable();
		hash.Add("time", fTime);
		hash.Add("amount", shakeAmount);
		hash.Add("islocal", true);
		iTween.ShakePosition(meshObject.gameObject, hash);
	}

	void HideMesh()
	{
		if (meshObject != null)
			meshObject.gameObject.SetActive (false);
	}
}
