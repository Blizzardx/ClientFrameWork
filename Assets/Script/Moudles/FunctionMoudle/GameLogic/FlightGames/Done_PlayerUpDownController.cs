using UnityEngine;
using System.Collections;

public class Done_PlayerUpDownController : MonoBehaviour {
	
	private float Vspeed;
	public Vector3 upOriginPos;
	public Vector3 downOriginPos;
	public GameObject FlightFly,FlightUpAndDown;
	public float GameHeight;
	public float slideSpeed;
	public static Done_PlayerUpDownController instance;
	[HideInInspector]
	public bool upAndDown = true;

	float moveVertical = 0f;

	public GameObject upButton,downButton;

	bool startAcceleration = false;

	void Awake()
	{
		if(instance == null)
			instance = this;
		if(upButton != null)
		{
			UIEventListener.Get (upButton).onPress = OnUpButtonPress;
		}
		if(downButton != null)
		{
			UIEventListener.Get (downButton).onPress = OnDownButtonPress;
		}
	}

	public void SetUpAndDown(bool up)
	{
		upAndDown = up;
		if (up) 
		{
			transform.localPosition = upOriginPos;
		} else {
			transform.localPosition = downOriginPos;
		}
		moveVertical = 0f;
		startAcceleration = false;
		if(upButton != null)
		{
			upButton.SetActive(up);
		}
		if(downButton != null)
		{
			downButton.SetActive(!up);
		}
	}

	void OnUpButtonPress (GameObject go, bool isPressed)
	{
		if(isPressed)
		{
			startAcceleration = true;
		}else{
			moveVertical = 0;
			startAcceleration = false;
		}
	}

	void OnDownButtonPress(GameObject go, bool isPressed)
	{
		if(isPressed)
		{
			startAcceleration = true;
		}else{
			moveVertical = 0;
			startAcceleration = false;
		}
	}

	void FixedUpdate()
	{
		if(startAcceleration)
		{
			moveVertical -=0.2f;
			if(moveVertical <-1)
				moveVertical = -1;
		}
		if(upAndDown)
		{
			if(moveVertical.Equals(-1))
			{
				Vspeed = 1;
			}else{
				Vspeed = 0;
			}

			transform.localPosition = new Vector3(upOriginPos.x,transform.localPosition.y+Vspeed,transform.localPosition.z+moveVertical);
			if(transform.localPosition.y > GameHeight)
			{
				if(upButton != null)
					upButton.SetActive(false);
				if(downButton != null)
					downButton.SetActive(false);
				iTween.CameraFadeAdd();
				iTween.CameraFadeTo(iTween.Hash("amount", 1.0f, "time", 1.0f, "delay", 0.0f));
				Invoke("SetUpState",2f);
			}
		}else{
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
		}
	}

	void SetUpState()
	{
		if(FlightFly != null)
		{
			FlightFly.SetActive(true);
		}
		if(FlightUpAndDown != null)
		{
			FlightUpAndDown.SetActive(false);
		}
		iTween.CameraFadeTo(iTween.Hash("amount", 0.0f, "time", 1.0f, "delay", 0.0f));
	}
}
