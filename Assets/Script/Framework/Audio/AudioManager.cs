using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Audio manager
/// </summary>
public class AudioIndexStruct
{
    public AudioIndexStruct(string path)
    {
        m_strPath = path;
    }
    public string m_strPath;
}
public class AudioManager:Singleton<AudioManager>
{
    //private members
    private GameObject                              m_AudioSourceRoot;
    private Dictionary<AudioId, AudioIndexStruct>   m_AudioResourceMap; 
    private Dictionary<AudioId, AudioClip>          m_UISoundClipStore;
    private Dictionary<AudioId, AudioClip>          m_EffectSoundClipStore;
    private Dictionary<AudioId, AudioClip>          m_BackgroundSoundClipStore;
    private List<AudioSource>                       m_EffectAudioSourceStore;
    private List<AudioSource>                       m_UIAudioSourceStore;
    private AudioSource                             m_BgAudioSource;
    private float                                   m_fUIVolume;
    private float                                   m_fEffectVolume;
    private float                                   m_fBgVolume;
    private bool                                    m_bIsMuteEffect;
    private bool                                    m_bIsMuteUI;
    private bool                                    m_bIsMuteBg;
    private int                                     m_StoreSizeMax      = 5;

    #region public interface
    public void Initialize()
    {
        m_fUIVolume     = 1.0f; //default volume 100%
        m_fEffectVolume = 1.0f; //default volume 100%
        m_fBgVolume     = 0.6f; //default volume 60%

        m_AudioSourceRoot           = ComponentTool.FindChild("AudioPlayer",AppManager.Instance.gameObject);
        m_UISoundClipStore          = new Dictionary<AudioId, AudioClip>();
        m_EffectSoundClipStore      = new Dictionary<AudioId, AudioClip>();
        m_BackgroundSoundClipStore  = new Dictionary<AudioId, AudioClip>();
        m_UIAudioSourceStore        = new List<AudioSource>();
        m_EffectAudioSourceStore    = new List<AudioSource>();
        m_AudioResourceMap          = new Dictionary<AudioId, AudioIndexStruct>();

        //add background audio source
        GameObject tmp              = new GameObject();
        tmp.transform.parent        = m_AudioSourceRoot.transform;
        tmp.name                    = "BackgroundAudioSource";
        tmp.transform.localPosition = Vector3.zero;
        m_BgAudioSource             = tmp.AddComponent<AudioSource>();
        m_BgAudioSource.volume      = m_fBgVolume;

        //execution sound setting
        MuteUISound(false);
        MuteEffectSound(false);
        MuteBackgroundSound(false);

        AudioDefiner.RegisterAudio();
    }
    public void Destructor()
    {
        for (int i = 0; i < m_EffectAudioSourceStore.Count; ++i)
        {
            GameObject.Destroy(m_EffectAudioSourceStore[i]);
        }
        for (int i = 0; i < m_UIAudioSourceStore.Count; ++i)
        {
            GameObject.Destroy(m_UIAudioSourceStore[i]);
        }
        m_EffectAudioSourceStore.Clear();
        m_UIAudioSourceStore.Clear();
        m_BgAudioSource.Stop();
    }
    public void SetUIVolume(float volume)
    {
        m_fUIVolume = volume;
        ClearAudioStore();
        foreach (AudioSource elem in m_UIAudioSourceStore)
        {
            elem.volume = m_fUIVolume;
        }
    }
    public void SetEffectVolume(float volume)
    {
        m_fEffectVolume = volume;
        ClearAudioStore();
        foreach (AudioSource elem in m_EffectAudioSourceStore)
        {
            elem.volume = m_fEffectVolume;
        }
    }
    public void SetBgVolume(float volume)
    {
        m_fBgVolume = volume;
        m_BgAudioSource.volume = m_fBgVolume;
    }
    public void MuteAll(bool status)
    {
        MuteEffectSound(status);
        MuteUISound(status);
        MuteBackgroundSound(status);
    }
    public void MuteEffectSound(bool status)
    {
        m_bIsMuteEffect = status;
        ClearAudioStore();
        foreach (AudioSource elem in m_EffectAudioSourceStore)
        {
            elem.mute = status;
        }
        //PlayerPrefs.SetInt(m_strEffectMuteTag, m_bIsMuteEffect ? 2 : 1);
    }
    public void MuteUISound(bool status)
    {
        m_bIsMuteUI = status;
        ClearAudioStore();
        foreach (AudioSource elem in m_UIAudioSourceStore)
        {
            elem.mute = status;
        }
        //PlayerPrefs.SetInt(m_strUIMuteTag, m_bIsMuteUI ? 2 : 1);
    }
    public void MuteBackgroundSound(bool status)
    {
        m_bIsMuteBg = status;
        m_BgAudioSource.mute = status;
        //PlayerPrefs.SetInt(m_strBgMuteTag, m_bIsMuteBg ? 2 : 1);
    }
    public void PlayUISound(AudioId type)
    {
        if (m_bIsMuteUI)
        {
            return;
        }

        GetUIAudioClip(type, (AudioClip tmpClip) => 
        {
            if (null != tmpClip)
            {
                AudioPlayUISound(tmpClip);
            }
        });
        
    }
    public void PlayEffectSound(AudioId type, Vector3 targetRoot,bool isLoop = false)
    {
        if (m_bIsMuteEffect)
        {
            return;
        }

        GetEffectAudioClip(type, (AudioClip clip) =>
        {
            if (null != clip)
            {
                AudioPlayEffectSound(clip, targetRoot,isLoop);
            }
        });
        
    }
    public void PlayBackgroundSound(AudioId type,bool isLoop  = false)
    {
        GetBgAudioClip(type, (AudioClip tmpClip) =>
        {
            if (null != tmpClip)
            {
                AudioPlayBg(tmpClip, isLoop);
            }
        });
        
    }
    public bool GetIsMuteEffect()
    {
        return m_bIsMuteEffect;
    }
    public bool GetIsMuteUI()
    {
        return m_bIsMuteUI;
    }
    public bool GetIsMuteBg()
    {
        return m_bIsMuteBg;
    }
    public void ClearAudioStore()
    {
        for (int i = 0; i < m_EffectAudioSourceStore.Count; )
        {
            if (!m_EffectAudioSourceStore[i].isPlaying)
            {
                AudioSource tmp = m_EffectAudioSourceStore[i];
                m_EffectAudioSourceStore.RemoveAt(i);
                GameObject.Destroy(tmp.gameObject);
            }
            else
            {
                ++i;
            }
        }
        for (int i = 0; i < m_UIAudioSourceStore.Count; )
        {
            if (!m_UIAudioSourceStore[i].isPlaying)
            {
                AudioSource tmp = m_UIAudioSourceStore[i];
                m_UIAudioSourceStore.RemoveAt(i);
                GameObject.Destroy(tmp.gameObject);
            }
            else
            {
                ++i;
            }
        }
    }
    public void RegisterAudio(AudioId id, AudioIndexStruct info)
    {
        m_AudioResourceMap.Add(id, info);
    }
    public void StopEffectSound(AudioId type)
    {
        string targetName = m_AudioResourceMap[type].m_strPath;
        for (int i = 0; i < m_EffectAudioSourceStore.Count;++i )
        {
            if (m_EffectAudioSourceStore[i].clip.name == targetName)
            {
                Debuger.Log("Remove effect sound " + targetName);
                AudioSource tmp = m_EffectAudioSourceStore[i];
                m_EffectAudioSourceStore.RemoveAt(i);
                GameObject.Destroy(tmp.gameObject);
                break;
            }
        }
    }
    #endregion

    #region system property
    private void GetUIAudioClip(AudioId type, Action<AudioClip> CallBack)
    {
        if (m_UISoundClipStore.ContainsKey(type))
        {
            CallBack(m_UISoundClipStore[type]);
        }
        else
        {
            LoadClip(type, (UnityEngine.Object clip) =>
            {
                AudioClip tmpElem = clip as AudioClip;
                if (null != tmpElem)
                {
                    m_UISoundClipStore.Add(type, tmpElem);
                    CallBack(tmpElem);
                }
                else
                {
                    // can't load audio resource
                    CallBack(null);
                    Debuger.LogError("Can't load audio clip named: " + type.ToString());
                }
            });
        }
    }
    private void GetBgAudioClip(AudioId type, Action<AudioClip> CallBack)
    {
        if (m_BackgroundSoundClipStore.ContainsKey(type))
        {
            CallBack(m_BackgroundSoundClipStore[type]);
        }
        else
        {
            LoadClip(type, (UnityEngine.Object clip) => 
            {
                AudioClip tmpElem = clip as AudioClip; 
                if (null != tmpElem)
                {
                    m_BackgroundSoundClipStore.Add(type, tmpElem);
                    CallBack(tmpElem);
                }
                else
                {
                    // can't load audio resource
                    CallBack(null);
                    Debuger.LogError("Can't load audio clip named: " + type.ToString());
                }
            });            
        }
    }
    private void GetEffectAudioClip(AudioId type, Action<AudioClip> CallBack)
    {
        if (m_EffectSoundClipStore.ContainsKey(type))
        {
            CallBack(m_EffectSoundClipStore[type]);
        }
        else
        {
            LoadClip(type, (UnityEngine.Object clip) =>
            {
                AudioClip tmpElem = clip as AudioClip;
                if (null != tmpElem)
                {
                    m_EffectSoundClipStore.Add(type, tmpElem);
                    CallBack(tmpElem);
                }
                else
                {
                    // can't load audio resource
                    CallBack(null);
                    Debuger.LogError("Can't load audio clip named: " + type.ToString());
                }
            });
        }
    }
    private void LoadClip(AudioId type,Action<UnityEngine.Object> callBack)
    {
        AudioIndexStruct tmp = null;
        if (type == AudioId.None)
        {
            callBack(null);
            return;
        }
        if (m_AudioResourceMap.TryGetValue(type, out tmp))
        {
            ResourceManager.Instance.LoadBuildInAssetsAsync(tmp.m_strPath, AssetType.Audio, callBack);
            return;
        }
        else
        {
            Debuger.Log("can't load audio : " + type);
            callBack(null);
        }

    }
    private void AudioPlayBg(AudioClip clip,bool isLoop)
    {
        if (m_BgAudioSource.clip != clip)
        {
            m_BgAudioSource.clip = clip;
            m_BgAudioSource.loop = isLoop;
            m_BgAudioSource.Play();
        }
    }
    private void AudioPlayUISound(AudioClip clip)
    {
        if (m_UIAudioSourceStore.Count >= m_StoreSizeMax)
        {
            ClearAudioStore();
        }
        GameObject tmp = new GameObject();
        tmp.transform.parent = m_AudioSourceRoot.transform;
        tmp.name = "AudioSource";
        tmp.transform.localPosition = Vector3.zero;
        AudioSource tmpSource = tmp.AddComponent<AudioSource>();
        tmpSource.clip = clip;
        tmpSource.volume = m_fUIVolume;
        tmpSource.Play();
        m_UIAudioSourceStore.Add(tmpSource);
    }
    private void AudioPlayEffectSound(AudioClip clip, Vector3 targetRoot,bool isLoop)
    {
        if (m_EffectAudioSourceStore.Count >= m_StoreSizeMax)
        {
            ClearAudioStore();
        }
        GameObject tmp = new GameObject();
        tmp.transform.position = targetRoot;
        tmp.name = "AudioSource";
        AudioSource tmpSource = tmp.AddComponent<AudioSource>();
        tmpSource.clip = clip;
        tmpSource.volume = m_fEffectVolume;
        tmpSource.loop = isLoop;
        tmpSource.Play();
        m_EffectAudioSourceStore.Add(tmpSource);
    }
    #endregion
}
