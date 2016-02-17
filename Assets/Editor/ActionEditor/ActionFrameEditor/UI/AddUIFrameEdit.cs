using System.Diagnostics;
using ActionEditor;
using Common.Auto;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

public class AddUIFrameEdit : AbstractFrameEdit {

	private readonly float WINDOW_MIN_WIDTH = 650f;
	private readonly float WINDOW_MIN_HIEGHT = 300f;
	private readonly float WINDOW_SPACE = 10f;
	private readonly float WINDOW_VETICAL_SPACE = 20f;

	private static AddUIFrameEdit m_Instance;
	private AddUIFrameConfig m_Config;
	private string m_instanceId;
	private string m_strResourceName;
	private string m_folderName;
	private Vector3 m_vLastPos;
	private Vector3 m_vLastRotation;
	private Vector3 m_vLastScale;
	private int m_UIFrameType;
	private Dictionary<int, string> m_uiActionFrameNameDict;
	public List<string> m_szActionFrameName;

	static public AddUIFrameEdit Instance
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

	private void InitFrameName()
	{
		m_uiActionFrameNameDict = new Dictionary<int, string>();
		m_uiActionFrameNameDict.Add(WindowID.Loading, "登录过场(Login)");
		m_uiActionFrameNameDict.Add(WindowID.WindowTest1, "窗口测试1(WindowTest1)");
		m_uiActionFrameNameDict.Add(WindowID.WindowTest2,"窗口测试2(WindowTest2)");
		m_uiActionFrameNameDict.Add(WindowID.WindowTest3,"窗口测试3(WindowTest3)");
		m_uiActionFrameNameDict.Add(WindowID.WindowProject1,"窗口工程(WindowProject1)");
		m_uiActionFrameNameDict.Add(WindowID.Login,"登录(Login)");
		m_uiActionFrameNameDict.Add(WindowID.Register, "注册(Register)");
		m_uiActionFrameNameDict.Add(WindowID.Alert, "提示(Alert)");
		m_uiActionFrameNameDict.Add(WindowID.AssetUpdate, "资源更新(AssetUpdate)");
		m_uiActionFrameNameDict.Add(WindowID.CreateChar, "创建角色(CreateChar)");
		m_uiActionFrameNameDict.Add(WindowID.ChangeScene, "切换场景");
		m_uiActionFrameNameDict.Add(WindowID.StoryBg, "故事背景");
		m_uiActionFrameNameDict.Add(WindowID.ChangeScene01, "入场景");
		m_uiActionFrameNameDict.Add(WindowID.ChangeScene02, "出场景");
        m_uiActionFrameNameDict.Add(WindowID.ChangeScene03, "游戏内切场");
        m_uiActionFrameNameDict.Add(WindowID.UIHead, "皇冠");
    }

	private void OnGUI()
	{
		DrawBaseInfo();
		
		EditorGUILayout.BeginHorizontal ();
		{
			m_UIFrameType = EditorGUILayout.Popup
			(
				m_UIFrameType, m_szActionFrameName.ToArray(), GUILayout.Width (150f)
			);
			if (GUILayout.Button ("创建UI", GUILayout.Width (100f))) 
			{
				CreateUI();
			}
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(5f);
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("生成实例ID:", GUILayout.Width(50f));
			m_instanceId = GUILayout.TextArea(m_instanceId);
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(WINDOW_VETICAL_SPACE);
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
		m_Instance = EditorWindow.GetWindow<AddUIFrameEdit>(false, "生成UI编辑", true);
	}

	protected override void OnPlay()
	{

	}

	protected override void OnSave()
	{
		if (string.IsNullOrEmpty(m_instanceId))
		{
			EditorUtility.DisplayDialog("", "保存失败，id 不能为空", "ok");
			return;
		}
		m_Config.WindowId = int.Parse(m_instanceId);
		
		//Save Data
		m_ActionFrameData.AddUIFrame = m_Config;
		ActionEditorWindow.Instance.SaveData(m_ActionFrameData);
		
		//Close Window
		m_Instance.Close();
	}

    private void Init()
    {
        InitFrameName();
        m_szActionFrameName = new List<string>(m_uiActionFrameNameDict.Count);
        foreach (KeyValuePair<int, string> pair in m_uiActionFrameNameDict)
        {
            m_szActionFrameName.Add(pair.Value);
        }
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.AddUIFrame;
            m_instanceId = m_Config.WindowId.ToString();
            int index = 0;
            foreach (var elem in m_uiActionFrameNameDict)
            {
                if (elem.Key.ToString() == m_instanceId)
                {
                    m_UIFrameType = index;
                }
                ++index;
            }
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
            m_strResourceName = string.Empty;
            m_instanceId = string.Empty;
            m_Config = new AddUIFrameConfig();
        }
    }

    private void CreateUI()
	{
        string name = m_szActionFrameName[m_UIFrameType];
        foreach(var elem in m_uiActionFrameNameDict)
        {
            if(elem.Value == name)
            {
                m_instanceId = elem.Key.ToString();
            }
        }
	}
}
