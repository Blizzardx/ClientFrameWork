using System.ComponentModel.Design.Serialization;
using UnityEngine;
using System.Collections.Generic;
using Config;

public abstract class TargetMethodBase
{
    private int m_iId;

    public int GetId()
    {
        return m_iId;
    }

    public TargetMethodBase(int id)
    {
        m_iId = id;
    }
    public abstract List<Ilife> GetTargetList(Ilife thisUnit, TargetData data, FuncContext context);
}

public enum EFuncTarget
{
    EFT_Target,
    EFT_User,
}
public class HandleTarget
{
    #region memory pool
    private static LinkedList<HandleTarget> m_HandlerTargetCollectionPool   = new LinkedList<HandleTarget>();
    private static HandleTarget CreateHandlerTarget()
    {
        HandleTarget instance = null;
        if (m_HandlerTargetCollectionPool.Count == 0)
        {
            // creat buffer
            for (int i = 0;i<20;++i)
            {
                m_HandlerTargetCollectionPool.AddLast(new HandleTarget());
            }

            instance = new HandleTarget();
        }
        else
        {
            instance = m_HandlerTargetCollectionPool.Last.Value;
            m_HandlerTargetCollectionPool.RemoveLast();
        }
        return instance;
    }
    public static HandleTarget GetHandleTarget(Ilife userTarget, params Ilife[] targetList)
    {
        HandleTarget instance = CreateHandlerTarget();
        instance.m_lstUser.Add(userTarget);
        if (null != targetList)
        {
            foreach (Ilife Child in targetList)
            {
                instance.m_lstTarget.Add(Child);
            }
        }

        return instance;
    }
    public static  HandleTarget GetHandleTarget(Ilife userTarget, List<Ilife> targetList)
    {
        HandleTarget instance = CreateHandlerTarget();
        instance.m_lstUser.Add(userTarget);
        instance.m_lstTarget = targetList;

        return instance;
    }
    public static void CollectionHandlerTargetInstance(HandleTarget instance)
    {
        instance.m_lstTarget.Clear();
        instance.m_lstUser.Clear();

        m_HandlerTargetCollectionPool.AddLast(instance);
    }
    private HandleTarget()
    {
    }
    #endregion


    // first always user in list.
    List<Ilife> m_lstUser = new List<Ilife>();
    List<Ilife> m_lstTarget = new List<Ilife>();
    //
    public List<Ilife> GetTarget(EFuncTarget eTarget)
    {
        switch (eTarget)
        {
            case EFuncTarget.EFT_Target:
                return m_lstTarget;
            case EFuncTarget.EFT_User:
                return m_lstUser;
        }

        return null;
    }
    public void AddTarget(params Ilife[] targetList)
    {
        if (null == targetList)
        {
            return;
        }

        foreach (Ilife target in targetList)
        {
            if (!m_lstTarget.Contains(target))
            {
                m_lstTarget.Add(target);
            }
        }
    }
    public void AddTarget(List<Ilife> targetList)
    {
        if (null == targetList)
        {
            return;
        }

        foreach (Ilife target in targetList)
        {
            if (!m_lstTarget.Contains(target))
            {
                m_lstTarget.Add(target);
            }
        }
    }
}

public static class TargetMethods
{
    static Dictionary<int, TargetMethodBase> TargetExec;
    static public void InitTargetMethods(Dictionary<int, TargetMethodBase> dataSource)
    {
        TargetExec = dataSource;
    }
    public static HandleTarget GetTargetList(Ilife srcLife, int iTargetGroupId, FuncContext context)
    {
        HandleTarget handle = HandleTarget.GetHandleTarget(srcLife);

        if (iTargetGroupId == 0)
        {
            return handle;
        }

        TargetGroup targetGroup =  ConfigManager.Instance.GetTargetGroup(iTargetGroupId);
        if (null == targetGroup)
        {
            Debug.LogError("target groupId:" + iTargetGroupId + " is not found.");
            return handle;
        }
        if (null == targetGroup.TargetDataList || 0 == targetGroup.TargetDataList.Count)
        {
            Debug.LogError("target groupId:" + iTargetGroupId + " targetDataList is null.");
            return handle;
        }
        TargetMethodBase handler = null;
        foreach (TargetData data in targetGroup.TargetDataList)
        {
            if (!TargetExec.TryGetValue(data.TargetId,out handler))
            {
                continue;
            }

            List<Ilife> targetUnitList = handler.GetTargetList(srcLife, data,context);
            handle.AddTarget(targetUnitList);
        }

        return handle;
    }


}