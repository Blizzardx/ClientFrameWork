using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Config;
using Config.Table;
using System;
using Random = UnityEngine.Random;

public enum StageState
{
	None,
	FlyUp,
	FreeFlightUp,
	CombatFlightGuide,
	CombatFlight,
	FreeFlightDown,
	FlyDown,
	Landing
}

public enum Axis
{
	None,
	Horizontal,
	Vertical
}

public class Flight_StageController :  SingletonTemplateMon<Flight_StageController> {

	public GameObject flyUpObject;
	public GameObject freeFlightObject;
	public GameObject combatFlightObject;
	public GameObject flyDownObject;
	public GameObject landing;
	public GameObject combatFlightGuideObject;

	public StageState stageState;

	[SerializeField]
	private float hFullTiltAngle = 50f;
	[SerializeField]
	private float hCentreAngleOffset = 0f;
	float hAngle = 0f;
	public static float hAxisValue=0;

	[SerializeField]
	private float vFullTiltAngle = -35;
	[SerializeField]
	private float vCentreAngleOffset = 40;
	float vAngle = 0f;
	public static float vAxisValue=0;

	private float frequency;

	List<FlightGameConfig> flightDataList = new List<FlightGameConfig>();
	public Flight_GameController gameController;

	public GameObject resourcePlayerObject,instantiatePlayerObject;
	public UIRoot uiRoot;
	public static bool userControl = false;

	public static bool isGuide = false;

	public TerrainScene terrainScene;

	void Awake()
	{
		_instance = this;
	}

	public void Initialize()
	{
		isGuide = PlayerManager.Instance.GetCharCounterData().GetFlag(4);
		flightDataList = ConfigManager.Instance.GetFlightGameConfigTable().FlightConfigList;
		frequency = 0f;
		if(gameController == null)
			gameController = transform.GetComponentInChildren<Flight_GameController>();

		StartSetStageState(StageState.FlyUp);
		if(uiRoot == null)
			uiRoot = transform.GetComponentInChildren<UIRoot>();
		uiRoot.gameObject.SetActive(true);
		//1111111111
		if(AdaptiveDifficultyManager.Instance != null)
		{
			GameDifficulty result = AdaptiveDifficultyManager.Instance.GetGameDifficulty("ObsFreq",40);
			SetDifficultyFrequency(result);
		}
		if (gameController != null && WindowManager.Instance.UIWindowCameraRoot != null)
		{
			if(gameController.uiManager != null)
			{
				gameController.uiManager.transform.parent = WindowManager.Instance.UIWindowCameraRoot.transform;
				gameController.uiManager.transform.localPosition = Vector3.zero;
			}
		}
	}

	public void StartSetStageState(StageState state)
	{
		stageState = state;
		if(WindowManager.Instance != null)
			WindowManager.Instance.OpenWindow(WindowID.Loading);
		Invoke("OnSetStageState",1f);
	}

	void OnSetStageState()
	{
		SetTerrainState(stageState);
	}

	void SetStageState(StageState state)
	{
		switch(state)
		{
			case StageState.FlyUp:
				if(flyUpObject != null)
					flyUpObject.SetActive(true);
				if(freeFlightObject != null)
					freeFlightObject.SetActive(false);
				if(combatFlightObject != null)
					combatFlightObject.SetActive(false);
				if(flyDownObject != null)
					flyDownObject.SetActive(false);
				if(landing != null)
					landing.SetActive(false);
				if(combatFlightGuideObject != null)
					combatFlightGuideObject.SetActive(false);
				break;

			case StageState.FreeFlightUp:
				if(flyUpObject != null)
					flyUpObject.SetActive(false);
				if(freeFlightObject != null)
					freeFlightObject.SetActive(true);
				if(combatFlightObject != null)
					combatFlightObject.SetActive(false);
				if(flyDownObject != null)
					flyDownObject.SetActive(false);
				if(landing != null)
					landing.SetActive(false);
				if(combatFlightGuideObject != null)
					combatFlightGuideObject.SetActive(false);
				break;

			case StageState.CombatFlight:
				if(flyUpObject != null)
					flyUpObject.SetActive(false);
				if(freeFlightObject != null)
					freeFlightObject.SetActive(false);
				if(combatFlightObject != null)
					combatFlightObject.SetActive(true);
				if(flyDownObject != null)
					flyDownObject.SetActive(false);
				if(landing != null)
					landing.SetActive(false);
				if(combatFlightGuideObject != null)
					combatFlightGuideObject.SetActive(false);
				break;

			case StageState.FreeFlightDown:
				if(flyUpObject != null)
					flyUpObject.SetActive(false);
				if(freeFlightObject != null)
					freeFlightObject.SetActive(true);
				if(combatFlightObject != null)
					combatFlightObject.SetActive(false);
				if(flyDownObject != null)
					flyDownObject.SetActive(false);
				if(landing != null)
					landing.SetActive(false);
				if(combatFlightGuideObject != null)
					combatFlightGuideObject.SetActive(false);
				break;

			case StageState.FlyDown:
				if(flyUpObject != null)
					flyUpObject.SetActive(false);
				if(freeFlightObject != null)
					freeFlightObject.SetActive(false);
				if(combatFlightObject != null)
					combatFlightObject.SetActive(false);
				if(flyDownObject != null)
					flyDownObject.SetActive(true);
				if(landing != null)
					landing.SetActive(false);
				if(combatFlightGuideObject != null)
					combatFlightGuideObject.SetActive(false);
				break;

			case StageState.Landing:
				if(flyUpObject != null)
					flyUpObject.SetActive(false);
				if(freeFlightObject != null)
					freeFlightObject.SetActive(false);
				if(combatFlightObject != null)
					combatFlightObject.SetActive(false);
				if(flyDownObject != null)
					flyDownObject.SetActive(false);
				if(landing != null)
					landing.SetActive(true);
				if(combatFlightGuideObject != null)
					combatFlightGuideObject.SetActive(false);
				break;
			case StageState.CombatFlightGuide:
				if(flyUpObject != null)
					flyUpObject.SetActive(false);
				if(freeFlightObject != null)
					freeFlightObject.SetActive(false);
				if(combatFlightObject != null)
					combatFlightObject.SetActive(false);
				if(flyDownObject != null)
					flyDownObject.SetActive(false);
				if(landing != null)
					landing.SetActive(false);
				if(combatFlightGuideObject != null)
					combatFlightGuideObject.SetActive(true);
				break;

			default:
				Debug.Log("Error State!");
				break;
		}
	}

	void Update()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			hAngle = Mathf.Atan2(Input.acceleration.x, -Input.acceleration.y)*Mathf.Rad2Deg + hCentreAngleOffset;
			hAxisValue = Mathf.InverseLerp(-hFullTiltAngle, hFullTiltAngle, hAngle)*2 - 1;
			hAxisValue = float.Parse(hAxisValue.ToString("F1"));

			vAngle = Mathf.Atan2(Input.acceleration.z, -Input.acceleration.y)*Mathf.Rad2Deg + vCentreAngleOffset;
			vAxisValue = Mathf.InverseLerp(-vFullTiltAngle, vFullTiltAngle, vAngle)*2 - 1;
			vAxisValue = float.Parse(vAxisValue.ToString("F1"));
		}
	}

	public void SetDifficultyFrequency(GameDifficulty result)
	{
		List<FlightGameConfig> flightList = new List<FlightGameConfig> ();
		for(int i=0;i<flightDataList.Count;i++)
		{
			if(flightDataList[i].Difficultyid >= result.MinDiff && flightDataList[i].Difficultyid <= result.MaxDiff)
			{
				flightList.Add(flightDataList[i]);
			}
		}
		if(flightList.Count >0)
		{
			if(flightList.Count.Equals(1))
			{
				frequency = (float)flightList[0].Frequency;
			}else{
				frequency = (float)flightList[Random.Range(0,flightList.Count-1)].Frequency;
			}
			Debug.Log("1:"+frequency.ToString());
		}else{
			frequency = (float)flightDataList[0].Frequency;
			Debug.Log("2:"+frequency.ToString());
		}

		if(gameController != null)
		{
			Flight_GameController.waveWait = frequency;
		}
	}

	private IEnumerator StartLoadScene(string targetSceneName)
	{
		var res = Application.LoadLevelAsync(targetSceneName);
		yield return res;

		GameObject tempObj = GameObject.Find("MainCamera");
		if (null != tempObj)
		{
			tempObj.SetActive(false);
		}
		if(resourcePlayerObject == null)
			resourcePlayerObject = (GameObject)Resources.Load("BuildIn/Item/Flight/PlayerObject");
		if(instantiatePlayerObject == null)
			instantiatePlayerObject = (GameObject)Instantiate(resourcePlayerObject);
		userControl = true;
		SetStageState(stageState);
		if(WindowManager.Instance != null)
			WindowManager.Instance.HideAllWindow();
	}

	void SetTerrainState(StageState state)
	{
		userControl = false;
		switch(state)
		{
			case StageState.FlyUp:
				if(terrainScene.flyUpTerrain !="")
					StartCoroutine(StartLoadScene(terrainScene.flyUpTerrain));
//				StartCoroutine(StartLoadScene("Copenhagen 01"));
				break;

			case StageState.Landing:
				if(terrainScene.landingTerrain !="")
					StartCoroutine(StartLoadScene(terrainScene.landingTerrain));
//				StartCoroutine(StartLoadScene("Copenhagen 02"));
				break;

			case StageState.FreeFlightUp:
				gameController.DestoryAsteroid();
				if(terrainScene.freeFlightUpTerrain !="")
					StartCoroutine(StartLoadScene(terrainScene.freeFlightUpTerrain));
//				StartCoroutine(StartLoadScene("Copenhagen_feixng01"));
				break;

			case StageState.CombatFlight:
				if(terrainScene.combatFlightTerrain !="")
					StartCoroutine(StartLoadScene(terrainScene.combatFlightTerrain));
//				StartCoroutine(StartLoadScene("Copenhagen_feixng02"));
				break;

			case StageState.FreeFlightDown:
				gameController.DestoryAsteroid();
				if(terrainScene.freeFlightDownTerrain !="")
					StartCoroutine(StartLoadScene(terrainScene.freeFlightDownTerrain));
//				StartCoroutine(StartLoadScene("Copenhagen_feixng01"));
				break;

			case StageState.FlyDown:
				if(terrainScene.flyDownTerrain !="")
					StartCoroutine(StartLoadScene(terrainScene.flyDownTerrain));
				if(WindowManager.Instance != null)
					WindowManager.Instance.HideAllWindow();
				SetStageState(stageState);
				break;

			case StageState.CombatFlightGuide:
				if(terrainScene.landingTerrain !="")
					StartCoroutine(StartLoadScene(terrainScene.landingTerrain));
//				StartCoroutine(StartLoadScene("Copenhagen_feixng02"));
				break;

			default:
				Debug.Log("Error State!");
				break;
		}
	}
}
