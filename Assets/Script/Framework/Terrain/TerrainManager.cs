using TerrainEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
    private TerrainTriggerManager   m_TriggerMgr;
    private TerrainEditorData       m_CurrentTerrainData;
    private Dictionary<int, Npc>    m_NpcMap;

    public void InitializeTerrain(int terrainId)
    {
        m_CurrentTerrainData = ConfigManager.Instance.GetTerrainEditorData(terrainId);
        if (null == m_CurrentTerrainData)
        {
            Debuger.LogError("error terrain id : " + terrainId);
            return;
        }
        if (null == m_TriggerMgr)
        {
            m_TriggerMgr = new TerrainTriggerManager();
        }
        m_TriggerMgr.InitTerrainTrigger(m_CurrentTerrainData.TriggerDataList);

        if (null == m_NpcMap)
        {
            m_NpcMap = new Dictionary<int, Npc>(m_CurrentTerrainData.NpcDataList.Count);
        }

        //init npc
        for (int i = 0; i < m_CurrentTerrainData.NpcDataList.Count; ++i)
        {
            TerrainNpcData elem = m_CurrentTerrainData.NpcDataList[i];

            Npc newNpc = new Npc();
            newNpc.Initialize(elem.Id);
            newNpc.GetTransformData().SetPosition(elem.Pos.GetVector3());
            newNpc.GetTransformData().SetRotation(elem.Rot.GetVector3());
            newNpc.GetTransformData().SetScale(elem.Scale.GetVector3());
        }

        TerrainTickTask.Instance.RegisterToUpdateList(Update);
        TerrainTickTask.Instance.SetStatus(true);
        GameCamera.OpenClick = true;
    }
    public void CloseTerrain()
    {
        m_TriggerMgr.ClearTrigger();
        ClearNpcStore();
        GameCamera.OpenClick = false;
        TerrainTickTask.Instance.UnRegisterFromUpdateList(Update);
        TerrainTickTask.Instance.SetStatus(false);
    }

    private void ClearNpcStore()
    {
        foreach (var elem in m_NpcMap)
        {
            elem.Value.Distructor();
        }
        m_NpcMap.Clear();
    }
    private void Update()
    {
        m_TriggerMgr.Update();
    }
}
