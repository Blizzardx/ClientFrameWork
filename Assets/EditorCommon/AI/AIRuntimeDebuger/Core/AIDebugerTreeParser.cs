using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BehaviourTree;
using UnityEngine;

public class AIDebugerTreeParser
{
    private GameObject m_Template;

    public AIDebugerTreeRoot CreateBehaviourTree(int iId, GameObject template, GameObject TreeRoot, XElement xml,bool isRuntime = false)
    {
        if (iId <= 0)
        {
            return null;
        }
        m_Template = template;
        AIDebugerTreeRoot root = new AIDebugerTreeRoot(iId, template);
        ComponentTool.Attach(TreeRoot.transform, root.m_ObjRoot.transform);

        ParseBTXml(root, xml);
        return root;
    }
    private void ParseBTXml(AIDebugerTreeRoot root, XElement xml)
    {
        IEnumerable<XElement> behaviorTrees = xml.Elements(BTDataKey.BEHAVIOUR_TREE_ROOT);
        if (null == behaviorTrees)
        {
            return;
        }

        foreach (XElement element in behaviorTrees)
        {
            int iID = 0;
            int.TryParse(element.Attribute(BTDataKey.BEHAVIOUR_TREE_ID).Value, out iID);
            if (iID != root.ID)
            {
                continue;
            }
            string desc = element.Attribute("desc").Value;
            root.Desc = desc;
            ParseBTNode(root, element);
            break;
        }
    }
    private void ParseBTNode(AIDebugerTreeNode root, XElement btNodeE)
    {
        IEnumerable<XElement> nodes = btNodeE.Elements(BTDataKey.NODE_NAME);
        if (null == nodes)
        {
            return;
        }

        foreach (XElement element in nodes)
        {
            AIDebugerTreeNode node = GetNode(element);
            if (null != node)
            {
                root.AddChild(node);
            }

            ParseBTNode(node, element);
        }
    }
    private Dictionary<string, string> GetProperty(XElement btNodeE)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        var propertyEList = btNodeE.Elements(BTDataKey.NODE_PROPERTY);
        if (propertyEList == null)
        {
            return dic;
        }
        foreach (XElement propertyE in propertyEList)
        {
            dic.Add(propertyE.Attribute(BTDataKey.NODE_KEY).Value, propertyE.Attribute(BTDataKey.NODE_VALUE).Value);
        }
        return dic;
    }
    private AIDebugerTreeNode GetNode(XElement btNodeE)
    {
        string szNodeType = btNodeE.Attribute(BTDataKey.NODE_FIRST_TYPE).Value;
        Dictionary<string, string> paramsDic = GetProperty(btNodeE);
        switch (szNodeType)
        {
            case BTDataKey.NODE_TYPE_SELECTOR:
                return ParseSelector(btNodeE, paramsDic);
            case BTDataKey.NODE_TYPE_SEQUENCE:
                return ParseSequence(btNodeE, paramsDic);
            case BTDataKey.NODE_TYPE_ACTION:
                return ParseAction(btNodeE, paramsDic);
            case BTDataKey.NODE_TYPE_CONDITION:
                return ParseCondition(btNodeE, paramsDic);
            case BTDataKey.NODE_TYPE_DECORATOR:
                return ParseDecorator(btNodeE, paramsDic);
        }

        return null;
    }

    #region Composites
    private AIDebugerTreeNode ParseSelector(XElement btNodeE, Dictionary<string, string> paramsDic)
    {
        return new AIDebugerTreeNode("选择节点",  m_Template);
    }
    private AIDebugerTreeNode ParseSequence(XElement btNodeE, Dictionary<string, string> paramsDic)
    {
        return new AIDebugerTreeNode("顺序节点",  m_Template);
    }
    #endregion

    #region Condition
    private AIDebugerTreeNode ParseCondition( XElement btNodeE, Dictionary<string, string> paramsDic)
    {
        return new AIDebugerTreeConditionNode("条件函数节点", m_Template, int.Parse(paramsDic[BTDataKey.NODE_TYPE_CONDITION_LIMIT]));
    }
    #endregion

    #region Action
    private AIDebugerTreeNode ParseAction(XElement btNodeE, Dictionary<string, string> paramsDic)
    {
        string szNodeName = btNodeE.Attribute(BTDataKey.NODE_SECOND_TYPE).Value;
        switch (szNodeName)
        {
            case BTDataKey.NODE_NAME_IDLE:
                return ParseA_Idle( btNodeE, paramsDic);
                break;
            case BTDataKey.NODE_NAME_MOVETO:
                return ParseA_Moveto( btNodeE, paramsDic);
                break;
        }

        return null;
    }
    private AIDebugerTreeNode ParseA_Moveto(XElement btNodeE, Dictionary<string, string> paramsDic)
    {
        int targetid = 0;
        int followPointid = 0;

        string tmpString = string.Empty;

        if (paramsDic.TryGetValue("targetId", out tmpString))
        {
            int.TryParse(tmpString, out targetid);
        }
        if (paramsDic.TryGetValue("followPointId", out tmpString))
        {
            int.TryParse(tmpString, out followPointid);
        }
        return new AIDebugerTreeMoveToNode("跟随玩家行为节点", m_Template, targetid, followPointid);
    }
    private AIDebugerTreeNode ParseA_Idle( XElement btNodeE, Dictionary<string, string> paramsDic)
    {
        return new AIDebugerTreeNode("休闲行为节点", m_Template);
    }
    #endregion

    #region Decorator
    private AIDebugerTreeNode ParseDecorator(XElement btNodeE, Dictionary<string, string> paramsDic)
    {
        string szNodeName = btNodeE.Attribute(BTDataKey.NODE_SECOND_TYPE).Value;
        switch (szNodeName)
        {
            case BTDataKey.NODE_NAME_INVERTER:
                return ParseInverter(btNodeE, paramsDic);
                break;
        }

        return null;
    }
    private AIDebugerTreeNode ParseInverter(XElement btNode, Dictionary<string, string> paramsDic)
    {
        return new AIDebugerTreeInverterNode("时间装饰节点", m_Template, int.Parse(paramsDic["inverter"]));
    }
    #endregion

    #region Gen xml
    private void GenXml(AIDebugerTreeNode parent, XElement btNodeE)
    {
        foreach (AIDebugerTreeNode node in parent.m_ChildList)
        {
            XElement xe = new XElement("btNode");
            btNodeE.Add(xe);

            if (node.m_strName == "选择节点")
            {
                xe.Add(new XAttribute("nodeType", BTDataKey.NODE_TYPE_SELECTOR));
            }
            else if (node.m_strName == "顺序节点")
            {
                xe.Add(new XAttribute("nodeType", BTDataKey.NODE_TYPE_SEQUENCE));
            }
            else if (node.m_strName == "时间装饰节点")
            {
                AIDebugerTreeInverterNode n = node as AIDebugerTreeInverterNode;
                xe.Add(new XAttribute("nodeType", BTDataKey.NODE_TYPE_DECORATOR));
                xe.Add(new XAttribute("name", BTDataKey.NODE_NAME_INVERTER));

                xe.Add(GetPropertyElement("inverter", n.m_iInverter));
            }
            else if (node.m_strName == "条件函数节点")
            {
                AIDebugerTreeConditionNode n = node as AIDebugerTreeConditionNode;
                xe.Add(new XAttribute("nodeType", BTDataKey.NODE_TYPE_CONDITION));
                xe.Add(GetPropertyElement("targetId", n.m_iLimitId));
                xe.Add(GetPropertyElement("limitId", n.m_iLimitId));
            }
            else if (node.m_strName == "休闲行为节点")
            {
                xe.Add(new XAttribute("nodeType", BTDataKey.NODE_TYPE_ACTION));
                xe.Add(new XAttribute("name", BTDataKey.NODE_NAME_IDLE));
            }
            else if (node.m_strName == "跟随玩家行为节点")
            {
                xe.Add(new XAttribute("nodeType", BTDataKey.NODE_TYPE_ACTION));
                xe.Add(new XAttribute("name", BTDataKey.NODE_NAME_MOVETO));
                AIDebugerTreeMoveToNode n = node as AIDebugerTreeMoveToNode;
                xe.Add(GetPropertyElement("targetId", n.m_iTargetId));
                xe.Add(GetPropertyElement("followPointId", n.m_iFollowPointId));
            }

            if (node.m_ChildList.Count > 0)
            {
                GenXml(node, xe);
            }
        }
    }
    private XElement GetPropertyElement(string key, object value)
    {
        XElement e = new XElement("property");
        e.Add(new XAttribute("key", key));
        e.Add(new XAttribute("value", value));
        return e;
    }
    public void GenXML(XElement root,AIDebugerTreeNode parent)
    {
        AIDebugerTreeRoot rootNode = parent as AIDebugerTreeRoot;
        XElement behaviorTreeE = new XElement("behaviorTree");
        root.Add(behaviorTreeE);
        behaviorTreeE.Add(new XAttribute("id", rootNode.ID));
        behaviorTreeE.Add(new XAttribute("desc", rootNode.Desc));

        if (rootNode.m_ChildList.Count > 0)
        {
            GenXml(rootNode, behaviorTreeE);
        }
    }

    #endregion
}