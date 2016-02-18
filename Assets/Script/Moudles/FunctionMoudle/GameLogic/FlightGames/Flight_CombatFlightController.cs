using UnityEngine;
using System.Collections;

[System.Serializable]
public class Flight_Boundary 
{
	public float xMin, xMax, yMin, yMax, zMin, zMax;
}

public class Flight_CombatFlightController : MonoBehaviour
{
	public float speed;
	public float tilt;
	public Flight_Boundary boundary;

	Vector3 movement = Vector3.zero;

	public static Flight_CombatFlightController instance;

	public float flickerTime;
	float lastFlickTime;
	public bool isFlicker = false;
	bool flicking = false;

	public GameObject playerObject;
	public int life;
	public Flight_GameController gameController;
	
	private float timer;
	public float duration;
	public bool useGrenade = false;

	public GameObject grenadeObject;

	[HideInInspector]
	public float hitCount;
	[HideInInspector]
	public float difficultyHitCount;
	public Flight_CombatFlight combatFlight;

	Animator[] anim;
	public float rollSpeed =10f;
	public GameObject entourage1,entourage2,spirit;

	public float grenadeScaleTime;
	public Vector3 grenadeScale;
	public Vector3 grenadeMeshScale;

	float tempTime;

	public BoxCollider boxCollider;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start ()
	{
		if(grenadeObject == null)
			grenadeObject = (GameObject)Resources.Load("BuildIn/Item/Flight/Flight_Grenade");
		if(combatFlight == null)
			combatFlight = transform.parent.GetComponent<Flight_CombatFlight>();
		if(boxCollider == null)
			boxCollider = transform.GetComponent<BoxCollider>();
	}

	public void SetAnimator()
	{
		anim = transform.GetComponentsInChildren<Animator>();
		if(anim != null)
		{
			if(anim.Length >0)
			{
				for(int i=0;i<anim.Length;i++)
				{
					anim[i].SetBool("Fly",false);
					anim[i].SetBool("Glide",true);
				}
			}
		}
	}

	public void ResetDefault()
	{
		transform.localPosition = Vector3.zero;
		hitCount = 0;
		difficultyHitCount = 0;
		playerObject.SetActive(true);
		useGrenade = false;
		HideWeapon();
		isFlicker = false;
		lastFlickTime = 0;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().position = Vector3.zero;
		tempTime = 0;
		SetCollider(true);
	}

	public void HideWeapon()
	{
		timer = Time.time;
		useGrenade = false;
		if(Flight_UIManager.Instance != null)
			Flight_UIManager.Instance.HideGrenade();
	}

	public void Fire()
	{
		if(grenadeObject != null)
		{
			GameObject go = (GameObject)Instantiate(grenadeObject);
			go.transform.position = transform.position+Vector3.forward*2;
			go.transform.localScale = Vector3.one;
			if(go.transform.GetChild(0))
			{
				go.transform.GetChild(0).localScale = grenadeMeshScale;
			}
			Flight_Grenade grenade = go.transform.GetComponent<Flight_Grenade>();
			grenade.Origin = transform.position;
			grenade.target = Flight_EnemyController.instance.transform.position;
			TweenScale tScale = TweenScale.Begin(go,grenadeScaleTime,grenadeScale);
			tScale.style = UITweener.Style.Once;
		}
		if (Time.time - tempTime > Flight_GameController.waveWait) {
			SetSpawnWaves();
		} else {
			CancelInvoke("SetSpawnWaves");
			Invoke("SetSpawnWaves",Time.time-tempTime);
		}
	}

	void SetSpawnWaves()
	{
		gameController.StartSpawnWaves ();
	}

	public void SetDefaultPosition()
	{
		transform.localRotation = Quaternion.identity;
		if(playerObject != null)
		{
			playerObject.transform.localPosition = combatFlight.roleMeshPositon;
			playerObject.transform.localEulerAngles = combatFlight.roleMeshPositon;
			if(playerObject.transform.GetChild(0))
			{
				playerObject.transform.GetChild(0).localPosition = Vector3.zero;
				playerObject.transform.GetChild(0).localEulerAngles = combatFlight.roleMeshRot;
			}
			if(entourage1.transform.childCount >0)
			{
				entourage1.transform.localEulerAngles = combatFlight.roleMeshRot;
				if(entourage1.transform.GetChild(0))
				{
					if(entourage1.transform.GetChild(0).GetChild(0))
					{
						entourage1.transform.GetChild(0).GetChild(0).localPosition = Vector3.zero;
						entourage1.transform.GetChild(0).localEulerAngles = combatFlight.roleMeshRot;
					}
				}
			}

			if(entourage2.transform.childCount >0)
			{
				entourage2.transform.localEulerAngles = combatFlight.roleMeshRot;
				if(entourage2.transform.GetChild(0))
				{
					if(entourage2.transform.GetChild(0).GetChild(0))
					{
						entourage2.transform.GetChild(0).GetChild(0).localPosition = Vector3.zero;
						entourage2.transform.GetChild(0).localEulerAngles = combatFlight.roleMeshRot;
					}
				}
			}

			if(spirit.transform.childCount >0)
			{
				spirit.transform.localEulerAngles = combatFlight.roleMeshRot;
				if(spirit.transform.GetChild(0))
				{
					if(spirit.transform.GetChild(0).GetChild(0))
					{
						spirit.transform.GetChild(0).GetChild(0).localPosition = Vector3.zero;
						spirit.transform.GetChild(0).localEulerAngles = combatFlight.roleMeshRot;
					}
				}
			}
		}
	}

	public void Flicker()
	{
		lastFlickTime = flickerTime;
		transform.localPosition = Vector3.forward*-2;
		SetDefaultPosition();
		isFlicker = true;
		life--;
		gameController.ShowPlayerLife(life);
		if (!PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
		{
			if(Flight_AudioManager.Instance != null)
			{
				Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu35);
			}
		}

//		HideWeapon();
		if(life <=0)
		{
			if(gameController != null)
			{
				gameController.GameOver();
			}
			DestoryShip();
		}
		if(combatFlight != null)
		{
			if(combatFlight.destroyByBoundary != null)
			{
				combatFlight.destroyByBoundary.dodgeCount = 0;
				combatFlight.destroyByBoundary.difficultyDodgeCount = 0;
				hitCount++;
				difficultyHitCount++;
				//1111111111
				if(AdaptiveDifficultyManager.Instance != null)
				{
					if(hitCount>=2)
					{
						AdaptiveDifficultyManager.Instance.SetUserTalent("HitObs2",40);
					}else{
						AdaptiveDifficultyManager.Instance.SetUserTalent("HitObs",40);
					}
					if(difficultyHitCount>=1)
					{
						GameDifficulty result = AdaptiveDifficultyManager.Instance.GetGameDifficulty("ObsFreq",40);
						if(Flight_StageController.Instance != null)
						{
							Flight_StageController.Instance.SetDifficultyFrequency(result);
						}
						difficultyHitCount =0;
					}
				}
			}
		}
	}

	void DestoryShip()
	{
		if (playerObject != null)
			playerObject.gameObject.SetActive (false);
	}

	public void SetCollider(bool open)
	{
		if (boxCollider != null)
			boxCollider.enabled = open;
	}
	
	void FixedUpdate ()
	{
		if (isFlicker && life >0) 
		{
			if(playerObject != null)
				playerObject.SetActive(flicking);
			flicking = !flicking;
			lastFlickTime -= 0.02f;
			if(lastFlickTime <=0)
			{
				isFlicker = false;
				if(playerObject != null)
				{
					playerObject.SetActive(true);
					anim = transform.GetComponentsInChildren<Animator>();
					if(anim != null)
					{
						if(anim.Length >0)
						{
							for(int i=0;i<anim.Length;i++)
							{
								anim[i].SetBool("Fly",false);
								anim[i].SetBool("Glide",true);
							}
						}
					}
					SetDefaultPosition();
				}
			}
		}
		if(!useGrenade)
		{
			if(Time.time-timer > duration && (!gameController.gameOver))
			{
				useGrenade = true;
				if(Flight_UIManager.Instance != null)
					Flight_UIManager.Instance.ShowGrenade();
				tempTime = Time.time;
			}
		}

		if(isFlicker) return;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{	
			if(Flight_StageController.userControl.Equals(false)) return;
			float moveHorizontal = 0f;
			if(Flight_StageController.vAxisValue.Equals(1f))
			{
				moveHorizontal = 0;
			}else{
				if(Flight_StageController.hAxisValue >0.2f || Flight_StageController.hAxisValue <-0.2f)
					moveHorizontal = float.Parse(Flight_StageController.hAxisValue.ToString("F1"));
			}
			movement = new Vector3 (moveHorizontal, 0, 0);
		} else {
			float moveHorizontal = Input.GetAxis ("Horizontal");
			movement = new Vector3 (moveHorizontal, 0, 0);
		}

		GetComponent<Rigidbody>().velocity = movement * speed;
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 		
			0f,
			0f
		);
		playerObject.transform.localRotation = Quaternion.Lerp
		(
			Quaternion.Euler(playerObject.transform.localEulerAngles),
			Quaternion.Euler(new Vector3(playerObject.transform.localEulerAngles.x,playerObject.transform.localEulerAngles.y,GetComponent<Rigidbody>().velocity.x * -tilt)),
			Time.deltaTime*rollSpeed
		);
		if(entourage1 != null)
		{
			entourage1.transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(entourage1.transform.localEulerAngles),
				Quaternion.Euler(new Vector3(entourage1.transform.localEulerAngles.x,entourage1.transform.localEulerAngles.y,GetComponent<Rigidbody>().velocity.x * -tilt)),
				Time.deltaTime*rollSpeed
			);
		}
		if(entourage2 != null)
		{
			entourage2.transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(entourage2.transform.localEulerAngles),
				Quaternion.Euler(new Vector3(entourage2.transform.localEulerAngles.x,entourage2.transform.localEulerAngles.y,GetComponent<Rigidbody>().velocity.x * -tilt)),
				Time.deltaTime*rollSpeed
			);
		}
		if(spirit != null)
		{
			spirit.transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(spirit.transform.localEulerAngles),
				Quaternion.Euler(new Vector3(spirit.transform.localEulerAngles.x,spirit.transform.localEulerAngles.y,GetComponent<Rigidbody>().velocity.x * -tilt)),
				Time.deltaTime*rollSpeed
			);
		}
	}
}
