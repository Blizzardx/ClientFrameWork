using ActionEditor;
using Common.Auto;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class Runtime_AttackExecFrameEdit : AbstractFrameEdit
{
    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static Runtime_AttackExecFrameEdit  m_Instance;
    private Runtime_AttackExecFrameConfig       m_Config;

    static public Runtime_AttackExecFrameEdit Instance
    {
        get
        {
            if (null == m_Instance)
            {
                CreateWindow();
            }

            return m_Instance;
        }
    }
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        m_Instance.SetBaseInfo(fTotalTime, fTime, eType, data);
        m_Instance.Init();
        Repaint();
    }
    private void OnGUI()
    {
        DrawBaseInfo();
    }
    public static void CloseWindow()
    {
        if (null == m_Instance)
        {
            return;
        }
        m_Instance.Close();
        m_Instance = null;
    }
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<Runtime_AttackExecFrameEdit>(false, "打击点编辑", true);

    }
    protected override void OnPlay()
    {
    }
    protected override void OnSave()
    {
        m_Config.Id = "";

        //Save Data
        m_ActionFrameData.Runtime_AttackExec = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        m_Instance.Close();
    }
    private void Init()
    {
        //Update Info
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.Runtime_AttackExec;

        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
            m_Config = new Runtime_AttackExecFrameConfig();
            m_Config.Id = "";
        }
    }
}