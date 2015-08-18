using System;
using Assets.Scripts.Core.Utils;
using UnityEngine;
using System.Collections.Generic;

public class ScriptManager : Singleton<ScriptManager>
{
    private string                      m_strCurrentRunningProjectPath;
    private Script                      m_ProjectScript;
    private Dictionary<string, Script>  m_WindowScript;

    #region common
    public void Initialize()
    {
        m_ProjectScript = new Script();
        m_WindowScript = new Dictionary<string, Script>();

        m_ProjectScript.Init();

        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (Debuger)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (GameObject)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (Transform)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (StageManager)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (WindowManager)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (WindowID)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (SceneManager)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (GameStateType)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (TimeManager)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (WindowLayer)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (ScriptManager)));
        m_ProjectScript.env.RegType(new CSLE.RegHelper_Type(typeof (WindowBase)));

        LoadProjectAndRun(Application.streamingAssetsPath + "/Project1");
    }
    public void LoadProjectAndRun(string path)
    {
        if (!string.IsNullOrEmpty(m_strCurrentRunningProjectPath))
        {
            ClearProjectAndStop(m_strCurrentRunningProjectPath);
        }

        m_strCurrentRunningProjectPath = path;
        m_ProjectScript.ClearValue();
        m_ProjectScript.BuildProject(m_strCurrentRunningProjectPath);
        m_ProjectScript.Eval("scriptMain.Instance.Initialize();");
        GameManager.Instance.RegisterToUpdateList(Update);
    }
    private void ClearProjectAndStop(string path)
    {
        GameManager.Instance.UnRegisterFromUpdateList(Update);
        m_ProjectScript.Eval("scriptMain.Instance.Quit();");
    }
    private void Update()
    {
        m_ProjectScript.Eval("scriptMain.Instance.Update();");
    }
    #endregion

    #region window
    public void LoadWindowScript(string path, string className)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window = new Script();
            string content = FileUtils.ReadStringFile(path);
            InitWindowScrit(ref window, className, path, content);
            m_WindowScript.Add(path, window);
        }
    }
    private void InitWindowScrit(ref Script script, string className, string path, string content)
    {
        script.Init();
        script.BuildFile(path, content);
        script.env.RegType(new CSLE.RegHelper_Type(typeof(Debuger)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(GameObject)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(Transform)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(StageManager)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(WindowManager)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(WindowID)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(SceneManager)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(GameStateType)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(TimeManager)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(WindowLayer)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(ScriptManager)));
        script.env.RegType(new CSLE.RegHelper_Type(typeof(WindowBase)));
        string callExpr = className + " sc = " + "new" + className + ";\n";
        script.Execute(callExpr);
    }
    public void WindowInitialize(string path, int id, GameObject root)
    {
        //var returnvalue = typeOfScript.function.MemberCall(content, thisOfScript, "GetHP", null);
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "sc.Initialize();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
    }
    public int WindowResetDeepth(string path, int deepth)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "return sc.ResetDeepth(" + deepth + ");\n";
            return (int)(window.Execute(callExpr));
        }
        Debuger.LogError("load script first");
        return 0;
    }
    public void WindowHide(string path)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "sc.Hide();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
    }
    public void WindowClose(string path)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "sc.Close();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
    }
    public bool WindowIsOpen(string path)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "return sc.IsOpen();\n";
            return (bool)(window.Execute(callExpr));
        }
        Debuger.LogError("load script first");
        return false;
    }
    public int WindowGetMaxDeepthValue(string path)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "return sc.GetMaxDeepthValue();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
        return 0;
    }
    public void WindowOnInit(string path)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "sc.OnInit();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
    }
    public void WindowOnOpen(string path, object param)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "sc.OnOpen();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
    }
    public void WindowOnClose(string path)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "sc.OnClose();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
    }
    public void WindowOnHide(string path)
    {
        Script window = null;
        if (!m_WindowScript.TryGetValue(path, out window))
        {
            window.ClearValue();
            string callExpr = "sc.OnHide();\n";
            window.Execute(callExpr);
        }
        Debuger.LogError("load script first");
    }
    #endregion
}
