using System.IO;
using Assets.Scripts.Core.Utils;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PublishMenu
{
    [MenuItem("Assets/Publish/Open Asset Builder")]
    public static void OnOpenPublisWindow()
    {
        EditorWindow.GetWindow<PublishEditor>();
    }
}
public class PublishEditor : EditorWindow
{
    private string m_strSourcePath;
    private string m_strOutputPath;
    private string m_strAssetVersion = "1.0";

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 20), "Asset Version");
        m_strAssetVersion = GUI.TextArea(new Rect(120, 0, 100, 20), m_strAssetVersion);
        

        if (GUI.Button(new Rect(0, 50, 200, 50), "select source path"))
        {
            m_strSourcePath = EditorUtility.OpenFolderPanel("select source path", m_strSourcePath, "");
            Debug.Log(m_strSourcePath);
        }
        if (GUI.Button(new Rect(0, 100, 200, 50), "select out put path"))
        {
            m_strOutputPath = EditorUtility.OpenFolderPanel("select out put path", m_strOutputPath, "");
            Debug.Log(m_strOutputPath);
        }
        GUI.Label(new Rect(0, 150, 100, 50), "Source Path");
        GUI.Label(new Rect(100, 150, 600, 50), m_strSourcePath);
        GUI.Label(new Rect(0, 200, 100, 50), "out put Path");
        GUI.Label(new Rect(100, 200, 600, 50), m_strOutputPath);

        if (GUI.Button(new Rect(0, 250, 100, 50), "Publish"))
        {
            PublishManager.Instance.SetPath(m_strSourcePath, m_strOutputPath, m_strAssetVersion);
            PublishManager.Instance.OnPublish();
        }
        if (GUI.Button(new Rect(0, 300, 100, 50), "Build Test"))
        {
            PublishManager.Instance.SetPath(m_strSourcePath, m_strOutputPath, m_strAssetVersion);
            PublishManager.Instance.BuildAssetBundle();
        }
        if (GUI.Button(new Rect(0, 350, 100, 50), "Display path"))
        {
            Debug.Log(Application.persistentDataPath);
        }
    }
}
public class PublishManager : Singleton<PublishManager>
{
    private string  m_strSourcePath;
    private string  m_strOutputPath;
    private string  m_strAssetVersion;
    private List<string> m_SkipFileStore;

    public void SetPath(string sourcePath, string outputPath, string assetVersion)
    {
        m_strSourcePath = sourcePath;
        m_strAssetVersion = assetVersion;
        m_strOutputPath = Path.Combine(outputPath, assetVersion);

        m_SkipFileStore = new List<string>()
        {
            ".meta",
        };
    }
    public void OnPublish()
    {
        Debug.Log("cleaning folder : " + m_strOutputPath);
        FileUtils.ClearFolder(m_strOutputPath);
        CopyFolderContentToDesc(new DirectoryInfo(m_strSourcePath));
    }
    private void CopyFolderContentToDesc(DirectoryInfo directory)
    {
        DirectoryInfo folder = directory;
        FileInfo[] files = folder.GetFiles();
        for (int i = 0; i < files.Length; ++i)
        {
            FileInfo elem = files[i];
            string path = GetDescPathName(elem.FullName);
            if (!string.IsNullOrEmpty(path))
            {
                FileUtils.EnsureFolder(path);
                elem.CopyTo(path);
            }
        }
        DirectoryInfo[] folders = folder.GetDirectories();
        for (int i = 0; i < folders.Length; ++i)
        {
            CopyFolderContentToDesc(folders[i]);
        }
    }
    private string GetDescPathName(string sourcePath)
    {
        if (CheckIsSkip(sourcePath))
        {
            return string.Empty;
        }

        Debug.Log("source path: " + sourcePath);
        string path = sourcePath.Substring(m_strSourcePath.Length);
        path = m_strOutputPath + path;
        Debug.Log("Destination path : " + path);
        return path;
    }
    private bool CheckIsSkip(string path)
    {
        for (int i = 0; i < m_SkipFileStore.Count; ++i)
        {
            if (path.Substring(path.Length - m_SkipFileStore[i].Length) == m_SkipFileStore[i])
            {
                Debug.Log("Skip file " + path);
                return true;
            }
        }
        return false;
    }
    public void BuildAssetBundle()
    {
        var a = Selection.activeGameObject;
        string path = m_strOutputPath + "/" + a.name + ".pkg";
        FileUtils.EnsureFolder(path);
        BuildPipeline.BuildAssetBundle(a, null, path);
    }
}