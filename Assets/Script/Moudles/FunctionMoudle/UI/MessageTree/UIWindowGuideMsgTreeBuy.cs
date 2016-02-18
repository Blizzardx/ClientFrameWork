using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIWindowGuideMsgTreeBuy : WindowBase
{
    private GameObject m_ObjBlock;
    private GameObject m_ObjFWPlanHighLight;
    private GameObject m_ObjMicHighLight;
    private GameObject m_ObjIcon0HighLight;
    private GameObject m_ObjIcon1HighLight;
    private GameObject m_ObjExitHighLight;
    private GameObject m_ObjItem0HighLight;
    private GameObject m_ObjItem1HighLight;
    private GameObject m_ObjButtonOk;
    private GameObject m_ObjButtonCancle;
    private UILabel m_LabelName1;
    private UILabel m_LabelName2;
    private UISprite m_SpriteIcon1;
    private UISprite m_SpriteIcon2;
    private List<FireworkPlanElement> m_FireworkPlanList;
    private UIWidget m_DropPanel;
    private BoxCollider m_Item0Collider;
    private BoxCollider m_Item1Collider;
    private GameObject m_ObjGuideHandRoot;

    //animation
    private List<GameObject> m_SpriteList;
    private float m_fRate = 20;
    private bool m_bIsPlayingAnim;
    private bool m_bIsShowFinAnim;
    private Vector3 m_vFromLocalPos = new Vector3(-630, -392, 0);
    private Vector3 m_vToLocalPos = new Vector3(-341, 11, 0);
    private float m_fDuringTime = 1.0f;
    private Vector3 m_vFromLocalPos1 = new Vector3(-415, -392, 0);
    private Vector3 m_vToLocalPos1 = new Vector3(-341, 11, 0);
    float mUpdate = 0f;
    int mIndex = 0;
    int fingerId = 0;

    public override void OnInit()
    {
        base.OnInit();
        m_ObjBlock = FindChild("Texture_Block");
        m_ObjFWPlanHighLight = FindChild("Sprite_HighlightfwPlan");
        m_ObjMicHighLight = FindChild("Sprite_HighlightMic");
        m_ObjIcon0HighLight = FindChild("Sprite_HighlightIcon0");
        m_ObjIcon1HighLight = FindChild("Sprite_HighlightIcon1");
        m_ObjItem0HighLight = FindChild("Sprite_HighlightItem0");
        m_ObjItem1HighLight = FindChild("Sprite_HighlightItem1");
        m_ObjExitHighLight = FindChild("Sprite_HighlightExit");

        m_ObjButtonOk = FindChild("Button_Ok");
        m_ObjButtonCancle = FindChild("Button_Cancle");
        m_LabelName1 = FindChildComponent<UILabel>("Label_Name1");
        m_LabelName2 = FindChildComponent<UILabel>("Label_Name2");
        m_SpriteIcon1 = FindChildComponent<UISprite>("Sprite_Icon0");
        m_SpriteIcon2 = FindChildComponent<UISprite>("Sprite_Icon1");
        AddChildElementClickEvent(OnExit, "Sprite_Exit");
        m_ObjGuideHandRoot = FindChild("Texture_GuideHand");

        var obj = FindChild("Button_Talk");
        UIEventListener.Get(obj).onPress = OnPressButton;

        m_DropPanel = FindChildComponent<UIWidget>("DrapPanel");
        m_FireworkPlanList = new List<FireworkPlanElement>();
        for (int i = 0; i < 7; ++i)
        {
            var objRoot = FindChild("Type_" + i);
            FireworkPlanElement elem = new FireworkPlanElement(objRoot);
            if (i == 0)
            {
                m_Item0Collider = objRoot.GetComponent<BoxCollider>();
            }
            else if( i == 1)
            {
                m_Item1Collider = objRoot.GetComponent<BoxCollider>();
            }
            elem.SetStatus(true, i < 3);
            m_FireworkPlanList.Add(elem);
            MyUIDragDropItem drag = objRoot.GetComponent<MyUIDragDropItem>();
            drag.RegisterDragEndAction(OnDragEnd);
        }
        SetButtonStatus(false, true);
        m_SpriteIcon1.gameObject.SetActive(false);
        //m_SpriteIcon2.gameObject.SetActive(false);
        //m_LabelName2.text = string.Empty;
        m_LabelName1.text = PlayerManager.Instance.GetCharBaseData().CharName;
        SetFWPHighlightStatus(false);
        SetMicHighlightStatus(false);
        SetIcon0HighlightStatus(false);
        SetIcon1HighlightStatus(false);
        SetExitHighLightStatus(false);
        SetItem0HighLightStatus(false);
        SetItem1HighLightStatus(false);
        SetItem0Dragable(false);
        SetItem1Dragable(false);

        m_SpriteList = new List<GameObject>();
        for(int i=0;i<2;++i)
        {
            m_SpriteList.Add(FindChild("Frame" + i));
        }
        m_ObjGuideHandRoot.SetActive(false);
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
        SetButtonStatus(false, true);
        string name = go.name.Substring(5);
        int id = 0;
        if (!int.TryParse(name, out id))
        {
            Debuger.LogWarning("wrong name " + name);
            return;
        }
        MessageTreeGuideLogic.Instance.OnSetItem(id);
    }
    public void SetIcon1Sprite(int id)
    {
        string name = MessageTreeLogic.ConvertIdToName(id);
        m_SpriteIcon1.gameObject.SetActive(!string.IsNullOrEmpty(name));
        m_SpriteIcon1.spriteName = name;
    }
    public void SetIcon2Sprite(int id)
    {
        string name = MessageTreeLogic.ConvertIdToName(id);
        m_SpriteIcon2.gameObject.SetActive(!string.IsNullOrEmpty(name));
        m_SpriteIcon2.spriteName = name;
    }
    public void SetBlock(bool status)
    {
        m_ObjBlock.SetActive(status);
    }
    public void SetFWPHighlightStatus(bool status)
    {
        m_ObjFWPlanHighLight.SetActive(status);
    }
    public void SetMicHighlightStatus(bool status)
    {
        m_ObjMicHighLight.SetActive(status);
    }
    public void SetIcon0HighlightStatus(bool status)
    {
        m_ObjIcon0HighLight.SetActive(status);
    }
    public void SetIcon1HighlightStatus(bool status)
    {
        m_ObjIcon1HighLight.SetActive(status);
    }
    public void SetExitHighLightStatus(bool status)
    {
        m_ObjExitHighLight.SetActive(status);
    }
    public void SetItem0HighLightStatus(bool status)
    {
        m_ObjItem0HighLight.SetActive(status);
    }
    public void SetItem1HighLightStatus(bool status)
    {
        m_ObjItem1HighLight.SetActive(status);
    }
    public void ShowFingerAnim(bool status,int fingerIndex = 0)
    {
        if (status == m_bIsShowFinAnim)
        {
            return;
        }
        fingerId = fingerIndex;
        m_ObjGuideHandRoot.SetActive(status);
        m_ObjGuideHandRoot.transform.localPosition = fingerIndex == 0 ? m_vFromLocalPos : m_vFromLocalPos1;
        m_bIsShowFinAnim = status;
        m_bIsPlayingAnim = true;
        mIndex = 0;
        mUpdate = Time.time + Mathf.Abs(1f / m_fRate);
    }    
    public void SetItem0Dragable(bool status)
    {
        m_Item0Collider.enabled = status;
    }
    public void SetItem1Dragable(bool status)
    {
        m_Item1Collider.enabled = status;
    }
    public void SetButtonStatus(bool isShowButton,bool status)
    {
        m_ObjButtonCancle.SetActive(isShowButton && !status);
        m_ObjButtonOk.SetActive(isShowButton && status);
    }
    public void SetItemBlockStatus(int index,bool isBlock)
    {
        if (index < 0 || index >= m_FireworkPlanList.Count)
        {
            return;
        }
        m_FireworkPlanList[index].SetStatus(true, !isBlock);
    }
    private void OnPressButton(GameObject go, bool status)
    {
        if (status)
        {
            MessageTreeGuideLogic.Instance.OnPressMic();
        }
        else
        {
            MessageTreeGuideLogic.Instance.OnReleaseMic();
        }
    }
    private void OnExit(GameObject obj)
    {
        MessageTreeGuideLogic.Instance.OnBuyExit();
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        UITickTask.Instance.RegisterToUpdateList(Update);
    }
    public override void OnClose()
    {
        base.OnClose();
        UITickTask.Instance.UnRegisterFromUpdateList(Update);
    }
    private void TriggerMove()
    {
        m_ObjGuideHandRoot.transform.localPosition = fingerId == 0 ? m_vFromLocalPos : m_vFromLocalPos1;
        TweenPosition tmp = m_ObjGuideHandRoot.AddComponent<TweenPosition>();
        tmp.from = fingerId == 0 ? m_vFromLocalPos : m_vFromLocalPos1;
        tmp.to = fingerId == 0 ? m_vToLocalPos : m_vToLocalPos1; ;
        tmp.duration = m_fDuringTime;
        tmp.AddOnFinished(OnFin);
    }
    private void OnFin()
    {
        m_ObjGuideHandRoot.transform.localPosition = fingerId == 0 ? m_vFromLocalPos : m_vFromLocalPos1; ;
        TweenPosition tmp = m_ObjGuideHandRoot.GetComponent<TweenPosition>();
        GameObject.Destroy(tmp);
        m_bIsPlayingAnim = true;
        mIndex = 0;
        mUpdate = Time.time + Mathf.Abs(1f / m_fRate);
    }
    private void Update()
    {
        if(!m_bIsShowFinAnim)
        {
            return;
        }
        if(m_bIsPlayingAnim)
        {
            if (m_fRate != 0 && m_SpriteList != null && m_SpriteList.Count > 0)
            {
                float time = Time.time;

                if (mUpdate < time)
                {
                    mUpdate = time;
                    m_SpriteList[mIndex].SetActive(false);
                    ++mIndex;
                    if (mIndex >= m_SpriteList.Count)
                    {
                        m_bIsPlayingAnim = false;
                        mIndex = 0;
                        TriggerMove();
                    }
                    else
                    {
                        mUpdate = time + Mathf.Abs(1f / m_fRate);
                    }
                    m_SpriteList[mIndex].SetActive(true);
                    
                }
            }
        }
    }
}
