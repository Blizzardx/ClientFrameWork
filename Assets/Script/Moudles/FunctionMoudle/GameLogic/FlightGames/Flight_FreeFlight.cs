using UnityEngine;
using System.Collections;

public class Flight_FreeFlight : MonoBehaviour {

	public Flight_FreeFlightController freeFlightController;
	public Vector3 upFreeOriginPosition;
	public Vector3 upRoleOriginRot;
	public Vector3 downFreeOriginPosition;
	public Vector3 downRoleOriginRot;

	void OnEnable()
	{
//		iTween.CameraFadeTo(iTween.Hash("amount", 0f, "time", 1.0f, "delay", 0.0f));
		if(freeFlightController == null)
			freeFlightController = transform.GetComponentInChildren<Flight_FreeFlightController>();

		if(Flight_StageController.Instance != null)
		{
			GameObject go = Flight_StageController.Instance.instantiatePlayerObject;
			go.transform.parent = freeFlightController.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			if(go.activeSelf.Equals(false))
			{
				go.SetActive(true);
			}
			freeFlightController.playerObject = go;
		}
		freeFlightController.enabled = true;
		freeFlightController.timer = Time.time;

		if(Flight_StageController.Instance != null)
		{
			switch(Flight_StageController.Instance.stageState)
			{
				case StageState.FreeFlightUp:
					freeFlightController.transform.localEulerAngles = Vector3.zero;
					freeFlightController.transform.localPosition = upFreeOriginPosition;
					                                                                                                                                                                                                                                            freeFlightController.playerObject.transform.localEulerAngles = Vector3.zero;
					freeFlightController.playerObject.transform.GetChild(0).localEulerAngles = upRoleOriginRot;
					if(freeFlightController.entourage1.transform.childCount >0)
					{
						freeFlightController.entourage1.transform.GetChild(0).localEulerAngles = upRoleOriginRot;
					}
					if(freeFlightController.entourage2.transform.childCount>0)
					{
						freeFlightController.entourage2.transform.GetChild(0).localEulerAngles = upRoleOriginRot;
					}
					if(freeFlightController.spirit.transform.childCount>0)
					{
						freeFlightController.spirit.transform.GetChild(0).localEulerAngles = upRoleOriginRot;
					}
					if (!PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
					{
						if(Flight_GuideManager.Instance != null)
						{
							Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.FreeFlightUpStep2);
						}
						if(Flight_AudioManager.Instance != null)
						{
							Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu28);
						}
					}else{
						Flight_FreeFlightAudio freeFlightAudio = transform.GetComponent<Flight_FreeFlightAudio>();
						if(freeFlightAudio != null)
						{
							freeFlightController.freeFlightAudio = freeFlightAudio;
							freeFlightAudio.PlayFreeFlightAudio(StageState.FreeFlightUp);
						}
					}
					break;

				case StageState.FreeFlightDown:
					freeFlightController.transform.localEulerAngles = Vector3.zero;
					freeFlightController.transform.localPosition = downFreeOriginPosition;
					
					freeFlightController.playerObject.transform.localEulerAngles = Vector3.zero;
					freeFlightController.playerObject.transform.GetChild(0).localEulerAngles = downRoleOriginRot;
					if(freeFlightController.entourage1.transform.childCount >0)
					{
						freeFlightController.entourage1.transform.GetChild(0).localEulerAngles = downRoleOriginRot;
					}
					if(freeFlightController.entourage2.transform.childCount>0)
					{
						freeFlightController.entourage2.transform.GetChild(0).localEulerAngles = downRoleOriginRot;
					}
					if(freeFlightController.spirit.transform.childCount>0)
					{
						freeFlightController.spirit.transform.GetChild(0).localEulerAngles = downRoleOriginRot;
					}
					if (!PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
					{
						if(Flight_GuideManager.Instance != null)
						{
							Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.FreeFlightDownStep7);
						}
						if(Flight_AudioManager.Instance != null)
						{
							Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu39);
							Flight_AudioManager.Instance.StopAudio(AudioStep.Yindaoyu38);
						}
					}else{
						Flight_FreeFlightAudio freeFlightAudio = transform.GetComponent<Flight_FreeFlightAudio>();
						if(freeFlightAudio != null)
						{
							freeFlightAudio.PlayFreeFlightAudio(StageState.FreeFlightDown);
							freeFlightController.freeFlightAudio = freeFlightAudio;
						}
					}
					Invoke("SetControl",1f);
					break;

				default:
					Debug.Log("Error StageState!");
					break;
			}

		}
		freeFlightController.SetAnimator();
	}

	void SetControl()
	{
		Flight_StageController.userControl = true;
	}
}
