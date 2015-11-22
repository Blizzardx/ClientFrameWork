using UnityEngine;
using System.Collections;

public class Done_GameController : MonoBehaviour
{
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	
	public GUIText playerHPText;
	public GUIText enemyHPText;
	public GUIText gameOverText;
	
	private bool gameOver;
	private int score;

	public GameObject FlightFly,FlightUpAndDown;
	public Transform asteroidParent;

	public Done_PlayerController playerController;

	void Start ()
	{
		gameOver = false;
		score = 0;
		StartCoroutine (SpawnWaves ());
	}
	
	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0; i < hazardCount; i++)
			{
				GameObject hazard = hazards [Random.Range (0, hazards.Length)];
				Vector3 spawnPosition = new Vector3 
				(
					Random.Range (-spawnValues.x, spawnValues.x), 
					Random.Range (-spawnValues.y, spawnValues.y), 
					spawnValues.z
				);
				Quaternion spawnRotation = Quaternion.identity;
				GameObject go = (GameObject)Instantiate (hazard, spawnPosition, spawnRotation);
				go.transform.parent = asteroidParent;
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
			
			if (gameOver)
			{
				break;
			}
		}
	}

	public void ShowPlayerLife(int life)
	{
		if(playerHPText != null)
			playerHPText.text = "PlayerHP:"+life.ToString();
	}

	public void ShowEnemyLife(int life)
	{
		if(enemyHPText != null)
			enemyHPText.text = "EnemyHP:"+life.ToString();
	}
	
	public void GameOver()
	{
		gameOverText.text = "Game Over!";
		gameOver = true;
	}

	public void Victory()
	{
		gameOverText.text = "Victory!";
		gameOver = true;
	}

	public void Land()
	{
		if (playerController != null)
		{
			playerController.SetGameEnd();
		}
		for(int i=0;i<asteroidParent.childCount;i++)
		{
			if(asteroidParent.GetChild(i).GetComponent<Done_DestroyByContact>())
			{
				asteroidParent.GetChild(i).GetComponent<Done_DestroyByContact>().DestoryBySelf();
			}
		}
		iTween.CameraFadeTo(iTween.Hash("amount", 1.0f, "time", 1.0f, "delay", 1.0f));
		Invoke("StartLanding",3f);
	}

	void StartLanding()
	{
		iTween.CameraFadeTo(iTween.Hash("amount", 0.0f, "time", 1.0f, "delay", 0.0f));
		if(FlightFly != null)
		{
			FlightFly.SetActive(false);
		}
		if(FlightUpAndDown != null)
		{
			FlightUpAndDown.SetActive(true);
		}
		if(Done_PlayerUpDownController.instance != null)
		{
			Done_PlayerUpDownController.instance.SetUpAndDown(false);
		}
	}
}