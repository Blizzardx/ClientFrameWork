using System;
using Assets.Scripts.Core.Utils;
using NetFramework.Auto;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    #region public interface
    public void Initialize()
    {
        AdaptiveUI();

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
        SdkManager.Instance.InitSdk();
        CustomMain.Instance.Initialize();
        /*// check asset
        AssetUpdate.Instance.BeginCheck(() =>
        {
            CustomMain.Instance.Initialize();
        });*/
        
    }
    public void Update()
    {
        TickTaskManager.Instance.Update();
        //Test();
    }
    public void OnAppQuit()
    {
        CustomMain.Instance.Quit();
        LogManager.Instance.OnQuit();
        NetWorkManager.Instance.Disconnect();
    }
    #endregion

    #region system function
   
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
    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            NetWorkManager.Instance.Connect("123.57.220.33", 8090);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            CSLoginMsg tmpmsg = new CSLoginMsg();
            tmpmsg.OsType = OSType.Android;
            tmpmsg.ChannelCode = ChannelCodeConstants.LOCAL_USER_PWD;
            tmpmsg.DeviceId = "10001";
            tmpmsg.DeviceModel = "aaa";
            tmpmsg.OsVersion = "ios8";
            tmpmsg.Terminal = Terminal.MOBIE;
            tmpmsg.UserpasswordLogin = new UserPasswordLogin();
            tmpmsg.UserpasswordLogin.Username = "baoxue456";
            tmpmsg.UserpasswordLogin.Password = "";

            NetWorkManager.Instance.SendMsgToServer(tmpmsg);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            CSApplyDisbandArmyMsg tmpMsg = new CSApplyDisbandArmyMsg();

            NetWorkManager.Instance.SendMsgToServer(tmpMsg);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
           StageManager.Instance.ChangeState(GameStateType.MainCityState);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            StageManager.Instance.ChangeState(GameStateType.LoginState);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            WindowManager.Instance.OpenWindow(WindowID.WindowTest1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            WindowManager.Instance.CloseWindow(WindowID.WindowTest1);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            WindowManager.Instance.OpenWindow(WindowID.WindowTest2);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            WindowManager.Instance.CloseWindow(WindowID.WindowTest2);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            WindowManager.Instance.OpenWindow(WindowID.WindowTest3);
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
