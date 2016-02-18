using UnityEngine;
using System.Collections;

public class FireworkEvent : MonoBehaviour {

	public FireworkGuideManager fireworkGuideManager;
	public string messageName;
	
	public void OnGameObjectClick()
	{
		if(fireworkGuideManager != null && messageName !="")
		{
			fireworkGuideManager.SendMessage(messageName);
			messageName ="";
		}
	}
}
