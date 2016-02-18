using UnityEngine;  
using System.Collections;  
using System;  

public class Flight_PlayerUpDownController : MonoBehaviour {
	
	private float Vspeed;
	public Vector3 upOriginPos;
	public Vector3 downOriginPos;
	public GameObject flightFly,flightUpAndDown,RoleDown;
	public float gameHeight;
	public float downHeight;

	public static Flight_PlayerUpDownController instance;
	[HideInInspector]
	public bool upAndDown = true;

	float moveVertical = 0f;

	public Flight_GameController gameController;

	public bool hover = false;
	public GameObject done_Player;

	public Vector3 originAcceleration;
	float x=0f;

	public Flight_FlyDownController playerDown;

	void Awake()
	{
		if(instance == null)
			instance = this;
		originAcceleration = Input.acceleration;
	}

//	void OnGUI()  
//	{  
//		GUI.Box(new Rect(5, 5, 500, 100), "X:"+String.Format("{0:0.0}", Input.acceleration.x));  
//		GUI.Box(new Rect(5, 150, 500, 100),"Y:"+String.Format("{0:0.0}", Input.acceleration.y));  
//		GUI.Box(new Rect(5,300, 500, 100), "Z:"+String.Format("{0:0.0}", Input.acceleration.z));
//		GUI.Box(new Rect(5,450, 500, 100), "V:"+String.Format("{0:0.0}", moveVertical));  
//	}  

	public void SetUpAndDown(bool up)
	{
		upAndDown = up;
		if (up) 
		{
			transform.localPosition = upOriginPos;
			transform.localEulerAngles = new Vector3(0,180,0);
			done_Player.transform.localEulerAngles = Vector3.zero;
		} else {
			transform.localPosition = downOriginPos;
			CancelInvoke("SetRoleDown");
			Invoke("SetRoleDown",10f);
		}
		moveVertical = 0f;

	}

	void SetRoleDown()
	{
		if (RoleDown != null)
		{
//			iTween.CameraFadeTo(iTween.Hash("amount", 1.0f, "time", 1.0f, "delay", 0.0f));
		}
	}

	void FixedUpdate()
	{
		if (originAcceleration.z.Equals (0))
			originAcceleration = Input.acceleration;
		if(upAndDown)
		{
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (originAcceleration.y - Input.acceleration.y > 0.2f && Input.acceleration.z - originAcceleration.z > 0.2f
				    ||Input.acceleration.y - originAcceleration.y >0.2f && Input.acceleration.z - originAcceleration.z > 0.2f)
				{
					moveVertical = -1f;
				} else {
//					moveVertical = 0f;
				}
			}else{
				moveVertical = Input.GetAxis ("Vertical");
			}
			if(moveVertical.Equals(-1))
			{
				Vspeed = 1;
			}else{
				Vspeed = 0;
			}

			transform.localPosition = new Vector3(upOriginPos.x,transform.localPosition.y+Vspeed,transform.localPosition.z+moveVertical);
			if(transform.localPosition.y > gameHeight)
			{
//				iTween.CameraFadeAdd();
//				iTween.CameraFadeTo(iTween.Hash("amount", 1.0f, "time", 1.0f, "delay", 0.0f));
				Invoke("SetUpState",2f);
			}
		}else{
			if(hover.Equals(false))
			{
				if(moveVertical.Equals(-1))
				{
					Vspeed = -1;
				}else{
					Vspeed = 0;
				}
				
				transform.localPosition = new Vector3(upOriginPos.x,transform.localPosition.y+Vspeed,transform.localPosition.z+moveVertical);
				
				if(transform.localPosition.y < upOriginPos.y)
				{
					transform.localPosition = new Vector3(transform.localPosition.x,upOriginPos.y,transform.localPosition.z+moveVertical);
					if(transform.localPosition.z <= upOriginPos.z)
					{
						SetUpAndDown(true);
					}
				}
			}else{
				float moveHorizontal = 0f;

				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
				{
//					if (Input.acceleration.y - originAcceleration.y > 0.2f && originAcceleration.z - Input.acceleration.z > 0.2f)
//					{
//						moveVertical = -1f;
//					}
//					else if(originAcceleration.y - Input.acceleration.y >0.2f && originAcceleration.z - Input.acceleration.z > 0.2f)
//					{
//						moveVertical = -1f;
//					}
//					else if(Input.acceleration.y - originAcceleration.y >0.1f && Input.acceleration.z - originAcceleration.z > 0.1f)
//					{
//						moveVertical = -1f;
//					}
//					else {
//						moveVertical = 0f;
//					}
				}

				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
				{
					if(Input.acceleration.x <-0.2f)
					{
						x -=0.1f;
						if(x<=-1f)
						{
							x = -1f;
						}
						moveHorizontal = -1;
					}
					else if(Input.acceleration.x > 0.2f)
					{
						x +=0.1f;
						if(x>=1f)
						{
							x = 1f;
						}
						moveHorizontal = 1;
					}
					else if(Input.acceleration.x>0 && Input.acceleration.x <0.2f)
					{
						x -=0.1f;
						if(x<=0)
							x=0;
						moveHorizontal=0;
					}
					else if(Input.acceleration.x<0 && Input.acceleration.x >-0.2f)
					{
						x +=0.1f;
						if(x>=0)
							x=0;
						moveHorizontal=0;
					}
				}else{
					moveHorizontal = Input.GetAxis ("Horizontal");
				}

				transform.localPosition = transform.localPosition+transform.forward+new Vector3(0,moveVertical,0);
				transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y+moveHorizontal,0); 
				done_Player.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, x*-60);

				if(transform.localPosition.x <-200)
				{
					transform.localPosition = new Vector3(-200,transform.localPosition.y,transform.localPosition.z);
				}
				if(transform.localPosition.x >200)
				{
					transform.localPosition = new Vector3(200,transform.localPosition.y,transform.localPosition.z);
				}
				if(transform.localPosition.z >200)
				{
					transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,200);
				}
				if(transform.localPosition.z <-200)
				{
					transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,-200);
				}

				if(transform.localPosition.y < downHeight)
				{
					SetUpAndDown(true);
				}
			}
		}
	}

	void SetUpState()
	{
		if(flightFly != null)
		{
			flightFly.SetActive(true);
		}
		if(flightUpAndDown != null)
		{
			flightUpAndDown.SetActive(false);
		}
		if(gameController != null)
		{
			gameController.gameObject.SetActive(true);
		}
//		iTween.CameraFadeTo(iTween.Hash("amount", 0.0f, "time", 1.0f, "delay", 0.0f));
	}
}
