using System;
using UnityEngine;
using System.Collections.Generic;

public enum MapType
{
    MapType_2d,
    MapType_3d,
}

public class MapIndexStruct
{
    public MapIndexStruct(string path,MapType type)
    {
        m_strPath = path;
        m_MapType = type;
    }
    public string  m_strPath;
    public MapType m_MapType;
}
public class MapManager : Singleton<MapManager>
{
    private Dictionary<MapId, MapIndexStruct>   m_MapIndexStore;
    private GameObject                          m_2dMapRoot;
    private GameObject                          m_3dMapRoot;
    private Dictionary<MapId, List<GameObject>>       m_MapInstanceStore;
 
    private const string                        m_str2dMapRoot = "2DMapRoot";
    private const string                        m_str3dMapRoot = "3DMapRoot";

    public void Initialize()
    {
        m_MapIndexStore = new Dictionary<MapId, MapIndexStruct>();
        m_MapInstanceStore = new Dictionary<MapId, List<GameObject>>();

        m_2dMapRoot = ComponentTool.FindChild(m_str2dMapRoot, null);
        m_3dMapRoot = ComponentTool.FindChild(m_str3dMapRoot, null);

		MapDefiner.RegisterMap();
    }
    public void RegisterMap(MapId id, MapIndexStruct info)
    {
        m_MapIndexStore.Add(id, info);
    }
    public void Destructor()
    {
        ClearMapInstance();
    }
    public void LoadMapAsync(MapId id, Action<GameObject> callBack)
    {
        MapIndexStruct info = GetMapInfo(id);
        string path = info.m_strPath;
        
        if (string.IsNullOrEmpty(path))
        {
            callBack(null);
            return;
        }
        ResourceManager.Instance.LoadBuildInAssetsAsync(path, AssetType.UI, (origin) =>
        {
            callBack(CreateMapInstance(id, info.m_MapType, origin as GameObject));
        });
    }
    public GameObject LoadMap(MapId id)
    {
        MapIndexStruct info = GetMapInfo(id);
        string path = info.m_strPath;

        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        GameObject origin = ResourceManager.Instance.LoadBuildInResource<GameObject>(path, AssetType.Map);
        return CreateMapInstance(id, info.m_MapType, origin);
    }
    public MapIndexStruct GetMapInfo(MapId id)
    {
        MapIndexStruct info = null;
        if (m_MapIndexStore.TryGetValue(id, out info))
        {
            return info;
        }
        else
        {
            Debuger.LogError("can't load map " + id);
            return null;
        }
    }
    public void Clear2DMap()
    {
        while (m_2dMapRoot.transform.childCount > 0)
        {
            GameObject.Destroy(m_2dMapRoot.transform.GetChild(0));
        }
    }
    public void Clear3DMap()
    {
        while (m_3dMapRoot.transform.childCount > 0)
        {
            GameObject.Destroy(m_2dMapRoot.transform.GetChild(0));
        }
    }
    public void ClearMapInstance()
    {
        foreach (var elem in m_MapInstanceStore)
        {
            foreach (var listElem in elem.Value)
            {
                GameObject.Destroy(listElem);
            }
        }
        m_MapInstanceStore.Clear();
    }
    private GameObject CreateMapInstance(MapId id,MapType type, GameObject origin)
    {
        if (null == origin)
        {
            return null;
        }

        GameObject instance = GameObject.Instantiate(origin) as GameObject;
        Transform root = type == MapType.MapType_2d ? m_2dMapRoot.transform : m_3dMapRoot.transform;
        ComponentTool.Attach(root, instance.transform);

        if (!m_MapInstanceStore.ContainsKey(id))
        {
            m_MapInstanceStore.Add(id, new List<GameObject>());
        }
        m_MapInstanceStore[id].Add(instance);

        return instance;
    }
}
