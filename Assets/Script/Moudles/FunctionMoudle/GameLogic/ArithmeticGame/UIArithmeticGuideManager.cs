using UnityEngine;
using System.Collections;

public class UIArithmeticGuideManager : SingletonTemplateMon<UIArithmeticGuideManager> {

	public static bool isGuide = false;

	public UITexture maskBackTexture;
	public UITexture maskTopTexture;
	public UITexture maskPlateTexture;
	public UITexture maskMonsterTexture;
	public UITexture maskPlateLeftTexture;
	public UITexture maskPlateRightTexture;
	public UITexture handTexture;
	public UISprite leftArrowSprite;
	public UISprite rightArrowSprite;
	public string audioPath ="GUIDE/30_ArithmeticGame/";
	BoxCollider boxCollider;
	int handIndex = 1;

	void Awake()
	{
		if (_instance == null)
			_instance = this;
		Initialization();
	}

	void Initialization()
	{
		if(boxCollider == null)
		{
			boxCollider = transform.GetComponent<BoxCollider>();
		}
		if(PlayerManager.Instance.GetCharCounterData().GetFlag(3))
		{
			isGuide = true;
			boxCollider.enabled = false;
		}else{
			isGuide = false;
			boxCollider.enabled = true;
		}
		if(maskBackTexture == null)
			maskBackTexture = transform.FindChild("maskBackTexture").GetComponent<UITexture>();
		if(maskTopTexture == null)
			maskTopTexture = transform.FindChild("maskTopTexture").GetComponent<UITexture>();
		if(maskPlateTexture == null)
			maskPlateTexture = transform.FindChild("maskPlateTexture").GetComponent<UITexture>();
		if(maskMonsterTexture == null)
			maskMonsterTexture = transform.FindChild("maskMonsterTexture").GetComponent<UITexture>();
		if(maskPlateLeftTexture == null)
			maskPlateLeftTexture = transform.FindChild("maskPlateLeftTexture").GetComponent<UITexture>();
		if(maskPlateRightTexture == null)
			maskPlateRightTexture = transform.FindChild("maskPlateRightTexture").GetComponent<UITexture>();
		if(handTexture == null)
			handTexture = transform.FindChild("handTexture").GetComponent<UITexture>();
		if(leftArrowSprite == null)
			leftArrowSprite = transform.FindChild("leftArrowSprite").GetComponent<UISprite>();
		if(rightArrowSprite == null)
			rightArrowSprite = transform.FindChild("rightArrowSprite").GetComponent<UISprite>();
		CloseAll();
	}

	void CloseAll()
	{
		if (maskBackTexture != null)
			maskBackTexture.alpha =0;
		if (maskTopTexture != null)
			maskTopTexture.gameObject.SetActive (false);
		if (maskPlateTexture != null)
			maskPlateTexture.gameObject.SetActive (false);
		if (maskMonsterTexture != null)
			maskMonsterTexture.gameObject.SetActive(false);
		if (maskPlateLeftTexture != null)
			maskPlateLeftTexture.gameObject.SetActive (false);
		if (maskPlateRightTexture != null)
			maskPlateRightTexture.gameObject.SetActive (false);
		if (handTexture != null)
			handTexture.gameObject.SetActive (false);
		if (leftArrowSprite != null)
			leftArrowSprite.gameObject.SetActive (false);
		if (rightArrowSprite != null)
			rightArrowSprite.gameObject.SetActive (false);
	}

	public void StartGuide()
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#140_G_D",Vector3.zero,false,SetStep1);
	}

	void SetStep1(string str)
	{
		if (maskBackTexture != null)
			maskBackTexture.alpha = 1f;
		if (maskTopTexture != null)
			maskTopTexture.gameObject.SetActive (true);
		TweenAlpha tAlpha = TweenAlpha.Begin(maskTopTexture.gameObject,0.2f,0f);
		tAlpha.from = 0.4f;
		tAlpha.style = UITweener.Style.PingPong;
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#141_G_D",Vector3.zero,false,SetStep2);
	}

	void SetStep2(string str)
	{
		if(maskTopTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskTopTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskTopTexture.alpha = 0.4f;
		maskTopTexture.gameObject.SetActive(false);
		if(maskPlateTexture != null)
			maskPlateTexture.gameObject.SetActive(true);
		StartCoroutine(ShowAllPlate());
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#142_G_D",Vector3.zero,false,SetStep3);
	}

	IEnumerator ShowAllPlate()
	{
		int i = 0;
		while(i<3)
		{
			if(maskPlateTexture != null)
			{
				if(UIPlateManager.instance != null)
				{
					if(UIPlateManager.instance.plateList.Count >0)
					{
						maskPlateTexture.transform.localPosition = UIPlateManager.instance.plateList[i].transform.localPosition;
						TweenAlpha tAlpha = TweenAlpha.Begin(maskPlateTexture.gameObject,0.2f,0f);
						tAlpha.from = 0.4f;
						tAlpha.style = UITweener.Style.PingPong;
						i++;
					}
				}
			}
			yield return new WaitForSeconds(1.3f);
		}
	}

	void SetStep3(string str)
	{
		if(maskPlateTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskPlateTexture.gameObject.GetComponent<TweenAlpha>());
		}
		maskPlateTexture.alpha = 0.4f;
		maskPlateTexture.gameObject.SetActive(false);

		maskMonsterTexture.gameObject.SetActive(true);
		maskPlateLeftTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha = TweenAlpha.Begin(maskPlateLeftTexture.gameObject,0.2f,0f);
		tAlpha.from = 0.4f;
		tAlpha.style = UITweener.Style.PingPong;

		maskPlateRightTexture.gameObject.SetActive(true);
		TweenAlpha tAlpha1 = TweenAlpha.Begin(maskPlateRightTexture.gameObject,0.2f,0f);
		tAlpha1.from = 0.4f;
		tAlpha1.style = UITweener.Style.PingPong;

		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#143_G_D",Vector3.zero,false,SetStep4);
	}

	void SetStep4(string str)
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#144_G_D",Vector3.zero,false,OpenPlateCollider);

		Vector3 tPosTo = new Vector3
		(
			UIPlateManager.instance.plateList[1].transform.localPosition.x,
			0,
			0
		);
		if(handTexture != null)
			handTexture.gameObject.SetActive(true);
		if(leftArrowSprite != null)
			leftArrowSprite.gameObject.SetActive(true);
		if(rightArrowSprite != null)
			rightArrowSprite.gameObject.SetActive(true);
		if (boxCollider != null)
			boxCollider.enabled = false;
		if(UIPlateManager.instance != null)
		{
			if(UIPlateManager.instance.plateList.Count >0)
			{
				UIPlateManager.instance.PlateColliderSwitch(false);
			}
		}
		TweenPosition tPos = TweenPosition.Begin(handTexture.gameObject,1f,tPosTo);
		tPos.from = UIPlateManager.instance.plateList[1].transform.localPosition;
		tPos.style = UITweener.Style.Loop;
		handIndex = 1;
		InvokeRepeating("ShowHandGuide",0f,1f);
	}

	void OpenPlateCollider(string str)
	{
		if(UIPlateManager.instance != null)
		{
			if(UIPlateManager.instance.plateList.Count >0)
			{
				for(int i=0;i<UIPlateManager.instance.plateList.Count;i++)
				{
					if(i!=0)
					{
						UIPlateManager.instance.plateList[i].GetComponent<BoxCollider>().enabled = true;
					}
				}
			}
		}
	}

	void ShowHandGuide()
	{
		TweenPosition tPos = handTexture.transform.GetComponent<TweenPosition>();

		if(UIPlateManager.instance != null)
		{
			if(UIPlateManager.instance.plateList.Count >0)
			{
				if(UIPlateManager.instance.plateList[handIndex].icon.alpha != 0f)
				{
					tPos.from = UIPlateManager.instance.plateList[handIndex].transform.localPosition;
					tPos.style = UITweener.Style.Loop;
				}
			}
		}

		if (UIPlateManager.instance.plateList[1].icon.alpha ==0)
		{
			handIndex = 2;
		}
		else if(UIPlateManager.instance.plateList[2].icon.alpha ==0)
		{
			handIndex = 1;
		}
		else if(UIPlateManager.instance.plateList[1].icon.alpha != 0 && UIPlateManager.instance.plateList[2].icon.alpha !=0)
		{
			if(handIndex==1)
				handIndex=2;
			else
				handIndex=1;
		}
	}

	public void PlayWrongDragAudio()
	{
		if(AudioPlayer.Instance != null)
		{
			if(!AudioPlayer.Instance.IsPlayingAudio(audioPath+"Yindaoyu_#147_G_D"))
			{
				AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#147_G_D",Vector3.zero,false,null);
			}
		}
	}

	public void PlayRightResult()
	{
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#145_G_D",Vector3.zero,false,SetFinishStep);
		CancelInvoke();
		if(maskPlateLeftTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskPlateLeftTexture.gameObject.GetComponent<TweenAlpha>());
			maskPlateLeftTexture.alpha = 0.4f;
		}
		if(maskPlateRightTexture.gameObject.GetComponent<TweenAlpha>())
		{
			Destroy(maskPlateRightTexture.gameObject.GetComponent<TweenAlpha>());
			maskPlateRightTexture.alpha = 0.4f;
		}
		if(handTexture.gameObject.GetComponent<TweenPosition>())
		{
			Destroy(handTexture.gameObject.GetComponent<TweenPosition>());
		}
		handTexture.transform.localPosition = Vector3.down * 300;
		if(leftArrowSprite != null)
			leftArrowSprite.gameObject.SetActive(false);
		if(rightArrowSprite != null)
			rightArrowSprite.gameObject.SetActive(false);
		CloseAll();
	}

	void SetFinishStep(string str)
	{
		isGuide = true;
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio(audioPath+"Yindaoyu_#149_G_D",Vector3.zero,false,null);
		if(UIArithmeticGameManager.Instance != null)
			UIArithmeticGameManager.Instance.SetNextRound();
	    PlayerManager.Instance.GetCharCounterData().SetFlag(3, true);
	}
}
