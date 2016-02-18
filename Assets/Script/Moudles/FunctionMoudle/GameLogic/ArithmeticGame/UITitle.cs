using UnityEngine;
using System.Collections;

public class UITitle : MonoBehaviour {

	public UISprite digitalSpriteBack;
	public UISprite digitalSprite;

	void Awake()
	{
		if(digitalSprite == null)
			digitalSprite = transform.FindChild("Title").GetComponent<UISprite>();
		if(digitalSpriteBack == null)
			digitalSpriteBack = transform.FindChild("TitleBack").GetComponent<UISprite>();
	}

	public void SetDigitalSprite(int digital)
	{
		if(digitalSprite == null) return;

		digitalSprite.alpha = 1;
		digitalSprite.spriteName = "a"+digital.ToString();
	}

	public void SetSetDigitalSpriteBack(string spriteName)
	{
		if(digitalSpriteBack == null || spriteName=="") return;

		digitalSpriteBack.spriteName = spriteName;
	}
}
