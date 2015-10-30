using System.Collections.Generic;
using UnityEngine;

public class TickTaskManager : Singleton<TickTaskManager>
{
    private List<AbstractTickTask> m_TickTaskStore;

    #region public interface
    public void InitializeTickTaskSystem()
    {
        m_TickTaskStore = new List<AbstractTickTask>(5);

        m_TickTaskStore.Add(new TimerTickTask());
        m_TickTaskStore.Add(new MessageTickTask());
        m_TickTaskStore.Add(new PlayerTickTask());
        m_TickTaskStore.Add(new UITickTask());
        m_TickTaskStore.Add(new PingTickTask());
        //m_TickTaskStore.Add(new TerrainTriggerTickTask());
        m_TickTaskStore.Add(new TimerTickTask());
    }
    public void Update()
    {
        foreach (AbstractTickTask tickTask in m_TickTaskStore)
        {
            tickTask.Tick();
        }
    }
	public void SyncLastTickTime(long lastTickTime)
    {
        foreach (AbstractTickTask tickTask in m_TickTaskStore)
        {
            tickTask.SetLastTickTime(lastTickTime);
        }
	}

    #endregion
}