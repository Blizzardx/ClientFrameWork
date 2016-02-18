using UnityEngine;
using System.Collections;

public class Flight_CombatFlightControllerGuide : MonoBehaviour {

	public GameObject playerObject;
	public float speed;
	public float tilt;
	public Flight_Boundary boundary;
	Vector3 movement = Vector3.zero;
	Animator[] anim;
	public float rollSpeed =10f;
	public GameObject grenadeObject;
	public float grenadeScaleTime;
	public Vector3 grenadeScale;
	public Vector3 grenadeMeshScale;
	public Flight_CombatFlightGuide combatFlightGuide;
	public Flight_GameControllerGuide gameControllerGuide;

	public bool moveing = false;
	float moveHorizontal= 0f;

	public static Flight_CombatFlightControllerGuide instance;

	[HideInInspector]
	public bool isGuide = false;

	void OnEnable()
	{
		instance = this;
		if(grenadeObject == null)
			grenadeObject = (GameObject)Resources.Load("BuildIn/Item/Flight/Flight_Grenade");
		moveing = false;
	}

	public void StopMoving()
	{
		moveing = false;
		movement = Vector3.zero;
		moveHorizontal = 0;
		GetComponent<Rigidbody>().Sleep();
		playerObject.transform.localRotation = Quaternion.identity;
		if(Flight_AudioManager.Instance != null)
		{
			Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu34,null,true);
		}
	}

	public void StartMoving()
	{
		moveing = true;
		movement = Vector3.zero;
		moveHorizontal = 0;
		GetComponent<Rigidbody>().WakeUp();
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

	public void SetDefaultPosition()
	{
		transform.localRotation = Quaternion.identity;
		if(playerObject != null)
		{
			playerObject.transform.localPosition = combatFlightGuide.roleMeshPositon;
			playerObject.transform.localEulerAngles = combatFlightGuide.roleMeshPositon;
		}
	}

	public void ResetDefault()
	{
		transform.localPosition = Vector3.zero;
		playerObject.SetActive(true);
		HideWeapon();
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().position = Vector3.zero;
	}

	public void Fire()
	{
		if (grenadeObject != null) 
		{
			if(Flight_AudioManager.Instance != null)
				Flight_AudioManager.Instance.StopAudio(AudioStep.Yindaoyu36);
			GameObject go = (GameObject)Instantiate (grenadeObject);
			go.transform.position = transform.position + Vector3.forward * 2;
			go.transform.localScale = Vector3.one;
			if (go.transform.GetChild (0)) {
				go.transform.GetChild (0).localScale = grenadeMeshScale;
			}
			Flight_Grenade grenade = go.transform.GetComponent<Flight_Grenade> ();
			grenade.Origin = transform.position;
			if(Flight_EnemyController.instance != null)
			{
				if(Flight_EnemyController.instance.enabled)
				{
					grenade.target = Flight_EnemyController.instance.transform.position;
				}
			}
			if(Flight_EnemyControllerGuide.instance.enabled)
			{
				grenade.target = Flight_EnemyControllerGuide.instance.transform.position;
			}
			TweenScale tScale = TweenScale.Begin (go, grenadeScaleTime, grenadeScale);
			tScale.style = UITweener.Style.Once;

			if(Flight_GuideManager.Instance != null)
			{
				Flight_GuideManager.Instance.uiGuide.CloseAll();
			}
		}
	}

	public void ShowWeapon()
	{
		if(Flight_UIManager.Instance != null)
			Flight_UIManager.Instance.ShowGrenade();
	}

	public void HideWeapon()
	{
		if(Flight_UIManager.Instance != null)
			Flight_UIManager.Instance.HideGrenade();
	}

	void FixedUpdate ()
	{
		if(moveing.Equals(false)) return;
		if(isGuide) return;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{	
			if(Flight_StageController.userControl.Equals(false)) return;
			moveHorizontal = 0f;
			if(Flight_StageController.vAxisValue.Equals(1f))
			{
				moveHorizontal = 0;
			}else{
				if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.CombatFlightStep4))
				{
					if(Flight_StageController.hAxisValue <=-0.2f)
					{
						moveHorizontal = float.Parse(Flight_StageController.hAxisValue.ToString("F1"));
					}else{
						moveHorizontal = 0f;
					}
				}
				if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.CombatFlightStep5))
				{
					if(Flight_StageController.hAxisValue >= 0.2f)
					{
						moveHorizontal = float.Parse(Flight_StageController.hAxisValue.ToString("F1"));
					}else{
						moveHorizontal = 0;
					}
				}
			}
			movement = new Vector3 (moveHorizontal, 0, 0);
		} else {
			if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.CombatFlightStep4))
			{
				if(Input.GetAxis ("Horizontal") <0)
				{
					moveHorizontal = Input.GetAxis ("Horizontal");
					movement = new Vector3 (moveHorizontal, 0, 0);
				}
			}
			if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.CombatFlightStep5))
			{
				if(Input.GetAxis ("Horizontal") >0)
				{
					moveHorizontal = Input.GetAxis ("Horizontal");
					movement = new Vector3 (moveHorizontal, 0, 0);
				}
			}
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
	}
}
