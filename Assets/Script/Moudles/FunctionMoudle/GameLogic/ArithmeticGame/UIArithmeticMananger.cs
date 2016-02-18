using UnityEngine;
using System.Collections;

public class UIArithmeticMananger : MonoBehaviour {

	public UISprite leftSprite, rightSprite, resultSprite1,resultSprite2;
	int leftdata,rightdata,resultdata;
	public static UIArithmeticMananger instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start()
	{
		InitObject();
	}

	void InitObject()
	{
		if(leftSprite == null)
			leftSprite = transform.FindChild("1/Icon").GetComponent<UISprite>();
		if(rightSprite == null)
			rightSprite = transform.FindChild("2/Icon").GetComponent<UISprite>();
		if(resultSprite1 == null)
			resultSprite1 = transform.FindChild("3/Icon1").GetComponent<UISprite>();
		if(resultSprite2 == null)
			resultSprite2 = transform.FindChild("3/Icon2").GetComponent<UISprite>();
	}
	
	public void ResetSprite()
	{
		if(leftSprite == null)
			leftSprite = transform.FindChild("1/Icon").GetComponent<UISprite>();
		leftSprite.alpha = 0;
		leftdata = 0;

		if(rightSprite == null)
			rightSprite = transform.FindChild("2/Icon").GetComponent<UISprite>();
		rightSprite.alpha = 0;
		rightdata = 0;

		if(resultSprite1 == null)
			resultSprite1 = transform.FindChild("3/Icon1").GetComponent<UISprite>();
		resultSprite1.alpha = 0;
		if(resultSprite2 == null)
			resultSprite2 = transform.FindChild("3/Icon2").GetComponent<UISprite>();
		resultSprite2.alpha = 0;
		resultdata = 0;
	}

	public void SetArithmeticLeftAndRight(PlateData data)
	{
		if (leftdata.Equals (0)) {
			leftSprite.spriteName = "a"+data.answer.ToString ();
			leftSprite.alpha = 1;
			leftdata = data.answer;
		} else {
			if(rightdata.Equals(0))
			{
				rightSprite.spriteName = "a"+data.answer.ToString();
				rightSprite.alpha = 1;
				rightdata = data.answer;
			}
		}

		if(leftSprite.alpha.Equals(1) && rightSprite.alpha.Equals(1))
		{
			if(UIPlateManager.instance != null)
				UIPlateManager.instance.PlateColliderSwitch(false);
			resultdata = leftdata+rightdata;
			Invoke("CheckResult",1f);
		}
	}

	void CheckResult()
	{
		if(resultdata < 10)
		{
			if(resultSprite1 != null)
			{
				resultSprite1.spriteName = "a"+resultdata.ToString();
				resultSprite1.alpha = 1;
				resultSprite1.transform.localPosition = new Vector3(0,14,0);
				resultSprite1.MakePixelPerfect();
			}
		}else{
			int ten = resultdata/10;
			int one = resultdata%10;
			if(resultSprite1 != null)
			{
				resultSprite1.spriteName = "a"+ten.ToString();
				resultSprite1.alpha = 1;
				resultSprite1.transform.localPosition = new Vector3(-15,14,0);
				resultSprite1.width = 30;
			}
			if(resultSprite2 != null)
			{
				resultSprite2.spriteName = "a"+one.ToString();
				resultSprite2.alpha = 1;
				resultSprite2.transform.localPosition = new Vector3(15,14,0);
				resultSprite2.width = 30;
			}
		}

		if(resultdata.Equals(UIPlateManager.instance.curQuestionData.result))
		{
			if(UIPlateManager.instance != null)
			{
				UIPlateManager.instance.PlateMoveDown();
			}
			UIArithmeticGameManager.rightCount++;
			UIArithmeticGameManager.wrongCount = 0;
			if(UIArithmeticGameManager.rightCount.Equals(1))
			{
				if(AdaptiveDifficultyManager.Instance != null)
				{
					AdaptiveDifficultyManager.Instance.SetUserTalent("Correct",30);
				}
			}
			if(UIArithmeticGameManager.rightCount >=2)
			{
				if(AdaptiveDifficultyManager.Instance != null)
				{
					AdaptiveDifficultyManager.Instance.SetUserTalent("Correct2",30);
				}
			}
		}else{
			CancelInvoke("DisposeWrong");
			Invoke("DisposeWrong",0.3f);
		}
	}

	void DisposeWrong()
	{
		ResetSprite();
		if(UIPlateManager.instance != null)
		{
			UIArithmeticGameManager.Instance.SetWrong();
			UIPlateManager.instance.PlateColliderSwitch(true);
			UIPlateManager.instance.PlateAlphaReturn();
			PlayWrongAnimation();
		}
		UIArithmeticGameManager.wrongCount++;
		UIArithmeticGameManager.rightCount = 0;
		if(UIArithmeticGameManager.wrongCount.Equals(1))
		{
			if(AdaptiveDifficultyManager.Instance != null)
			{
				AdaptiveDifficultyManager.Instance.SetUserTalent("Wrong",30);
			}
		}
		if(UIArithmeticGameManager.wrongCount >=2)
		{
			if(AdaptiveDifficultyManager.Instance != null)
			{
				AdaptiveDifficultyManager.Instance.SetUserTalent("Wrong2",30);
			}
		}
	}

	void PlayWrongAnimation()
	{
		if(PlayMonsterAnimations.instance != null)
			PlayMonsterAnimations.instance.ShakeHead();
	}

}
