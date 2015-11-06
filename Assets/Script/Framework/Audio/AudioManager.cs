using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager:Singleton<AudioManager>
{
    public class AudioElementStruct
    {
        public GameObject   m_Root;
        public AudioClip    m_AudioClip;
        public AudioSource m_AudioSource;
        public bool         m_bIsLoop;
    }

    private List<AudioElementStruct>    m_AudioClipList;
    private GameObject                  m_AudioRoot;
    private Stack<AudioElementStruct>   m_AudioStorePool;
    private const int                   m_iPoolSize = 10;
    public void Initialize()
    {
        m_AudioRoot = ComponentTool.FindChild("AudioPlayer", null);
        m_AudioClipList = new List<AudioElementStruct>(m_iPoolSize);
        m_AudioStorePool = new Stack<AudioElementStruct>(m_iPoolSize);

        InitAudioPool();
    }
    public void PlayAudio(string resource,Vector3 postion,bool isLoop)
    {
        CheckResource();

        AudioElementStruct elem = CreateElement(resource, postion, isLoop);
        if (null != elem)
        {
            m_AudioClipList.Add(elem);
        }
    }
    public void StopAudio(string resource)
    {
        for (int i = 0; i < m_AudioClipList.Count; )
        {
            AudioElementStruct elem = m_AudioClipList[i];
            if (elem.m_Root.name == resource)
            {
                CollectionAudio(elem);
                m_AudioClipList.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }
    private AudioElementStruct CreateElement(string resource, Vector3 postion, bool isLoop)
    {
        AudioElementStruct elem = null;
        if (m_AudioStorePool.Count > 0)
        {
            elem = m_AudioStorePool.Pop();
        }
        else
        {
            elem = new AudioElementStruct();
            elem.m_Root = new GameObject("AudioClip");
            ComponentTool.Attach(m_AudioRoot.transform, elem.m_Root.transform);
            elem.m_AudioSource = elem.m_Root.AddComponent<AudioSource>();
        }

        elem.m_AudioClip = ResourceManager.Instance.LoadBuildInResource<AudioClip>(resource, AssetType.Audio);
        if (elem.m_AudioClip == null)
        {
            return null;
        }
        elem.m_Root.transform.position = postion;
        elem.m_AudioSource.clip = elem.m_AudioClip;
        elem.m_AudioSource.loop = isLoop;
        elem.m_bIsLoop = isLoop;
        elem.m_AudioSource.Play();
        elem.m_Root.name = resource;
        return elem;
    }
    private void InitAudioPool()
    {
        for (int i = 0; i < m_iPoolSize; ++i)
        {
            AudioElementStruct elem = new AudioElementStruct();
            elem.m_AudioClip = null;
            elem.m_Root = new GameObject("AudioClip");
            ComponentTool.Attach(m_AudioRoot.transform, elem.m_Root.transform);
            elem.m_Root.transform.position = Vector3.zero;
            elem.m_AudioSource = elem.m_Root.AddComponent<AudioSource>();
            elem.m_AudioSource.clip = elem.m_AudioClip;
            elem.m_AudioSource.playOnAwake = false;
            m_AudioStorePool.Push(elem);
        }
    }
    private void CheckResource()
    {
        for (int i = 0; i < m_AudioClipList.Count;)
        {
            AudioElementStruct elem = m_AudioClipList[i];
            if (!elem.m_bIsLoop && !elem.m_AudioSource.isPlaying)
            {
                CollectionAudio(elem);
                m_AudioClipList.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }
    private void CollectionAudio(AudioElementStruct elem)
    {
        elem.m_AudioSource.Stop();
        elem.m_AudioSource.clip = null;
        if (m_AudioStorePool.Count > m_iPoolSize)
        {
            GameObject.Destroy(elem.m_Root);
        }
        else
        {
            elem.m_AudioClip = null;
            m_AudioStorePool.Push(elem);
        }
    }
    public void Destructor()
    {
        for (int i = 0; i < m_AudioClipList.Count;++i )
        {
            CollectionAudio(m_AudioClipList[i]);
        }
        m_AudioClipList.Clear();
    }
}
