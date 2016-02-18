using UnityEngine;
using System.Collections;

public class Flight_ObjectLanding : MonoBehaviour {

	public Flight_LandingController landingController;
	public float speed;

	void FixedUpdate() 
	{
		if(landingController.startUp)
		{
			transform.localPosition = new Vector3
			(
				transform.localPosition.x,
				transform.localPosition.y-speed,
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
			if (!PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
			{
				if(Flight_GuideManager.Instance != null)
				{
					Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.End);
				}
				if(Flight_AudioManager.Instance != null)
				{
					Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu47,ChangeState);
					Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.End);
				}
			}else{
				WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
			}
			this.enabled = false;
		}
	}

	void ChangeState(string str)
	{
        WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
        //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
    }
}
