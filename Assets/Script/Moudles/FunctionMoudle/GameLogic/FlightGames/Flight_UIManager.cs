using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flight_UIManager : SingletonTemplateMon<Flight_UIManager> {

	public UITexture grenadeTexture;
	private List<GameObject> playerHPObjectList;
	public GameObject playerHPManagerObject;
	public UISprite playerHPOriginSprite;
	public UIGrid playerHPGrid;
	public GameObject tipsManager;
	public UILabel Label;
	public GameObject slotManagerObject;

	void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(this);
	}

	public void Initialize()
	{
		InitObject();
		RegisterEvent();
	}

	void InitObject()
	{
		if (playerHPManagerObject == null)
			playerHPManagerObject = transform.FindChild("PlayerHPManager").gameObject;

		UIAnchor anchor = playerHPManagerObject.GetComponent<UIAnchor>();
		if(anchor != null)
		{
			anchor.uiCamera = WindowManager.Instance.GetUICamera();
			anchor.transform.localPosition = new Vector3(anchor.transform.localPosition.x,anchor.transform.localPosition.y,0f);
		}

		if(playerHPGrid == null)
			playerHPGrid = playerHPManagerObject.GetComponentInChildren<UIGrid>();
		if(playerHPOriginSprite == null)
			playerHPOriginSprite = playerHPManagerObject.transform.FindChild("OriginSprite").GetComponent<UISprite>();

		if(tipsManager == null)
			tipsManager = transform.FindChild("TipsManager").gameObject;
		if(Label == null)
			Label = tipsManager.GetComponentInChildren<UILabel>();

		if (slotManagerObject == null)
			slotManagerObject = transform.FindChild ("PlayerHPManager/SlotManager").gameObject;
		slotManagerObject.SetActive(false);
	}

	void InitHP(int hp)
	{
		playerHPObjectList = new List<GameObject>();
		int max = hp;
		for(int i=0;i<max;i++)
		{
			GameObject go = (GameObject)Instantiate(playerHPOriginSprite.gameObject);
			go.SetActive(true);
			go.transform.parent = playerHPGrid.transform;
			go.name = (i+1).ToString();
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			playerHPObjectList.Add(go);
		}
		if(playerHPGrid != null)
			playerHPGrid.Reposition();
		if (slotManagerObject != null)
			slotManagerObject.SetActive(true);
	}

	void RegisterEvent()
	{
		UIEventListener.Get(grenadeTexture.gameObject).onClick = OnGrenadeClick;
	}

	void OnGrenadeClick(GameObject go)
	{
		HideGrenade();
		if (Flight_StageController.Instance.stageState.Equals (StageState.CombatFlightGuide)) 
		{
			if(Flight_CombatFlightControllerGuide.instance != null)
			{
				Flight_CombatFlightControllerGuide.instance.HideWeapon();
				Flight_CombatFlightControllerGuide.instance.Fire();
			}
		}
		if (Flight_StageController.Instance.stageState.Equals(StageState.CombatFlight))
		{
			if(Flight_CombatFlightController.instance != null)
			{
				Flight_CombatFlightController.instance.HideWeapon();
				Flight_CombatFlightController.instance.Fire();
			}
		}
	}

	public void ShowGrenade()
	{
		TweenAlpha tAlpha = TweenAlpha.Begin(grenadeTexture.gameObject,0.2f,0.6f);
		tAlpha.from = 0f;
		tAlpha.style = UITweener.Style.PingPong;
		if(grenadeTexture.GetComponent<BoxCollider>())
		{
			grenadeTexture.GetComponent<BoxCollider>().enabled = true;
		}
	}

	public void InitializeHP(int hp)
	{
		if(playerHPObjectList == null)
			playerHPObjectList = new List<GameObject>();
		if(playerHPObjectList.Count.Equals(0))
			InitHP(hp);
		else
			ResetPlayerHP();
	}

	public void HideGrenade()
	{
		TweenAlpha tAlpha = TweenAlpha.Begin(grenadeTexture.gameObject,0.2f,0f);
		tAlpha.style = UITweener.Style.Once;
		if(grenadeTexture.GetComponent<BoxCollider>())
		{
			grenadeTexture.GetComponent<BoxCollider>().enabled = false;
		}
	}
	
	public void ResetPlayerHP()
	{
		if(playerHPObjectList == null) return;
		
		for(int i=0;i<playerHPObjectList.Count;i++)
		{
			if(playerHPObjectList[i] != null)
			{
				playerHPObjectList[i].transform.localPosition = new Vector3(playerHPGrid.cellWidth*i,0,0);
			}
		}
		if (slotManagerObject != null)
			slotManagerObject.SetActive(true);
	}

	public void RemovePlayerHP(int life)
	{
		for(int i=playerHPObjectList.Count-1;i>=0;i--)
		{
			if(i.Equals(life))
			{
				TweenPosition tPos = TweenPosition.Begin(playerHPObjectList[i],1f,new Vector3(transform.position.x,1000,0));
				tPos.from = playerHPObjectList[i].transform.localPosition;
				tPos.style = UITweener.Style.Once;
			}
		}
	}

	public void ShowLabel(string text)
	{
		if(tipsManager != null)
			tipsManager.SetActive(true);
		if (Label != null)
			Label.text = text;
	}

	public void ShowAll()
	{
		gameObject.SetActive(true);
		Initialize();
		if(tipsManager != null)
			tipsManager.SetActive(false);
	}

	public void HideAll()
	{
		gameObject.SetActive(false);
	}
}
