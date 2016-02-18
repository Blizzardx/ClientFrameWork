using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Config;
using Config.Table;

[System.Serializable]
public class QuestionData
{
	public List<int> answerList;
	public int rightOne;
	public int rightTwo;
	public int result;
}

[System.Serializable]
public class PlateData
{
	public int answer;
}

public class UIPlateManager : MonoBehaviour {

	public List<UIPlate> plateList;
	public List<UITitle> titleList;
	public GameObject originPlate,originTitle;
	public static UIPlateManager instance;
	[HideInInspector]
	public QuestionData curQuestionData;
	public UISprite resultSprite1,resultSprite2;

	public GameObject MoveObject,titleObject;

	void Awake()
	{
		if (instance == null)
			instance = this;
		InitObject();
	}

	void InitObject()
	{
		if (MoveObject == null)
			MoveObject = transform.FindChild("MoveManager").gameObject;
		if (titleObject == null)
			titleObject = transform.FindChild("TitleManager").gameObject;
		if (originPlate == null)
			originPlate = MoveObject.transform.FindChild("OriginTree").gameObject;
		if (originTitle == null)
			originTitle = titleObject.transform.FindChild ("OriginTitle").gameObject;
	}

	public void InitData(ArithmeticQuestion arithmeticQuestion)
	{
		if(arithmeticQuestion == null) return;
		InitObject();
		curQuestionData.answerList.Clear();
		string tmpStr = arithmeticQuestion.OptionContent;
		string[] str = tmpStr.Split(',');
		for(int i=0;i<str.Length;i++)
		{
			curQuestionData.answerList.Add(int.Parse(str[i]));
		}
		for(int i=0;i<arithmeticQuestion.ItemList.Count;i++)
		{
			switch(i)
			{
				case 0:
				curQuestionData.rightOne = arithmeticQuestion.ItemList[i].Expression;
				break;
				case 1:
				curQuestionData.rightTwo = arithmeticQuestion.ItemList[i].Expression;
				break;
			}
		}
		curQuestionData.result = curQuestionData.rightOne + curQuestionData.rightTwo;

		if(originPlate != null && originTitle != null)
		{
			DestoryPlate();
			for(int i=0;i<curQuestionData.answerList.Count;i++)
			{
				GameObject go = (GameObject)Instantiate(originPlate);
				go.transform.parent = MoveObject.transform;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;
				UIPlate plate = go.transform.GetComponent<UIPlate>();
				if(plate != null)
					plateList.Add(plate);

				go = (GameObject)Instantiate(originTitle);
				go.transform.parent = titleObject.transform;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;
				UITitle title = go.transform.GetComponent<UITitle>();
				if(title != null)
					titleList.Add(title);
			}
			SetPlatePosition();
		}
	}

	public void InitRound(int round)
	{
		if(!curQuestionData.answerList.Count.Equals(plateList.Count))
			return;

		for(int i=0;i<plateList.Count;i++)
		{
			if(plateList[i] != null)
			{
				plateList[i].data.answer = curQuestionData.answerList[i];
				plateList[i].SetSprite();
			}
		}

		for(int i=0;i<titleList.Count;i++)
		{
			if(titleList[i] != null)
			{
				titleList[i].SetDigitalSprite(curQuestionData.answerList[i]);
			}
		}

		switch(titleList.Count)
		{
			case 3:
				for(int i=0;i<titleList.Count;i++)
				{
					if(i<1)
					{
						titleList[i].SetSetDigitalSpriteBack("zuochapai");
					}
					else if(i==1)
					{
						titleList[i].SetSetDigitalSpriteBack("zhongjianchapai");
					}
					else
					{
						titleList[i].SetSetDigitalSpriteBack("youchapai");
					}
				}
				break;

			case 4:
				for(int i=0;i<titleList.Count;i++)
				{
					if(i<2)
					{
						titleList[i].SetSetDigitalSpriteBack("zuochapai");
					}else{
						titleList[i].SetSetDigitalSpriteBack("youchapai");
					}
				}
				break;

			case 5:
				for(int i=0;i<titleList.Count;i++)
				{
					if(i<2)
					{
						titleList[i].SetSetDigitalSpriteBack("zuochapai");
					}
					else if(i==2)
					{
						titleList[i].SetSetDigitalSpriteBack("zhongjianchapai");
					}
					else
					{
						titleList[i].SetSetDigitalSpriteBack("youchapai");
					}
				}
				break;

			default:
				Debug.Log("Error Option Count!");
				break;
		}

		SetResultSprite();
		PlateMoveUp();
	}

	public void PlateAlphaReturn()
	{
		for(int i=0;i<plateList.Count;i++)
		{
			plateList[i].SetSprite();
		}
	}

	public void PlateColliderSwitch(bool open)
	{
		for(int i=0;i<plateList.Count;i++)
		{
			if(plateList[i] != null)
				plateList[i].SetColliderEnable(open);
		}
	}

	public void SetAllPlateBack()
	{
		for(int i=0;i<plateList.Count;i++)
		{
			if(plateList[i] != null)
				plateList[i].ResetPostion(false);
		}
	}

	void SetResultSprite()
	{
		if(resultSprite1 == null)
			resultSprite1 = transform.FindChild("Prompt/Icon1").GetComponent<UISprite>();
		if(resultSprite2 == null)
			resultSprite2 = transform.FindChild("Prompt/Icon2").GetComponent<UISprite>();

		resultSprite1.alpha = 0;
		resultSprite2.alpha = 0;

		int resultdata = curQuestionData.result;
		if(resultdata < 10)
		{
			if(resultSprite1 != null)
			{
				resultSprite1.spriteName = "p"+resultdata.ToString();
				resultSprite1.alpha = 1;
				resultSprite1.transform.localPosition = new Vector3(-6,22,0);
				resultSprite1.width = 171;
			}
			if(resultSprite2 != null)
			{
				resultSprite2.alpha =0;
			}
		}else{
			int ten = resultdata/10;
			int one = resultdata%10;
			if(resultSprite1 != null)
			{
				resultSprite1.spriteName = "p"+ten.ToString();
				resultSprite1.alpha = 1;
				resultSprite1.transform.localPosition = new Vector3(-60,22,0);
				resultSprite1.width = 130;
			}
			if(resultSprite2 != null)
			{
				resultSprite2.spriteName = "p"+one.ToString();
				resultSprite2.alpha = 1;
				resultSprite2.transform.localPosition = new Vector3(60,22,0);
				resultSprite2.width = 130;
			}
		}
	}

	public void PlateMoveDown()
	{
		if(MoveObject == null) return;

		CancelInvoke("SetEnd");
		TweenPosition tPos = TweenPosition.Begin(MoveObject,0.3f,new Vector3(0,-1000,0));
		tPos.from = Vector3.zero;
		tPos.style = UITweener.Style.Once;

		Invoke("SetRoundEnd",0.3f);
	}

	public void PlateMoveUp()
	{
		if(MoveObject == null) return;

		CancelInvoke("SetRoundStart");

		TweenPosition tPos = TweenPosition.Begin(MoveObject, 0.3f, Vector3.zero);
		tPos.from = new Vector3(0,-1000,0);
		tPos.style = UITweener.Style.Once;

		Invoke("SetRoundStart",0.3f);
	}

	void SetPlatePosition()
	{
		if(plateList.Count <=0 || plateList == null) return;

		float cellWidth = 0;
		float cellHeight = 0;
		Vector3 originPos =Vector3.zero;
		switch(plateList.Count)
		{
		   case 3:
				cellWidth = 305;
				cellHeight = 52.5f;
				originPos = new Vector3(-310,-207.5f,0);
				for(int i=0;i<plateList.Count;i++)
				{
					if(i<2)
						plateList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y-i*cellHeight,0);
					else
						plateList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y,0);
					plateList[i].gameObject.SetActive(true);
				}

				cellWidth = 327.5f;
				cellHeight = 54;
				originPos = new Vector3(-327.5f,-54,0);
				for(int i=0;i<titleList.Count;i++)
				{
					if(i<2)
						titleList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y-i*cellHeight,0);
					else
						titleList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y,0);
					titleList[i].gameObject.SetActive(true);
				}
				break;

			case 4:
				cellWidth = 300;
				cellHeight = 70;
				originPos = new Vector3(-420,-175,0);
				for(int i=0;i<plateList.Count;i++)
				{
					if(i<2)
						plateList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y-i*cellHeight,0);
					else
						plateList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,-245+(i-2)*cellHeight,0);
					plateList[i].gameObject.SetActive(true);
				}

				for(int i=0;i<titleList.Count;i++)
				{
					if(i<2)
					{
						originPos = new Vector3(-425,-15,0);
						cellWidth = 250;
						cellHeight = 73;
						titleList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y-i*cellHeight,0);
					}else{
						originPos = new Vector3(235,-88,0);
						cellWidth = 275;
						cellHeight = 73;
						titleList[i].gameObject.transform.localPosition = new Vector3(originPos.x+(i-2)*cellWidth,originPos.y+(i-2)*cellHeight,0);
					}
					titleList[i].gameObject.SetActive(true);
				}
				break;

			 case 5:
				cellWidth = 305;
				cellHeight = 52.5f;
				originPos = new Vector3(-615,-155,0);
				for(int i=0;i<plateList.Count;i++)
				{
					if(i<3)
						plateList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y-i*cellHeight,0);
					else
						plateList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,-260+(i-2)*cellHeight,0);
					plateList[i].gameObject.SetActive(true);
				}

				cellWidth = 327.5f;
				cellHeight = 54;
				originPos = new Vector3(-655,0,0);
				for(int i=0;i<titleList.Count;i++)
				{
					if(i<3)
						titleList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,originPos.y-i*cellHeight,0);
					else
						titleList[i].gameObject.transform.localPosition = new Vector3(originPos.x+i*cellWidth,-108+(i-2)*cellHeight,0);
					titleList[i].gameObject.SetActive(true);
				}
				break;
		}
	}

	void SetRoundStart()
	{
		PlateColliderSwitch(true);
	}

	void SetRoundEnd()
	{
		if (!UIArithmeticGuideManager.isGuide) 
		{
			if(UIArithmeticGuideManager.Instance != null)
			{
				UIArithmeticGuideManager.Instance.PlayRightResult();
			}
		} else {
			if(UIArithmeticGameManager.Instance != null)
				UIArithmeticGameManager.Instance.SetNextRound();
		}
	}

	void DestoryPlate()
	{
		if(plateList.Count <=0 || plateList == null) return;

		for(int i=0;i<plateList.Count;i++)
		{
			Destroy(plateList[i].gameObject);
		}
		plateList.Clear();

		for(int i=0;i<titleList.Count;i++)
		{
			Destroy(titleList[i].gameObject);
		}
		titleList.Clear();
	}
}
