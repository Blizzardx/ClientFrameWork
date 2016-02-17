using UnityEngine;
using System.Collections;

public class UIPanel_InverterNodeEditor : AINodeEditorBase
{
    public UIInput m_InputInverterId;


    public override AIDebugerTreeNode GetNode()
    {
        int InverterId = 0;
        int.TryParse(m_InputInverterId.value, out InverterId);

        return new AIDebugerTreeInverterNode(m_strNodeName, UIWindow_BTViewPanel.GetInstance.m_NodeTempalte, InverterId); ;
    }

    public override void InitPanel(AIDebugerTreeNode node)
    {
        m_EditNode = node as AIDebugerTreeInverterNode;

        m_InputInverterId.value = ((AIDebugerTreeInverterNode)(m_EditNode)).m_iInverter.ToString();
    }
    public override void ResetToRef()
    {
        base.ResetToRef();
        var node = m_EditNode as AIDebugerTreeInverterNode;
        int InverterId = 0;
        int.TryParse(m_InputInverterId.value, out InverterId);
        node.m_iInverter = InverterId;
    }
}
