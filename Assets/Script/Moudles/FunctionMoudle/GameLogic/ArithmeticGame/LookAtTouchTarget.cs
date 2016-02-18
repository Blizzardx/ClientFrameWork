using UnityEngine;
using System.Collections;

public class LookAtTouchTarget : MonoBehaviour {
	
	public Transform target;
	public float speed = 1f;
	
	Transform mTrans;

	public static LookAtTouchTarget instance;

	[HideInInspector]
	public bool isDrag = false;
	[HideInInspector]
	public bool canDrag = false;

	public Camera camera;
	public Animator animator;
	public GameObject orcObject;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}
	
	void Start ()
	{
		mTrans = transform;
		canDrag = true;
	}

	public void ResetRotation()
	{
		if(animator != null)
		{
			animator.transform.localRotation = Quaternion.identity;
		}
		if(orcObject != null)
		{
			orcObject.transform.localRotation = Quaternion.identity;
		}
	}
	
	void LateUpdate ()
	{
		if(canDrag == false) return;

		if(isDrag)
		{
			if(Application.platform == RuntimePlatform.WindowsEditor || 
			   Application.platform == RuntimePlatform.WindowsPlayer)
			{
				Vector3 pos = Input.mousePosition;
				pos.x -= Screen.width * 0.5f;
				pos.y -= Screen.height * 0.5f;
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				target.transform.localPosition = pos;
			}
			if(Application.platform == RuntimePlatform.Android || 
			   Application.platform == RuntimePlatform.IPhonePlayer)
			{
				Vector3 pos = Input.GetTouch(0).position;
				pos.x -= Screen.width * 0.5f;
				pos.y -= Screen.height * 0.5f;
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				target.transform.localPosition = pos;
			}
			if (target != null)
			{
				Vector3 tmpPos = new Vector3(target.transform.localPosition.x,
//				                             -Mathf.Abs(target.transform.localPosition.y),
				                             -Screen.height/2,
				                             target.transform.localPosition.z);
				Vector3 dir = tmpPos - mTrans.position;
				float mag = dir.magnitude;
				
				if (mag > 0.001f)
				{
					Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x,0,dir.y));
					mTrans.rotation = Quaternion.Slerp(mTrans.rotation, lookRot, Mathf.Clamp01(speed * Time.deltaTime));
				}
			}
		}else{
			mTrans.localEulerAngles = Vector3.Lerp(mTrans.localRotation.eulerAngles,
			                                       new Vector3(0,180,0),0.1f);
			ResetRotation();
		}
	}
}
