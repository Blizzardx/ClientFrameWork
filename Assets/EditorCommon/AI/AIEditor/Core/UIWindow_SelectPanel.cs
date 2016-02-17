using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BehaviourTree;
using UnityEngine;
using System.Collections.Generic;

public class UIWindow_SelectPanel : MonoBehaviour
{
    public GameObject m_ChildTemplate;
    public Transform m_SelectRoot;
    public UIGrid m_Grid;
    public Action<int> m_OnCreateRootCallback;
    public Action<int,string> m_OnCreateRootCallBack;
    public List<AIPlanElement> m_ElemList = new List<AIPlanElement>();
    public GameObject m_SelectPanel;
    public GameObject m_CreateRootPanel;
    public UIInput m_InputId;
    public UIInput m_InputDesc;

    public void SelectPlane(Action<int> onCallback)
    {
        gameObject.SetActive(true);
        m_SelectPanel.SetActive(true);
        m_OnCreateRootCallback = onCallback;
        ClearPanel();
        ParseBTXml();
    }
    private void ParseBTXml()
    {
        XElement xml = UIWindow_BTViewPanel.LoadAIConfig();
        IEnumerable<XElement> behaviorTrees = xml.Elements(BTDataKey.BEHAVIOUR_TREE_ROOT);
        if (null == behaviorTrees)
        {
            return;
        }

        foreach (XElement element in behaviorTrees)
        {
            int iID = 0;
            int.TryParse(element.Attribute(BTDataKey.BEHAVIOUR_TREE_ID).Value, out iID);
            string desc = element.Attribute("desc").Value;

            GameObject objElem = GameObject.Instantiate(m_ChildTemplate);
            objElem.SetActive(true);
            ComponentTool.Attach(m_SelectRoot, objElem.transform);
            AIPlanElement elem = objElem.GetComponent<AIPlanElement>();
            elem.Init(iID, desc, OnClickSelect);
            m_ElemList.Add(elem);
        }
        m_Grid.Reposition();
    }
    public void CreateRoot(Action<int,string> onCallback)
    {
        m_OnCreateRootCallBack = onCallback;
        gameObject.SetActive(true);
        m_CreateRootPanel.SetActive(true);
    }
    public void OnClickSelect(GameObject sender)
    {
        m_SelectPanel.SetActive(false);
        gameObject.SetActive(false);
        AIPlanElement elem = sender.transform.parent.gameObject.GetComponent<AIPlanElement>();
        m_OnCreateRootCallback(elem.m_iId);
    }
    public void ClearPanel()
    {
        for (int i = 0; i < m_ElemList.Count; ++i)
        {
            GameObject.Destroy(m_ElemList[i].gameObject);
        }
        m_ElemList.Clear();
    }
    public void OnClickOk()
    {
        gameObject.SetActive(false);
        m_CreateRootPanel.SetActive(false);
        if (!string.IsNullOrEmpty(m_InputId.value) && !string.IsNullOrEmpty(m_InputDesc.value))
        {
            m_OnCreateRootCallBack(int.Parse(m_InputId.value), m_InputDesc.value);
        }
        else
        {
            m_OnCreateRootCallBack(0, null);
        }
    }
    public void OnClickCancle()
    {
        gameObject.SetActive(false);
        m_CreateRootPanel.SetActive(false);
    }
}
