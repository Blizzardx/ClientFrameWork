using BehaviourTree;
using UnityEngine;
using System.Collections;

public class UIWindowAIDebugAttach : WindowBase
{
    private GameObject          m_TreeRootObj;
    private GameObject          m_NodeTemplate;
    private AIDebugerTreeRoot   m_TreeRoot;
    private AIAgent m_AIAgent;

    public override void OnInit()
    {
        base.OnInit();
        m_TreeRootObj = FindChild("TreeRoot");
        m_NodeTemplate = FindChild("Button_Node");
        AddChildElementClickEvent(OnClickExit, "Button_Exit");
    }
    private void OnClickExit(GameObject go)
    {
        Close();
    }
    public override void OnOpen(object param)
    {
        base.OnOpen(param);
        if (!(param is AIAgent))
        {
            return;
        }

        ClearWindow();

        m_AIAgent = param as AIAgent;
        var m_TreeParser = new AIDebugerTreeParser();
        m_TreeRoot = m_TreeParser.CreateBehaviourTree(m_AIAgent.GetID(), m_NodeTemplate, m_TreeRootObj, ConfigManager.Instance.GetAIConfigTable());
        InitTree(); 

        m_TreeRoot.Render(0);
        m_AIAgent.SetDebugMode(true);
        UITickTask.Instance.RegisterToUpdateList(Update);
    }
    public override void OnClose()
    {
        base.OnClose();
        m_AIAgent.SetDebugMode(false);
        UITickTask.Instance.UnRegisterFromUpdateList(Update);
    }
    private void ClearWindow()
    {
        m_TreeRoot = null;
        for (int i = 0; i < m_TreeRootObj.transform.childCount; ++i)
        {
            GameObject.Destroy(m_TreeRootObj.transform.GetChild(i).gameObject);
        }
    }
    private void Update()
    {
        m_TreeRoot.Render(0);
    }
    private void InitTree()
    {
        InitTree(m_AIAgent.GetRoot(), m_TreeRoot);
    }
    private void InitTree(BTNode source,AIDebugerTreeNode desc)
    {
        desc.m_NodeData = source;

        for (int i = 0; i < source.GetChildList().Count; ++i)
        {
            InitTree(source.GetChildList()[i], desc.m_ChildList[i]);
        }
    }
}
