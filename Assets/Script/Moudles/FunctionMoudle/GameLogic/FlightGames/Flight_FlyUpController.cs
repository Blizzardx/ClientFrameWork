using UnityEngine;
using System.Collections;

public class Flight_FlyUpController : MonoBehaviour {
	
	[HideInInspector]
	public float vOriginAcceleration;
//	public GameObject accelerationEffect;

	Vector3 movement = Vector3.zero;
	float moveVertical = 0f;
	[SerializeField]
	private float speed;
	[HideInInspector]
	public bool startUp = false;
	public float acceleration = 0f;

	public Flight_Camera fCamera;
	public Flight_Camera fTerrainCamera;

	public float upTimer = 8f;
	float timer;

	public Flight_ObjectUp objectUp;
	Animator[] anim;
	public GameObject roleObject;
	public Flight_FlyUp flyUp;

	public GameObject entourage1,entourage2,spirit;

	[HideInInspector]
	public bool isGuide = false;

	public void SetDefault()
	{
		transform.localPosition = Vector3.zero;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().position = Vector3.zero;
		acceleration = 0f;
		timer = Time.time;
		anim = transform.GetComponentsInChildren<Animator>();
		if(anim != null)
		{
			if(anim.Length >0)
			{
				for(int i=0;i<anim.Length;i++)
				{
					anim[i].SetBool("Fly",false);
				}
			}
		}
		moveVertical = 0f;
		if (roleObject != null)
		{
			roleObject.transform.localPosition = Vector3.zero;
			roleObject.transform.localEulerAngles = Vector3.zero;
			if(flyUp != null)
				roleObject.transform.GetChild(0).localEulerAngles = flyUp.roleMeshRot;
		}

//		if (accelerationEffect != null)
//			accelerationEffect.SetActive (false);
	}

	void FixedUpdate()
	{
		if(isGuide) return;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Flight_StageController.userControl.Equals(false))
			{
				SetDefault();
				return;
			}
			if(Flight_StageController.vAxisValue < vOriginAcceleration)
			{
				if(Mathf.Abs(Flight_StageController.vAxisValue - vOriginAcceleration) > 0.4f)
				{
					if(anim != null)
					{
						if(anim.Length >0)
						{
							for(int i=0;i<anim.Length;i++)
							{
								anim[i].SetBool("Fly",true);
							}
						}
					}
					moveVertical =1f;
				}
			}
		}else{
			if(Input.GetKeyDown(KeyCode.W))
			{
				if(anim != null)
				{
					if(anim.Length >0)
					{
						for(int i=0;i<anim.Length;i++)
						{
							anim[i].SetBool("Fly",true);
						}
					}
				}
				moveVertical = 1f;
			}
		}

		if (moveVertical > 0) 
		{
			if(Flight_GuideManager.Instance != null)
			{
				Flight_GuideManager.Instance.CloseGuide();
			}
			startUp = true;
		} else {
			startUp = false;
		}

		if(moveVertical.Equals(1f))
		{
			if(Time.time - timer > upTimer)
			{
				if(objectUp != null)
				{
					objectUp.SetEvent();
					acceleration = 0f;
					this.enabled = false;
					return;
				}
			}
		}else{
			timer = Time.time;
		}

		movement = new Vector3 (0, moveVertical, 0);
		GetComponent<Rigidbody>().velocity = movement;
		if (moveVertical.Equals (0f)) 
		{
			if(acceleration >0f)
			{
				if(acceleration <=0f)
				{
					acceleration = 0f;
				}
				GetComponent<Rigidbody> ().position = new Vector3
				(
					GetComponent<Rigidbody>().position.x, 
					Mathf.Clamp (acceleration, acceleration-=0.1f,1),
					GetComponent<Rigidbody>().position.z
				);
			}
			if(acceleration <0f)
			{
//				if(accelerationEffect.activeSelf)
//				{
//					accelerationEffect.SetActive(false);
//				}
			}
		} else {
			if(!acceleration.Equals(1f))
			{
				acceleration = GetComponent<Rigidbody>().position.y;
			}
			if(acceleration >=1f)
			{
				acceleration = 1f;
				if(!Flight_StageController.isGuide)
				{
					if(Flight_AudioManager.Instance != null)
					{
						Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu27);
					}
				}
			}
			GetComponent<Rigidbody>().position = new Vector3
			(
				GetComponent<Rigidbody>().position.x,
				Mathf.Clamp (acceleration,0, 1f),
				GetComponent<Rigidbody>().position.z
			);
		}
	}
}
