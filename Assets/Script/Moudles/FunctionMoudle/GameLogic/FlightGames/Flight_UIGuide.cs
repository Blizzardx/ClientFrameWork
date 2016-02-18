using UnityEngine;
using System.Collections;

public class Flight_UIGuide : MonoBehaviour {
	enum RollEnum
	{
		None,
		Left,
		Right,
	}
	public Texture upTexture1,upTexture2;
	public Texture frontTexture;
	public Texture downTexture1,downTexture2;
	public UISprite upArrow,downArrow,leftArrow,rightArrow;

	public string upArrow1,upArrow2,downArrow1,downArrow2,leftArrow1,leftArrow2,rightArrow1,rightArrow2;

	public UITexture guideTexture;
	public UITexture guideTextureTwo;
	public float time;
	public float repeatRate;
	public Vector3 targetRot;
	public float rollRepeatRate;
	public float changeMultiple;
	private bool showLeftAndRight = false;
	public UITexture backTexture,maskTextureLeft,maskTextureRight,maskTextureCenter;

	RollEnum rollenum = RollEnum.Left;
	void Start()
	{
		Initialization();
	}

	public void Initialization()
	{
		if(guideTexture == null)
			guideTexture = transform.FindChild("guideTexture").GetComponent<UITexture>();
		if(guideTextureTwo == null)
			guideTextureTwo = transform.FindChild("guideTextureTwo").GetComponent<UITexture>();
		if(upArrow == null)
			upArrow = transform.FindChild("upArrow").GetComponent<UISprite>();
		if(downArrow == null)
			downArrow = transform.FindChild("downArrow").GetComponent<UISprite>();
		if(leftArrow == null)
			leftArrow = transform.FindChild("leftArrow").GetComponent<UISprite>();
		if(rightArrow == null)
			rightArrow = transform.FindChild("rightArrow").GetComponent<UISprite>();
		if(backTexture == null)
			backTexture = transform.FindChild("backTexture").GetComponent<UITexture>();
		if(maskTextureLeft == null)
			maskTextureLeft = transform.FindChild("maskTextureLeft").GetComponent<UITexture>();
		if(maskTextureRight == null)
			maskTextureRight = transform.FindChild("maskTextureRight").GetComponent<UITexture>();
		if(maskTextureCenter == null)
			maskTextureCenter = transform.FindChild("maskTextureCenter").GetComponent<UITexture>();
		CloseAll();
	}

	void SetGuideTextureDefaultTransform()
	{
		if(guideTexture != null)
		{
			guideTexture.transform.localPosition = Vector3.zero;
			guideTexture.transform.localRotation = Quaternion.identity;
			guideTexture.transform.localScale = Vector3.one;
		}
		if(guideTextureTwo != null)
		{
			guideTextureTwo.transform.localPosition = Vector3.zero;
			guideTextureTwo.transform.localRotation = Quaternion.identity;
			guideTextureTwo.transform.localScale = Vector3.one;
		}
	}

	public void CloseAll()
	{
		if(guideTexture != null)
		{
			if(guideTexture.gameObject.activeSelf)
				guideTexture.gameObject.SetActive(false);
		}
		if(guideTextureTwo != null)
		{
			if(guideTextureTwo.gameObject.activeSelf)
				guideTextureTwo.gameObject.SetActive(false);
		}
		if(leftArrow != null)
		{
			if(leftArrow.gameObject.activeSelf)
				leftArrow.gameObject.SetActive(false);
		}
		if(rightArrow != null)
		{
			if(rightArrow.gameObject.activeSelf)
				rightArrow.gameObject.SetActive(false);
		}
		if(upArrow != null)
		{
			if(upArrow.gameObject.activeSelf)
				upArrow.gameObject.SetActive (false);
		}
		if(downArrow != null)
		{
			if(downArrow.gameObject.activeSelf)
				downArrow.gameObject.SetActive(false);
		}
		CloseAllMask();
		showLeftAndRight = false;
		SetGuideTextureDefaultTransform();
		CancelInvoke();
	}

	public void CloseAllMask()
	{
		if(backTexture != null)
		{
			if(backTexture.gameObject.activeSelf)
				backTexture.gameObject.SetActive(false);
		}
		if(maskTextureLeft != null)
		{
			if(maskTextureLeft.gameObject.activeSelf)
				maskTextureLeft.gameObject.SetActive(false);
		}
		if(maskTextureRight != null)
		{
			if(maskTextureRight.gameObject.activeSelf)
				maskTextureRight.gameObject.SetActive(false);
		}
		if(maskTextureCenter != null)
		{
			if(maskTextureCenter.gameObject.activeSelf)
				maskTextureCenter.gameObject.SetActive(false);
		}
	}

	public void ShowLeftMask()
	{
		CloseAllMask();
		if(backTexture != null)
		{
			backTexture.gameObject.SetActive(true);
		}
		if(maskTextureLeft != null)
		{
			maskTextureLeft.gameObject.SetActive(true);
		}
	}

	public void ShowRightMask()
	{
		CloseAllMask();
		if(backTexture != null)
		{
			backTexture.gameObject.SetActive(true);
		}
		if(maskTextureRight != null)
		{
			maskTextureRight.gameObject.SetActive(true);
		}
	}

	public void ShowCenterMask()
	{
		CloseAllMask();
		if(backTexture != null)
		{
			backTexture.gameObject.SetActive(true);
		}
		if(maskTextureCenter != null)
		{
			maskTextureCenter.gameObject.SetActive(true);
		}
	}

	public void SetUpGuide()
	{
		ClearEffect();
		if(guideTexture != null)
		{
			guideTexture.gameObject.SetActive(true);
			guideTexture.mainTexture = upTexture1;
			guideTexture.MakePixelPerfect();
		}
		if(guideTextureTwo != null)
		{
			guideTextureTwo.gameObject.SetActive(true);
			guideTextureTwo.mainTexture = upTexture2;
			guideTextureTwo.MakePixelPerfect();
		}
		SetGuideTextureDefaultTransform();
		if(leftArrow != null)
		{
			leftArrow.gameObject.SetActive(true);
			leftArrow.spriteName = upArrow1;
		}
		if (rightArrow != null)
		{
			rightArrow.gameObject.SetActive(true);
			rightArrow.spriteName = upArrow2;
		}
		if(upArrow != null)
		{
			upArrow.gameObject.SetActive (false);
		}
		if(downArrow != null)
		{
			downArrow.gameObject.SetActive(false);
		}
		CancelInvoke();
		InvokeRepeating("ShowUpGuide",time,repeatRate);
	}

	void ShowUpGuide()
	{
		if(guideTexture != null && guideTextureTwo != null)
		{
			if(guideTexture.gameObject.activeSelf)
			{
				guideTexture.gameObject.SetActive(false);
				guideTextureTwo.gameObject.SetActive(true);
			}else{
				guideTexture.gameObject.SetActive(true);
				guideTextureTwo.gameObject.SetActive(false);
			}
		}
	}

	public void SetDownGuide()
	{
		ClearEffect();
		if(guideTexture != null)
		{
			guideTexture.gameObject.SetActive(true);
			guideTexture.mainTexture = downTexture1;
			guideTexture.MakePixelPerfect();
		}
		if(guideTextureTwo != null)
		{
			guideTextureTwo.gameObject.SetActive(true);
			guideTextureTwo.mainTexture = downTexture2;
		}
		SetGuideTextureDefaultTransform();
		if(leftArrow != null)
		{
			leftArrow.gameObject.SetActive(true);
			leftArrow.spriteName = downArrow1;
		}
		if (rightArrow != null)
		{
			rightArrow.gameObject.SetActive(true);
			rightArrow.spriteName = downArrow2;
		}
		if(upArrow != null)
		{
			upArrow.gameObject.SetActive (false);
		}
		if(downArrow != null)
		{
			downArrow.gameObject.SetActive(false);
		}
		CancelInvoke();
		InvokeRepeating("ShowDownGuide",time,repeatRate);
	}

	void ShowDownGuide()
	{
		if(guideTexture != null && guideTextureTwo != null)
		{
			if(guideTexture.gameObject.activeSelf)
			{
				guideTexture.gameObject.SetActive(false);
				guideTextureTwo.gameObject.SetActive(true);
			}else{
				guideTexture.gameObject.SetActive(true);
				guideTextureTwo.gameObject.SetActive(false);
			}
		}
	}

	public void SetLeftGuide()
	{
		rollenum = RollEnum.Left;
		CancelInvoke();
		if(guideTexture != null)
		{
			guideTexture.gameObject.SetActive(true);
			guideTexture.mainTexture = frontTexture;
			guideTexture.MakePixelPerfect();
		}
		if(guideTextureTwo != null)
		{
			guideTextureTwo.gameObject.SetActive(false);
		}
		SetGuideTextureDefaultTransform();
		if(leftArrow != null)
		{
			leftArrow.gameObject.SetActive(false);
		}
		if (rightArrow != null)
		{
			rightArrow.gameObject.SetActive(false);
		}
		if(upArrow != null)
		{
			upArrow.gameObject.SetActive (true);
			upArrow.spriteName = leftArrow1;
		}
		if(downArrow != null)
		{
			downArrow.gameObject.SetActive(true);
			downArrow.spriteName = leftArrow2;
		}
		ShowLeftGuide();
	}

	void ShowLeftGuide()
	{
		Quaternion quaternion = Quaternion.Euler(targetRot);
		TweenRotation tRotation = TweenRotation.Begin(guideTexture.gameObject,rollRepeatRate,quaternion);
		tRotation.from = Vector3.zero; 
		tRotation.style = UITweener.Style.Loop;
	}

	public void SetRightGuide()
	{
		rollenum = RollEnum.Right;
		CancelInvoke();
		if(guideTexture != null)
		{
			guideTexture.gameObject.SetActive(true);
			guideTexture.mainTexture = frontTexture;
			guideTexture.MakePixelPerfect();
		}
		if(guideTextureTwo != null)
		{
			guideTextureTwo.gameObject.SetActive(false);
		}
		SetGuideTextureDefaultTransform();
		if(leftArrow != null)
		{
			leftArrow.gameObject.SetActive(false);
		}
		if (rightArrow != null)
		{
			rightArrow.gameObject.SetActive(false);
		}
		if(upArrow != null)
		{
			upArrow.gameObject.SetActive (true);
			upArrow.spriteName = rightArrow1;
		}
		if(downArrow != null)
		{
			downArrow.gameObject.SetActive(true);
			downArrow.spriteName = rightArrow2;
		}
		ShowRightGuide();
	}

	void ShowRightGuide()
	{
		Quaternion quaternion = Quaternion.Euler(new Vector3(targetRot.x,targetRot.y,targetRot.z*-1));
		Quaternion temp = Quaternion.Euler(new Vector3(0,0,-0.01f));
		TweenRotation tRotation = TweenRotation.Begin(guideTexture.gameObject,rollRepeatRate,quaternion);
		tRotation.from = temp.eulerAngles; 
		tRotation.style = UITweener.Style.Loop;
	}

	public void ClearEffect()
	{
		if(guideTexture.GetComponent<TweenRotation>())
		{
			Destroy(guideTexture.GetComponent<TweenRotation>());
		}
		CancelInvoke();
		StopAllCoroutines();
	}

	public void SetLeftAndRightGuide()
	{
		ClearEffect();
		showLeftAndRight = true;
		StartCoroutine(StartShowLeftAndRightGuide());
	}

	IEnumerator StartShowLeftAndRightGuide()
	{
		while(true)
		{
			if(rollenum.Equals(RollEnum.Left))
			{
				SetRightGuide();
			}else{
				SetLeftGuide();
			}
			yield return new WaitForSeconds(rollRepeatRate*changeMultiple);
			if(!showLeftAndRight)
			{
				break;
			}
		}
	}
}
