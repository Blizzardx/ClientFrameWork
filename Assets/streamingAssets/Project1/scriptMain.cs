using UnityEngine;
using System.Collections;

public class scriptMain
{
    private static scriptMain m_Instance;
    public static scriptMain Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new scriptMain();
            }
            return m_Instance;
        }
    }

    public void Initialize()
    {
        Debuger.Log("Script initialize finished");
        //WindowManager.Instance.RegisterWindow(WindowID.WindowTest3, "Window/UIWindow_test3", WindowLayer.Window, typeof(UIWindowTest4));
        //ScriptManager.Instance.test(typeof(UIWindowTest4));
        UIWindowTest4 a = new UIWindowTest4();
        a.a();
    }
    public void Update()
    {
    }
    public void Quit()
    {

    }

}