using Common.Auto;
using TerrainEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : Singleton<TerrainManager>
{
    private TerrainTriggerManager   m_TriggerMgr;
    private TerrainEditorData       m_CurrentTerrainData;
    private Dictionary<int, Npc>    m_NpcMap;
    private List<int>               m_ActionState;
    private bool                    m_bIsActive;

    public void InitializeTerrain(int terrainId,bool IsShowTrigger,bool isInitNpcAI = true)
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
        m_TriggerMgr.InitTerrainTrigger(m_CurrentTerrainData.TriggerDataList, IsShowTrigger);

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
            newNpc.SetAIStatus(isInitNpcAI);
            m_NpcMap.Add(elem.Id,newNpc);
        }

        TerrainTickTask.Instance.RegisterToUpdateList(Update);
        TerrainTickTask.Instance.SetStatus(true);
        GameCamera.OpenClick = true;
        m_bIsActive = true;
        m_ActionState = new List<int>();
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_START, OnActionStar);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
    }
    public void CloseTerrain()
    {
        m_TriggerMgr.ClearTrigger();
        ClearNpcStore();
        GameCamera.OpenClick = false;
        TerrainTickTask.Instance.UnRegisterFromUpdateList(Update);
        TerrainTickTask.Instance.SetStatus(false);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_START, OnActionStar);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
    }
    public PlayerInitPosData GetPlayerInitPos()
    {
        return m_CurrentTerrainData.PlayerInitPos;
    }
    public void DestroyNpcById(int id)
    {
        Npc instance = null;
        if (m_NpcMap.TryGetValue(id, out instance))
        {
            instance.Distructor();
            m_NpcMap.Remove(id);
        }
        else
        {
            Debuger.LogWarning("can't find npc by id" + id);
        }
    }
    public void CreateNpcById(int id, ThriftVector3 pos, ThriftVector3 rot, ThriftVector3 scale)
    {
        //check
        if (m_NpcMap.ContainsKey(id))
        {
            Debuger.LogWarning("already exist npc id " + id);
            return;
        }
        Npc newNpc = new Npc();
        newNpc.Initialize(id);
        newNpc.GetTransformData().SetPosition(pos.GetVector3());
        newNpc.GetTransformData().SetRotation(rot.GetVector3());
        newNpc.GetTransformData().SetScale(scale.GetVector3());
        newNpc.SetAIStatus(true);
        m_NpcMap.Add(id, newNpc);
    }
    public Dictionary<int, Npc> GetNpcList()
    {
        return m_NpcMap;
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
        if (!m_bIsActive)
        {
            return;
        }
        m_TriggerMgr.Update();
    }
    private void OnActionStar(MessageObject obj)
    {
        if (!(obj.msgValue is ActionParam))
        {
            return;
        }
        ActionParam param = obj.msgValue as ActionParam;
        m_ActionState.Add(param.Id);
        m_bIsActive = false;
        GameCamera.OpenClick = false;
    }
    private void OnActionFinish(MessageObject obj)
    {
        if (!(obj.msgValue is ActionParam))
        {
            return;
        }
        ActionParam param = obj.msgValue as ActionParam;
        for (int i = 0; i < m_ActionState.Count; ++i)
        {
            if (param.Id == m_ActionState[i])
            {
                m_ActionState.RemoveAt(i);
                break;
            }
        }
        if (m_ActionState.Count == 0)
        {
            // open trigger
            m_bIsActive = true;
            GameCamera.OpenClick = true;
        }
    }
}
