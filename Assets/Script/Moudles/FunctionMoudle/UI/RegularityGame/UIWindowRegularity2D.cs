//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : UIWindowRegularity2D
//
// Created by : Baoxue at 2015/12/23 15:22:10
//
//
//========================================================================

using Config;
using Config.Table;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Regularity2DWindowParam
{
    public List<string>                     m_OptionList;
    public List<RegularityGameOption>       m_PilesList;
    public Action<bool>                     m_ResultCallBack;
}
public class UIWindowRegularity2D : WindowBase
{
    private class PilesElement
    {
        public UISprite m_SpriteItem;
        public GameObject m_RootObj;
        public bool m_bIsVisable;
        public string m_strName;
    }

    private class OptionElement
    {
        public UISprite m_SpriteItem;
        public GameObject m_RootObj;
        public string m_strName;
    }
    private GameObject  m_OptionTemplate;
    private GameObject  m_PilesTemplate;
    private UIGrid      m_OptionRoot;
    private UIGrid      m_PilesRoot;
    private Action<bool> m_ResultCallBack;
    private GameObject m_Character;
    private GameObject m_CharacterRootObj;
    private List<PilesElement> m_PilesList;
    private List<OptionElement> m_OptionList;
 
    public override void OnInit()
    {
        m_OptionTemplate = FindChild("OptionalTextureTemplate");
        m_PilesTemplate = FindChild("PilesTemplate");
        m_OptionRoot = FindChildComponent<UIGrid>("OptionalRoot");
        m_PilesRoot = FindChildComponent<UIGrid>("PilesRoot");
        m_Character = FindChild("Character");
        m_CharacterRootObj = FindChild("CharRoot");
        m_PilesList = new List<PilesElement>();
        m_OptionList = new List<OptionElement>();

        base.OnInit();
    }
    public void ResetWindow(Regularity2DWindowParam param)
    {
        ClearWindow();

        for (int i = 0; i < param.m_PilesList.Count; ++i)
        {
            var data = param.m_PilesList[i];
            PilesElement elem = new PilesElement();
            elem.m_RootObj = GameObject.Instantiate(m_PilesTemplate);
            elem.m_RootObj.transform.parent = m_PilesRoot.transform;
            elem.m_bIsVisable = data.IsVisable;
            elem.m_SpriteItem = ComponentTool.FindChildComponent<UISprite>("Sprite_Option", elem.m_RootObj);
            elem.m_SpriteItem.spriteName = elem.m_bIsVisable ? data.Name : "";
            elem.m_strName = data.Name;
            elem.m_RootObj.SetActive(true);
            elem.m_RootObj.transform.localScale = Vector3.one;
            m_PilesList.Add(elem);
        }

        for (int i = 0; i < param.m_OptionList.Count; ++i)
        {
            var data = param.m_OptionList[i];
            OptionElement elem = new OptionElement();
            elem.m_RootObj = GameObject.Instantiate(m_OptionTemplate);
            elem.m_RootObj.transform.parent = m_OptionRoot.transform;
            elem.m_SpriteItem = ComponentTool.FindChildComponent<UISprite>("Sprite_Option", elem.m_RootObj);
            elem.m_SpriteItem.spriteName = data;
            elem.m_RootObj.GetComponent<MyUIDragDropItem>().RegisterDragEndAction(OnDragEnd);
            elem.m_strName = data;
            elem.m_RootObj.SetActive(true);
            elem.m_RootObj.transform.localScale = Vector3.one;
            m_OptionList.Add(elem);
        }
        m_ResultCallBack = param.m_ResultCallBack;

        m_OptionRoot.repositionNow = true;
        m_PilesRoot.repositionNow = true;
    }
    public void ShowWin()
    {
        
    }
    public void ShowLose()
    {
        
    }
    private void OnDragEnd(MyUIDragDropItem item)
    {
        for (int i = 0; i < m_PilesList.Count; ++i)
        {
            //check drag over
            if (IsInRect(item.GetComponent<UIWidget>(), m_PilesList[i].m_RootObj.GetComponent<UIWidget>()))
            {
                OptionElement option = null;
                foreach (var elem in m_OptionList)
                {
                    if (elem.m_RootObj == item.gameObject)
                    {
                        option = elem;
                        break;
                    }
                }
                OnSelect(m_PilesList[i],option);
                break;
            }
        }
    }
    private void OnSelect(PilesElement piles,OptionElement option)
    {
        if (!piles.m_bIsVisable)
        {
            return;
        }
        piles.m_SpriteItem.spriteName = option.m_strName;
        option.m_RootObj.SetActive(false);
    }
    private void ClearWindow()
    {
        foreach (var elem in m_OptionList)
        {
            elem.m_RootObj.transform.parent = null;
            GameObject.Destroy(elem.m_RootObj);
        }
        m_OptionList.Clear();

        foreach (var elem in m_PilesList)
        {
            elem.m_RootObj.transform.parent = null;
            GameObject.Destroy(elem.m_RootObj);
        }
        m_PilesList.Clear();


    }
    private bool IsInRect(UIWidget des, UIWidget source)
    {
        float width = WindowManager.Instance.GetUIRoot().transform.localScale.x*source.width;
        float height = WindowManager.Instance.GetUIRoot().transform.localScale.x*source.height;

        if (des.transform.position.x > source.transform.position.x - width / 2 &&
            des.transform.position.x < source.transform.position.x + width / 2 &&
            des.transform.position.y > source.transform.position.y - height / 2 &&
            des.transform.position.y < source.transform.position.y + height / 2)
        {
            return true;
        }
        return false;
    }
    private bool IsRectCross(UIWidget des, UIWidget source)
    {
        float width = WindowManager.Instance.GetUIRoot().transform.localScale.x * source.width;
        float height = WindowManager.Instance.GetUIRoot().transform.localScale.x * source.height;
        float dwidth = WindowManager.Instance.GetUIRoot().transform.localScale.x * des.width;
        float dheight = WindowManager.Instance.GetUIRoot().transform.localScale.x * des.height;

        if (des.transform.position.x + dwidth / 2 > source.transform.position.x - width / 2 &&
            des.transform.position.x - dwidth / 2 < source.transform.position.x + width / 2 &&
            des.transform.position.y + dheight / 2 > source.transform.position.y - height / 2 &&
            des.transform.position.y - dheight / 2 < source.transform.position.y + height / 2)
        {
            return true;
        }
        return false;
    }
}

