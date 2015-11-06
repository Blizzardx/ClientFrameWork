using System;
using System.IO;
using Assets.Scripts.Core.Utils;
using Assets.Scripts.Framework.Network;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    #region public interface
    public void Initialize()
    {
        AdaptiveUI();
        ClearTmpCache();

        TimeManager.Instance.Initialize();
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
        MapManager.Instance.Initialize();
        FuncMethodDef.InitFuncMethod();
        LimitMethodDef.InitLimitMethod();
        TargetMethodDef.InitTargetMethod();
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
	private int i=0;
    private Player a;
    private List<Vector3> moveTarget;
    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            a = new Player();
            a.Initialize(1);
            moveTarget = new List<Vector3>(10);
            moveTarget.Add(new Vector3(0, 0, 1));
            moveTarget.Add(new Vector3(0, 0, 2));
            moveTarget.Add(new Vector3(2, 0, 3));
            moveTarget.Add(new Vector3(4, 0, 3));
            moveTarget.Add(new Vector3(5, 0, 4));
            moveTarget.Add(new Vector3(6, 0, 5));
            moveTarget.Add(new Vector3(7, 0, 6));
            moveTarget.Add(new Vector3(7, 0, 10));
            moveTarget.Add(new Vector3(8, 0, 11));
            moveTarget.Add(new Vector3(9, 0, 12));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            a.GetTransformData().MoveTo(moveTarget, 0.5f, 0.01f);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            a.GetStateController().TryEnterState(ELifeState.Idle, false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            a.GetStateController().TryEnterState(ELifeState.Walk, false);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            a.GetStateController().TryEnterState(ELifeState.Run, false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            WindowManager.Instance.OpenWindow(WindowID.WindowTest1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            List<AssetFile> tmp = new List<AssetFile>();

            for (int i = 0; i < 40; ++i)
            {
                AssetFile newElem = new AssetFile("tmp" + i.ToString(),
                    Application.persistentDataPath + "/downloadTest/tmp" + i.ToString() + ".txt",
                    "http://112.126.74.237:8080/client/config/targetConfig_txtpkg.bytes");
                tmp.Add(newElem);
            }
            AssetsDownloader.Instance.BeginDownload(tmp,
                (buffer, fileInfo) =>
                {
                    Debuger.Log("Done : " + fileInfo.Name);
                },
                (e, fileInfo) =>
                {
                    Debuger.Log("error : " + e.Message);
                },
                (process, file) =>
                {
                    Debuger.Log("process : " + process);
                }, 
                () =>
                {
                    Debuger.Log("All done");
                });
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            AudioManager.Instance.PlayAudio("music_defeat", Vector3.zero, false);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            AudioManager.Instance.PlayAudio("music_menu", Vector3.zero, true);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            AudioManager.Instance.StopAudio("music_menu");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            WindowManager.Instance.CloseWindow(WindowID.WindowTest3);
        }
		if (Input.GetKeyDown (KeyCode.L)) 
		{
			Debuger.Log ("test" + i++);
		}
        if (Input.GetKeyDown(KeyCode.M))
        {
            List<int> a = null;
            a.Add(0);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            string content = FileUtils.ReadStringFile("test");
            byte[] tmp = System.Text.Encoding.UTF8.GetBytes(content );
            FileUtils.WriteByteFile(Application.persistentDataPath + "/byte.jpg", tmp);
            Debuger.Log(Application.persistentDataPath);
            
        }
    }
    #endregion

}
