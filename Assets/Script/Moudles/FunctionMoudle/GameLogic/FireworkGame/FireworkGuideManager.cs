using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireworkGuideManager : SingletonTemplateMon<FireworkGuideManager> {

	public GameObject anchor_BottomLeft, anchor_BottomRight;
	public UITexture maskBackTexture;
	public UITexture maskLeftTexture;
	public UITexture maskRightTexture;
	public UITexture maskBottomTexture;
	public UITexture maskLeftAllTexture;
	public UITexture maskRightAllTexture;
	public UITexture maskCenterTexture;
	public UITexture maskTopLeftTexture;
	public UITexture handTexture;
	public Texture[] handTextureArray;
	public float handduration;
	int handIndex=0;
	public string audioPath ="GUIDE/FireworkGame/";
	BoxCollider boxCollider;
	public List<BoxCollider> leftColliderList;
	public List<BoxCollider> rightColliderList;
	public GameObject exitButton;
	public float timer = 0f;

	void Awake()
	{
		if (_instance == null)
			_instance = this;
	}

	void Start()
	{
		Initialization();
	}

	void Initialization()
	{
		if (anchor_BottomLeft == null)
			anchor_BottomLeft = transform.FindChild ("Anchor_BottomLeft").gameObject;
		if (anchor_BottomRight == null)
			anchor_BottomRight = transform.FindChild ("Anchor_BottomRight").gameObject;
		if(maskBackTexture == null)
			maskBackTexture = transform.FindChild("maskBackTexture").GetComponent<UITexture>();
		if(maskLeftTexture == null)
			maskLeftTexture = transform.FindChild("Anchor_BottomLeft/maskLeftTexture").GetComponent<UITexture>();
		if(maskRightTexture == null)
			maskRightTexture = transform.FindChild("Anchor_BottomLeft/maskRightTexture").GetComponent<UITexture>();
		if(maskBottomTexture == null)
			maskBottomTexture = transform.FindChild("Anchor_BottomRight/maskBottomTexture").GetComponent<UITexture>();
		if(maskLeftAllTexture == null)
			maskLeftAllTexture = transform.FindChild("Anchor_BottomLeft/maskLeftAllTexture").GetComponent<UITexture>();
		if(maskRightAllTexture == null)
			maskRightAllTexture = transform.FindChild("Anchor_BottomRight/maskRightAllTexture").GetComponent<UITexture>();
		if(maskCenterTexture == null)
			maskCenterTexture = transform.FindChild("Anchor_BottomLeft/maskCenterTexture").GetComponent<UITexture>();
		if(handTexture == null)
			handTexture = transform.FindChild("handTexture").GetComponent<UITexture>();
		if(maskTopLeftTexture == null)
			maskTopLeftTexture = transform.FindChild("Anchor_TopLeft/maskTopLeftTexture").GetComponent<UITexture>();
		if(boxCollider == null)
		{
			boxCollider = transform.GetComponent<BoxCollider>();
			boxCollider.enabled = true;
		}
		CloseAll();
		SwitchAllLeftCollider(false,-1);
		SwitchAllRightCollider(false,-1);
		if (exitButton != null)
			exitButton.gameObject.SetActive (false);
		StartGuide();
	}

	void CloseAll()
	{
		if (maskBackTexture != null)
			maskBackTexture.alpha = 0;
		if (maskLeftTexture != null)
			maskLeftTexture.gameObject.SetActive (false);
		if (maskRightTexture != null)
			maskRightTexture.gameObject.SetActive (false);
		if (maskBottomTexture != null)
			maskBottomTexture.gameObject.SetActive(false);
		if (maskLeftAllTexture != null)
			maskLeftAllTexture.gameObject.SetActive (false);
		if (maskRightAllTexture != null)
			maskRightAllTexture.gameObject.SetActive (false);
		if (maskCenterTexture != null)
			maskCenterTexture.gameObject.SetActive (false);
		if(handTexture != null)
			handTexture.gameObject.SetActive(false);
		if(maskTopLeftTexture != null)
			maskTopLeftTexture.gameObject.SetActive(false);
	}

	public void FinishGuide()
	{
		if (maskBackTexture != null)
			maskBackTexture.alpha = 1;
		if(boxCollider != null)
		{
			boxCollider.enabled = true;
		}
		if(exitButton.GetComponent<BoxCollider>())
		{
			exitButton.GetComponent<BoxCollider>().enabled = false;
			if(exitButton.gameObject.GetComponent<TweenAlpha>())
			{
				Destroy(exitButton.gameObject.GetComponent<TweenAlpha>());
			}
		}
		PlayerManager.Instance.GetCharCounterData().SetFlag(7, true);
	}

	void SwitchAllLeftCollider(bool open,int index)
	{
		if(leftColliderList.Count <=0) return;

		if(index >= 0)
		{
			for(int i=0;i<leftColliderList.Count;i++)
			{
				if(i.Equals(index))
				{
					leftColliderList[i].enabled = open;
					if(open)
					{
						//fireworkEvent = leftColliderList[i].gameObject.AddComponent<FireworkEvent>();
					}
				}else{
					leftColliderList[i].enabled = !open;
				}
			}
		}else{
			for(int i=0;i<leftColliderList.Count;i++)
			{
				leftColliderList[i].enabled = open;
			}
		}
	}

	void SwitchAllRightCollider(bool open,int index)
	{
		if(rightColliderList.Count <=0) return;
		
		if(index >= 0)
		{
			for(int i=0;i<rightColliderList.Count;i++)
			{
				if(i.Equals(index))
				{
					rightColliderList[i].enabled = open;
					if(open)
					{
						//fireworkEvent = rightColliderList[i].gameObject.AddComponent<FireworkEvent>();
					}
				}else{
					rightColliderList[i].enabled = !open;
				}
			}
		}else{
			for(int i=0;i<rightColliderList.Count;i++)
			{
				rightColliderList[i].enabled = open;
			}
		}
	}
	
	public void StartShowHand()
	{
		CancelInvoke("ShowHand");
		if(handTexture != null)
		{
			handTexture.gameObject.SetActive(true);
		}
		handIndex = 0;
		InvokeRepeating("ShowHand",0,handduration);
	}

	void ShowHand()
	{
		if(handTextureArray.Length >0)
		{
			if(handTextureArray[handIndex] != null)
			{
				if(handTexture != null)
				{
					handTexture.mainTexture = handTextureArray[handIndex];
					handIndex++;
					if(handIndex >=handTextureArray.Length)
					{
						handIndex =0;
					}
				}
			}
		}
	}

	public void StopShowHand()
	{
		CancelInvoke("ShowHand");
		if(handTexture != null)
		{
			handTexture.gameObject.SetActive(false);
		}
	}

	public void StartGuide()
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#73_G_D",Vector3.zero,false,SetStep1);
		if (maskBackTexture != null)
			maskBackTexture.alpha = 1;
	}

	void SetStep1(string str)
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#74_G_D",Vector3.zero,false,null);
		maskLeftTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha = TweenAlpha.Begin(maskLeftTexture.gameObject,0.2f,0f);
		tAlpha.from = 0.4f;
		tAlpha.style = UITweener.Style.PingPong;
		if (handTexture != null && anchor_BottomLeft != null)
		{
			handTexture.transform.parent = anchor_BottomLeft.transform;
			handTexture.transform.localPosition = new Vector3 (220,390,0f);
		}
		if (boxCollider != null)
			boxCollider.enabled = false;
		SwitchAllLeftCollider(true,1);
		StartShowHand();
	}

	public void SetStep2()
	{
		if (AudioPlayer.Instance != null) 
		{
			AudioPlayer.Instance.PlayAudio (audioPath + "Yindaoyu_#75_G_D", Vector3.zero, false, null);
			AudioPlayer.Instance.StopAudio (audioPath + "Yindaoyu_#74_G_D");
		}
		if(maskLeftTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskLeftTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskLeftTexture.gameObject.SetActive(false);

		maskRightTexture.gameObject.SetActive (true);
		TweenAlpha tAlpha = TweenAlpha.Begin(maskRightTexture.gameObject,0.2f,0f);
		tAlpha.from = 0.4f;
		tAlpha.style = UITweener.Style.PingPong;

		maskCenterTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha1 = TweenAlpha.Begin(maskCenterTexture.gameObject,0.2f,0f);
		tAlpha1.from = 0.4f;
		tAlpha1.style = UITweener.Style.PingPong;

		if(handTexture != null)
			handTexture.transform.localPosition = new Vector3 (343f,348f,0f);
		SwitchAllLeftCollider(true,2);
	}

	public void SetStep3()
	{
		if(AudioPlayer.Instance != null)
		{
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#76_G_D",Vector3.zero,false,SetStep4);
			AudioPlayer.Instance.StopAudio(audioPath+"Yindaoyu_#75_G_D");
		}
		StopShowHand();
		if(maskRightTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskRightTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskRightTexture.gameObject.SetActive (false);
		SwitchAllLeftCollider(false,-1);
	}

	void SetStep4(string str)
	{
		if(maskCenterTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskCenterTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskCenterTexture.gameObject.SetActive (false);

		maskRightAllTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha = TweenAlpha.Begin(maskRightAllTexture.gameObject,0.2f,0f);
		tAlpha.from = 0.4f;
		tAlpha.style = UITweener.Style.PingPong;
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#77_G_D",Vector3.zero,false,SetStep5);
	}

	void SetStep5(string str)
	{
		Debug.Log(str);
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#78_G_D",Vector3.zero,false,null);
		if(maskRightAllTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskRightAllTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskRightAllTexture.gameObject.SetActive (false);

		maskBottomTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha = TweenAlpha.Begin(maskBottomTexture.gameObject,0.2f,0f);
		tAlpha.from = 0.4f;
		tAlpha.style = UITweener.Style.PingPong;
		if (handTexture != null && anchor_BottomLeft != null)
		{
			handTexture.transform.parent = anchor_BottomRight.transform;
			handTexture.transform.localPosition = new Vector3 (910f,12f,0f);
		}
		StartShowHand();
		SwitchAllRightCollider(true,2);
	}

	void StartSetStep6()
	{
		SwitchAllRightCollider(false,-1);
		Invoke("SetStep6",0.5f);
	}

	void SetStep6()
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#79_G_D",Vector3.zero,false,SetStep7);
	}

	void SetStep7(string str)
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#80_G_D",Vector3.zero,false,null);
		if(maskBottomTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskBottomTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskBottomTexture.gameObject.SetActive(false);
		StopShowHand();

		maskLeftAllTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha = TweenAlpha.Begin(maskLeftAllTexture.gameObject,0.2f,0f);
		tAlpha.from = 0.4f;
		tAlpha.style = UITweener.Style.PingPong;

		maskRightAllTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha1 = TweenAlpha.Begin(maskRightAllTexture.gameObject,0.2f,0f);
		tAlpha1.from = 0.4f;
		tAlpha1.style = UITweener.Style.PingPong;
	
		boxCollider.enabled = true;
		UIEventListener.Get (boxCollider.gameObject).onClick = SetStep8;
	}

	void SetStep8(GameObject go)
	{
		SwitchAllLeftCollider (true, -1);
		SwitchAllRightCollider (true, -1);
		maskBackTexture.alpha = 0f;
		boxCollider.enabled = false;
		if(maskLeftAllTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskLeftAllTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskLeftAllTexture.gameObject.SetActive (false);

		if(maskRightAllTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskRightAllTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskRightAllTexture.gameObject.SetActive (false);
		timer = 0f;
		InvokeRepeating("ShowExitButton",0,1);
	}

	void ShowExitButton()
	{
		timer++;
		if(timer.Equals(2))
		{
			if(exitButton != null)
			{
				exitButton.gameObject.SetActive(true);
				maskTopLeftTexture.gameObject.SetActive(true);
				TweenAlpha tAlpha = TweenAlpha.Begin(maskTopLeftTexture.gameObject,0.2f,0f);
				tAlpha.from = 0.4f;
				tAlpha.style = UITweener.Style.PingPong;
				if(AudioPlayer.Instance != null)
					AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#81_G_D",Vector3.zero,false,null);
			}
		}
		if(timer.Equals(120))
		{
			FireworkGameLogicGuide.Instance.Exit();
		}
	}

	public void HideExitButtonShowTexture()
	{
		if(exitButton.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(exitButton.gameObject.GetComponent<TweenAlpha>());
		}
	}
}
