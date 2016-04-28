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
    public TargetMethodBase()
    {
        DecodeId();
    }
    private void DecodeId()
    {
        m_iId = -1;
        string name = this.GetType().ToString();
        if (!name.StartsWith("Target_"))
        {
            UnityEngine.Debug.LogError("Function method with wrong name " + name);
            return;
        }

        // 
        var tmpname = name.Substring(7);
        int index = tmpname.LastIndexOf("_");
        if (index < 0)
        {
            UnityEngine.Debug.LogError("Function method with wrong name " + name);
            return;
        }
        tmpname = tmpname.Substring(0, index);
        int id = 0;
        if (!int.TryParse(tmpname, out id))
        {
            UnityEngine.Debug.LogError("Function method with wrong name " + name);
            return;
        }
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
    public static HandleTarget GetHandleTarget(Ilife userTarget, params Ilife[] targetList)
    {
        HandleTarget instance = new HandleTarget();
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
        HandleTarget instance = new HandleTarget();
        instance.m_lstUser.Add(userTarget);
        instance.m_lstTarget = targetList;

        return instance;
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
    static public void InitTargetMethods(List<TargetMethodBase> dataSource)
    {
        TargetExec = new Dictionary<int, TargetMethodBase>();
        foreach (var elem in dataSource)
        {
            if (elem.GetId() == -1)
            {
                continue;
            }
            TargetExec.Add(elem.GetId(), elem);
        }
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