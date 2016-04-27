using System;
using System.IO;
using System.Text;
using Assets.Scripts.Core.Utils;
using Assets.Scripts.Framework.Network;
using UnityEngine;
using System.Collections.Generic;
using Moudles.BaseMoudle.Converter;
using Cache;
using NetWork.Auto;
using NetWork;
using Config;
using GameConfigTools.Model;

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
        WindowManager.Instance.Initialize();
        SystemMsgHandler.Instance.RegisterSystemMsg();
       // ScriptManager.Instance.Initialize();
        AudioPlayer.Instance.Initialize();
        ConverterManager.Instance.Initialize();
        FuncMethodDef.InitFuncMethod();
        LimitMethodDef.InitLimitMethod();
        TargetMethodDef.InitTargetMethod();
        ActionManager.Instance.Initialize();
        // check asset
        AssetUpdateManager.Instance.CheckUpdate(() =>
        {
            ConfigManager.Instance.InitBigConfigData();
            AdaptiveDifficultyManager.Instance.Initialize();
            CustomMain.Instance.Initialize();
        });
    }
    public void Update()
    {
        TickTaskManager.Instance.Update();
        Test();
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

    #region test
    private void Test()
    {
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
        {
            TipManager.Instance.Alert("", "你确定要退出游戏吗？", "确定", "取消", (res) =>
            {
                if (res)
                {
                    Application.Quit();
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            WindowBase window = WindowManager.Instance.GetWindow(WindowID.MissionDebugAttach);
            if (null == window)
            {
                WindowManager.Instance.OpenWindow(WindowID.MissionDebugAttach);
            }
            else
            {
                if (window.IsOpen())
                {
                    window.Close();
                }
                else
                {
                    WindowManager.Instance.OpenWindow(WindowID.MissionDebugAttach);
                }
            }
        } 
        if(Input.GetKeyDown(KeyCode.C) || Input.touchCount == 3)
        {
            string filepath = ResourceManager.Instance.GetStreamAssetPath() + "/config.xml";

            ResourceManager.Instance.LoadStreamingAssetsData(filepath, (www) =>
            {
                SysConfig tmp = XmlConfigBase.DeSerialize<SysConfig>(www.text);
                Debuger.LogError("system config decode result : " + tmp == null);
                if (tmp != null)
                {
                    Debuger.LogError(tmp.ServerConfigPath);
                    Debuger.LogError(tmp.ClientConfigPath);
                    Debuger.LogError(tmp.ExcelPath);
                    Debuger.LogError(tmp.ConfigCenterUrl);
                    Debuger.LogError(tmp.UploadUrl);

                    Debuger.LogError("begin encode resoult");
                    string res = XmlConfigBase.Serialize(tmp);
                    Debuger.LogError("encode result:" + res);
                    FileUtils.WriteStringFile(Application.persistentDataPath + "/tesw.xml", res);

                    tmp = XmlConfigBase.DeSerialize<SysConfig>(res);
                    Debuger.LogError("xxx system config decode result : " + tmp == null);
                    if (tmp != null)
                    {
                        Debuger.LogError(tmp.ServerConfigPath);
                        Debuger.LogError(tmp.ClientConfigPath);
                        Debuger.LogError(tmp.ExcelPath);
                        Debuger.LogError(tmp.ConfigCenterUrl);
                        Debuger.LogError(tmp.UploadUrl);

                        Debuger.LogError("begin encode resoult");
                        res = XmlConfigBase.Serialize(tmp);
                        Debuger.LogError("encode result:" + res);
                        FileUtils.WriteStringFile(Application.persistentDataPath + "/tesw1.xml", res);
                    }
                }

                
            });

             
        }
    }
    #endregion

}
