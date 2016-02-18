using UnityEngine;
using System.Collections;

public class Flight_ObjectUp : MonoBehaviour {

	public Flight_FlyUpController flyUpController;
	public float speed;

	void FixedUpdate() 
	{
		if(flyUpController.startUp)
		{
			transform.localPosition = new Vector3
			(
				transform.localPosition.x,
				transform.localPosition.y+speed,
				transform.localPosition.z
			);
		}
	}

	public void SetEvent()
	{
		if(Flight_StageController.Instance != null)
		{
//			iTween.CameraFadeAdd();
//			iTween.CameraFadeTo(iTween.Hash("amount", 1f, "time", 1.0f, "delay", 0.0f));
			Flight_StageController.Instance.StartSetStageState(StageState.FreeFlightUp);
		}
	}
}
