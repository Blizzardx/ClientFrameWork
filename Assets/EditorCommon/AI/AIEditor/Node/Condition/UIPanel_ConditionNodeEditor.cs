using UnityEngine;
using System.Collections;

public class UIPanel_ConditionNodeEditor : AINodeEditorBase
{
    public UIInput m_InputLimitId;


    public override AIDebugerTreeNode GetNode()
    {
        int limitId = 0;
        int.TryParse(m_InputLimitId.value, out limitId);
       
        return new AIDebugerTreeConditionNode(m_strNodeName, UIWindow_BTViewPanel.GetInstance.m_NodeTempalte, limitId); ;
    }

    public override void InitPanel(AIDebugerTreeNode node)
    {
        m_EditNode = node as AIDebugerTreeConditionNode;

        m_InputLimitId.value = ((AIDebugerTreeConditionNode)(m_EditNode)).m_iLimitId.ToString();
    }

    public override void ResetToRef()
    {
        base.ResetToRef();
        var node = m_EditNode as AIDebugerTreeConditionNode;
        int limitId = 0;
        int.TryParse(m_InputLimitId.value, out limitId);
        node.m_iLimitId = limitId;
    }
}
