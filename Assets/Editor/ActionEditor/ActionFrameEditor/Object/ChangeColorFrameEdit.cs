using UnityEngine;
using System.Collections;
using UnityEditor;
using ActionEditor;

public class ChangeColorFrameEdit : AbstractFrameEdit {

    #region Property
    static public ChangeColorFrameEdit Instance
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

    private static ChangeColorFrameEdit m_Instance;

    private Common.Auto.ThriftVector3 m_Config;
    private float m_fTickTime;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        Color n = EditorGUILayout.ColorField("New Color", new Color(m_Config.GetVector3().x, m_Config.GetVector3().y, m_Config.GetVector3().z));
        m_Config.SetVector3(new Vector3(n.r, n.g, n.b));
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
            m_Config = m_ActionFrameData.Vector3Frame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new Common.Auto.ThriftVector3();
        }

        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
    }
    protected override void OnSave()
    {
        m_ActionFrameData.Vector3Frame = m_Config;
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
        m_Instance = EditorWindow.GetWindow<ChangeColorFrameEdit>(false, "变色", true);
    }
    #endregion
}
