using System.Collections;
using System.IO;
using Thrift.Protocol;
using Thrift.Transport;
using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

public enum AssetType
{
    UI,
    Animation,
    Audio,
}
public class LoadResourceElement
{
    public string           realPath;
    public ResourceRequest  request;
    public bool             isFinishedDownload;
}
public class ResourceManager : SingletonTemplateMon<ResourceManager>
{
    private Dictionary<string, UnityEngine.Object>                      m_AssetStore;
    private Dictionary<string, List<Action<UnityEngine.Object>>>        m_AssetLoadCallBackStore;
    private List<string>                                                m_LoadingStore; 

    public void Initialize()
    {
        m_AssetStore                = new Dictionary<string, UnityEngine.Object>();
        m_AssetLoadCallBackStore    = new Dictionary<string, List<Action<UnityEngine.Object>>>();
        m_LoadingStore              = new List<string>();
    }
    public void Destructor()
    {
        m_AssetStore.Clear();
        m_AssetLoadCallBackStore.Clear();
        m_LoadingStore.Clear();
    }
    public T LoadBuildInResource<T>(string path,AssetType type ) where T : UnityEngine.Object
    {
        string realPath = GetRealPath(path, type);
        
        //add build in tag
        realPath = "BuildIn/" +  realPath;

        Debuger.Log(realPath);
        UnityEngine.Object res = null;
        m_AssetStore.TryGetValue(realPath, out res);
        if (null != res)
        {
            return res as T;
        }
        else
        {
            res = Resources.Load<T>(realPath);
            if (m_AssetStore.ContainsKey(realPath))
            {
                m_AssetStore[realPath] = res;
            }
            else
            {
                m_AssetStore.Add(realPath, res);
            }
            return res as T;
        }
    }
    public void LoadAssetsAsync(string path, AssetType type, Action<UnityEngine.Object> CallBack)
    {
        StartLoadResource(path, type, CallBack);
    }
    public IEnumerator StartLoadResource(string path, AssetType type, Action<UnityEngine.Object> CallBack) 
    {
        string realPath = GetRealPath(path, type);
        UnityEngine.Object res = null;
        m_AssetStore.TryGetValue(realPath, out res);
        if (null != res)
        {
            CallBack(res);
            yield return null;
        }

        //register to call back store
        RegisterToCallBackStore(realPath,CallBack);

        if (IsAssetsOnLoading(realPath))
        {
            yield return null;
        }
        else
        {
            //start load
            ResourceRequest result = Resources.LoadAsync(realPath);

            //add to download list
            RegisterToLoadingStore(realPath);

            yield return result;

            res = result.asset;
            if (m_AssetStore.ContainsKey(realPath))
            {
                m_AssetStore[realPath] = res;
            }
            else
            {
                m_AssetStore.Add(realPath, res);
            }

            UnRegisterFromLoadingStore(realPath);
            DoResourceCallBack(realPath,res);
        }
    }
    public T DecodeTemplate<T>(string path) where T : TBase, new()
    {
        var membuffer = new TMemoryBuffer(Resources.Load<TextAsset>(path).bytes);
        TProtocol protocol = (new TCompactProtocol(membuffer));
        var temp = new T();
        temp.Read(protocol);
        return temp;
    }
    private void RegisterToCallBackStore(string realPath,Action<UnityEngine.Object> callBack)
    {
        if (m_AssetLoadCallBackStore.ContainsKey(realPath))
        {
            for (int i = 0; i < m_AssetLoadCallBackStore[realPath].Count; ++i)
            {
                if (m_AssetLoadCallBackStore[realPath][i] == callBack)
                {
                    return;
                }
            }
            m_AssetLoadCallBackStore[realPath].Add(callBack);
        }
        else
        {
            m_AssetLoadCallBackStore[realPath] = new List<Action<Object>>();
            m_AssetLoadCallBackStore[realPath].Add(callBack);
        }
    }
    private void DoResourceCallBack(string realPath,UnityEngine.Object result)
    {
        for (int i = 0; i < m_AssetLoadCallBackStore[realPath].Count; ++i)
        {
            m_AssetLoadCallBackStore[realPath][i](result);
        }
        m_AssetLoadCallBackStore[realPath].Clear();
    }
    private void RegisterToLoadingStore(string realPath)
    {
        m_LoadingStore.Add(realPath);
    }
    private void UnRegisterFromLoadingStore(string realPath)
    {
        m_LoadingStore.Remove(realPath);
    }
    private bool IsAssetsOnLoading(string realPath)
    {
        for (int i = 0; i < m_LoadingStore.Count; ++i)
        {
            if (realPath == m_LoadingStore[i])
            {
                return true;
            }
        }
        return false;
    }
    private string GetRealPath(string path, AssetType type)
    {
        switch (type)
        {
           case AssetType.UI:
                path = "UI/Prefab/" + path;
                break;
            case AssetType.Animation:
                path = "Animation/" + path;
                break;
            case AssetType.Audio:
                path = "Audio/" + path;
                break;
            default:
            {
                // do nothing
            }
                break;
        }
        return path;
    }
    private void Awake()
    {
        _instance = this;
    }
}
