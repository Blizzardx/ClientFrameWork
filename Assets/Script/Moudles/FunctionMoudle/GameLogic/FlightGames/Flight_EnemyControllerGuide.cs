using UnityEngine;
using System.Collections;

public class Flight_EnemyControllerGuide : MonoBehaviour {

	public static Flight_EnemyControllerGuide instance;
	public Animator animator;
	public Vector3 shakeAmount;
	public Vector3 originPos;
	public GameObject originExplodeObject;
	public GameObject meshObject;
	GameObject explodeObject;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	public void Flicker()
	{
		SetExplodeAnimator();
		transform.localPosition = originPos;
		ShakeMesh(1);
		SetExplode();
	}

	public void SetExplodeAnimator()
	{
		animator.Play("beizha");
	}

	void SetExplode()
	{
		SetExplodeAnimator();
		CreateExplodeObject();
		HideMesh();
		CancelInvoke ("DestoryExplodeObject");
		Invoke("DestoryExplodeObject",1f);
	}

	void CreateExplodeObject()
	{
		if(originExplodeObject == null) return;
		
		explodeObject = (GameObject)Instantiate(originExplodeObject);
		explodeObject.transform.localPosition =transform.localPosition;
	}

	void DestoryExplodeObject()
	{
		if(explodeObject != null)
		{
			GameObject.Destroy(explodeObject);
			explodeObject = null;
		}

		if(Flight_StageController.Instance != null)
		{
			Flight_StageController.Instance.StartSetStageState(StageState.CombatFlight);
		}
		if(Flight_GuideManager.Instance != null)
		{
			if(Flight_GuideManager.Instance.uiGuide != null)
			{
				Flight_GuideManager.Instance.uiGuide.CloseAllMask();
			}
		}
	}

	public void SetDefault()
	{
		Hashtable hash = new Hashtable();
		hash.Add("time", 0);
		hash.Add("amount", shakeAmount);
		hash.Add("islocal", true);
		iTween.ShakePosition(meshObject.gameObject, hash);
	}
	
	void ShakeMesh(float fTime)
	{
		Hashtable hash = new Hashtable();
		hash.Add("time", fTime);
		hash.Add("amount", shakeAmount);
		hash.Add("islocal", true);
		iTween.ShakePosition(meshObject.gameObject, hash);
	}

	public void ShowMesh()
	{
		if (meshObject != null)
			meshObject.gameObject.SetActive (true);
	}

	public void HideMesh()
	{
		if (meshObject != null)
			meshObject.gameObject.SetActive (false);
	}
}
