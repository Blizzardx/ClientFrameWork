using NetWork.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FireworkPlanElement
{
    public GameObject m_ObjRoot;
    public GameObject m_ObjBlock0Root;
    public GameObject m_ObjBlock1Root;

    public FireworkPlanElement(GameObject root)
    {
        m_ObjRoot = root;
        m_ObjBlock0Root = ComponentTool.FindChild("SpriteBlock0", m_ObjRoot);
        m_ObjBlock1Root = ComponentTool.FindChild("SpriteBlock1", m_ObjRoot);
    }
    public void SetStatus(bool active = true,bool isAvliable = true,int bolckLevel = 1)
    {
        m_ObjRoot.SetActive(active);
        if (isAvliable)
        {
            m_ObjBlock0Root.SetActive(false);
            m_ObjBlock1Root.SetActive(false);
        }
        else
        {
            m_ObjBlock0Root.SetActive(bolckLevel == 0);
            m_ObjBlock1Root.SetActive(bolckLevel == 1);
        }
    }
    public bool IsActive()
    {
        if( !m_ObjRoot.activeSelf || m_ObjBlock0Root.activeSelf || m_ObjBlock1Root.activeSelf)
        {
            return false;
        }
        return true;
    }
}
public class UIWindowMsgTreeBuy:WindowBase
{
    private GameObject m_ObjButtonRoot;
    private GameObject m_ObjButtonOk;
    private GameObject m_ObjButtonCancle;
    private UISprite m_SpriteMyIcon;
    private UISprite m_SpriteOtherIcon;
    private UILabel m_LabelMyName;
    private UILabel m_LabelOtherName;
    private int m_iCurrentSellItemId;
    private bool m_bIsWaitingBid;
    private List<FireworkPlanElement> m_FireworkPlanList;
    private UIWidget m_DropPanel;

    public override void OnInit()
    {
        base.OnInit();
        m_SpriteMyIcon = FindChildComponent<UISprite>("Sprite_Icon1");
        m_SpriteOtherIcon = FindChildComponent<UISprite>("Sprite_Icon0");
        m_ObjButtonRoot = FindChild("ButtonRoot");
        m_ObjButtonOk = FindChild("Button_Ok");
        m_ObjButtonCancle = FindChild("Button_Cancle");
        m_LabelMyName = FindChildComponent<UILabel>("Label_Name1");
        m_LabelOtherName = FindChildComponent<UILabel>("Label_Name2");

        var obj = FindChild("Button_Talk");
        UIEventListener.Get(obj).onPress = OnPressButton;

        AddChildElementClickEvent(OnClickBack, "Sprite_Exit");

        m_DropPanel = FindChildComponent<UIWidget>("DrapPanel");
        m_FireworkPlanList = new List<FireworkPlanElement>();
        for (int i = 0; i < 7; ++i)
        {
            var objRoot = FindChild("Type_" + i);
            FireworkPlanElement elem = new FireworkPlanElement(objRoot);
            elem.SetStatus(true,ItemManager.Instance.IsExistItem(i));
            m_FireworkPlanList.Add(elem);
            MyUIDragDropItem drag = objRoot.GetComponent<MyUIDragDropItem>();
            drag.RegisterDragEndAction(OnDragEnd);
            //UIEventListener.Get(objRoot).onClick = OnClickItem;;
        }

        m_ObjButtonRoot.SetActive(false);
        m_SpriteMyIcon.gameObject.SetActive(false);
        m_SpriteOtherIcon.gameObject.SetActive(false);
        m_LabelOtherName.text = string.Empty;
        m_LabelMyName.text = PlayerManager.Instance.GetCharBaseData().CharName;
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        MessageTreeLogic.Instance.RegisterInitBuyDeal(OnInitDeal);
        MessageTreeLogic.Instance.RegisterBidResp(OnBidCallBack);
        MessageTreeLogic.Instance.RegisterBidResponse(OnBidResponse);
        RefreshItem();
    }
    public override void OnClose()
    {
        base.OnClose();
        MessageTreeLogic.Instance.RegisterInitBuyDeal(null);
        MessageTreeLogic.Instance.RegisterBidResp(null);
        MessageTreeLogic.Instance.RegisterBidResponse(null);
        TipManager.Instance.CloseAlert();
    }
    private void OnDragEnd(MyUIDragDropItem go)
    {
        if(ComponentTool.IsInRect(go.GetComponent<UIWidget>(), m_DropPanel,WindowManager.Instance.GetUIRoot().transform.localScale.x))
        {
            OnClickItem(go.gameObject);
        }
    }
    private void OnClickItem(GameObject go)
    {
        if (m_bIsWaitingBid)
        {
            PlayAudio("Yindaoyu_#121a1_G_D");
            return;
        }

        //check 
        if (!Check(go))
        {
            return;
        }

        
        SetButtonStatus(false, true);
        string name = go.name.Substring(5);
        int id = 0;
        if (!int.TryParse(name, out id))
        {
            Debuger.LogWarning("wrong name " + name);
            return;
        }
        m_iCurrentSellItemId = id;
        MessageTreeLogic.Instance.OnBidItem(id);
    }
    private void OnPressButton(GameObject go,bool status)
    {
        if(status)
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
    private void SetButtonStatus(bool rootStatus,bool subStatus)
    {
        m_ObjButtonRoot.SetActive(rootStatus);
        m_ObjButtonOk.SetActive(subStatus);
        m_ObjButtonCancle.SetActive(!subStatus);
    }
    private void OnClickBack(GameObject go)
    {
        TipManager.Instance.Alert("", "确定要关闭交易吗", "OK", "Cancle", (res) => 
        {
            if(res)
            {
                MessageTreeLogic.Instance.OnClickExitDeal();
            }
        });        
    }
    private void OnInitDeal(BuyResponse resp)
    {
        m_LabelOtherName.text = resp.SellerName;
        m_SpriteMyIcon.gameObject.SetActive(true);
        m_SpriteMyIcon.spriteName = MessageTreeLogic.ConvertIdToName(resp.ItemId);
        m_SpriteOtherIcon.gameObject.SetActive(false);
    }
    private void OnBidResponse(bool res)
    {
        m_FireworkPlanList[m_iCurrentSellItemId].SetStatus(true, false);

        SetButtonStatus(true, res);
        m_bIsWaitingBid = res;
        if (res)
        {
            RefreshItem();
            PlayAudio("Yindaoyu_#121a7_G_D");
            TipManager.Instance.Alert("交易成功");
        }
        else
        {
            if (!CheckCanContinue())
            {
                TipManager.Instance.Alert("", "交易失败", "OK", (boolres) => { MessageTreeLogic.Instance.OnClickExitDeal(); });
            }
            else
            {
                PlayAudio("Yindaoyu_#121a2_G_D");
            }
        }

    }
    private void OnBidCallBack(BidResponse res)
    {        
        if(res.Success)
        {
            m_bIsWaitingBid = true;
            m_SpriteOtherIcon.gameObject.SetActive(true);
            m_SpriteOtherIcon.spriteName = MessageTreeLogic.ConvertIdToName(m_iCurrentSellItemId);
        }
        else
        {
            TipManager.Instance.Alert("", "你已经掉线", "OK", (res2) => { MessageTreeLogic.Instance.ClearSale(); });
        }
    }
    private bool Check(GameObject obj)
    {
        for(int i=0;i<m_FireworkPlanList.Count;++i)
        {
            if(obj == m_FireworkPlanList[i].m_ObjRoot)
            {
                return m_FireworkPlanList[i].IsActive();
            }
        }
        return false;
    }
    private bool CheckCanContinue()
    {
        int index = 0;
        for(int i=0;i<m_FireworkPlanList.Count;++i)
        {
            index = m_FireworkPlanList[i].IsActive() ? index : index + 1;
        }
        return index < m_FireworkPlanList.Count;
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