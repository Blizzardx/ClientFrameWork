using UnityEngine;
using System.Collections;

public class UIPlateDragDropItem : UIDragDropItem {

	UIPlate plate;

	void Awake()
	{
		if (plate == null)
			plate = transform.GetComponent<UIPlate>();;
	}

	protected override void OnDragDropMove (Vector3 delta)
	{
		if(plate != null)
		{
			if(plate.canDrag == false) return;
		}
		if(UIArithmeticGameManager.gameOver) return;
		plate.backSprite.depth = 8;
		plate.icon.depth = 9;
		mTrans.localPosition += delta;
	}
}
