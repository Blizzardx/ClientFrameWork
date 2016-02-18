using UnityEngine;
using System.Collections;

public class UIArithmeticTipsManager : MonoBehaviour {

	public static UIArithmeticTipsManager instance;
	private BoxCollider boxCollider;
	UIPanel panel;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start()
	{
		InitObject();
		ShowMark(false);
	}

	void InitObject()
	{
		if(boxCollider == null)
			boxCollider = transform.GetComponent<BoxCollider>();
		if(panel == null)
			panel = transform.GetComponent<UIPanel>();
	}

	public void ShowMark(bool show)
	{
		if (show) 
		{
			TweenAlpha tAlpha = TweenAlpha.Begin (panel.gameObject, 0.2f, 1);
			tAlpha.from = 0;
			tAlpha.style = UITweener.Style.Once;
			boxCollider.enabled = true;
		} else {
			TweenAlpha tAlpha = TweenAlpha.Begin (panel.gameObject, 0.2f, 0);
			tAlpha.from = 1;
			tAlpha.style = UITweener.Style.Once;
			boxCollider.enabled = false;
		}
	}
}
