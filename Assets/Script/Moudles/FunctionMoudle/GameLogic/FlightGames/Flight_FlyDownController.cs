using UnityEngine;
using System.Collections;
public class Flight_FlyDownController : MonoBehaviour {

	[HideInInspector]
	public Vector3 originAcceleration;
	[HideInInspector]
	public float vOriginAcceleration;
	public float moveVertical = 0f;

	Vector3 movement = Vector3.zero;
	public float speed;
	public float acceleration = 0f;

	public GameObject accelerationEffect;
	public Flight_Camera fCamera;
	public Flight_Camera fTerrainCamera;
	public ParticleSystem cloudRise;

	public bool startDown = false;
	public float downSpeed = 0;
//	[HideInInspector]
	private float aDownSpeed;
	Animator[] anim;

	public GameObject entourage1,entourage2,spirit;

	public bool isGuide = false;

	public void SetDefault()
	{
		moveVertical = 0;
		if(fCamera.enabled.Equals(true))
			fCamera.enabled = false;
		if(fTerrainCamera.enabled.Equals(true))
			fTerrainCamera.enabled = false;
		if(accelerationEffect.activeSelf)
			accelerationEffect.SetActive(false);
		movement = new Vector3 (0, moveVertical, 0);
		startDown = false;
		downSpeed = 0.005f;
		aDownSpeed = 0f;
		cloudRise.playbackSpeed = downSpeed;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().position = Vector3.zero;
		acceleration = 0f;
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

//	GUIStyle vStyle;
//	void OnGUI()
//	{
//		vStyle = new GUIStyle();
//		vStyle.normal.background = null;
//		vStyle.normal.textColor=new Color(1,0,0);
//		vStyle.fontSize = 40;
//		GUI.Label(new Rect(0,0,200,200),"i"+Input.acceleration.y.ToString(),vStyle);
//		GUI.Label(new Rect(0,150,200,200),"i-o"+Mathf.Abs(Input.acceleration.y-originAcceleration.y).ToString(),vStyle);
//		GUI.Label(new Rect(0,300,200,200),"o"+originAcceleration.y.ToString(),vStyle);
//	}

	void FixedUpdate ()
	{
		if(isGuide) return;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Flight_StageController.userControl.Equals(false)) return;
			if(Mathf.Abs(Input.acceleration.y-originAcceleration.y)>0.4f || Input.acceleration.y < -0.8f)
			{
				moveVertical =-1;
				accelerationEffect.SetActive(true);
				if(fCamera.enabled.Equals(false))
					fCamera.enabled = true;
				if(fTerrainCamera.enabled.Equals(false))
					fTerrainCamera.enabled = true;
				startDown = true;
				if(Flight_GuideManager.Instance != null)
				{
					Flight_GuideManager.Instance.CloseGuide();
				}
			}
//			if(Flight_StageController.vAxisValue.Equals(1))
//			{
//				if(Input.acceleration.y > 0.4f)
//				{
//					moveVertical =-1;
//					accelerationEffect.SetActive(true);
//					if(fCamera.enabled.Equals(false))
//						fCamera.enabled = true;
//					if(fTerrainCamera.enabled.Equals(false))
//						fTerrainCamera.enabled = true;
//					startDown = true;
//				}else{
//
//					moveVertical = 0;
//					accelerationEffect.SetActive(false);
//					if(fCamera.enabled.Equals(true))
//						fCamera.enabled = false;
//					if(fTerrainCamera.enabled.Equals(true))
//						fTerrainCamera.enabled = false;
//					startDown = false;
//				}
//			}else{
//				if(Flight_StageController.vAxisValue-vOriginAcceleration > 0.4f)
//				{
//					moveVertical =-1;
//					accelerationEffect.SetActive(true);
//					if(fCamera.enabled.Equals(false))
//						fCamera.enabled = true;
//					if(fTerrainCamera.enabled.Equals(false))
//						fTerrainCamera.enabled = true;
//					startDown = true;
//				}else{
//					moveVertical = 0;
//					accelerationEffect.SetActive(false);
//					if(fCamera.enabled.Equals(true))
//						fCamera.enabled = false;
//					if(fTerrainCamera.enabled.Equals(true))
//						fTerrainCamera.enabled = false;
//					startDown = false;
//				}
//			}
			movement = new Vector3 (0, moveVertical, 0);
		}else {
			if(Input.GetKeyDown(KeyCode.W))
			{
				moveVertical =-1;
				if(fCamera.enabled.Equals(false))
					fCamera.enabled = true;
				if(fTerrainCamera.enabled.Equals(false))
					fTerrainCamera.enabled = true;
				if(!accelerationEffect.activeSelf)
					accelerationEffect.SetActive(true);
				movement = new Vector3 (0, moveVertical, 0);
				startDown = true;
				if(Flight_GuideManager.Instance != null)
				{
					Flight_GuideManager.Instance.CloseGuide();
				}
			}
			
			if(Input.GetKeyDown(KeyCode.S))
			{
				moveVertical = 0;
				if(fCamera.enabled.Equals(true))
					fCamera.enabled = false;
				if(fTerrainCamera.enabled.Equals(true))
					fTerrainCamera.enabled = false;
				if(accelerationEffect.activeSelf)
					accelerationEffect.SetActive(false);
				movement = new Vector3 (0, moveVertical, 0);
				startDown = false;
			}
		}

		if (startDown) 
		{	
			aDownSpeed+=0.1f;
			if(aDownSpeed>1)
				aDownSpeed=1;
			downSpeed = Mathf.Lerp(0f,1f,aDownSpeed);
			cloudRise.playbackSpeed = downSpeed;
		} else {
			aDownSpeed-=0.05f;
			if(aDownSpeed<=0.005f)
				aDownSpeed=0.005f;
			downSpeed = Mathf.Lerp(0f,1f,aDownSpeed);
			cloudRise.playbackSpeed = downSpeed;
		}

		GetComponent<Rigidbody>().velocity = movement * speed;
		if (moveVertical.Equals (0f) && acceleration <0) 
		{
			if(acceleration >=0)
				acceleration = 0;
			GetComponent<Rigidbody> ().position = new Vector3
			(
				0.0f, 
				Mathf.Clamp (GetComponent<Rigidbody>().position.y, acceleration+=0.1f, 0f),
				0.0f
			);
		} else {
			if(!acceleration.Equals(-1))
			{
				acceleration = GetComponent<Rigidbody>().position.y;
			}
			if(acceleration<=-1)
				acceleration = -1;
			GetComponent<Rigidbody>().position = new Vector3
			(
				0.0f,
				Mathf.Clamp (acceleration, -1, 0f),
				0.0f
			);
		}
	}
}
