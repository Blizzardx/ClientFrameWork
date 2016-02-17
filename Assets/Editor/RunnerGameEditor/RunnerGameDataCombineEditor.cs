using System;
using System.Collections.Generic;
using System.Net.Mime;
using Assets.Scripts.Core.Utils;
using Communication;
using Config.Table;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
public class RunnerGameDataCombineEditor : EditorWindow
{
    public const string PrefixName          = "TrunkElement_";
    public const string m_strDataOutputPath = "Assets/EditorCommon/EditorResources/mmAdv/1.0/";
    public const string m_strXmlName        = "runnerTrunkConfig_txtpkg.xml";
    public const string m_strByteName       = "runnerTrunkConfig_txtpkg.bytes";
    private  string m_strXMLInputPath       = "";

    private List<RunnerTrunkElementConfig> m_TmpConfigList;
    private static RunnerGameDataCombineEditor m_MainWnd;

    [MenuItem("Editors/RunnerGame/关卡数据上传")]
    static void CreateWindow()
    {
        m_MainWnd = EditorWindow.GetWindow<RunnerGameDataCombineEditor>(false, "关卡数据上传编辑器", true);
        m_MainWnd.Init();
    }
    public void Init()
    {
        m_strXMLInputPath = PlayerPrefs.GetString("RunnerGameXmlInputPath", string.Empty);

        if (string.IsNullOrEmpty(m_strXMLInputPath))
        {
            Tip("input path can't be null");
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        m_strXMLInputPath = EditorGUILayout.TextField(m_strXMLInputPath);
        if (GUILayout.Button("重置路径", GUILayout.Width(300f)))
        {
            if (string.IsNullOrEmpty(m_strXMLInputPath))
            {
                Tip("input path can't be null");
            }
            else
            {
                PlayerPrefs.SetString("RunnerGameXmlInputPath", m_strXMLInputPath);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("合并障碍信息到xml", GUILayout.Width(300f)))
        {
            CombineXml();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("序列化xml到byte", GUILayout.Width(300f)))
        {
            ConvertToByteData();
        }
        EditorGUILayout.EndHorizontal();
    }
    public void CombineXml()
    {
        try
        {
            DirectoryInfo folder = new DirectoryInfo(m_strXMLInputPath);
            FileInfo[] files = folder.GetFiles();
            m_TmpConfigList = new List<RunnerTrunkElementConfig>();
            for (int i = 0; i < files.Length; ++i)
            {
                if (CheckName(files[i].Name))
                {
                    LoadFile(files[i].FullName);
                }
            }
            CombineData();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Tip(e.Message);
        }
        
    }
    public void ConvertToByteData()
    {
        try
        {
            string xmlData = FileUtils.ReadStringFile(m_strDataOutputPath + "/" + m_strXmlName);

            if (string.IsNullOrEmpty(xmlData))
            {
                Tip("xml 文件为空");
                return;
            }

            TrunkConfigTable table = new TrunkConfigTable();
            table.TrunkConfigXml = xmlData;

            byte[] data = ThriftSerialize.Serialize(table);
            FileUtils.WriteByteFile(m_strDataOutputPath + "/" + m_strByteName, data);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Tip(e.Message);
            throw;
        }
        
    }
    private void LoadFile(string path)
    {
        string content = FileUtils.ReadStringFile(path);
        RunnerTrunkElementConfig config = XmlConfigBase.DeSerialize<RunnerTrunkElementConfig>(content);
        m_TmpConfigList.Add(config);
    }
    private void CombineData()
    {
        if (m_TmpConfigList.Count <= 0)
        {
            Tip("没找到 " + PrefixName + " 文件");
            return;
        }

        RunnerTrunkTableConfig table = new RunnerTrunkTableConfig();
        table.TrunkList = new List<RunnerTrunkConfig>();

        for (int i = 0; i < m_TmpConfigList.Count; ++i)
        {
            RunnerTrunkConfig elem = RunnerTrunkElementConfig.ConvertToConfig(m_TmpConfigList[i]);
            table.TrunkList.Add(elem);
        }

        string xmlData = XmlConfigBase.Serialize(table);
        FileUtils.WriteStringFile(xmlData, m_strDataOutputPath + "/" + m_strXmlName);
    }
    private bool CheckName(string name)
    {
        return name.StartsWith(PrefixName);
    }
    private void Tip(string tip)
    {
        var option = EditorUtility.DisplayDialog("警告", tip,"OK");
    }
}
