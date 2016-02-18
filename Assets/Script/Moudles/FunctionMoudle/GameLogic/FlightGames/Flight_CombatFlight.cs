using UnityEngine;
using System.Collections;

public class Flight_CombatFlight : MonoBehaviour {

	public Flight_CombatFlightController combatFlightController;
	public GameObject terrainCamera;
	public Camera roleCamera;
	public Vector3 terrainCameraPos;
	public Vector3 terrainCameraRot;
	public Vector3 roleCameraPos;
	public Vector3 roleCameraRot;
	public Vector3 roleMeshPositon;
	public Vector3 roleMeshRot;

	public Flight_DestroyByBoundary destroyByBoundary;
	public Flight_EnemyController enemyController;

	void OnEnable()
	{
		Initialization();
	}

	public void Initialization()
	{
//		iTween.CameraFadeAdd();
//		iTween.CameraFadeTo(iTween.Hash("amount", 0f, "time", 1.0f, "delay", 0.0f));
		Invoke("SetControl",1f);
		if(combatFlightController == null)
			combatFlightController = transform.GetComponentInChildren<Flight_CombatFlightController>();
		if(destroyByBoundary == null)
			destroyByBoundary = transform.GetComponentInChildren<Flight_DestroyByBoundary>();

		if(Flight_StageController.Instance != null)
		{
			GameObject go = Flight_StageController.Instance.instantiatePlayerObject;
			go.transform.parent = combatFlightController.transform;
			go.transform.localPosition = roleMeshPositon;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			if(go.activeSelf.Equals(false))
			{
				go.SetActive(true);
			}
			combatFlightController.playerObject = go;
		}

		combatFlightController.combatFlight = this;
		combatFlightController.ResetDefault();
		
		if (destroyByBoundary != null)
			destroyByBoundary.dodgeCount = 0;
		
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
		combatFlightController.SetAnimator();
		combatFlightController.SetDefaultPosition();

		roleCamera.gameObject.SetActive(true);
		if(enemyController != null)
			enemyController.SetDefault();

		if (PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
		{
			Flight_CombatFlightAudio combatFlightAudio = transform.GetComponent<Flight_CombatFlightAudio>();
			if(combatFlightAudio != null)
			{
				combatFlightAudio.PlayCombatFlightAudio(1);
			}
		} else {
			if(Flight_AudioManager.Instance != null)
			{
				Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu37);
			}
		}
	}

    void SetControl()
	{
		Flight_StageController.userControl = true;
	}
}
