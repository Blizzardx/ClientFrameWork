using TerrainEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainTriggerNodeEditorWindow : EditorWindow
{
    private TerrainTriggerData m_NodeInfo;
    private static TerrainTriggerNodeEditorWindow   m_Instance;
    private List<string>                            m_InputBuffer;

    static public TerrainTriggerNodeEditorWindow Instance
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
    public static void CloseWindow()
    {
        if (null == m_Instance)
        {
            return;
        }
        m_Instance.Close();
        m_Instance = null;
    }
    static void CreateWindow()
    {
        m_Instance = EditorWindow.GetWindow<TerrainTriggerNodeEditorWindow>(false, "触发器节点编辑器", true);
    }
    public void OpenWindow(TerrainTriggerData data)
    {
        m_NodeInfo = data;
        m_InputBuffer = new List<string>(4);
        for (int i = 0; i < 5; ++i)
        {
            m_InputBuffer.Add(string.Empty);
        }
        m_InputBuffer[0] = m_NodeInfo.TargetMethodId.ToString();
        m_InputBuffer[1] = m_NodeInfo.EnterLimitMethodId.ToString();
        m_InputBuffer[2] = m_NodeInfo.ExitLimitMethodId.ToString();
        m_InputBuffer[3] = m_NodeInfo.EnterFuncMethodId.ToString();
        m_InputBuffer[4] = m_NodeInfo.ExitFuncMethodId.ToString();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("目标函数ID", GUILayout.Width(120f));
            m_InputBuffer[0] = GUILayout.TextArea(m_InputBuffer[0]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("进入条件函数ID", GUILayout.Width(120f));
            m_InputBuffer[1] = GUILayout.TextArea(m_InputBuffer[1]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("离开条件函数ID", GUILayout.Width(120f));
            m_InputBuffer[2] = GUILayout.TextArea(m_InputBuffer[2]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("进入功能函数ID", GUILayout.Width(120f));
            m_InputBuffer[3] = GUILayout.TextArea(m_InputBuffer[3]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("离开功能函数ID", GUILayout.Width(120f));
            m_InputBuffer[4] = GUILayout.TextArea(m_InputBuffer[4]);
        }
        EditorGUILayout.EndHorizontal();

        int tmpData = m_NodeInfo.TargetMethodId;
        if (int.TryParse(m_InputBuffer[0], out tmpData))
        {
            m_NodeInfo.TargetMethodId = tmpData;
        }
        tmpData = m_NodeInfo.EnterLimitMethodId;
        if (int.TryParse(m_InputBuffer[1], out tmpData))
        {
            m_NodeInfo.EnterLimitMethodId = tmpData;
        }
        tmpData = m_NodeInfo.ExitLimitMethodId;
        if (int.TryParse(m_InputBuffer[2], out tmpData))
        {
            m_NodeInfo.ExitLimitMethodId = tmpData;
        }
        tmpData = m_NodeInfo.EnterFuncMethodId;
        if (int.TryParse(m_InputBuffer[3], out tmpData))
        {
            m_NodeInfo.EnterFuncMethodId = tmpData;
        }
        tmpData = m_NodeInfo.ExitFuncMethodId;
        if (int.TryParse(m_InputBuffer[4], out tmpData))
        {
            m_NodeInfo.ExitFuncMethodId = tmpData;
        }
    }
}
