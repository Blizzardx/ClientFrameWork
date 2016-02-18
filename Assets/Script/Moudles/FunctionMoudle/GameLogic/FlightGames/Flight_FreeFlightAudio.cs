using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flight_FreeFlightAudio : MonoBehaviour {

	public string audioPath = "WorldGame";
	public List<string> audioUpNameList;
	public List<string> audioDownNameList;
	int index =0;
	public bool finish = false;

	void OnEnable()
	{
		index = 0;
		finish = false;
	}

	public void PlayFreeFlightAudio(StageState stageState)
	{
		switch(stageState)
		{
			case StageState.FreeFlightUp:
				index = 0;
				finish = false;
				PlayFreeFlightUpAudio("");
				break;
			case StageState.FreeFlightDown:
				index = 0;
				finish = false;
				PlayFreeFlightDownAudio("");
				break;
		}
	}

	void PlayFreeFlightUpAudio(string str)
	{
		if(index <audioUpNameList.Count-1)
		{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+audioUpNameList[index],Vector3.zero,false,PlayFreeFlightUpAudio);
		}else{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+audioUpNameList[index],Vector3.zero,false,OnAudioFinish);
		}
		index++;
	}

	void OnAudioFinish(string str)
	{
		finish = true;
	}

	void PlayFreeFlightDownAudio(string str)
	{
		if(index <audioDownNameList.Count-1)
		{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+audioDownNameList[index],Vector3.zero,false,PlayFreeFlightDownAudio);
		}else{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+audioDownNameList[index],Vector3.zero,false,OnAudioFinish);
		}
		index++;
	}
}
