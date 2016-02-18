using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIWindowMsgTreeSell:WindowBase
{
    private GameObject  m_ObjButtonRoot;
    private GameObject  m_ObjButtonOk;
    private GameObject  m_ObjButtonCancle;
    private int         m_iCurrentSellItemId;
    private UISprite m_SpriteMyIcon;
    private UISprite m_SpriteOtherIcon;
    private UILabel m_LabelMyName;
    private UILabel m_LabelOtherName;
    private List<FireworkPlanElement> m_FireworkPlanList;
    private GameObject m_ButtonTalk;
    private UIWidget m_DropPanel;

    public override void OnInit()
    {
        base.OnInit();

        m_SpriteMyIcon = FindChildComponent<UISprite>("Sprite_Icon0");
        m_SpriteOtherIcon = FindChildComponent<UISprite>("Sprite_Icon1");
        m_ObjButtonRoot = FindChild("ButtonRoot");
        m_ObjButtonOk = FindChild("Button_Ok");
        m_ObjButtonCancle = FindChild("Button_Cancle");
        m_LabelMyName = FindChildComponent<UILabel>("Label_Name1");
        m_LabelOtherName = FindChildComponent<UILabel>("Label_Name2");

        m_ButtonTalk = FindChild("Button_Talk");
        UIEventListener.Get(m_ButtonTalk).onPress = OnPressButton;
        UIEventListener.Get(m_ObjButtonOk).onClick = OnClickAccept;
        UIEventListener.Get(m_ObjButtonCancle).onClick = OnClickRefuse;

        AddChildElementClickEvent(OnClickBack, "Sprite_Exit");

        m_DropPanel = FindChildComponent<UIWidget>("DrapPanel");
        m_FireworkPlanList = new List<FireworkPlanElement>();
        for (int i = 0; i < 7; ++i)
        {
            var objRoot = FindChild("Type_" + i);
            FireworkPlanElement elem = new FireworkPlanElement(objRoot);
            elem.SetStatus(true, ItemManager.Instance.IsExistItem(i));
            m_FireworkPlanList.Add(elem);
            MyUIDragDropItem drag = objRoot.GetComponent<MyUIDragDropItem>();
            drag.RegisterDragEndAction(OnDragEnd);
            //UIEventListener.Get(objRoot).onClick = OnClickItem;;
        }

        m_ObjButtonRoot.SetActive(false);
        m_SpriteMyIcon.gameObject.SetActive(false);
        m_SpriteOtherIcon.gameObject.SetActive(false);
        m_ButtonTalk.SetActive(false);
        m_LabelOtherName.text = string.Empty;
        m_LabelMyName.text = PlayerManager.Instance.GetCharBaseData().CharName;
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        MessageTreeLogic.Instance.RegisterBidCallBack(OnBuyerBid);
        MessageTreeLogic.Instance.REgisterSellerOpenChatCallBack(OnCreateConnect);
        MessageTreeLogic.Instance.RegisterSellerInitBuyerCallBack(OnResetBuyerCallBack);
        MessageTreeLogic.Instance.RegisterSellerAccRefCallBack(OnAcceptRefCallBack);
        RefreshItem();
    }
    public override void OnClose()
    {
        base.OnClose();
        MessageTreeLogic.Instance.RegisterBidCallBack(null);
        MessageTreeLogic.Instance.REgisterSellerOpenChatCallBack(null);
        MessageTreeLogic.Instance.RegisterSellerInitBuyerCallBack(null);
        MessageTreeLogic.Instance.RegisterSellerAccRefCallBack(null);
        TipManager.Instance.CloseAlert();
    }
    private void OnClickBack(GameObject go)
    {
        if(MessageTreeLogic.Instance.GetCurrentStatus() == MessageTreeLogic.MTStatus.None)
        {
            MessageTreeLogic.Instance.OnClickExitDeal();
        }
        else
        {
            TipManager.Instance.Alert("", "确定要关闭交易吗", "OK", "Cancle", (res) =>
            {
                if (res)
                {
                    MessageTreeLogic.Instance.OnClickExitDeal();
                }
            });
        }        
    }
    private void OnDragEnd(MyUIDragDropItem go)
    {
        if (ComponentTool.IsInRect(go.GetComponent<UIWidget>(), m_DropPanel, WindowManager.Instance.GetUIRoot().transform.localScale.x))
        {
            OnClickItem(go.gameObject);
        }
    }
    private void OnClickItem(GameObject go)
    { 
        //check 
        if (!Check(go))
        {
            return;
        }
        string name = go.name.Substring(5);
        int id = 0;
        if (!int.TryParse(name, out id))
        {
            Debuger.LogWarning("wrong name " + name);
            return;
        }
        m_iCurrentSellItemId = id;
        MessageTreeLogic.Instance.OnClickSellItem(id, OnSetSellSucceed);
    }
    private void OnClickAccept(GameObject go)
    {
        MessageTreeLogic.Instance.OnClickAcceptBid();
    }
    private void OnClickRefuse(GameObject go)
    {
        MessageTreeLogic.Instance.OnClickRefuseBid();
    }
    private void OnPressButton(GameObject go, bool status)
    {
        if (status)
        {
            Debuger.Log("pressed");
            MessageTreeLogic.Instance.StartRecord();
        }
        else
        {
            Debuger.Log("released");
            MessageTreeLogic.Instance.EndRecord();
        }
    }
    private void OnSetSellSucceed()
    {
        m_SpriteMyIcon.gameObject.SetActive(true);
        m_SpriteMyIcon.spriteName = MessageTreeLogic.ConvertIdToName(m_iCurrentSellItemId);
    }
    private void SetButtonStatus(bool rootStatus)
    {
        m_ObjButtonRoot.SetActive(rootStatus);
    }
    private void OnBuyerBid(int id)
    {
        SetButtonStatus(true);
        m_SpriteOtherIcon.gameObject.SetActive(true);
        m_SpriteOtherIcon.spriteName = MessageTreeLogic.ConvertIdToName(id);
    }
    private void OnCreateConnect(OpenChatEvent chat)
    {
        m_ButtonTalk.SetActive(true);
        m_SpriteOtherIcon.gameObject.SetActive(false);
        m_LabelOtherName.text = chat.BidderName;
    }
    private void OnResetBuyerCallBack()
    {
        m_ButtonTalk.SetActive(false);
        SetButtonStatus(false);
        OnLoseBuyer();
    }
    private void OnLoseBuyer()
    {
        m_SpriteOtherIcon.gameObject.SetActive(false);
        m_LabelOtherName.text = string.Empty;
    }
    private void OnAcceptRefCallBack(bool isAcceptOption,bool isSucceed)
    {
        if(isSucceed)
        {
            SetButtonStatus(false);
            if(isAcceptOption)
            {
                RefreshItem();
                PlayAudio("Yindaoyu_#121a7_G_D");
                TipManager.Instance.Alert("交易成功");
            }
        }
        else
        {
            PlayAudio("Yindaoyu_#121a6_G_D");
            TipManager.Instance.Alert("", "你已经掉线", "OK", (res) => { MessageTreeLogic.Instance.ClearSale(); });
        }
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
    private void RefreshItem()
    {
        for (int i = 0; i < m_FireworkPlanList.Count; ++i)
        {            
            FireworkPlanElement elem = m_FireworkPlanList[i];
            elem.SetStatus(true, ItemManager.Instance.IsExistItem(i));         
        }
    }
    private void PlayAudio(string name)
    {
        name = "GUIDE/MessageTree/" + name;
        if (!AudioPlayer.Instance.IsPlayingAudio(name))
        {
            AudioPlayer.Instance.PlayAudio(name, Vector3.zero, false);
        }
    }
}
