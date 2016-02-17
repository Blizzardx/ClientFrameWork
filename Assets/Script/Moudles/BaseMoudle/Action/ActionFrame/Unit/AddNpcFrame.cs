//========================================================================
// Copyright(C): CYTX
//
// FileName : AddNpcFrame
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/20 14:04:55
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActionEditor;
using Config;
public class AddNpcFrame : AbstractActionFrame
{

    private AddNpcFrameConfig m_Config;
    private GameObject m_CreatedNpcObject;
    private Npc m_Npc;

    public AddNpcFrame(ActionPlayer action, ActionFrameData data)
        : base(action, data)
    {
        m_Config = m_FrameData.AddNpcFrame;
    }

    #region Public Interface
    public override bool IsTrigger(float fRealTime)
    {
        if (null == m_FrameData)
        {
            return false;
        }

        if (fRealTime >= m_FrameData.Time)
        {
            return true;
        }

        return false;
    }
    public override bool IsFinish(float fRealTime)
    {
        return false;
    }
    protected override void Execute()
    {
        AddNpc(m_Config.Id, m_Config.Pos.GetVector3(), m_Config.Rot.GetVector3(), m_Config.Scale.GetVector3());
        Dictionary<int, GameObject> generatedNpc = new Dictionary<int, GameObject>();
        if (m_CreatedNpcObject)
        {
            generatedNpc.Add(m_Config.TempId, m_CreatedNpcObject);
            m_ActionPlayer.InsertGeneratedObjects(generatedNpc);
        }
    }
    public override void Play()
    {
        throw new System.NotImplementedException();
    }
    public override void Pause(float fTime)
    {
        throw new System.NotImplementedException();
    }
    public override void Stop()
    {
        throw new System.NotImplementedException();
    }
    public override void Destory()
    {
        //clear npc
        if (m_Npc != null)
        {
            m_Npc.Distructor();
        }
        if (null != m_CreatedNpcObject)
        {
            GameObject.Destroy(m_CreatedNpcObject);
        }
    }
    #endregion

    #region System Functions
    private void AddNpc(int id, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        //NpcConfig tmpConfig = ConfigManager.Instance.GetNpcConfig(id);
        //if (null == tmpConfig)
        //{
        //    Debuger.LogWarning("id 错误 ，错误 id= " + id);
        //    return;
        //}
        //GameObject sourceObj = ResourceManager.Instance.LoadBuildInResource<GameObject>(tmpConfig.ModelResource,
        //    AssetType.Char);
        //if (null == sourceObj)
        //{
        //    Debuger.LogWarning("模型 id 错误 ，错误 id= " + id);
        //    return;
        //}
        //GameObject instance = GameObject.Instantiate(sourceObj);
        //ComponentTool.Attach(m_ObjNpcRoot.transform, instance.transform);

        m_Npc = new Npc();
        m_Npc.Initialize(id);
        m_Npc.GetTransformData().SetPosition(position);
        m_Npc.GetTransformData().SetRotation(rotation);
        m_Npc.GetTransformData().SetScale(scale);

        CharTransformData chardata = (CharTransformData)m_Npc.GetTransformData();
        m_CreatedNpcObject = chardata.GetGameObject();
        m_CreatedNpcObject.transform.position = position;
        m_CreatedNpcObject.transform.eulerAngles = rotation;
        m_CreatedNpcObject.transform.localScale = scale;
    }
    #endregion
}

