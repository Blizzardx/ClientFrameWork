using UnityEngine;
using System.Collections;

public class Flight_ObjectDown : MonoBehaviour {

	public Flight_FlyDownController playerDown;
	public float stageHeight;
	public float speed;

	void FixedUpdate() 
	{
		if(playerDown != null)
		{
			if(playerDown.startDown)
				transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y-speed,transform.localPosition.z);
		}
		if(transform.localPosition.y < stageHeight)
		{
			playerDown.fCamera.enabled = false;
			playerDown.fTerrainCamera.enabled = false;
			if(Flight_StageController.Instance != null)
			{
//				iTween.CameraFadeTo(iTween.Hash("amount", 1f, "time", 1.0f, "delay", 0.0f));
				Flight_StageController.Instance.StartSetStageState(StageState.Landing);
				this.enabled = false;
			}
		}
	}
}
