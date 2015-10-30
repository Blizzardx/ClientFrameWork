using Assets.Scripts.Core.Utils;
using Communication;
using TerrainEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainListWindow : EditorWindow
{
    private static TerrainListWindow m_Instance;
    private TerrainEditorDataArray m_DataList;

    static public TerrainListWindow Instance
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
        m_Instance = EditorWindow.GetWindow<TerrainListWindow>(false, "地形方案列表", true);
    }
    public void OpenWindow()
    {
        m_DataList = TerrainEditorWindow.Instance.GetTerrainEditFileList();
        
    }

    private void OnGUI()
    {
        if (null == m_DataList)
        {
            return;
        }

        for (int i = 0; i < m_DataList.DataList.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("名称: " + m_DataList.DataList[i].MapName, GUILayout.Width(100f)))
                {
                }
                if (GUILayout.Button("ID: " + m_DataList.DataList[i].ID, GUILayout.Width(100f)))
                {
                }
                if (GUILayout.Button("选择", GUILayout.Width(100f)))
                {
                    ChoiseMap(m_DataList.DataList[i]);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void ChoiseMap(TerrainEditorData data)
    {
        m_Instance.Close();
        m_Instance = null;
        TerrainEditorWindow.Instance.OpenTerrain(data);
    }
}
