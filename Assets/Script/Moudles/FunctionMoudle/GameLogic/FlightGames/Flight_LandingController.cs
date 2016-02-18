using UnityEngine;
using System.Collections;

public class Flight_LandingController : MonoBehaviour {
	
	[HideInInspector]
	public Vector3 originAcceleration;
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

	public float upTimer = 8f;
	float timer;
	public Flight_ObjectLanding objectLanding;
//	public Flight_Camera fCamera;
//	public Flight_Camera fTerrainCamera;
	Animator[] anim;
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
		startUp = false;
		moveVertical = 0f;
//		if(accelerationEffect != null)
//			accelerationEffect.SetActive(false);
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

	void FixedUpdate()
	{
		if(isGuide) return;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Flight_StageController.userControl.Equals(false)) return;
			if(Mathf.Abs(Input.acceleration.y-originAcceleration.y)>0.4f || Input.acceleration.y < -0.8f)
			{
				moveVertical = -1f;
				if(Flight_GuideManager.Instance != null)
				{
					Flight_GuideManager.Instance.CloseGuide();
				}
			}
//			if(Flight_StageController.vAxisValue > vOriginAcceleration)
//			{
//				if(Mathf.Abs(Flight_StageController.vAxisValue - vOriginAcceleration) > 0.4f)
//				{
//					moveVertical = -1f;
//				}
//			}
//			if(Flight_StageController.vAxisValue.Equals(1))
//			{
//				if(Input.acceleration.y >0.4f)
//				{
//					moveVertical = -1f;
//				}
//			}
		}else{
//			moveVertical = Input.GetAxis ("Vertical");
			if(Input.GetKeyDown(KeyCode.W))
			{
				moveVertical = -1;
				if(Flight_GuideManager.Instance != null)
				{
					Flight_GuideManager.Instance.CloseGuide();
				}
			}
		}
		
		if (moveVertical < 0) {
			startUp = true;
//			if (accelerationEffect != null)
//			{
//				if (!accelerationEffect.activeSelf) 
//				{
//					accelerationEffect.SetActive (true);
//				}
//			}
//			if(fCamera.enabled.Equals(false))
//				fCamera.enabled = true;
//			if(fTerrainCamera.enabled.Equals(false))
//				fTerrainCamera.enabled = true;
		} else {
			startUp = false;
//			if(fCamera.enabled.Equals(true))
//				fCamera.enabled = false;
//			if(fTerrainCamera.enabled.Equals(true))
//				fTerrainCamera.enabled = false;
		}

		if( moveVertical.Equals(-1f))
		{
			if(Time.time - timer > upTimer)
			{
				if(objectLanding != null)
				{
					objectLanding.SetEvent();
					objectLanding.enabled = false;
					moveVertical = 0;
				}
			}
		}else{
			timer = Time.time;
		}
		
		movement = new Vector3 (0, moveVertical, 0);
		GetComponent<Rigidbody>().velocity = movement;
		if (moveVertical.Equals (0f)) 
		{
			if(acceleration <0f)
			{
				if(acceleration >0f)
				{
					acceleration = 0f;
				}
				GetComponent<Rigidbody> ().position = new Vector3
				(
					GetComponent<Rigidbody>().position.x, 
					Mathf.Clamp (acceleration+=0.1f,acceleration, 0f),
					GetComponent<Rigidbody>().position.z
				);
			}
			if(acceleration >= 0)
			{
//				if (accelerationEffect.activeSelf) 
//				{
//					accelerationEffect.SetActive (false);
//				}
			}
		} else {
			if(!acceleration.Equals(-1f))
			{
				acceleration = GetComponent<Rigidbody>().position.y;
			}
			if(acceleration <=-1f)
				acceleration = -1f;
			GetComponent<Rigidbody>().position = new Vector3
			(
				GetComponent<Rigidbody>().position.x,
				Mathf.Clamp (acceleration,-1f, 0f),
				GetComponent<Rigidbody>().position.z
			);
		}
	}

}
