using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Config.Table;
using RegularityGame;
using UnityEngine;

public class RegularityWindowParam
{
    public List<string>             m_OptionalList;
    public Action<string, Vector3>  onReleaseCallBack;
    public RegularityGameConfigTable configTable;
}
public class UIWindowRegularity : WindowBase
{
    public class RegularityOptionalElement
    {
        public RegularityOptionalElement(string name, GameObject objRoot)
        {
            m_ObjRoot = objRoot;
            m_strName = name;
        }
        public GameObject   m_ObjRoot;
        public string       m_strName;
    }
    private GameObject          m_ObjOptionalRoot;
    private GameObject          m_ObjOptionalTemplate;
    private Dictionary<string, RegularityOptionalElement> m_OptionalMap;
    private Action<string, Vector3> m_OnReleaseCallBack;
    private Camera m_UICamera;
    private GameObject m_ObjWinRoot;
    private GameObject m_ObjLoseRoot;
    private UIPopupList m_PopList;
    private GameObject m_ButtonRoot;
    private UIGrid m_Grid;
    private UILabel m_LabelLeftTime;
    private UILabel m_LabelLeftCount;
    private List<GameObject> m_FlowerList;
    private bool m_bIsPlayingAnim;

    public override void OnInit()
    {
        base.OnInit();
        m_ObjOptionalRoot = FindChild("OptionalRoot");
        m_ObjOptionalTemplate = FindChild("OptionalTextureTemplate");
        m_OptionalMap = new Dictionary<string, RegularityOptionalElement>();
        m_UICamera = WindowManager.Instance.GetUICamera();
        m_ObjWinRoot = FindChild("Win");
        m_ObjLoseRoot = FindChild("Lose");
        m_PopList = FindChildComponent<UIPopupList>("PopupList");
        m_ButtonRoot = FindChild("ButtonRoot");
        m_Grid = m_ObjOptionalRoot.GetComponent<UIGrid>();
        m_LabelLeftTime = FindChildComponent<UILabel>("Label_LeftTime");
        m_LabelLeftCount = FindChildComponent<UILabel>("Label_LeftCount");
        m_FlowerList = new List<GameObject>();

        for (int i = 0; i < 3; ++i)
        {
            var obj = FindChild("Flower"+i);
            m_FlowerList.Add(obj);
        }

        AddChildElementClickEvent(OnClickReset, "UIButton_ResetElem");
        AddChildElementClickEvent(OnClickResetById, "UIButton_Reset");
        AddChildElementClickEvent(OnClickBack, "UIButton_BackElem");
        AddChildElementClickEvent(OnClickBack, "UIButton_Back");
        AddChildElementClickEvent(OnClickBack, "Button_Exit");

        m_ObjLoseRoot.SetActive(false);
        m_ObjWinRoot.SetActive(false);
        m_ButtonRoot.SetActive(false);

    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        if (!(param is RegularityWindowParam))
        {
            return;
        }

        RegularityWindowParam windowParam = param as RegularityWindowParam;
        for (int i = 0; i < windowParam.m_OptionalList.Count; ++i)
        {
            string name = windowParam.m_OptionalList[i];

            GameObject child = GameObject.Instantiate(m_ObjOptionalTemplate);
            child.SetActive(true);
            ComponentTool.Attach(m_ObjOptionalRoot.transform, child.transform);
            UITexture texture = child.GetComponent<UITexture>();
            texture.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture>(name, AssetType.Texture);

            UIEventListener.Get(child).onClick = OnClickOption;

            m_OptionalMap.Add(name, new RegularityOptionalElement(name, child));
        }
        List<string> list = new List<string>();
        for(int i=0;i<windowParam.configTable.RegularityConfigMap.Count;++i)
        {
            list.Add(i.ToString());
        }
        m_PopList.items = list;
		m_PopList.value = list [0];
        m_OnReleaseCallBack = windowParam.onReleaseCallBack;
        m_bIsPlayingAnim = false;
    }
    public void ResetAnswer(List<string> OptionalList)
    {
        foreach (var elem in m_OptionalMap)
        {
            elem.Value.m_ObjRoot.transform.parent = null;
            GameObject.Destroy(elem.Value.m_ObjRoot);
        }
        m_OptionalMap.Clear();

        for (int i = 0; i < OptionalList.Count; ++i)
        {
            string name = OptionalList[i];

            GameObject child = GameObject.Instantiate(m_ObjOptionalTemplate);
            child.SetActive(true);
            ComponentTool.Attach(m_ObjOptionalRoot.transform, child.transform);
            UITexture texture = child.GetComponent<UITexture>();
            texture.mainTexture = ResourceManager.Instance.LoadBuildInResource<Texture>(name, AssetType.Texture);

            UIEventListener.Get(child).onClick = OnClickOption;
            m_OptionalMap.Add(name, new RegularityOptionalElement(name, child));
        }
        m_bIsPlayingAnim = false;
        m_Grid.Reposition();
    }
    private void OnClickOption(GameObject obj)
    {
        if(m_bIsPlayingAnim)
        {
            return;
        }
        //check
        GameObject root = obj.gameObject;
        string name = string.Empty;
        foreach (var elem in m_OptionalMap)
        {
            if (elem.Value.m_ObjRoot == root)
            {
                name = elem.Key;
            }
        }
        m_OnReleaseCallBack(name, m_UICamera.WorldToScreenPoint(root.transform.position));
    }
    public void OnDisableOption(string name)
    {
        m_bIsPlayingAnim = true;
        RegularityOptionalElement root = null;
        if (m_OptionalMap.TryGetValue(name, out root))
        {
            root.m_ObjRoot.GetComponent<BoxCollider>().enabled = false;
            TriggerToShowDisableOption(root.m_ObjRoot, () => { root.m_ObjRoot.SetActive(false); });
        }
    }
    public void OnEnableOption(string name)
    {
        m_bIsPlayingAnim = false;
           RegularityOptionalElement root = null;
        if (m_OptionalMap.TryGetValue(name, out root))
        {
            root.m_ObjRoot.SetActive(true);
            root.m_ObjRoot.GetComponent<BoxCollider>().enabled = true;
            TriggerToShowEnableOption(root.m_ObjRoot, () => { });
        }
    }
    public void OnWin()
    {
        // show win panel
        m_ObjWinRoot.SetActive(true);
        m_ButtonRoot.SetActive(true);
    }
    public void OnLose()
    {
        // show lose panel
        m_ObjLoseRoot.SetActive(true);
        m_ButtonRoot.SetActive(true);
    }
    public void SetLeftTime(float time)
    {
        m_LabelLeftTime.text = time.ToString();
    }
    public void SetLeftCount(int count)
    {
        m_LabelLeftCount.text = count.ToString();
    }
    private void OnClickReset(GameObject obj)
    {
        bool status = m_ObjWinRoot.activeSelf;
        m_ObjWinRoot.SetActive(false);
        m_ObjLoseRoot.SetActive(false);
        m_ButtonRoot.SetActive(false);
        //RegularityGameLogic.Instance.Restart();
        if (status)
        {
            RegularityAlphaGameLogic.Instance.OnBack();
        }
        else
        {
            RegularityAlphaGameLogic.Instance.OnReset();
        }
    }
    private void OnClickResetById(GameObject go)
    {
        m_ObjWinRoot.SetActive(false);
        m_ObjLoseRoot.SetActive(false);
        m_ButtonRoot.SetActive(false);
        //RegularityGameLogic.Instance.ResetSceneByUI(int.Parse(m_PopList.value));
    }
    private void OnClickBack(GameObject obj)
    {
        m_ObjWinRoot.SetActive(false);
        m_ObjLoseRoot.SetActive(false);
        m_ButtonRoot.SetActive(false);
        //RegularityGameLogic.Instance.OnBack();
        RegularityAlphaGameLogic.Instance.OnBack();
    }
    public void SetLeftFlower(int mILeftRedFlower)
    {
        for (int i = 0; i < m_FlowerList.Count; ++i)
        {
            m_FlowerList[i].SetActive(i < mILeftRedFlower);
        }
    }
    private void TriggerToShowEnableOption(GameObject optionObj, Action CallBack)
    {
        TweenScale tween = optionObj.GetComponent<TweenScale>();
        if (null != tween)
        {
            GameObject.Destroy(tween);
        }
        tween = optionObj.AddComponent<TweenScale>();
        tween.from = Vector3.zero;
        tween.to = Vector3.one;
        tween.duration = 0.5f;
        tween.AddOnFinished(() =>
        {
            GameObject.Destroy(tween);
            CallBack();
        });
    }
    private void TriggerToShowDisableOption(GameObject optionObj, Action CallBack)
    {
        TweenScale tween = optionObj.GetComponent<TweenScale>();
        if (null != tween)
        {
            GameObject.Destroy(tween);
        }
        tween = optionObj.AddComponent<TweenScale>();
        tween.from = Vector3.one;
        tween.to = Vector3.zero;
        tween.duration = 0.5f;
        tween.AddOnFinished(() =>
        {
            GameObject.Destroy(tween);
            optionObj.transform.localScale = Vector3.one;
            CallBack();
        });
    }
}