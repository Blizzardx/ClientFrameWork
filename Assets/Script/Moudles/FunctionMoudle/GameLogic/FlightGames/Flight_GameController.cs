using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

[System.Serializable]
public class Obstacle
{
	public Vector3 position;
}

public class Flight_GameController : MonoBehaviour
{
	public GameObject[] hazards;
//	public Vector3 spawnValues;
	public List<Obstacle> obstacleList;
	public float spawnWait;
	public float startWait;
//	[HideInInspector]
	public static float waveWait;
	
	public bool gameOver;
	
	public Transform asteroidParent;

	public Flight_CombatFlight combatFlight;
	public Flight_CombatFlightController playerController;
	public Flight_EnemyController enemyController;
	public Flight_UIManager uiManager;
	public Flight_CombatFlightAudio combatFlightAudio;

	List<Obstacle> currentObstacleList;
	List<int> tempIntList;
	int temp=-1;

	public Camera roleCamera;
	public float spawnCount;

	void OnEnable()
	{
		gameOver = false;
		currentObstacleList = new List<Obstacle>();
		tempIntList = new List<int>();
		if(roleCamera != null)
			roleCamera.gameObject.SetActive(true);
		Invoke("StartSpawnWaves",1f);
		Invoke("ShowUI",1f);
		if (enemyController != null)
		{
			enemyController.gameObject.SetActive(true);
			enemyController.meshObject.SetActive(true);
			enemyController.life = 3;
			enemyController.isFlicker = false;
			ShowEnemyLife(enemyController.life);
//			enemyController.SetFireAnimator();
		}
		if(playerController != null)
		{
			playerController.gameObject.SetActive(true);
			playerController.life = 3;
			playerController.isFlicker = false;
			playerController.SetAnimator();
			playerController.SetDefaultPosition();
		}
	}

	public void StartSpawnWaves()
	{
		StopAllCoroutines();
		StartCoroutine (SpawnWaves ());
	}

	public void ShowUI()
	{
		uiManager.ShowAll();
		uiManager.InitializeHP(playerController.life);
	}
	
	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			currentObstacleList.Clear();
			tempIntList.Clear();
			if(temp != -1)
			{
				currentObstacleList.Add(obstacleList[temp]);
				tempIntList.Add(temp);
			}
			temp = -1;
			while(currentObstacleList.Count <spawnCount)
			{
				int i = Random.Range(0,obstacleList.Count-1);
				if(!currentObstacleList.Contains(obstacleList[i]))
				{
					currentObstacleList.Add(obstacleList[i]);
					tempIntList.Add(i);
				}
			}

			for(int i=0;i<obstacleList.Count;i++)
			{
				if(i<tempIntList.Count)
				{
					if(!tempIntList.Contains(i))
					{
						temp = i;
					}
				}else{
					if(temp == -1)
						temp = i;
				}
			}
			enemyController.SetThrowAnimator(true);
			for (int i = 0; i < currentObstacleList.Count; i++)
			{
				GameObject hazard = hazards [Random.Range (0, hazards.Length)];
				if(i <currentObstacleList.Count)
				{
					Vector3 spawnPosition = currentObstacleList[i].position;
					//				(
					//					Random.Range (-spawnValues.x, spawnValues.x), 
					//					Random.Range (-spawnValues.y, spawnValues.y), 
					//					spawnValues.z
					//				);
					Quaternion spawnRotation = Quaternion.identity;
					GameObject go = (GameObject)Instantiate (hazard, spawnPosition, spawnRotation);
					go.transform.parent = asteroidParent;
				}
				yield return new WaitForSeconds (spawnWait);
			}
			enemyController.SetThrowAnimator(false);
			yield return new WaitForSeconds (waveWait);
			
			if (gameOver || playerController.useGrenade)
			{
				break;
			}
		}
	}

	public void ShowPlayerLife(int life)
	{
		if(Flight_UIManager.Instance != null)
			Flight_UIManager.Instance.RemovePlayerHP(life);
	}

	public void ShowEnemyLife(int life)
	{

	}
	
	public void GameOver()
	{
//		if(uiManager != null)
//			uiManager.ShowLabel("Game Over!");
		Action<bool> fun = (res) =>
		{
			ResetGame();
			WindowManager.Instance.HideWindow(WindowID.LosePanel);
		};
		WindowManager.Instance.OpenWindow(WindowID.LosePanel, fun);
		gameOver = true;
		if(playerController != null)
		{
			playerController.gameObject.SetActive(false);
			playerController.HideWeapon();
		}
		if(enemyController != null)
			enemyController.gameObject.SetActive(false);
		DestoryAsteroid();
		//1111111111
		if(AdaptiveDifficultyManager.Instance != null)
		{
			AdaptiveDifficultyManager.Instance.SetUserTalent("Lose",40);
		}
	}

	public void Victory()
	{
		gameOver = true;
		if(playerController != null)
		{
//			playerController.gameObject.SetActive(false);
			playerController.HideWeapon();
			playerController.SetCollider(false);
		}
		if(enemyController != null)
			enemyController.gameObject.SetActive(false);
		DestoryAsteroid();
		//1111111111
		if(AdaptiveDifficultyManager.Instance != null)
		{
			AdaptiveDifficultyManager.Instance.SetUserTalent("Win",40);
		}
		if (!PlayerManager.Instance.GetCharCounterData ().GetFlag (4)) 
		{
			if(Flight_AudioManager.Instance != null)
			{
				Flight_AudioManager.Instance.ChangeAudioStep(AudioStep.Yindaoyu38);
			}
			CancelInvoke("ToLanding");
			Invoke("ToLanding",2f);
		}else{
			if(combatFlightAudio != null)
			{
				combatFlightAudio.PlayCombatFlightAudio(2);
			}
		}
	}

	public void ToLanding()
	{
		Action<bool> fun = (res) =>
		{
			Land();
			WindowManager.Instance.HideWindow(WindowID.WinPanel);
		};
		WindowManager.Instance.OpenWindow(WindowID.WinPanel, fun);
	}

	public void ResetGame()
	{
		Flight_StageController.userControl = false;
		DestoryAsteroid();
		playerController.gameObject.SetActive (false);
		enemyController.gameObject.SetActive(false);
		Invoke("HideUI",1f);
		Invoke("RestartGame", 1f);
	}

	public void DestoryAsteroid()
	{
		for(int i=0;i<asteroidParent.childCount;i++)
		{
			if(asteroidParent.GetChild(i).GetComponent<Flight_DestroyByContact>())
			{
				asteroidParent.GetChild(i).GetComponent<Flight_DestroyByContact>().DestoryBySelf();
			}
		}
	}

	public void RestartGame()
	{
		if(combatFlight != null)
		{
			OnEnable();
			combatFlight.Initialization();
		}
		Flight_StageController.userControl = true;
	}
	      
	public void Land()
	{
		DestoryAsteroid();
//		iTween.CameraFadeAdd();
//		iTween.CameraFadeTo(iTween.Hash("amount", 1.0f, "time", 1.0f, "delay", 1.0f));
		Invoke("HideUI",1f);
		Invoke("HideRoleCamera",1f);
		Invoke("StartLanding",1f);
	}

	public void HideUI()
	{
		if(uiManager != null)
			uiManager.HideAll();
	}

	void HideRoleCamera()
	{
		if(roleCamera != null)
			roleCamera.gameObject.SetActive(false);
	}

	void StartLanding()
	{
		if(Flight_StageController.Instance != null)
			Flight_StageController.Instance.StartSetStageState(StageState.FreeFlightDown);
	}
}