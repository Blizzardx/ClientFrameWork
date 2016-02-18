using System;
using TerrainEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainTrigger :  ITransformBehaviour
{
    protected TerrainTriggerData    m_NodeData;
    private HashSet<Ilife>          m_CurrentTargetList;
    private TransformDataBase       m_TransformData;
    private bool                    m_bIsShowObject;
    private GameObject              m_TriggerObject;

    public void InitTrigger(TerrainTriggerData data,bool isShowTrigger)
    {
        m_bIsShowObject = isShowTrigger;
        m_CurrentTargetList = new HashSet<Ilife>();
        m_NodeData = data;
        m_TransformData = new TransformDataBase();
        m_TransformData.SetPosition(m_NodeData.Pos.GetVector3());
        m_TransformData.SetRotation(m_NodeData.Rot.GetVector3());
        m_TransformData.SetScale(m_NodeData.Scale.GetVector3());

        if (m_bIsShowObject)
        {
            //create obj
            CreateObject();
        }
    }
    public virtual bool ExecNode()
    {
        HandleTarget target = TargetMethods.GetTargetList(this, m_NodeData.TargetMethodId,null);

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
                    Debuger.Log("trigger exit " + elem.GetInstanceId());
                    HandleTarget tmpTarget = HandleTarget.GetHandleTarget(this, elem);
                    if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.ExitLimitMethodId, null))
                    {
                        FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.ExitFuncMethodId, null);
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
                Debuger.Log("trigger enter " + targetList[i].GetInstanceId());
                HandleTarget tmpTarget = HandleTarget.GetHandleTarget(this,targetList[i]);
                if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.EnterLimitMethodId, null))
                {
                    FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.EnterFuncMethodId, null);
                }
                HandleTarget.CollectionHandlerTargetInstance(tmpTarget);
            }
            
            if( !m_CurrentTargetList.Contains(targetList[i]))
            {
                if (!m_NodeData.IsTick)
                {
                    //进入
                    Debuger.Log("trigger enter " + targetList[i].GetInstanceId());
                    HandleTarget tmpTarget = HandleTarget.GetHandleTarget(this, targetList[i]);
                    if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.EnterLimitMethodId, null))
                    {
                        FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.EnterFuncMethodId, null);
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
            HandleTarget tmpTarget = HandleTarget.GetHandleTarget(this, elem);
            if (LimitMethods.HandleLimitExec(tmpTarget, m_NodeData.ExitLimitMethodId, null))
            {
                //离开
                Debuger.Log("trigger exit " + elem.GetInstanceId());
                FuncMethods.HandleFuncExec(tmpTarget, m_NodeData.ExitFuncMethodId, null);
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
    public TransformDataBase GetTransformData()
    {
        return m_TransformData;
    }
    public int GetInstanceId()
    {
        return 0;
    }
    public void Distructor()
    {
        if (m_bIsShowObject && m_TriggerObject != null)
        {
            GameObject.Destroy(m_TriggerObject);
        }
    }
    private void CreateObject()
    {
        string path = string.Empty;
        switch (m_NodeData.AreaType)
        {
            case ETriggerAreaType.Sphere:
                path = "Trigger_Sphere";
                break;
                case ETriggerAreaType.Cube:
                path = "Trigger_Cube";
                break;
        }
        m_TriggerObject = GameObject.Instantiate(ResourceManager.Instance.LoadBuildInResource<GameObject>(path, AssetType.Trigger));
        if (null != m_TriggerObject)
        {
            m_TriggerObject.transform.position = m_NodeData.Pos.GetVector3();
            m_TriggerObject.transform.eulerAngles = m_NodeData.Rot.GetVector3();
            m_TriggerObject.transform.localScale = m_NodeData.Scale.GetVector3();
        }
        else
        {
            Debuger.LogError("Can't load trigger at : " + path);
        }

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
    
    public void InitTerrainTrigger(List<TerrainTriggerData> triggerDataList,bool isShowTrigger)
    {
        if (null == m_TriggerList)
        {
            m_TriggerList = new List<TerrainTrigger>();
        }

        m_TriggerList.Clear();
        m_nLastIndex = 0;

        if (m_TriggerList.Capacity < triggerDataList.Count)
        {
            m_TriggerList.Capacity = triggerDataList.Count;
        }

        for (int i = 0; i < triggerDataList.Count; ++i)
        {
            TerrainTrigger node = new TerrainTrigger();
            node.InitTrigger(triggerDataList[i],isShowTrigger);
            m_TriggerList.Add(node);
        }
    }
    public void ClearTrigger()
    {
        foreach (var elem in m_TriggerList)
        {
            elem.Distructor();
        }
        m_TriggerList.Clear();
        m_nLastIndex = 0;
    }
    public void Update()
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
        m_nLastIndex = 0;
    }
}
