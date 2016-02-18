using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIArithmeticTimeManager : MonoBehaviour
{
	public long fixedTime = 180;
	private long lastTime;
	public UILabel timeLabel;

	public static UIArithmeticTimeManager instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start()
	{
		InitTime();
	}

	void InitObject()
	{
		if(timeLabel == null)
			timeLabel = transform.FindChild("TimeLabel").GetComponent<UILabel>();
	}

	public void InitTime()
	{
		CancelInvoke("CountDown");
		lastTime = fixedTime;
		if(lastTime % 60 <10)
			timeLabel.text = ((lastTime / 60) % 60).ToString() + ":"+"0"+(lastTime % 60).ToString();
		else
			timeLabel.text = ((lastTime / 60) % 60).ToString() + ":"+(lastTime % 60).ToString();
		InvokeRepeating("CountDown", 1, 1);
	}

	public void StopCountDown()
	{
		CancelInvoke("CountDown");
	}

	void CountDown()
	{
		lastTime -= 1;
		if(lastTime % 60 <10)
			timeLabel.text = ((lastTime / 60) % 60).ToString() + ":"+"0"+(lastTime % 60).ToString();
		else
			timeLabel.text = ((lastTime / 60) % 60).ToString() + ":"+(lastTime % 60).ToString();
		if(lastTime.Equals(0))
		{
			CancelInvoke("CountDown");
			if(UIArithmeticGameManager.Instance != null)
				UIArithmeticGameManager.Instance.Failure();
		}
	}
}