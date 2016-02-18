using UnityEngine;
using System.Collections;

public class UIReturnDropZone : MonoBehaviour {
	
	void OnDrop(GameObject go)
	{
		if(go.transform.GetComponent<UIPlate>())
		{
			UIPlate plate = go.transform.GetComponent<UIPlate>();
			plate.backSprite.depth = 6;
			plate.icon.depth = 7;
			plate.ResetPostion(true);
			if(!UIArithmeticGuideManager.isGuide)
			{
				if(UIArithmeticGuideManager.Instance != null)
				{
					UIArithmeticGuideManager.Instance.PlayWrongDragAudio();
				}
			}
		}
	}
}
