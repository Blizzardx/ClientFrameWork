using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct AudioStep
{
	public const string None ="None";
	public const string Yindaoyu25 = "Yindaoyu_#25a1_G_D";
	public const string Yindaoyu26 = "Yindaoyu_#26_G_D";
	public const string Yindaoyu27 = "Yindaoyu_#27_G_D";
	public const string Yindaoyu28 = "Yindaoyu_#28_G_D";
	public const string Yindaoyu29 = "Yindaoyu_#29_G_D";
	public const string Yindaoyu30 = "Yindaoyu_#30_G_D";
	public const string Yindaoyu31 = "Yindaoyu_#31_G_D";
	public const string Yindaoyu32 = "Yindaoyu_#32_G_D";
	public const string Yindaoyu33 = "Yindaoyu_#33_G_D";
	public const string Yindaoyu34 = "Yindaoyu_#34_G_D";
	public const string Yindaoyu35 = "Yindaoyu_#35_G_D";
	public const string Yindaoyu36 = "Yindaoyu_#36_G_D";
	public const string Yindaoyu37 = "Yindaoyu_#37_G_D";
	public const string Yindaoyu38 = "Yindaoyu_#38_G_D";
	public const string Yindaoyu39 = "Yindaoyu_#39_G_D";
	public const string Yindaoyu40 = "Yindaoyu_#40_G_D";
	public const string Yindaoyu41 = "Yindaoyu_#41_G_D";
	public const string Yindaoyu42 = "Yindaoyu_#42_G_D";
	public const string Yindaoyu43 = "Yindaoyu_#43_G_D";
	public const string Yindaoyu44 = "Yindaoyu_#44_G_D";
	public const string Yindaoyu45 = "Yindaoyu_#45_G_D";
	public const string Yindaoyu46 = "Yindaoyu_#46_G_D";
	public const string Yindaoyu47 = "Yindaoyu_#47_G_D";
	public const string Yindaoyu48 = "Yindaoyu_#48_G_D";
	public const string End ="End";
}

public class Flight_AudioManager : SingletonTemplateMon<Flight_AudioManager> {

	public string audioPath = "GUIDE/40_FlightGames/";
	public static string staticAudioStep="";

	void Awake()
	{
		_instance = this;
	}

	public void ChangeAudioStep(string audioStep,Action<string> onFinishedCallBack = null,bool isrepeat = false)
	{
		if(staticAudioStep.Equals(AudioStep.End)) return;
		if(staticAudioStep.Equals(audioStep) && isrepeat.Equals(false)) return;

		staticAudioStep = audioStep;
		if(!audioStep.Equals(AudioStep.End))
			Play(staticAudioStep,onFinishedCallBack);
	}

	public void Play(string name,Action<string> onFinishedCallBack = null)
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+name,Vector3.zero,false,onFinishedCallBack);
	}

	public void StopAudio(string resource)
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.StopAudio(audioPath+resource);
	}
}
