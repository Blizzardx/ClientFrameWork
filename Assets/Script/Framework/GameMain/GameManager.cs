using System;
using NetFramework.Auto;
using Thrift.Protocol;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    private List<Action>    m_UpdateList;
    private List<Action>    m_UnregisterUpdateListStore;
    private bool            m_bIsUpdateListBusy;

    #region public interface
    public void Initialize()
    {
        m_UpdateList                    = new List<Action>();
        m_UnregisterUpdateListStore     = new List<Action>();
        m_bIsUpdateListBusy             = false;

        TimeManager.Instance.Initialize();
		LogManager.Instance.Initialize ();
        ResourceManager.Instance.Initialize();
        TickTaskManager.Instance.InitializeTickTaskSystem();
        StageManager.Instance.Initialize();
        SceneManager.Instance.Initialize();
        MessageManager.Instance.Initialize();
        WindowManager.Instance.Initialize();
        SystemMsgHandler.Instance.RegisterSystemMsg();
        DataBindManager.Instance.Initialize();

    }
    public void Update()
    {
        TimeManager.Instance.Update();
        MessageManager.Instance.Update();
        TickTaskManager.Instance.Update();

        ExcutionUpdateList();

        Test();
    }
    public void OnAppQuit()
    {
        LogManager.Instance.OnQuit();
    }
    public void RegisterToUpdateList(Action element)
    {
        for (int i = 0; i < m_UpdateList.Count; ++i)
        {
            if (element == m_UpdateList[i])
            {
                return;
            }
        }
        m_UpdateList.Add(element);
    }
    public void UnRegisterFromUpdateList(Action element)
    {
        if (!m_bIsUpdateListBusy)
        {
            m_UpdateList.Remove(element);
        }
        else
        {
            for (int i = 0; i < m_UnregisterUpdateListStore.Count; ++i)
            {
                if (element == m_UnregisterUpdateListStore[i])
                {
                    return;
                }
            }
            m_UnregisterUpdateListStore.Add(element);
        }
    }
    #endregion

    #region system function
    private void ExcutionUpdateList()
    {
        m_bIsUpdateListBusy = true;
        foreach (Action elem in m_UpdateList)
        {
            elem();
        }
        m_bIsUpdateListBusy = false;
        ExcutionUnregister();
    }
    private void ExcutionUnregister()
    {
        if (m_UnregisterUpdateListStore.Count == 0)
        {
            return;
        }
        for (int i = 0; i < m_UnregisterUpdateListStore.Count; ++i)
        {
            m_UpdateList.Remove(m_UnregisterUpdateListStore[i]);
        }
        m_UnregisterUpdateListStore.Clear();
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
		 
    }
    #endregion
}
