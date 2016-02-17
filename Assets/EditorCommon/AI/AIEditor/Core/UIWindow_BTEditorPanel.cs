using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIWindow_BTEditorPanel : MonoBehaviour

{
    public UIPopupList m_FirstPoplist;
    public UIPopupList m_SecondPoplist;

    private List<string> m_FirstTagList = new List<string> { "复合节点", "装饰节点", "条件节点", "行为节点", };

    private List<string> m_CompositeTagList = new List<string> { "选择节点", "顺序节点", };
    private List<string> m_DecoratorTagList = new List<string> { "时间装饰节点", };
    private List<string> m_ConditionTagList = new List<string> { "条件函数节点", };
    private List<string> m_ActionTagList = new List<string> { "休闲行为节点", "跟随玩家行为节点", };

    private AINodeEditorBase m_CurrentEditPanel;

    public UIPanel_ConditionNodeEditor m_ConditionEditorPanel;
    public UIPanel_MoveNodeEditor m_MovetoEditorPanel;
    public UIPanel_IdleNodeEditor m_IdleEditorPanel;
    public UIPanel_SelectorNodeEditor m_SelectorEditorPanel;
    public UIPanel_SequenceNodeEditor m_SequenceEditorPanel;
    public UIPanel_InverterNodeEditor m_InverterEditorPanel;

    private Action<AIDebugerTreeNode>   m_CreateCallBack;
    private Action                      m_EditCallBack;
    private bool m_bIsInit;

	// Use this for initialization
	void Start ()
	{
	    Init();
	}
    private void Init()
    {
        if (m_bIsInit)
        {
            return;
        }
        m_FirstPoplist.items = m_FirstTagList;
        m_FirstPoplist.value = m_FirstTagList[0];
        m_FirstPoplist.onChange.Add(new EventDelegate(OnChangeFirst));
        m_SecondPoplist.items = m_CompositeTagList;
        m_SecondPoplist.onChange.Add(new EventDelegate(OnChangeSecond));
        m_SecondPoplist.value = m_CompositeTagList[0];
        m_bIsInit = true;
    }
    private void OnChangeFirst()
    {
        switch (m_FirstPoplist.value)
        {
            case "复合节点":
                m_SecondPoplist.items = m_CompositeTagList;
                break;
            case "装饰节点":
                m_SecondPoplist.items = m_DecoratorTagList;
                break;
            case "条件节点":
                m_SecondPoplist.items = m_ConditionTagList;
                break;
            case "行为节点":
                m_SecondPoplist.items = m_ActionTagList;
                break;
        }
        m_SecondPoplist.value = m_SecondPoplist.items[0];
    }
    private void OnChangeSecond()
    {
        AINodeEditorBase newPanel = null;
        switch (m_SecondPoplist.value)
        {
            case "选择节点":
                newPanel = m_SelectorEditorPanel;
                break;
            case "顺序节点":
                newPanel = m_SequenceEditorPanel;
                break;
            case "时间装饰节点":
                newPanel = m_InverterEditorPanel;
                break;
            case "条件函数节点":
                newPanel = m_ConditionEditorPanel;
                break;
            case "休闲行为节点":
                newPanel = m_IdleEditorPanel;
                break;
            case "跟随玩家行为节点":
                newPanel = m_MovetoEditorPanel;
                break;
        }
        if (m_CurrentEditPanel != newPanel)
        {
            if (null != m_CurrentEditPanel)
            {
                m_CurrentEditPanel.SetWindowStatus(false);
            }
            m_CurrentEditPanel = newPanel;
            if (null != m_CurrentEditPanel)
            {
                m_CurrentEditPanel.SetWindowStatus(true);
            }
        }
    }
    // Update is called once per frame
	void Update () 
    {
	
	}
    public void Ok()
    {
        gameObject.SetActive(false);
        if (m_CreateCallBack != null)
        {
            m_CreateCallBack(m_CurrentEditPanel.GetNode());
        }
        else
        {
            m_CurrentEditPanel.ResetToRef();
            m_EditCallBack();
        }
    }
    public void Cancle()
    {
        gameObject.SetActive(false);
    }
    public void OnCreateNode(Action<AIDebugerTreeNode> OnCreateCallBack)
    {
        Init();
        m_SecondPoplist.enabled = true;
        m_FirstPoplist.enabled = true;
        m_CreateCallBack = OnCreateCallBack;
        m_EditCallBack = null;
        gameObject.SetActive(true);   
    }
    public void OnEditNode(AIDebugerTreeNode node,Action OnEditCallBack)
    {
        Init();
        m_EditCallBack = OnEditCallBack;
        m_CreateCallBack = null;
        gameObject.SetActive(true);

        switch (node.m_strName)
        {
            case "选择节点":
            case "顺序节点":
                m_FirstPoplist.value = "复合节点";
                m_SecondPoplist.items = m_CompositeTagList;
                break;
            case "时间装饰节点":
                m_FirstPoplist.value = "装饰节点";
                m_SecondPoplist.items = m_DecoratorTagList;
                break;
            case "条件函数节点":
                m_FirstPoplist.value = "条件节点";
                m_SecondPoplist.items = m_ConditionTagList;
                break;
            case "休闲行为节点":
            case "跟随玩家行为节点":
                m_FirstPoplist.value = "行为节点";
                m_SecondPoplist.items = m_ActionTagList;
                break;
        }
        m_SecondPoplist.value = node.m_strName;

        m_SecondPoplist.enabled = false;
        m_FirstPoplist.enabled = false;

        OnChangeSecond();
        m_CurrentEditPanel.InitPanel(node);

    }
}
