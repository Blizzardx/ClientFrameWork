using UnityEngine;
using System.Collections;

public class Flight_FlyDown : MonoBehaviour {

	public Flight_FlyDownController flyDownController;
	public GameObject terrainCameraObject;
	public Camera roleCamera;
	public Vector3 terrainCameraPos;
	public Vector3 terrainCameraRot;
	public Vector3 roleCameraPos;
	public Vector3 roleCameraRot;
	public Vector3 roleMeshRot;
	public Vector3 roleLeaderMeshPos;

	void OnEnable()
	{
//		iTween.CameraFadeAdd();
//		iTween.CameraFadeTo(iTween.Hash("amount", 0f, "time", 1.0f, "delay", 0.0f));
		Invoke("SetControl",1f);
		if(flyDownController == null)
			flyDownController = transform.GetComponent<Flight_FlyDownController>();

		if(Flight_StageController.Instance != null)
		{
			GameObject go = Flight_StageController.Instance.instantiatePlayerObject;
			go.transform.parent = flyDownController.transform;
			go.transform.localPosition = roleLeaderMeshPos;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			if(go.activeSelf.Equals(false))
			{
				go.SetActive(true);
			}
			go.transform.GetChild(0).localEulerAngles = roleMeshRot;
			if(flyDownController.entourage1.transform.childCount >0)
			{
				flyDownController.entourage1.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			if(flyDownController.entourage2.transform.childCount>0)
			{
				flyDownController.entourage2.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			if(flyDownController.spirit.transform.childCount>0)
			{
				flyDownController.spirit.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
		}
		if (flyDownController != null)
		{
			flyDownController.SetDefault();
			flyDownController.originAcceleration = Input.acceleration;
			flyDownController.vOriginAcceleration = Flight_StageController.vAxisValue;
		}
		if(terrainCameraObject != null)
		{
			terrainCameraObject.transform.localPosition = terrainCameraPos;
			terrainCameraObject.transform.localEulerAngles = terrainCameraRot;
			terrainCameraObject.SetActive(true);
			Flight_ObjectDown objectDown = terrainCameraObject.transform.GetComponentInChildren<Flight_ObjectDown>();
			if(objectDown != null)
				objectDown.enabled = true;
		}
		if(roleCamera != null)
		{
			roleCamera.transform.localPosition = roleCameraPos;
			roleCamera.transform.localEulerAngles = roleCameraRot;
			roleCamera.gameObject.SetActive(true);
		}
		flyDownController.SetAnimator();

		if(Flight_StageController.isGuide)
		{
			flyDownController.isGuide = false;
		}else{
			flyDownController.isGuide = true;
			if(Flight_GuideManager.Instance != null)
			{
				Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.FlyDownStep8);
			}
			if(Flight_AudioManager.Instance != null)
			{
				Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu44,SetTwoAudio);
			}
		}
	}

	void SetTwoAudio(string str)
	{
		if(Flight_AudioManager.Instance != null)
		{
			Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu45,SetGuideControl);
		}
	}

	void SetGuideControl(string str)
	{
		flyDownController.isGuide = false;
	}

	void SetControl()
	{
		Flight_StageController.userControl = true;
	}
}
