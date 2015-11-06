using TerrainEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainTrigger :  ITransformBehaviour
{
    protected TerrainTriggerData    m_NodeData;
    private HashSet<Ilife>          m_CurrentTargetList;
    private TransformData           m_TransformData;

    public void InitTrigger(TerrainTriggerData data)
    {
        m_CurrentTargetList = new HashSet<Ilife>();
        m_NodeData = data;
    }
    public virtual bool ExecNode()
    {
        HandleTarget target = TargetMethods.GetTargetList(this, m_NodeData.TargetMethodId);

        if (target.GetTarget(EFuncTarget.EFT_Target).Count == 0)
        {
            if (m_CurrentTargetList.Count == 0)
            {
                // no target ,not trigger
                return true;
            }
            else
            {
                foreach (Ilife elem in m_CurrentTargetList)
                {
                    //离开
                    HandleTarget tmpTarget = HandleTarget.GetHandleTarget(elem);
                    if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.ExitLimitMethodId))
                    {
                        FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.ExitFuncMethodId);
                    }
                    HandleTarget.CollectionHandlerTargetInstance(tmpTarget);
                }
                m_CurrentTargetList.Clear();
            }
            return true;
        }

        List<Ilife> targetList = target.GetTarget(EFuncTarget.EFT_Target);
        for (int i = 0; i < targetList.Count; ++i)
        {
            if (m_NodeData.IsTick)
            {
                //进入
                HandleTarget tmpTarget = HandleTarget.GetHandleTarget(targetList[i]);
                if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.EnterLimitMethodId))
                {
                    FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.EnterFuncMethodId);
                }
                HandleTarget.CollectionHandlerTargetInstance(tmpTarget);
            }
            
            if( !m_CurrentTargetList.Contains(targetList[i]))
            {
                if (!m_NodeData.IsTick)
                {
                    //进入
                    HandleTarget tmpTarget = HandleTarget.GetHandleTarget(targetList[i]);
                    if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.EnterLimitMethodId))
                    {
                        FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.EnterFuncMethodId);
                    }
                    HandleTarget.CollectionHandlerTargetInstance(tmpTarget);
                }
            }
            else
            {
                m_CurrentTargetList.Remove(targetList[i]);
            }
        }

        foreach (Ilife elem in m_CurrentTargetList)
        {
            HandleTarget tmpTarget = HandleTarget.GetHandleTarget(elem);
            if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.ExitLimitMethodId))
            {
                //离开
                FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.ExitFuncMethodId);
            }
            HandleTarget.CollectionHandlerTargetInstance(tmpTarget);
        }
        ResetCurrentTargetList(targetList);

        //collection target instance
        HandleTarget.CollectionHandlerTargetInstance(target);

        return true;
    }
    public Vector3 GetPosition()
    {
        return m_NodeData.Pos.GetVector3();
    }
    private void ResetCurrentTargetList(List<Ilife> targetList)
    {
        m_CurrentTargetList.Clear();
        for (int i = 0; i < targetList.Count; ++i)
        {
            m_CurrentTargetList.Add(targetList[i]);
        }
    }
    public TransformData GetTransformData()
    {
        return m_TransformData;
    }

    public int GetInstanceId()
    {
        return 0;
    }
}
public class TerrainTriggerManager
{
    private List<TerrainTrigger>        m_TriggerList;
    private int                         m_nTmpCount;
    private int                         m_nTmpTimeCount;
    private int                         m_nLastIndex;
    private const int                   m_nProcessNodeMaxTimeLimit = 20;
    private const int                   m_nTriggerCheckTimeOutCount = 10;
    private TerrainEditorData           m_TerrainData;
    
    public void InitTerrainTrigger(int terrainId)
    {
        if (null == m_TriggerList)
        {
            m_TriggerList = new List<TerrainTrigger>();
        }
        ClearTrigger();

        // add trigger node to list
        m_TerrainData = ConfigManager.Instance.GetTerrainEditorData(terrainId);
        if (null == m_TerrainData)
        {
            return;
        }

        if (m_TriggerList.Capacity < m_TerrainData.TriggerDataList.Count)
        {
            m_TriggerList.Capacity = m_TerrainData.TriggerDataList.Count;
        }

        for (int i = 0; i < m_TerrainData.TriggerDataList.Count; ++i)
        {
            TerrainTrigger node = new TerrainTrigger();
            node.InitTrigger(m_TerrainData.TriggerDataList[i]);
            m_TriggerList.Add(node);
        }
        TerrainTriggerTickTask.Instance.RegisterToUpdateList(Update);
        TerrainTriggerTickTask.Instance.SetStatus(true);
    }
    public void ClearTrigger()
    {
        m_TriggerList.Clear();
        m_nLastIndex = 0;
        TerrainTriggerTickTask.Instance.UnRegisterFromUpdateList(Update);
        TerrainTriggerTickTask.Instance.SetStatus(false);
    }
    private void Update()
    {
        m_nTmpCount = 0;
        m_nTmpTimeCount = (int) (TimeManager.Instance.Now);

        for (; m_nLastIndex < m_TriggerList.Count; ++m_nLastIndex,++m_nTmpCount)
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
            if (!m_TriggerList[m_nLastIndex].ExecNode())
            {
                break;
            }
        }
    }
}
