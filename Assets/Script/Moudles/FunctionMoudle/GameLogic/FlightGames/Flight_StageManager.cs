using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TerrainScene
{
	public string flyUpTerrain;
	public string freeFlightUpTerrain;
	public string combatFlightTerrain;
	public string freeFlightDownTerrain;
	public string flyDownTerrain;
	public string landingTerrain;
}

public class Flight_StageManager : SingletonTemplateMon<Flight_StageManager> {

	public static int stageIndex =1;
	public string prefabPath = "BuildIn/Item/Flight/StageController";
	GameObject originGo,go;

	void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(this);
	}

	public void Initialize()
	{
		if(Resources.Load(prefabPath+stageIndex.ToString()))
		{
			originGo = (GameObject)Resources.Load(prefabPath+stageIndex.ToString());
			go = (GameObject)Instantiate(originGo);
			go.transform.parent = this.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			if(go.GetComponent<Flight_StageController>())
			{
				Flight_StageController stageController = go.GetComponent<Flight_StageController>();
				stageController.Initialize();
			}
		}
	}
}
