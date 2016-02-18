//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : EffectManager
//
// Created by : Baoxue at 2015/11/24 19:32:25
//
//
//========================================================================

using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

public class EffectContainer : MonoBehaviour
{
    [SerializeField]
    private uint        m_iId;

    private static Dictionary<uint, GameObject> m_EffectInstanceIdMap = new Dictionary<uint, GameObject>();

    public static void InsertToMap(uint instanceId, GameObject instance)
    {
        if (!m_EffectInstanceIdMap.ContainsKey(instanceId))
        {
            m_EffectInstanceIdMap.Add(instanceId, instance);
        }
    }
    public static void RemoveFromMap(uint instanceId)
    {
        m_EffectInstanceIdMap.Remove(instanceId);
    }
    public static GameObject GetInstance(uint instanceId)
    {
        GameObject res = null;
        m_EffectInstanceIdMap.TryGetValue(instanceId, out res);
        return res;
    }
    public static GameObject EffectFactory(string name, uint id = 0)
    {
        GameObject source = ResourceManager.Instance.LoadBuildInResource<GameObject>(name, AssetType.Effect);
        if (null == source)
        {
            Debuger.LogWarning("can't load effect " + name);
            return null;
        }
        
        GameObject instance = GameObject.Instantiate(source);
        EffectContainer container = instance.AddComponent<EffectContainer>();
        container.Initialize(id);
        
        // create effect
        return instance;
    }
    public static int GetEffectCount()
    {
        return m_EffectInstanceIdMap.Count;
    }
    public uint GetId()
    {
        return m_iId;
    }
    private void OnDestroy()
    {
        // to do:
        RemoveFromMap(m_iId);
    }
    private void Initialize(uint id)
    {
        m_iId = id == 0 ? CreateInstanceId() : id;
        InsertToMap(m_iId, this.gameObject);
    }
    public static uint CreateInstanceId()
    {
        uint instanceId = (uint) TimeManager.Instance.Now;
        instanceId = instanceId << 4;
        uint randomValue = (uint)Random.Range(0, 16);
        instanceId |= randomValue;
        return instanceId;
    }
    public static bool CheckEffectavailable(uint id)
    {
        return m_EffectInstanceIdMap.ContainsKey(id);
    }
}
