using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

public class AIDebugerTreeMoveToNode : AIDebugerTreeNode
{
    public int m_iTargetId;
    public int m_iFollowPointId;

    public AIDebugerTreeMoveToNode(string name, GameObject template,int targetId,int followPointId)
        : base(name, template)
    {
        m_iTargetId = targetId;
        m_iFollowPointId = followPointId;
    }
}