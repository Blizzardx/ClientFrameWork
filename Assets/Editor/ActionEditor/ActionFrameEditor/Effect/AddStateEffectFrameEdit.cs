using System.Diagnostics;
using ActionEditor;
using Common.Auto;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AddStateEffectFrameEdit : AbstractFrameEdit {

	//readonly
	private float WINDOW_MIN_WIDTH = 650f;
	private float WINDOW_MIN_HIEGHT = 300f;
	
	private static AddStateEffectFrameEdit m_Instance;
	private AddStateEffectFrameConfig m_Config;
	private string m_strResourceName;
	private uint m_instanceId;
    private string[] m_TargetTypePopList = new[] { "摄像机", "npc", "玩家", };

    static public AddStateEffectFrameEdit Instance
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
		GUILayout.Space(5f);
		EditorGUILayout.BeginHorizontal();
		{
            EditorGUILayout.TextArea("特效文件:", GUILayout.Width(50));
            m_strResourceName = EditorGUILayout.TextField(m_strResourceName, GUILayout.Width(400f));
        }
        EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("创建特效"))
			{
				CreateEffect();
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.TextField("生成实例ID：" + m_instanceId);
		}
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space(5f);
		
		EditorGUILayout.BeginHorizontal();
		{
			Vector3 tmpPos = m_Config.Pos.GetVector3();
            tmpPos = EditorGUILayout.Vector3Field("特效位置:", tmpPos, GUILayout.Width(200f));
            m_Config.Pos.SetVector3(tmpPos);
		}
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		{
            Vector3 tmpRot = m_Config.Rot.GetVector3();
            tmpRot = EditorGUILayout.Vector3Field("特效方向:", tmpRot, GUILayout.Width(200f));
            m_Config.Rot.SetVector3(tmpRot);
        }
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            m_Config.IsAttach = EditorGUILayout.Toggle("Is attach", m_Config.IsAttach);
        }
        EditorGUILayout.EndHorizontal();

        if (m_Config.IsAttach)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("挂点名称:", GUILayout.Width(50));
            m_Config.AttachPoingName = EditorGUILayout.TextField(m_Config.AttachPoingName, GUILayout.Width(400f));

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            m_Config.EntityType =
                (EntityType)
                    (EditorGUILayout.Popup((int)m_Config.EntityType, m_TargetTypePopList, GUILayout.Width(100f)));

            if (m_Config.EntityType == EntityType.Npc)
            {
                EditorGUILayout.LabelField("npc id: ");
                m_Config.AttachNpcId = EditorGUILayout.IntField(m_Config.AttachNpcId);
            }
            EditorGUILayout.EndHorizontal();
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
	private static void CreateWindow()
	{
		m_Instance = EditorWindow.GetWindow<AddStateEffectFrameEdit>(false, "非运行时生成特效编辑", true);
		
	}
	protected override void OnPlay()
	{
	}
	protected override void OnSave()
	{
		if (m_instanceId == 0)
		{
			EditorUtility.DisplayDialog("", "保存失败，instance id 不能为空", "ok");
			return;
		}
        m_Config.InstanceId = (int)(m_instanceId);
		m_Config.EffectName = m_strResourceName;
		
		//Save Data
		m_ActionFrameData.AddStateEffectFrame = m_Config;
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
			m_Config = m_ActionFrameData.AddStateEffectFrame;
			m_strResourceName = m_Config.EffectName;
            m_instanceId = (uint)(m_Config.InstanceId);
		}
		else
		{
			m_ActionFrameData = new ActionFrameData();
			m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
			m_strResourceName = string.Empty;
            m_instanceId = 0;
			m_Config = new AddStateEffectFrameConfig();
			m_Config.EffectName = string.Empty;
			m_Config.Pos = new ThriftVector3();
			m_Config.Rot = new ThriftVector3();
			m_Config.InstanceId = 0;
            m_Config.AttachPoingName = string.Empty;
        }
	}
	private void CreateEffect()
	{
		//check name
		var source = ResourceManager.Instance.LoadBuildInResource<GameObject>(m_strResourceName, AssetType.Effect);
		if (null == source)
		{
			// log error
			EditorUtility.DisplayDialog("", "特效文件读取失败", "ok");
		}
		else
		{
			m_instanceId = EffectContainer.CreateInstanceId();
			Debuger.Log(m_instanceId);
		}
	}

	private void CheckEffect()
	{
		var source = ResourceManager.Instance.LoadBuildInResource<GameObject>(m_strResourceName, AssetType.Effect);
		if (null == source)
		{
			EditorUtility.DisplayDialog("", "所查看的特效文件不存在", "ok");
		}
		else
		{
			UnityEngine.Object eobj = (UnityEngine.Object)source;
			UnityEngine.Object[] objs = {eobj};
			Selection.objects = new UnityEngine.Object[0];
			Selection.objects = objs;
			
			EditorGUIUtility.PingObject(objs[0]);
		}
	}
}
