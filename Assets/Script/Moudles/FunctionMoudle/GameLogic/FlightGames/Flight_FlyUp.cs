using UnityEngine;
using System.Collections;

public class Flight_FlyUp : MonoBehaviour {

	public Flight_FlyUpController flyUpController;
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
		if(flyUpController == null)
			flyUpController = transform.GetComponentInChildren<Flight_FlyUpController>();

		flyUpController.enabled = true;

		if(Flight_StageController.Instance != null)
		{
			GameObject go = Flight_StageController.Instance.instantiatePlayerObject;
			go.transform.parent = flyUpController.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			if(go.activeSelf.Equals(false))
			{
				go.SetActive(true);
			}
			flyUpController.roleObject = go;
			go.transform.GetChild(0).localEulerAngles = roleMeshRot;
			if(flyUpController.entourage1.transform.childCount >0)
			{
				flyUpController.entourage1.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			if(flyUpController.entourage2.transform.childCount>0)
			{
				flyUpController.entourage2.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			if(flyUpController.spirit.transform.childCount>0)
			{
				flyUpController.spirit.transform.GetChild(0).localEulerAngles = roleMeshRot;
			}
			BoxCollider[] colliderArray = flyUpController.GetComponentsInChildren<BoxCollider>();
			for(int i=0;i<colliderArray.Length;i++)
			{
				colliderArray[i].enabled = false;
			}
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			flyUpController.vOriginAcceleration = Flight_StageController.vAxisValue;
		}
		if(terrainCameraObject != null)
		{
			terrainCameraObject.transform.localPosition = terrainCameraPos;
			terrainCameraObject.transform.localEulerAngles = terrainCameraRot;
			terrainCameraObject.SetActive(true);
			Flight_ObjectUp objectUp = terrainCameraObject.transform.GetComponentInChildren<Flight_ObjectUp>();
			if(objectUp != null)
				objectUp.enabled = true;
		}
		if(roleCamera != null)
		{
			roleCamera.transform.localPosition = roleCameraPos;
			roleCamera.transform.localEulerAngles = roleCameraRot;
			roleCamera.gameObject.SetActive(true);
		}
		flyUpController.SetDefault();
		flyUpController.gameObject.SetActive(true);

		if(Flight_StageController.isGuide)
		{
			flyUpController.isGuide = false;
		}else{
			if(Flight_AudioManager.Instance != null)
			{
				Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu25,FinishedCallBackOne);
			}
			flyUpController.isGuide = true;
		}
	}

	void FinishedCallBackOne(string str)
	{
		if (Flight_GuideManager.Instance != null) 
		{
			Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.FlyUpStep1);
		}
		if(Flight_AudioManager.Instance != null)
		{
			Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu26,FinishedCallBackTwo);
		}
	}

	void FinishedCallBackTwo(string str)
	{
		flyUpController.isGuide = false;
	}
}
