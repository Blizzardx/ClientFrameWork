using UnityEngine;
using System.Collections;
public class GM : SingletonTemplateMon<GM>
{
    public const string m_strChaneToGM  = "ChangeToGM";
    public const string m_strOpenLog    = "OpenLog";
    public const string m_strResetUserTalent = "ResetUserTalent";
    public const string m_strResetProcess = "ResetProcess";
    public const string m_strOpenDebuger = "OpenDebug";
    public const string m_strCloseDebuger = "CloseDebug";
    public const string m_strOpenDebugMode = "OpenDebugMode";
    public const string m_strCloseDebugMode = "CloseDebugMode";
    public bool m_bIsGMEnable;

    private void Awake()
    {
        _instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bIsGMEnable)
        {
            return;
        }

        if (Input.touchCount >= 4 || Input.GetKeyDown(KeyCode.F12))
        {
            WindowManager.Instance.OpenWindow(WindowID.UIGM);
        }
    }
    public bool InputGM(string cmd)
    {
        if(cmd.Equals(m_strChaneToGM))
        {
            ChangeToGM();
            return true;
        }
        if(!IsGMID())
        {
            return false;
        }
        if (cmd.Equals(m_strOpenLog))
        {
            OpenLog();
            return true;
        }
        if (cmd.Equals(m_strResetUserTalent))
        {
            OpenLog();
            return true;
        }
        if(cmd.Equals(m_strResetProcess))
        {
            ResetProcess();
            return true;
        }
        if (cmd.Equals(m_strOpenDebuger))
        {
            OpenDebug();
            return true;
        }
        if (cmd.Equals(m_strCloseDebuger))
        {
            CloseDebug();
            return true;
        }
        if(cmd.Equals(m_strOpenDebugMode))
        {
            OpenDebugMode(true);
            return true;
        }
        if (cmd.Equals(m_strCloseDebugMode))
        {
            OpenDebugMode(false);
            return true;
        }
        return false;
    }
    public bool IsGMID()
    {
        if (PlayerManager.Instance == null)
        {
            return false;
        }
        if (PlayerManager.Instance.GetCharBaseData() == null)
        {
            return false;
        }
       return PlayerManager.Instance.GetCharBaseData().CharRole > 0;
    }
    private void OpenLog()
    {
        var obj = GameObject.Find("Reporter");
        if (null != obj)
        {
            obj.SetActive(true);
        }
        else
        {
            var o = Resources.Load("BuildIn/Widget/Reporter");
            if(null != o)
            {
                GameObject newobj = GameObject.Instantiate(o) as GameObject;
                ComponentTool.Attach(null, newobj.transform);
            }
        }
    }
    private void ResetUserTalent()
    {
        AdaptiveDifficultyManager.Instance.ResetUserTalentToDefault();
        Debuger.Log("reset user talent");
    }
    private void ChangeToGM()
    {
        if (PlayerManager.Instance == null)
        {
            return;
        }
        if (PlayerManager.Instance.GetCharBaseData() == null)
        {
            return;
        }
        if(PlayerManager.Instance.GetCharBaseData().CharRole > 0)
        {
            return;
        }
        PlayerManager.Instance.GetCharBaseData().CharRole = 1;
    }
    private void ResetProcess()
    {
        if (PlayerManager.Instance == null)
        {
            return;
        }
        if(PlayerManager.Instance.GetCharBaseData() == null)
        {
            return;
        }
        PlayerManager.Instance.GetCharBaseData().CharDeatail = null;
    }
    private void OpenDebug()
    {
        var comp = GetComponent<AppManager>();
        if (comp != null)
        {
            comp.m_bIsShowDebugMsg = true;
        }
        Debuger.IsEnableLog = true;
    }
    private void CloseDebug()
    {
        var comp = GetComponent<AppManager>();
        if (comp != null)
        {
            comp.m_bIsShowDebugMsg = false;
        }
        Debuger.IsEnableLog = false;
    }
    private void OpenDebugMode(bool status)
    {
        var comp = GetComponent<AppManager>();
        if (comp != null)
        {
            comp.m_bIsDebugMode = status;
        }
        PlayerPrefs.SetInt("IsDebugMode", status ? 1 : 0);
    }
}
