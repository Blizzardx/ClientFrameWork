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
        int delIndex = -1;
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
                if (GUILayout.Button("复制", GUILayout.Width(100f)))
                {
                    Copy(m_DataList.DataList[i]);
                }
                if (GUILayout.Button("删除", GUILayout.Width(100f)))
                {
                    delIndex = i;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        if (delIndex != -1)
        {
            if (EditorUtility.DisplayDialog("警告", "确定要删除吗？", "ok"))
            {
                Delete(delIndex);
            }
        }
    }

    private void ChoiseMap(TerrainEditorData data)
    {
        m_Instance.Close();
        m_Instance = null;
        TerrainEditorWindow.Instance.OpenTerrain(data);
        EditorWindow.FocusWindowIfItsOpen(typeof(TerrainEditorWindow));
    }

    private void Copy(TerrainEditorData data)
    {
        int max = 0;
        for (int i = 0; i < m_DataList.DataList.Count; ++i)
        {
            if (m_DataList.DataList[i].ID > max)
            {
                max = m_DataList.DataList[i].ID;
            }
        }
        ++ max;
        TerrainEditorData elem = new TerrainEditorData();
        elem.ID = max;
        elem.MapName = data.MapName;
        elem.MapResName = data.MapResName;
        elem.MapSceneName = data.MapSceneName;
        elem.NpcDataList = data.NpcDataList;
        elem.PlayerInitPos = data.PlayerInitPos;
        elem.TriggerDataList = data.TriggerDataList;

        m_DataList.DataList.Add(elem);

        Save();
    }
    private void Delete(int index)
    {
        if (index >= 0 && index < m_DataList.DataList.Count)
        {
            m_DataList.DataList.RemoveAt(index);
        }
        Save();
    }
    private void Save()
    {
        TerrainEditorWindow.Instance.SaveTerrainEditFileList(m_DataList,null);
    }
}
