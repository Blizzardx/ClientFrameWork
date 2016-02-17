using UnityEngine;
using System.Collections;

public class UIPanel_MoveNodeEditor : AINodeEditorBase
{
    public UIInput m_InputTargetId;
    public UIInput m_InputFollowPointId;

    public override AIDebugerTreeNode GetNode()
    {
        int targetId = 0;
        int.TryParse(m_InputTargetId.value, out targetId);
        int followPointId = 0;
        int.TryParse(m_InputFollowPointId.value, out followPointId);

        return new AIDebugerTreeMoveToNode(m_strNodeName, UIWindow_BTViewPanel.GetInstance.m_NodeTempalte, targetId,followPointId); ;
    }
    public override void InitPanel(AIDebugerTreeNode node)
    {
        m_EditNode = node as AIDebugerTreeMoveToNode;

        m_InputTargetId.value = ((AIDebugerTreeMoveToNode)(m_EditNode)).m_iTargetId.ToString();
        m_InputFollowPointId.value = ((AIDebugerTreeMoveToNode)(m_EditNode)).m_iFollowPointId.ToString();

    }

    public override void ResetToRef()
    {
        base.ResetToRef();
        var node = m_EditNode as AIDebugerTreeMoveToNode;
        int followPintId = 0;
        int.TryParse(m_InputFollowPointId.value, out followPintId);
        node.m_iFollowPointId = followPintId;
        int targetId = 0;
        int.TryParse(m_InputTargetId.value, out targetId);
        node.m_iTargetId = targetId;
    }
}
