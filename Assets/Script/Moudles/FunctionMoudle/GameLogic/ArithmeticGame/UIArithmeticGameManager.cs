using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Config;
using Config.Table;
using System;
using Random = UnityEngine.Random;

public class UIArithmeticGameManager : SingletonTemplateMon<UIArithmeticGameManager> {

	private int round=1;
	public int life = 3;

	List<ArithmeticQuestion> arithmeticQuestionList = new List<ArithmeticQuestion>();
	ArithmeticQuestion arithmeticQuestion;
	List<ArithmeticTimer> arithmeticTimerList = new List<ArithmeticTimer>();
	int timer =0;

	[SerializeField]
	private float timerDifficulty;

	public Transform parentTrans;
	public GameObject sceneObject;
	public static float rightCount;
	public static float wrongCount;

	Dictionary<string,ArithmeticQuestion> usedQuestionDic = new Dictionary<string, ArithmeticQuestion>();
	public float panelResultTime;
	public static bool gameOver = false;
	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
//		Initialize();
	}

	public void Initialize()
	{
		rightCount = 0;
		wrongCount = 0;
		UICamera uiCamera = WindowManager.Instance.GetUICamera().GetComponent<UICamera>();
		usedQuestionDic.Clear();
		if(uiCamera != null)
		{
			uiCamera.allowMultiTouch = false;
		}
		if (parentTrans != null && WindowManager.Instance.UIWindowCameraRoot != null)
			parentTrans.parent = WindowManager.Instance.UIWindowCameraRoot.transform;
		parentTrans.transform.localPosition = Vector3.zero;
		arithmeticQuestionList = ConfigManager.Instance.GetArithmeticConfigTable().QuestionList;
		arithmeticTimerList = ConfigManager.Instance.GetArithmeticConfigTable().TimerList;
//		SetTimer();
		SetQuestion();
		if (!UIArithmeticGuideManager.isGuide) 
		{
			arithmeticQuestion = new ArithmeticQuestion();
			arithmeticQuestion.Difficulty = 0.1f;
			arithmeticQuestion.OptionContent = "4,1,2";
			arithmeticQuestion.ItemList = new List<ArithmeticItem>();
			ArithmeticItem item = new ArithmeticItem();
			item.Expression = 1;
			item.Operation= "+";
			arithmeticQuestion.ItemList.Add(item);

			ArithmeticItem item1 = new ArithmeticItem();
			item1.Expression = 2;
			item1.Operation= "";
			arithmeticQuestion.ItemList.Add(item1);
			if(UIArithmeticGuideManager.Instance != null)
			{
				UIArithmeticGuideManager.Instance.StartGuide();	
			}
		}

		if (UIPlateManager.instance != null)
		{
			UIPlateManager.instance.InitData(arithmeticQuestion);
			UIPlateManager.instance.InitRound(round);
		}
		gameOver = false;
	}

	void SetTimer()
	{
		if (arithmeticTimerList == null || arithmeticTimerList.Count <= 0)
		{	
			timer = 180;
			return;
		}
		//1111111111
		GameDifficulty result = AdaptiveDifficultyManager.Instance.GetGameDifficulty("MathDiff",30);
		List<ArithmeticTimer> tempTimerList = new List<ArithmeticTimer> ();
		for(int i=0;i<arithmeticTimerList.Count;i++)
		{
			if(arithmeticTimerList[i].Difficulty >= result.MinDiff && arithmeticTimerList[i].Difficulty <= result.MaxDiff)
			{
				tempTimerList.Add(arithmeticTimerList[i]);
			}
		}
		if(tempTimerList.Count.Equals(1))
		{
			timerDifficulty = (float)arithmeticTimerList[0].Difficulty;
		}else{
			timerDifficulty = (float)arithmeticTimerList[Random.Range(0,tempTimerList.Count-1)].Difficulty;
		}

		for(int i=0;i<arithmeticTimerList.Count;i++)
		{
			if(Convert.ToInt32(timerDifficulty*10).Equals(Convert.ToInt32(arithmeticTimerList[i].Difficulty*10)))
			{
				timer = arithmeticTimerList[i].Timer;
			}
		}
	}

	public void ClearLocalQuestionData()
	{
		if(usedQuestionDic == null)
			usedQuestionDic = new Dictionary<string, ArithmeticQuestion>();
		else
			usedQuestionDic.Clear();
	}

	public void SetQuestion()
	{
		if (arithmeticQuestionList == null || arithmeticQuestionList.Count <= 0)
		{	
			return;
		}

		//1111111111
		GameDifficulty result = AdaptiveDifficultyManager.Instance.GetGameDifficulty("MathDiff",30);
		List<ArithmeticQuestion> arithmeticList = new List<ArithmeticQuestion> ();
		for(int i=0;i<arithmeticQuestionList.Count;i++)
		{
			if(arithmeticQuestionList[i].Difficulty >= result.MinDiff && arithmeticQuestionList[i].Difficulty <= result.MaxDiff)
			{
				arithmeticList.Add(arithmeticQuestionList[i]);
			}
		}

		if(arithmeticList.Count.Equals(1))
		{
			arithmeticQuestion = arithmeticList[0];
		}else{
			arithmeticQuestion = arithmeticList[Random.Range(0,arithmeticList.Count-1)];
			while(usedQuestionDic.ContainsKey(arithmeticQuestion.OptionContent))
			{
				arithmeticQuestion = arithmeticList[Random.Range(0,arithmeticList.Count-1)];
			}
			usedQuestionDic.Add(arithmeticQuestion.OptionContent,arithmeticQuestion);
		}
	}

	void SetVictoryPanel()
	{
		Action<bool> fun = (res) =>
		{
			if(res)
			{
				Restart();
				WindowManager.Instance.HideWindow(WindowID.WinPanel);
				if(UIArithmeticTipsManager.instance != null)
					UIArithmeticTipsManager.instance.ShowMark(false);
			}else{
                WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
            }
        };
		WindowManager.Instance.OpenWindow(WindowID.WinPanel, fun);
		if(UIArithmeticTipsManager.instance != null)
			UIArithmeticTipsManager.instance.ShowMark(true);
	}

	public void Victory()
	{
		Invoke("SetVictoryPanel",panelResultTime);
		gameOver = true;
		if(UIPlateManager.instance != null)
		{
			UIPlateManager.instance.SetAllPlateBack();
			UIPlateManager.instance.PlateColliderSwitch(false);
		}
		if(LookAtTouchTarget.instance != null)
		{
			LookAtTouchTarget.instance.canDrag = false;
		}
		//1111111111
		if(AdaptiveDifficultyManager.Instance != null)
		{
			AdaptiveDifficultyManager.Instance.SetUserTalent("Win",30);
		}
	}

	void SetFailurePanel()
	{
//		if(UIArithmeticTipsManager.instance != null)
//		{
//			string text = "Game Over!";
//			UIArithmeticTipsManager.instance.SetLabel(text);
//			UIArithmeticTipsManager.instance.ShowTips(true);
//		}
		Action<bool> fun = (res) =>
		{
			if(res)
			{
				Restart();
				WindowManager.Instance.HideWindow(WindowID.LosePanel);
				if(UIArithmeticTipsManager.instance != null)
					UIArithmeticTipsManager.instance.ShowMark(false);
			}else{
                WorldSceneDispatchController.Instance.ExecuteExitNodeGame();
                //StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
            }
        };
		WindowManager.Instance.OpenWindow(WindowID.LosePanel, fun);
		if(UIArithmeticTipsManager.instance != null)
			UIArithmeticTipsManager.instance.ShowMark(true);
	}
	
	public void Failure()
	{
		Invoke("SetFailurePanel",panelResultTime);
		gameOver = true;
		if(UIPlateManager.instance != null)
		{
			UIPlateManager.instance.SetAllPlateBack();
			UIPlateManager.instance.PlateColliderSwitch(false);
		}
		if(LookAtTouchTarget.instance != null)
		{
			LookAtTouchTarget.instance.canDrag = false;
		}
		//1111111111
		if(AdaptiveDifficultyManager.Instance != null)
		{
			AdaptiveDifficultyManager.Instance.SetUserTalent("Lose",30);
		}
	}

	public void Restart()
	{
		gameOver = false;
		round = 1;
		life = 3;
//		SetTimer();
		ClearLocalQuestionData();
		SetQuestion();
		if (UIPlateManager.instance != null)
		{
			UIPlateManager.instance.InitData(arithmeticQuestion);
			UIPlateManager.instance.InitRound(round);
		}
		if(UIArithmeticMananger.instance != null)
		{
			UIArithmeticMananger.instance.ResetSprite();
		}
		if(UIFlowerManager.instance != null)
		{
			UIFlowerManager.instance.ResetFlower();
		}
//		if(UIArithmeticTipsManager.instance != null)
//		{
//			UIArithmeticTipsManager.instance.SetLabel("");
//			UIArithmeticTipsManager.instance.ShowTips(false);
//		}
		if(LookAtTouchTarget.instance != null)
		{
			LookAtTouchTarget.instance.canDrag = true;
			LookAtTouchTarget.instance.ResetRotation();
		}
	}

	public void SetWrong()
	{
		if (life > 0)
			life--;
		if(UIFlowerManager.instance != null)
			UIFlowerManager.instance.RemoveFlower(life);
		if(life.Equals(0))
		{
			Failure();
		}
	}

	public void SetNextRound()
	{
		SetQuestion();
		round++;
		if (round > 3)
		{
			Victory();
		}else{
			if(UIPlateManager.instance != null)
			{
				UIPlateManager.instance.InitData(arithmeticQuestion);
				UIPlateManager.instance.InitRound(round);
			}
			if(UIArithmeticMananger.instance != null)
			{
				UIArithmeticMananger.instance.ResetSprite();
			}
		}
	}
}
