using System;
using Cache;
using NetWork;
using NetWork.Auto;
using UnityEngine;
using System.Collections.Generic;

public class LoginLogic : LogicBase<LoginLogic>
{
    private LoginRequest m_LoginRequest;
    private RegisterRequest m_RegisterRequest;
    private bool m_bIsCreateChar = false;
    private int m_iCharId;
    private string m_strDefaultUsername;
    private string m_strDefaultPassword;
    private const string m_strUsernameKey = "username";
    private const string m_strpasswordKey = "password";
    public override void StartLogic()
    {
        Debuger.Log("StartLogic LoginLogic");

        WindowManager.Instance.CloseAllWindow();
        WindowManager.Instance.OpenWindow(WindowID.Login); 
        
        SyncDataTickTask.SetSyncStatus(false);
    }
    public override void EndLogic()
    {
        Debuger.Log("EndLogic LoginLogic");
    }
    public void DoLogin(string username, string password)
    {
        m_strDefaultPassword = password;
        m_strDefaultUsername = username;

        m_LoginRequest = new LoginRequest();
        m_LoginRequest.Password = password;
        m_LoginRequest.Username = username;

        AsyncLoginRequest login = new AsyncLoginRequest(m_LoginRequest);
        login.TryRequest();
    }
    public void DoRegister(string username, string password, string password2)
    {
        m_RegisterRequest = new RegisterRequest();
        m_RegisterRequest.Username = username;
        m_RegisterRequest.Password = password;


        AsyncRegisterRequest register = new AsyncRegisterRequest(m_RegisterRequest);
        register.TryRequest();
    }
    public void DoCreateChar(sbyte age, string name, sbyte gender)
    {
        CreateNewCharRequest request = new CreateNewCharRequest();
        request.Age = age;
        request.CharName = name;
        request.Gender = gender;

        AsyncCreateCharRequest register = new AsyncCreateCharRequest(request);
        register.TryRequest();
    }
    public void OnLoginResponse(LoginResponse resp)
    {
        if (null != resp.FailSystemInfo)
        {
            TipManager.Instance.Alert(ConfigManager.Instance.GetMessage(resp.FailSystemInfo.MessageId));
        }
        else
        {
            SaveDefaultLoginRequest(m_strDefaultUsername, m_strDefaultPassword);

            if (resp.PlayerInfo.CharId == 0)
            {
                //create char
                WindowManager.Instance.OpenWindow(WindowID.CreateChar);
            }
            else
            {
                //try enter game
                TryEnterGame(resp.PlayerInfo.CharId);
            }
        }
    }
    public void OnRegisterResponse(RegisterResponse resp)
    {
        if (null != resp.FailSystemInfo)
        {
            TipManager.Instance.Alert(ConfigManager.Instance.GetMessage(resp.FailSystemInfo.MessageId));
        }
        else
        {
            //do login
            DoLogin(m_RegisterRequest.Username, m_RegisterRequest.Password);
        }
    }
    private void TryEnterGame(int charid)
    {
        var entergameRequest = new EnterGameRequest();
        entergameRequest.CharId = charid;
        m_iCharId = charid;
        AsyncEnterGame enterGame = new AsyncEnterGame(entergameRequest);
        enterGame.TryRequest();
    }
    public void OnCreateChar(CreateNewCharResponse resp)
    {
        if (null != resp.FailSystemInfo)
        {
            TipManager.Instance.Alert(ConfigManager.Instance.GetMessage(resp.FailSystemInfo.MessageId));
        }
        else
        {
            m_bIsCreateChar = true;
            TryEnterGame(resp.CharId);
        }
    }
    public void OnEnterGame(EnterGameResponse resp)
    {
        if (null != resp.CharacterInfo)
        {
            DoEnterGame(resp.CharacterInfo);
        }
        else
        {
            TipManager.Instance.Alert("", "系统错误", "重试", (res) =>
            {
                TryEnterGame(m_iCharId);
            });
        }
    }
    private void DoEnterGame(NetWork.Auto.CharacterInfo charInfo)
    {
        SyncDataTickTask.SetSyncStatus(true);
        PlayerManager.Instance.Initialize(charInfo);
        MissionManager.Instance.InitMissionMgr(PlayerManager.Instance.GetMissionData().MissionList);
        WorldSceneDispatchController.Instance.StartLogic();
        WorldSceneDispatchController.Instance.EnterWorldScene();

        MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_GAMELOGIC_SCENE_TRIGGER, GameLogicSceneType.Login));
        if(m_bIsCreateChar)
        {
            MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_GAMELOGIC_SCENE_TRIGGER, GameLogicSceneType.CreateChar));
            m_bIsCreateChar = false;
        }
    }
    public void GetDefaultLoginRequest(ref string username, ref string password)
    {
        try
        {
            //encode pkg
            CacheKeyInfo keyInfo = CacheKeyContants.STRING_KEY.BuildCacheInfo("");
            string content = CacheManager.GetInsance().Get(keyInfo) as string;
            Dictionary<string, object> root = Json.Deserialize(content) as Dictionary<string, object>;
            username = root[m_strUsernameKey] as string;
            password = root[m_strpasswordKey] as string;
        }
        catch (Exception)
        {

            Debuger.LogWarning("error on decode username");
        }
    }
    private void SaveDefaultLoginRequest(string username, string password)
    {
        try
        {
            Dictionary<string, object> contentInstance = new Dictionary<string, object>();
            contentInstance.Add(m_strUsernameKey, username);
            contentInstance.Add(m_strpasswordKey, password);
            string content = Json.Serialize(contentInstance);

            //decode pkg
            CacheKeyInfo keyInfo = CacheKeyContants.STRING_KEY.BuildCacheInfo("");
            CacheManager.GetInsance().Set(keyInfo, content);
        }
        catch (Exception)
        {

            Debuger.LogWarning("error on encode username");
        }
    }
}
