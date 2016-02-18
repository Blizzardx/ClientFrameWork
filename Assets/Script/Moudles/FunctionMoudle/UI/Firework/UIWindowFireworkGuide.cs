using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;

public class UIWindowFireworkGuide : WindowBase {

	private class ColorItem
	{
		public GameObject       obj;
		public Color            color;
	}
	private List<ColorItem> m_ColorList;
	private UISprite m_ColorPanel;
	private GameObject m_BlockBg;
	private List<FireworkPlanElement> m_FireworkPlanList;
	
	public override void OnInit()
	{
		base.OnInit();
		
		m_ColorList = new List<ColorItem>();
		for (int i = 0; i < 5; ++i)
		{
			ColorItem elem = new ColorItem();
			elem.obj = FindChild("Sprite_Color"+(i));
			UIEventListener.Get(elem.obj).onClick = OnDropEnd;
			var texture = ComponentTool.FindChildComponent<UITexture>("Sprite_Color", elem.obj);
			elem.color = texture.color;
			
			m_ColorList.Add(elem);
		}
		
		m_FireworkPlanList = new List<FireworkPlanElement>();
		for (int i = 0; i < 7; ++i)
		{
			var objRoot = FindChild("Type_" + i);
			FireworkPlanElement elem = new FireworkPlanElement(objRoot);
			elem.SetStatus(true, i<3);
			m_FireworkPlanList.Add(elem);
			UIEventListener.Get(objRoot).onClick = OnClickFire; ;
		}
		
		m_ColorPanel = FindChildComponent<UISprite>("Sprite_ColorPanel");
		m_BlockBg = FindChild("BlockBg");
		AddChildElementClickEvent(OnClickExit,"Button_Exit");
	}
	private void OnClickExit(GameObject go)
	{
		if(FireworkGuideManager.Instance != null)
		{
			FireworkGuideManager.Instance.FinishGuide();
		}
		if(AudioPlayer.Instance != null)
			AudioPlayer.Instance.PlayAudio("GUIDE/FireworkGame/Yindaoyu_#82_G_D",Vector3.zero,false,ConfirmExit);
	}

	private void ConfirmExit(string str)
	{
		FireworkGameLogicGuide.Instance.Exit();
	}

	private void OnDropEnd(GameObject go)
	{
		ColorItem selectedItem = null;
		foreach (var elem in m_ColorList)
		{
			if (elem.obj == go)
			{
				selectedItem = elem;
				break;
			}
		}
		
		selectedItem.obj.SetActive(false);
		
		Color res = Color.white;
		for (int i = 0; i < m_ColorList.Count; ++i)
		{
			if (!m_ColorList[i].obj.activeSelf)
			{
				res = ColorCombine(res ,m_ColorList[i].color);   
			}
		}
		//combine color
		m_ColorPanel.color = res;
		if(go != null)
		{
			FireworkEvent fireworkEvent = go.GetComponent<FireworkEvent>();
			if(fireworkEvent != null)
				fireworkEvent.OnGameObjectClick();
			if(FireworkGuideManager.Instance != null)
			{
				if(FireworkGuideManager.Instance.timer >60)
				{
					FireworkGuideManager.Instance.HideExitButtonShowTexture();
				}
			}
		}
	}

	private void OnClickFire(GameObject go)
	{
		//check 
//		if (!Check(go))
//		{
//			return;
//		}
		string name = go.name.Substring(5);
		int id = 0;
		if (!int.TryParse(name, out id))
		{
			Debuger.LogWarning("wrong name " + name);
			return;
		}
		//m_BlockBg.SetActive(true);
		FireworkGameLogicGuide.Instance.Fire("yanhua_" + (id + 1), m_ColorPanel.color);

		if(go != null)
		{
			FireworkEvent fireworkEvent = go.GetComponent<FireworkEvent>();
			if(fireworkEvent != null)
			{
				fireworkEvent.OnGameObjectClick();
			}
			if(FireworkGuideManager.Instance != null)
			{
				if(FireworkGuideManager.Instance.timer >60)
				{
					FireworkGuideManager.Instance.HideExitButtonShowTexture();
				}
			}
		}
	}

	public override void OnOpen(object param)
	{
		base.OnOpen(param);
	}
	public override void OnClose()
	{
		base.OnClose();
	}
	public void Reset()
	{
		foreach (var elem in m_ColorList)
		{
			elem.obj.SetActive(true);
		}
		m_ColorPanel.color = Color.white;
		//m_BlockBg.SetActive(false);
	}
	private Color ColorCombine(Color a, Color b)
	{
		/*if (a == Color.white)
        {
            a = Color.black;
        }
        Color res = new Color();
        float fr = 1f;
        float fg = 1f;
        float fb = 1f;

        if (a.r + b.r > 1)
        {
            fr = 2.0f;
        }
        if (a.g + b.g > 1)
        {
            fg = 2.0f;
        }
        if (a.b + b.b > 1)
        {
            fb = 2.0f;
        }
        res.r = (a.r + b.r) / fr;
        res.g = (a.g + b.g) / fg;
        res.b = (a.b + b.b) / fb;
        res.a = 1.0f;
        return res;
*/
		if (a == Color.white)
		{
			return b;
		}
		if (b == Color.white)
		{
			return a;
		}
		HSBColor hsbA = HSBColor.FromColor(a);
		HSBColor hsbB = HSBColor.FromColor(b);
		
		HSBColor res = new HSBColor(0.5f * (hsbA.h + hsbB.h), hsbB.s, hsbB.b);
		return res.ToColor();
	}
	private bool Check(GameObject obj)
	{
		for (int i = 0; i < m_FireworkPlanList.Count; ++i)
		{
			if (obj == m_FireworkPlanList[i].m_ObjRoot)
			{
				return m_FireworkPlanList[i].IsActive();
			}
		}
		return false;
	}
}
