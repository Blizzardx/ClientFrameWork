
using System.Collections.Generic;
using UnityEngine;

public class TickTaskManager : Singleton<TickTaskManager>
{
    /// <summary>
    /// tick task type define
    /// </summary>
    public enum TickTaskType
    {
        PingTickTask,
        Max,
    }

    private Dictionary<TickTaskType, AbstractTickTask> m_TickTaskStore;

    #region public interface
    public void InitializeTickTaskSystem()
    {
        m_TickTaskStore = new Dictionary<TickTaskType, AbstractTickTask>();

        for (int i = 0; i < (int)(TickTaskType.Max); ++i)
        {
            AbstractTickTask elem = TickTaskFactory((TickTaskType)(i));
            if (null == elem)
            {
                Debuger.LogError("tick task is null " + (TickTaskType)(i));
            }
            m_TickTaskStore.Add((TickTaskType)(i),elem);
        }
    }
    public void SetTickTaskStatus(TickTaskType type,bool bStatus)
    {
        AbstractTickTask ticktask = null;
        m_TickTaskStore.TryGetValue(type, out ticktask);
        if (null != ticktask)
        {
            ticktask.m_bIsActive = bStatus;
        }
    }
    public void Update()
    {
        foreach (KeyValuePair<TickTaskType,AbstractTickTask> tickTask in m_TickTaskStore)
        {
            if (tickTask.Value.m_bIsActive)
            {
                tickTask.Value.Tick();
            }
        }
    }
	public void SyncLastTickTime(long lastTickTime)
    {
        foreach (KeyValuePair<TickTaskType, AbstractTickTask> tickTask in m_TickTaskStore)
        {
            if (tickTask.Value.m_bIsActive)
            {
                tickTask.Value.SetLastTickTime(lastTickTime);
            }
        }
	}

    #endregion

    #region system function
    private AbstractTickTask TickTaskFactory(TickTaskType type)
    {
        switch (type)
        {
            case TickTaskType.PingTickTask:
                return new PingTickTask();
            default:
                return null;
        }
        return null;
    }
    #endregion
}