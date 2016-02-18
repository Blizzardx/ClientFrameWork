using UnityEngine;
using System.Collections;

public class Flight_FreeFlightController : MonoBehaviour {

	float moveHorizontal = 0f;
	public float moveHorizontalRatio = 0f;
	public GameObject playerObject;

	[HideInInspector]
	public float timer;
	public float duration;
	public float minZoneX,maxZoneX;
	public float minZoneZ,maxZoneZ;
	public float rollAngle;
	public float moveRollSpeed = 100f;
	public float rollSpeed =10f;
	public float speed = 0.5f;
	Animator[] anim;

	public GameObject entourage1,entourage2,spirit;

	public Flight_FreeFlightAudio freeFlightAudio;

	public void SetAnimator()
	{
		anim = transform.GetComponentsInChildren<Animator>();
		if(anim != null)
		{
			if(anim.Length >0)
			{
				for(int i=0;i<anim.Length;i++)
				{
					anim[i].SetBool("Fly",true);
					anim[i].SetBool("Glide",false);
				}
			}
		}
	}

	void FixedUpdate()
	{	
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			if(Flight_StageController.userControl.Equals(false)) return;
			if(Flight_StageController.vAxisValue.Equals(1f))
			{
				moveHorizontal = 0f;
			}else{
				if(Flight_StageController.hAxisValue >0.2f || Flight_StageController.hAxisValue <-0.2f)
				{
					if(Flight_StageController.hAxisValue <-0.2f)
					{
						if(Flight_GuideManager.Instance != null)
						{
							if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightUpStep2))
							{
								Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.FreeFlightUpStep3);
								if(Flight_AudioManager.Instance != null)
								{
									Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu29);
									Flight_AudioManager.Instance.StopAudio(AudioStep.Yindaoyu28);
								}
							}
							if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightDownStep7))
							{
								Flight_GuideManager.Instance.CloseGuide();
							}
						}
					}
					if(Flight_StageController.hAxisValue > 0.2f)
					{
						if(Flight_GuideManager.Instance != null)
						{
							if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightUpStep3))
							{
								Flight_GuideManager.Instance.CloseGuide();
								if(Flight_AudioManager.Instance != null)
								{
									Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu30);
									Flight_AudioManager.Instance.StopAudio(AudioStep.Yindaoyu29);
								}
							}
							if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightDownStep7))
							{
								Flight_GuideManager.Instance.CloseGuide();
							}
						}
					}
					moveHorizontal = float.Parse(Flight_StageController.hAxisValue.ToString("F1"));
				}else{
					moveHorizontal = 0f;
				}
			}
		} else {
			moveHorizontal = Input.GetAxis ("Horizontal");
			if(moveHorizontal <0)
			{
				if(Flight_GuideManager.Instance != null)
				{
					if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightUpStep2))
					{
						Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.FreeFlightUpStep3);
						if(Flight_AudioManager.Instance != null)
						{
							Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu29);
							Flight_AudioManager.Instance.StopAudio(AudioStep.Yindaoyu28);
						}
					}
					if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightDownStep7))
					{
						Flight_GuideManager.Instance.CloseGuide();
					}
				}
			}
			if(moveHorizontal >0)
			{
				if(Flight_GuideManager.Instance != null)
				{
					if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightUpStep3))
					{
						Flight_GuideManager.Instance.CloseGuide();
						if(Flight_AudioManager.Instance != null)
						{
							Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu30);
							Flight_AudioManager.Instance.StopAudio(AudioStep.Yindaoyu29);
						}
					}
					if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.FreeFlightDownStep7))
					{
						Flight_GuideManager.Instance.CloseGuide();
					}
				}
			}
		}

		transform.localPosition = transform.localPosition+transform.forward*speed;
		transform.localRotation = Quaternion.Lerp
		(
			Quaternion.Euler(transform.localEulerAngles),
			Quaternion.Euler(new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y+moveHorizontal*moveHorizontalRatio,transform.localEulerAngles.z)),
			Time.deltaTime*moveRollSpeed
		);
//		playerObject.transform.localEulerAngles = Quaternion.Euler (0.0f, 0.0f, moveHorizontal*-rollAngle);
		playerObject.transform.localRotation = Quaternion.Lerp
		(
			Quaternion.Euler(playerObject.transform.localEulerAngles),
			Quaternion.Euler(new Vector3(playerObject.transform.localEulerAngles.x,playerObject.transform.localEulerAngles.y,moveHorizontal*-rollAngle)),
			Time.deltaTime*rollSpeed
		);

		if(entourage1 != null)
		{
			entourage1.transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(entourage1.transform.localEulerAngles),
				Quaternion.Euler(new Vector3(entourage1.transform.localEulerAngles.x,entourage1.transform.localEulerAngles.y,moveHorizontal*-rollAngle)),
				Time.deltaTime*rollSpeed
			);
		}
		if(entourage2 != null)
		{
			entourage2.transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(entourage2.transform.localEulerAngles),
				Quaternion.Euler(new Vector3(entourage2.transform.localEulerAngles.x,entourage2.transform.localEulerAngles.y,moveHorizontal*-rollAngle)),
				Time.deltaTime*rollSpeed
			);
		}
		if(spirit != null)
		{
			spirit.transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(spirit.transform.localEulerAngles),
				Quaternion.Euler(new Vector3(spirit.transform.localEulerAngles.x,spirit.transform.localEulerAngles.y,moveHorizontal*-rollAngle)),
				Time.deltaTime*rollSpeed
			);
		}

		if(transform.localPosition.x <minZoneX)
		{
			transform.localPosition = new Vector3(minZoneX,transform.localPosition.y,transform.localPosition.z);
			transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(transform.localEulerAngles),
				Quaternion.Euler(new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y+1,transform.localEulerAngles.z)),
				Time.deltaTime*moveRollSpeed
			);
		}
		if(transform.localPosition.x >maxZoneX)
		{
			transform.localPosition = new Vector3(maxZoneX,transform.localPosition.y,transform.localPosition.z);
			transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(transform.localEulerAngles),
				Quaternion.Euler(new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y-1,transform.localEulerAngles.z)),
				Time.deltaTime*moveRollSpeed
			);
		}
		if(transform.localPosition.z >maxZoneZ)
		{
			transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,maxZoneZ);
			transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(transform.localEulerAngles),
				Quaternion.Euler(new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y+1,transform.localEulerAngles.z)),
				Time.deltaTime*moveRollSpeed
			);
		}
		if(transform.localPosition.z <minZoneZ)
		{
			transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,minZoneZ);
			transform.localRotation = Quaternion.Lerp
			(
				Quaternion.Euler(transform.localEulerAngles),
				Quaternion.Euler(new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y-1,transform.localEulerAngles.z)),
				Time.deltaTime*moveRollSpeed
			);
		}

		if(Time.time-timer > duration)
		{
			if(Flight_StageController.Instance != null)
			{
				switch(Flight_StageController.Instance.stageState)
				{
					case StageState.FreeFlightUp:
						if(Flight_GuideManager.Instance != null)
						{
							if (PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
							{
								if(freeFlightAudio.finish)
								{
									Flight_StageController.Instance.StartSetStageState(StageState.CombatFlight);
									this.enabled = false;
								}
							}else{
								Flight_StageController.Instance.StartSetStageState(StageState.CombatFlightGuide);
								this.enabled = false;
							}
						}else{
							Flight_StageController.Instance.StartSetStageState(StageState.CombatFlight);
							this.enabled =false;
						}
						break;
					
					case StageState.FreeFlightDown:
						if (PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
						{
							if(freeFlightAudio.finish)
							{
								Flight_StageController.Instance.StartSetStageState(StageState.FlyDown);
								this.enabled = false;
							}
						}else{
							Flight_StageController.Instance.StartSetStageState(StageState.FlyDown);
							this.enabled = false;
						}
						
						break;
					
					default:
						Debug.Log("Error StageState!");
						break;
				}
			}
		}
	}
}
