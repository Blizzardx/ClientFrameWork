using System;
using System.IO;
using Assets.Scripts.Core.Utils;
using Assets.Scripts.Framework.Network;
using UnityEngine;
using System.Collections.Generic;
using Moudles.BaseMoudle.Converter;
using Cache;
using NetWork.Auto;
using NetWork;

public class GameManager : Singleton<GameManager>
{
    #region public interface
    public void Initialize()
    {
        AdaptiveUI();
        ClearTmpCache();

        TimeManager.Instance.Initialize();
        CacheManager.Init(Application.persistentDataPath + "/Cache");
		LogManager.Instance.Initialize (AppManager.Instance.m_bIsShowDebugMsg);
        ResourceManager.Instance.Initialize();
        TickTaskManager.Instance.InitializeTickTaskSystem();
        StageManager.Instance.Initialize();
        SceneManager.Instance.Initialize();
        MessageManager.Instance.Initialize();
        WindowManager.Instance.Initialize();
        SystemMsgHandler.Instance.RegisterSystemMsg();
        ScriptManager.Instance.Initialize();
        AudioManager.Instance.Initialize();
        ConverterManager.Instance.Initialize();
        FuncMethodDef.InitFuncMethod();
        LimitMethodDef.InitLimitMethod();
        TargetMethodDef.InitTargetMethod();
        ActionManager.Instance.Initialize();
        //CustomMain.Instance.Initialize();
        // check asset
        AssetUpdateManager.Instance.CheckUpdate(() =>
        {
            CustomMain.Instance.Initialize();
        });
        Debuger.Log(Application.persistentDataPath + " "); 
    }
    public void Update()
    {
        TickTaskManager.Instance.Update();
    }
    public void OnAppQuit()
    {
        CustomMain.Instance.Quit();
        LogManager.Instance.OnQuit();
        NetWorkManager.Instance.Disconnect();
    }
    #endregion

    #region system function

    private void ClearTmpCache()
    {
        var m_strTmpCache = Application.persistentDataPath + "/tmp/";
        if (Directory.Exists(m_strTmpCache))
        {
            Directory.Delete(m_strTmpCache, true);
        }
    }
    private void AdaptiveUI()
    {
        int ManualWidth = 1920;
        int ManualHeight = 1080;
        UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot>();
        if (uiRoot != null)
        {
            if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
                uiRoot.manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
            else
                uiRoot.manualHeight = ManualHeight;
        }
    }
    #endregion
    
}
