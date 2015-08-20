using System;
using UnityEngine;
using System.Collections.Generic;

public class MapIndexStruct
{
    public MapIndexStruct(string path)
    {
        m_strPath = path;
    }
    public string m_strPath;
}
public class MapManager : Singleton<MapManager>
{
    private Dictionary<MapId, MapIndexStruct> m_MapIndexStore;
 
    public void Initialize()
    {
        m_MapIndexStore = new Dictionary<MapId, MapIndexStruct>();
		MapDefiner.RegisterMap();
    }
    public void RegisterMap(MapId id, MapIndexStruct info)
    {
        m_MapIndexStore.Add(id, info);
    }
    public void Destructor()
    {
        
    }
    public void LoadMapAsync(MapId id, Action<UnityEngine.Object> callBack)
    {
        string path = GetMapPath(id);
        if (string.IsNullOrEmpty(path))
        {
            callBack(null);
            return;
        }
        ResourceManager.Instance.LoadAssetsAsync(path, AssetType.UI, callBack);
    }
    public UnityEngine.Object LoadMap(MapId id)
    {
        string path = GetMapPath(id);
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
       return ResourceManager.Instance.LoadBuildInResource<UnityEngine.Object>(path, AssetType.Map);
    }
    public string GetMapPath(MapId id)
    {
        MapIndexStruct info = null;
        if (m_MapIndexStore.TryGetValue(id, out info))
        {
            return info.m_strPath;
        }
        else
        {
            Debuger.LogError("can't load map " + id);
            return null;
        }
    }
}
