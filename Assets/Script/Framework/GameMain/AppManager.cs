using UnityEngine;
using System.Collections;

public class AppManager : SingletonTemplateMon<AppManager>
{
    public bool m_bIsShowDebugMsg;
    public bool m_bIsShowTerrainTrigger;
    public bool m_bIsDebugMode;

    void Awake()
    {
        _instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(this);
        if (!m_bIsDebugMode)
        {
            m_bIsDebugMode = PlayerPrefs.GetInt("IsDebugMode",0) == 1;
        }
    }
	void Start () 
    {
	    GameManager.Instance.Initialize();
	}
	void Update () 
    {
	    GameManager.Instance.Update();
	}
    void OnDestroy()
    {
        
    }
    void OnApplicationQuit()
    {
        GameManager.Instance.OnAppQuit();
    }
}
