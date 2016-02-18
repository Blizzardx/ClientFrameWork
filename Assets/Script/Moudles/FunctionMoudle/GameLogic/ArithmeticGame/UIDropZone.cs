using UnityEngine;
using System.Collections;

public class UIDropZone : MonoBehaviour {

	private BoxCollider boxCollider;
	UISprite sprite;
	UIPlate plate;

	void Start()
	{
		InitObject();
	}

	void InitObject()
	{
		if(boxCollider == null)
			boxCollider = transform.GetComponent<BoxCollider>();
	}

	void OnDrop(GameObject go)
	{
		if(go.transform.GetComponent<UIPlate>())
		{
			UIPlate plate = go.transform.GetComponent<UIPlate>();
			plate.backSprite.depth = 6;
			plate.icon.depth = 7;
			if(plate.icon != null)
			{
				plate.icon.transform.parent = this.transform;
				plate.icon.transform.localPosition = Vector3.zero;
				plate.icon.alpha = 0;
				sprite = plate.icon;
				this.plate = plate;
				if(plate.data.answer == UIPlateManager.instance.curQuestionData.rightOne ||
				   plate.data.answer == UIPlateManager.instance.curQuestionData.rightTwo)
				{
					plate.ResetPostion(false);
					plate.SetColliderEnable(false);
					SetCollierEnable(false);
					Invoke("SetSpriteRightBack",0.2f);
					if(PlayMonsterAnimations.instance != null)
					{
						PlayMonsterAnimations.instance.PlayEatAnimation();
					}
				}else{
					plate.ResetPostion(true);
					SetCollierEnable(false);
					plate.SetColliderEnable(false);
					Invoke("SetSpriteWrongBack",0.2f);
					if(PlayMonsterAnimations.instance != null)
					{
						PlayMonsterAnimations.instance.PlayEatAnimation();
					}
				}
			}
		}
	}

	void SetSpriteRightBack()
	{
		sprite.alpha = 0;
		SetCollierEnable (true);
		if(sprite != null && plate != null)
		{
			sprite.transform.parent = plate.transform;
			if(sprite.GetComponent<SpringPosition>())
			{
				SpringPosition sp = sprite.GetComponent<SpringPosition>();
				sp.enabled = true;
				sp.onFinished = plate.HideSpirte;
			}
		}
		if(UIArithmeticMananger.instance != null)
		{
			UIArithmeticMananger.instance.SetArithmeticLeftAndRight(plate.data);
		}
	}

	void SetSpriteWrongBack()
	{
		SetCollierEnable (true);
//		sprite.alpha = 1;
		sprite.alpha = 0;
		if(sprite != null && plate != null)
		{
			sprite.transform.parent = plate.transform;
			if(sprite.GetComponent<SpringPosition>())
			{
				SpringPosition sp = sprite.GetComponent<SpringPosition>();
				sp.enabled = true;
				sp.onFinished = plate.HideSpirte;
			}
		}
		if(UIArithmeticGameManager.Instance != null)
		{
			UIArithmeticMananger.instance.SetArithmeticLeftAndRight(plate.data);
		}
	}

	void SetCollierEnable(bool enable)
	{
		boxCollider.enabled = enable;
	}
}
