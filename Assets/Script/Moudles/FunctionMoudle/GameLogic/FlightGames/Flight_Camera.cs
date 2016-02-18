using UnityEngine;
using System.Collections;

public class Flight_Camera : MonoBehaviour {

	public GameObject target;
	public bool ShakeTerrain = false;
	[SerializeField]
	private float enableX,enableY,enablyZ,disableX,disableY,disableZ;
//	public float p,r;
	void Start()
	{
		target = this.gameObject;
	}

	void ShakeCamera(float fTime, Vector3 vAmount)
	{
		Hashtable hash = new Hashtable();
		hash.Add("time", fTime);
		hash.Add("amount", vAmount);
		hash.Add("islocal", true);
		iTween.ShakePosition(gameObject, hash);
	}

	void OnEnable()
	{
//		iTween.CameraFadeAdd();
//		iTween.CameraFadeTo(iTween.Hash("amount", 0f, "time", 1.0f, "delay", 0.0f));
		iTween.Stop(gameObject);
		float fTime = 10000f;
		ShakeCamera(fTime,new Vector3(enableX,enableY,enablyZ));
	}

	void OnDisable()
	{
		iTween.Stop(gameObject);
		float fTime = 0.5f;
		ShakeCamera(fTime,new Vector3(disableX,disableY,disableZ));
	}

//	void FixedUpdate () 
//	{
//		if(target != null && playerDown != null)
//		{
//			if(playerDown.moveCamera)
//			{
//
//			}
//				if(target.transform.localPosition.z>2f)
//				{
//					target.transform.localPosition = new Vector3
//					(
//							target.transform.localPosition.x,target.transform.localPosition.y,target.transform.localPosition.z-p
//					);
//				}
//				if(target.transform.localEulerAngles.x <60f)
//				{
//					target.transform.localEulerAngles = new Vector3
//					(
//						target.transform.localEulerAngles.x+r,target.transform.localEulerAngles.y,target.transform.localEulerAngles.z
//					);
//				}
//			}else{
//				if(target.transform.localPosition.z<4f)
//				{
//					target.transform.localPosition = new Vector3
//					(
//						target.transform.localPosition.x,target.transform.localPosition.y,target.transform.localPosition.z+p
//					);
//				}
//				if(target.transform.localEulerAngles.x >45f)
//				{
//					target.transform.localEulerAngles = new Vector3
//					(
//						target.transform.localEulerAngles.x-r,target.transform.localEulerAngles.y,target.transform.localEulerAngles.z
//					);
//				}
//			}
//		}
//	}
}
