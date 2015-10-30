using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;

public class TerrainTriggerNode :Ilife
{
    protected int       m_nTargetId;
    protected int       m_nLimitId;
    protected int       m_nFuncId;
    protected Vector3   m_vPosition;
    protected Vector3   m_vForword;
    protected Vector3   m_vScale;

    public virtual bool ExecNode()
    {
        HandleTarget target = TargetMethods.GetTargetList(this, m_nTargetId);
        if (target.GetTarget(EFuncTarget.EFT_Target).Count == 0)
        {
            // no target ,not trigger
            return true;
        }

        if (LimitMethods.HandleLimitExec(target, m_nLimitId))
        {
            if (FuncMethods.HandleFuncExec(target, m_nFuncId) == -1)
            {
                return true;
            }
        }

        //collection target instance
        HandleTarget.CollectionHandlerTargetInstance(target);

        return true;
    }

    public Vector3 GetPosition()
    {
        return m_vPosition;
    }

    public Vector3 GetForword()
    {
        return m_vForword;
    }

    public Vector3 GetScale()
    {
        return m_vScale;
    }
}
public class TerrainTrigger
{
    private List<TerrainTriggerNode>    m_TriggerMap;
    private int                         m_nTmpCount;
    private int                         m_nTmpTimeCount;
    private int                         m_nLastIndex;
    private bool                        m_bIsProcessing;
    private const int                   m_nProcessNodeMaxTimeLimit = 20;
    private const int                   m_nTriggerCheckTimeOutCount = 10;

    public void InitTerrainTrigger(int terrainId)
    {
        if (null == m_TriggerMap)
        {
            m_TriggerMap = new List<TerrainTriggerNode>();
        }
        m_TriggerMap.Clear();
        m_nLastIndex = 0;
        // add trigger node to list

        m_bIsProcessing = false;
    }
    public void Update()
    {
        m_nTmpCount = 0;
        m_nTmpTimeCount = (int) (TimeManager.Instance.Now);

        for (; m_nLastIndex < m_TriggerMap.Count; ++m_nLastIndex,++m_nTmpCount)
        {
            if (m_nTmpCount >= m_nTriggerCheckTimeOutCount)
            {
                m_nTmpCount = 0;

                if (m_nTmpTimeCount >= m_nProcessNodeMaxTimeLimit)
                {
                    // time out 
                    break;
                }
            }

            // process node
            if (!m_TriggerMap[m_nLastIndex].ExecNode())
            {
                break;
            }
        }
    }
}
