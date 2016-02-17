using UnityEngine;
using System.Collections;
using UnityEditor;
using ActionEditor;

public class EnableMeshRenderFrameEdit : AbstractFrameEdit {

    #region Property
    static public EnableMeshRenderFrameEdit Instance
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
    #endregion

    #region Field
    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static EnableMeshRenderFrameEdit m_Instance;

    private boolCommonConfig m_Config;
    private float m_fTickTime;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        //m_Config.Distance = EditorGUILayout.FloatField("Distance", (float)m_Config.Distance);
        m_Config.Value = EditorGUILayout.Toggle("显示", m_Config.Value);
        GUILayout.Space(5f);
    }
    void Update()
    {

    }
    void OnDestroy()
    {

    }
    #endregion

    #region Public Interface
    public void OpenWindow(float fTotalTime, float fTime, EActionFrameType eType, ActionFrameData data)
    {
        m_Instance.SetBaseInfo(fTotalTime, fTime, eType, data);
        m_Instance.Init();
        Repaint();
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
    #endregion

    #region System Event
    protected void Init()
    {
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.BoolFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new boolCommonConfig();
        }

        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
    }
    protected override void OnSave()
    {
        m_ActionFrameData.BoolFrame = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);
        m_Instance.Close();
    }
    protected override void OnPlay()
    {
    }
    #endregion

    #region System Functions
    private static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<EnableMeshRenderFrameEdit>(false, "显示", true);
    }
    #endregion
}
