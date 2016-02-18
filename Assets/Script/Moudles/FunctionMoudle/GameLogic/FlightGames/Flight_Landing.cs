using UnityEngine;
using System.Collections;

public class Flight_Landing : MonoBehaviour {

	public Flight_LandingController landingUpController;
	public GameObject terrainCameraObject;
	public Camera roleCamera;
	public Vector3 terrainCameraPos;
	public Vector3 terrainCameraRot;
	public Vector3 roleCameraPos;
	public Vector3 roleCameraRot;
	public Vector3 roleMeshRot;

	void OnEnable()
	{
//		iTween.CameraFadeAdd();
//		iTween.CameraFadeTo(iTween.Hash("amount", 0f, "time", 1.0f, "delay", 0.0f));
		Invoke("SetControl",1f);
		if(landingUpController == null)
			landingUpController = transform.GetComponentInChildren<Flight_LandingController>();
		landingUpController.enabled = true;

		if(Flight_StageController.Instance != null)
		{
			GameObject go = Flight_StageController.Instance.instantiatePlayerObject;
			go.transform.parent = landingUpController.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			if(go.activeSelf.Equals(false))
			{
				go.SetActive(true);
			}
			go.transform.GetChild(0).localEulerAngles = roleMeshRot;
			if(landingUpController.entourage1.transform.childCount >0)
			{
				landingUpController.entourage1.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			if(landingUpController.entourage2.transform.childCount>0)
			{
				landingUpController.entourage2.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			if(landingUpController.spirit.transform.childCount>0)
			{
				landingUpController.spirit.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			BoxCollider[] colliderArray = landingUpController.GetComponentsInChildren<BoxCollider>();
			for(int i=0;i<colliderArray.Length;i++)
			{
				colliderArray[i].enabled = false;
			}
		}
		
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			landingUpController.vOriginAcceleration = Flight_StageController.vAxisValue;
			landingUpController.originAcceleration = Input.acceleration;
		}
		if(terrainCameraObject != null)
		{
			terrainCameraObject.transform.localPosition = terrainCameraPos;
			terrainCameraObject.transform.localEulerAngles = terrainCameraRot;
			terrainCameraObject.SetActive(true);
			Flight_ObjectLanding objectLanding = terrainCameraObject.transform.GetComponentInChildren<Flight_ObjectLanding>();
			if(objectLanding != null)
				objectLanding.enabled = true;
		}
		if(roleCamera != null)
		{
			roleCamera.transform.localPosition = roleCameraPos;
			roleCamera.transform.localEulerAngles = roleCameraRot;
			roleCamera.gameObject.SetActive(true);
		}

		landingUpController.SetDefault();
		landingUpController.gameObject.SetActive(true);
		landingUpController.SetAnimator();
		if(Flight_StageController.isGuide)
		{
			landingUpController.isGuide = false;
		}else{
			landingUpController.isGuide = true;
			if(Flight_GuideManager.Instance != null)
			{
				Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.LandingStep9);
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
		landingUpController.isGuide = false;
	}

	void SetControl()
	{
		Flight_StageController.userControl = true;
	}
}
