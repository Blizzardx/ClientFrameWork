using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flight_CombatFlightGuide : MonoBehaviour {

	public Flight_CombatFlightControllerGuide combatFlightControllerGuide;
	public GameObject terrainCamera;
	public Camera roleCamera;
	public Vector3 terrainCameraPos;
	public Vector3 terrainCameraRot;
	public Vector3 roleCameraPos;
	public Vector3 roleCameraRot;
	public Vector3 roleMeshPositon;
	public Vector3 roleMeshRot;
	public Flight_EnemyControllerGuide enemyControllerGuide;
	public Flight_GameControllerGuide gameControllerGuide;

	void OnEnable()
	{
		Initialization();
	}

	public void Initialization()
	{
		if(combatFlightControllerGuide == null)
			combatFlightControllerGuide = transform.GetComponentInChildren<Flight_CombatFlightControllerGuide>();
		if(enemyControllerGuide == null)
			enemyControllerGuide = transform.GetComponentInChildren<Flight_EnemyControllerGuide>();
		if(gameControllerGuide == null)
			gameControllerGuide = transform.GetComponentInChildren<Flight_GameControllerGuide>();

		if(Flight_StageController.Instance != null)
		{
			GameObject go = Flight_StageController.Instance.instantiatePlayerObject;
			go.transform.parent = combatFlightControllerGuide.transform;
			go.transform.localPosition = roleMeshPositon;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			if(go.activeSelf.Equals(false))
			{
				go.SetActive(true);
			}
			combatFlightControllerGuide.playerObject = go;
		}
		combatFlightControllerGuide.ResetDefault();

		if(terrainCamera != null)
		{
			terrainCamera.transform.localPosition = terrainCameraPos;
			terrainCamera.transform.localEulerAngles = terrainCameraRot;
			terrainCamera.SetActive(true);
		}

		if(roleCamera != null)
		{
			roleCamera.transform.localPosition = roleCameraPos;
			roleCamera.transform.localEulerAngles = roleCameraRot;
			roleCamera.gameObject.SetActive(false);
		}
		combatFlightControllerGuide.SetAnimator();
		combatFlightControllerGuide.SetDefaultPosition();

		roleCamera.gameObject.SetActive(true);

		if(enemyControllerGuide != null)
		{
			enemyControllerGuide.HideMesh();
		}
		if(gameControllerGuide != null)
		{
			gameControllerGuide.SetLeftGuide();
		}

		if(Flight_StageController.isGuide)
		{
			combatFlightControllerGuide.isGuide = false;
		}else{
			if(Flight_AudioManager.Instance != null)
			{
				Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu31,TwoAudio);
			}
			combatFlightControllerGuide.isGuide = true;
		}
	}

	void TwoAudio(string str)
	{
		if(Flight_AudioManager.Instance != null)
		{
			Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu32,ThreeAudio);
		}
	}

	void ThreeAudio(string str)
	{
		if(Flight_GuideManager.Instance != null)
		{
			Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.CombatFlightStep4);
		}
		if(Flight_AudioManager.Instance != null)
		{
			Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu33,SetControl);
		}
	}

	void SetControl(string str)
	{
		combatFlightControllerGuide.isGuide = false;
	}
}
