using UnityEngine;
using System.Collections;

public class UIPlate : MonoBehaviour {
	
	public PlateData data;
	public UISprite icon;
	public UISprite backSprite;
	private SpringPosition springPosition;
	[HideInInspector]
	public Vector3 originPos;
	private BoxCollider boxCollider;
	[HideInInspector]
	public bool canDrag = false;

	void Awake()
	{
		InitObject();
	}

	void InitObject()
	{
		if(icon == null)
			icon = transform.FindChild("Icon").GetComponent<UISprite>();
		if(backSprite == null)
			backSprite = transform.FindChild("Background").GetComponent<UISprite>();
		if(boxCollider == null)
			boxCollider = transform.GetComponent<BoxCollider>();

		if(springPosition == null)
			springPosition = transform.GetComponent<SpringPosition>();
		if (springPosition != null)
			springPosition.target = transform.localPosition;

		originPos = transform.localPosition;
	}

	void OnDragStart ()
	{
		if (LookAtTouchTarget.instance != null)
			LookAtTouchTarget.instance.isDrag = true;
	}

	void OnDragEnd()
	{
		if (LookAtTouchTarget.instance != null)
			LookAtTouchTarget.instance.isDrag = false;
	}

	void OnDrop(GameObject go)
	{
		UIPlate plate = go.transform.GetComponent<UIPlate>();
		if (plate != null)
		{
			plate.ResetPostion(true);
		}
	}

	public void SetSprite()
	{
		if(icon == null) return;
		if(data == null || data.answer <=0) return;

		icon.alpha = 0;
		icon.spriteName = data.answer.ToString();
		icon.alpha = 1;
		if(backSprite != null)
			backSprite.alpha = 1;
		SetColliderEnable (true);
		canDrag = true;
	}

	public void ResetPostion(bool canDrag)
	{
		if(springPosition != null)
			springPosition.enabled = true;
		this.canDrag = canDrag;
	}

	public void SetColliderEnable(bool active)
	{
		if(boxCollider == null)
			boxCollider = transform.GetComponent<BoxCollider>();

		boxCollider.enabled = active;
	}

	public void HideSpirte()
	{
		if(icon == null)
			icon = transform.FindChild("Icon").GetComponent<UISprite>();
		icon.alpha = 0;

		if(backSprite == null)
			backSprite = transform.FindChild("Background").GetComponent<UISprite>();
		backSprite.alpha = 0;

		SetColliderEnable (false);
	}
}
