using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flight_CombatFlightAudio : MonoBehaviour {

	public Flight_GameController gameController;
	public string audioPath = "WorldGame";
	int index =0;
	public List<string> startAudioList;
	public List<string> endAudioList;

	void OnEnable()
	{
		index = 0;
		if(gameController == null)
			gameController = transform.parent.GetComponentInChildren<Flight_GameController>();
	}

	public void PlayCombatFlightAudio(int number)
	{
		switch(number)
		{
			case 1:
				index = 0;
				PlayStartAudio("");
				break;
			case 2:
				index = 0;
				PlayEndAudio("");
				break;
		}
	}

	void PlayStartAudio(string str)
	{
		if(index <startAudioList.Count-1)
		{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+startAudioList[index],Vector3.zero,false,PlayStartAudio);
		}else{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+startAudioList[index],Vector3.zero,false,null);
		}
		index++;
	}

	void PlayEndAudio(string str)
	{
		if(index <endAudioList.Count-1)
		{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+endAudioList[index],Vector3.zero,false,PlayEndAudio);
		}else{
			if(AudioPlayer.Instance != null)
				AudioPlayer.Instance.PlayAudio(audioPath+endAudioList[index],Vector3.zero,false,OnAudioFinish);
		}
		index++;
	}

	void OnAudioFinish(string str)
	{
		if(gameController != null)
			gameController.ToLanding();
	}
}
