using UnityEngine;
using System.Collections.Generic;
using Common.Config;

public abstract class TargetMethodBase
{
    public abstract List<Ilife> GetTargetList(Ilife thisUnit, TargetData data);
}

public enum EFuncTarget
{
    EFT_Target,
    EFT_User,
}
public class HandleTarget
{
    // first always user in list.
    List<Ilife> m_lstUser = new List<Ilife>();
    List<Ilife> m_lstTarget = new List<Ilife>();

    public HandleTarget(Ilife userTarget, params Ilife[] targetList)
    {
        m_lstUser.Add(userTarget);
        if (null != targetList)
        {
            foreach (Ilife Child in targetList)
            {
                m_lstTarget.Add(Child);
            }
        }
    }

    public HandleTarget(Ilife userTarget, List<Ilife> targetList)
    {
        m_lstUser.Add(userTarget);
        m_lstTarget = targetList;
    }

    //
    public List<Ilife> GetTarget(EFuncTarget eTarget)
    {
        switch (eTarget)
        {
            case EFuncTarget.EFT_Target:
                return m_lstTarget;
            case EFuncTarget.EFT_User:
                return m_lstUser;
                break;
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
    public static HandleTarget GetTargetList(Ilife srcLife, int iTargetGroupId)
    {
        HandleTarget handle = new HandleTarget(srcLife);
        if (iTargetGroupId <= 0)
        {
            return handle;
        }

        TargetGroup targetGroup = new TargetGroup();//DataManager.Instance.GetTargetGroup(iTargetGroupId);
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

            List<Ilife> targetUnitList = handler.GetTargetList(srcLife, data);
            handle.AddTarget(targetUnitList);
        }

        return handle;
    }


}