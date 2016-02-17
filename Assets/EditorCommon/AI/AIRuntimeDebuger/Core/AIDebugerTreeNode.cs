using System;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;


public class AIDebugerTreeNode
{
    public List<AIDebugerTreeNode>  m_ChildList;
    public GameObject               m_ObjRoot;
    public GameObject               m_ObjChildRoot;
    public UILabel                  m_LabelName;
    private bool                    m_bIsSelected;
    public string                   m_strName;
    public BTNode                   m_NodeData;
    public bool                     m_bIsRuntime;

    public AIDebugerTreeNode(string name, GameObject template)
    {
        m_bIsRuntime = UIWindow_BTViewPanel.GetInstance == null;
        m_strName = name;
        m_ChildList = new List<AIDebugerTreeNode>();
        m_ObjRoot = GameObject.Instantiate(template);
        m_ObjRoot.SetActive(true);

        m_ObjChildRoot = ComponentTool.FindChild("Root", m_ObjRoot);
        m_LabelName = ComponentTool.FindChildComponent<UILabel>("Label", m_ObjRoot);
        m_LabelName.text = name;
        if (!m_bIsRuntime)
        {
            UIEventListener.Get(m_ObjRoot).onDoubleClick = UIWindow_BTViewPanel.GetInstance.OnDoubleClick;
            UIEventListener.Get(m_ObjRoot).onClick = UIWindow_BTViewPanel.GetInstance.OnClick;
        }
    }
    public void AddChild(AIDebugerTreeNode node)
    {
        if (null == node)
        {
            return;
        }
        m_ChildList.Add(node);
        ComponentTool.Attach(m_ObjChildRoot.transform,node.m_ObjRoot.transform);
    }
    public void RemoveChild(AIDebugerTreeNode node)
    {
        if (null == node)
        {
            return;
        }
        m_ChildList.Remove(node);
    }
    public float Render(float offset)
    {
        Debuger.Log(m_strName + "in: " + offset);
        DoRender(offset);

        offset = 0;
        if (m_ObjChildRoot.activeSelf)
        {
            foreach (var elem in m_ChildList)
            {
                offset += elem.Render(offset);
            }
        }
        Debuger.Log(m_strName + "out: " + (offset + 50));
        return offset+50;
    }
    public void SetSelected(GameObject node)
    {
        if (m_ObjRoot == node)
        {
            SetSelected(true);
        }
        else
        {
            SetSelected(false);
        }

        foreach (var elem in m_ChildList)
        {
            elem.SetSelected(node);
        }
    }
    public void SetActive(GameObject node)
    {
        if (m_ObjRoot == node)
        {
            SetActive();
        }
        else
        {
            foreach (var elem in m_ChildList)
            {
                elem.SetActive(node);
            }
        }
    }
    public void SetActive()
    {
        if (m_ChildList.Count <= 0)
        {
            return;
        }
        bool status = !m_ObjChildRoot.activeSelf;
        m_ObjChildRoot.SetActive(status);
    }
    public AIDebugerTreeNode GetSelectedNode()
    {
        if (m_bIsSelected)
        {
            return this;
        }
        for (int i = 0; i < m_ChildList.Count; ++i)
        {
            AIDebugerTreeNode res = m_ChildList[i].GetSelectedNode();
            if (null != res)
            {
                return res;
            }
        }
        return null;
    }
    public AIDebugerTreeNode GetNodeRoot(AIDebugerTreeNode child)
    {
        for (int i = 0; i < m_ChildList.Count; ++i)
        {
            if (m_ChildList[i] == child)
            {
                return this;
            }
        }
        for (int i = 0; i < m_ChildList.Count; ++i)
        {
            var res = m_ChildList[i].GetNodeRoot(child);
            if (res != null)
            {
                return res;
            }
        }
        return null;
    }
    public void SetSelected(bool status)
    {
        if (status)
        {
            m_LabelName.color = new Color(1, 0, 0);
        }
        else
        {
            m_LabelName.color = new Color(0, 0, 0);
        }
        m_bIsSelected = status;
    }
    private void DoRender(float pos)
    {
        m_ObjRoot.transform.localPosition = new Vector3(m_ObjRoot.transform.localPosition.x, -pos,
            m_ObjRoot.transform.localPosition.z);

        if (m_bIsRuntime)
        {
            switch (m_NodeData.CurrentStatus)
            {
                case EBTState.True:
                    m_LabelName.color = Color.yellow;
                    break;
                case EBTState.False:
                    m_LabelName.color = Color.red;
                    break;
                case EBTState.Running:
                    m_LabelName.color = Color.green;
                    break;
                case EBTState.UnReach:
                    m_LabelName.color = Color.gray;
                    break;
            }
        }
    }
}