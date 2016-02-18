using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flight_GameControllerGuide : MonoBehaviour {

	public GameObject hazard;
	public List<Obstacle> obstacleList;
	public int hazardCount;
	public Transform asteroidParent;
	public List<GameObject> instantiateHazardsList;
	public float moveSpeed;
	public bool moving = false;
	public Vector3 originPosition;
	public Vector3 eventPosition;
	public Vector3 destoryPositon;
	public Flight_CombatFlightControllerGuide combatFlightControllerGuide;
	public Flight_EnemyControllerGuide enemyControllerGuide;
	public Flight_UIManager uiManager;
	private List<Obstacle> curObstacleList;

	[HideInInspector]
	public bool right = false;

	void OnEnable()
	{
		Invoke("ShowUI",1f);
	}

	public void Initialization()
	{
		curObstacleList = new List<Obstacle>();
		DestoryInstantiateHazards();
		asteroidParent.localPosition = originPosition;
		moving = false;
	}

	public void ShowUI()
	{
		uiManager.ShowAll();
	}

	public void SetLeftGuide()
	{
		right = false;
		Initialization();
		for(int i=0;i<obstacleList.Count;i++)
		{
			if(i != 3)
				curObstacleList.Add(obstacleList[i]);
		}
		for (int i = 0; i < hazardCount; i++)
		{
			GameObject go = (GameObject)Instantiate(hazard);
			go.transform.parent = asteroidParent;
			go.transform.localPosition = curObstacleList[i].position;;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			instantiateHazardsList.Add(go);
		}
		moving = true;
	}

	public void SetRightGuide()
	{
		right = true;
		Initialization();
		for(int i=0;i<obstacleList.Count;i++)
		{
			if(i != 4)
				curObstacleList.Add(obstacleList[i]);
		}
		for (int i = 0; i < hazardCount; i++)
		{
			GameObject go = (GameObject)Instantiate(hazard);
			go.transform.parent = asteroidParent;
			go.transform.localPosition = curObstacleList[i].position;;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			instantiateHazardsList.Add(go);
		}
		moving = true;
	}

	public void SetCombatGuide()
	{
		Initialization();

		if(Flight_AudioManager.Instance != null)
		{
			Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu36,null);
		}
		if(Flight_GuideManager.Instance.uiGuide != null)
		{
			Flight_GuideManager.Instance.uiGuide.CloseAll();
		}
		if(enemyControllerGuide != null)
		{
			enemyControllerGuide.ShowMesh();
		}
		CancelInvoke("StartCombatCudie");
		Invoke("StartCombatCudie",5f);
	}

	public void StartCombatCudie()
	{
		if(Flight_GuideManager.Instance != null)
			Flight_GuideManager.Instance.ChangeGuideStep(GuideStep.CombatFlightStep6);

		if(combatFlightControllerGuide != null)
		{
			combatFlightControllerGuide.ShowWeapon();
		}
	}

	void DestoryInstantiateHazards()
	{
		if(instantiateHazardsList.Count <=0) return;

		for(int i=0;i<instantiateHazardsList.Count;i++)
		{
			Destroy(instantiateHazardsList[i]);
		}
		instantiateHazardsList.Clear();
	}

	void Update() 
	{
		if(!moving) return;
		asteroidParent.localPosition = asteroidParent.localPosition + Vector3.back * moveSpeed;
		if(asteroidParent.transform.localPosition.z < destoryPositon.z)
		{
			if(Flight_GuideManager.Instance != null)
			{
				if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.CombatFlightStep4))
				{
					SetRightGuide();
					Flight_GuideManager.Instance.uiGuide.CloseAll();
				}else{
					if(Flight_GuideManager.Instance.guideStep.Equals(GuideStep.CombatFlightStep5))
					{
						SetCombatGuide();
					}
				}
			}
		}
	}
}
