using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFlowerManager : MonoBehaviour {

	public UIGrid uiGrid;
	public UISprite originSprite;
	public static UIFlowerManager instance;
	private List<GameObject> flowerObjectList;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start()
	{
		InitObject();
		InitFlower();
	}

	void InitObject()
	{
		if(uiGrid == null)
			uiGrid = transform.GetComponentInChildren<UIGrid>();
		if(originSprite == null)
			originSprite = transform.FindChild("OriginSprite").GetComponent<UISprite>();
	}

	void InitFlower()
	{
		if(UIArithmeticGameManager.Instance == null) return;

		flowerObjectList = new List<GameObject>();
		int max = UIArithmeticGameManager.Instance.life;
		for(int i=0;i<max;i++)
		{
			GameObject go = (GameObject)Instantiate(originSprite.gameObject);
			go.SetActive(true);
			go.transform.parent = uiGrid.transform;
			go.name = (i+1).ToString();
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			flowerObjectList.Add(go);
		}
		if(uiGrid != null)
			uiGrid.Reposition();
	}

	public void ResetFlower()
	{
		if(flowerObjectList == null) return;

		for(int i=0;i<flowerObjectList.Count;i++)
		{
			if(flowerObjectList[i] != null)
			{
				if(flowerObjectList[i].GetComponent<TweenPosition>())
					Destroy(flowerObjectList[i].GetComponent<TweenPosition>());
				flowerObjectList[i].transform.localPosition = new Vector3(uiGrid.cellWidth*i,0,0);
			}
		}
	}

	public void RemoveFlower(int life)
	{
		for(int i=flowerObjectList.Count-1;i>=0;i--)
		{
			if(i.Equals(life))
			{
				TweenPosition tPos = TweenPosition.Begin(flowerObjectList[i],1f,new Vector3(transform.position.x,1000,0));
				tPos.from = flowerObjectList[i].transform.localPosition;
				tPos.style = UITweener.Style.Once;
			}
		}
	}
}
