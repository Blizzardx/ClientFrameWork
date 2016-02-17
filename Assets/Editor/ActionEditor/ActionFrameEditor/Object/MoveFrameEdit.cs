using UnityEngine;
using System.Collections;
using UnityEditor;
using ActionEditor;
using System.Collections.Generic;

public class MoveFrameEdit : AbstractFrameEdit
{
    #region Property
    static public MoveFrameEdit Instance
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

    private static MoveFrameEdit m_Instance;

    private MoveTransformFrameConfig m_Config;
    private float m_fTickTime;

    int curPos = 0;
    int lastPos = 0;
    private string[] displayStrings;
    //Vector3 lastPos;
    Vector3 lastPosData;

    MoveFrameHelper helper;
    #endregion

    #region MonoBehavior
    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        m_Config.MoveTime = EditorGUILayout.FloatField("持续时间", (float)m_Config.MoveTime,GUILayout.Width(300));
        m_Config.IsAutoRotate = EditorGUILayout.Toggle("是否面向前方",m_Config.IsAutoRotate);

        EditorGUILayout.LabelField("选择位置节点", GUILayout.Width(150f));
        curPos = EditorGUILayout.Popup(curPos, displayStrings, GUILayout.Width(100f));
        if (curPos != lastPos)
        {
            Selection.activeGameObject = helper.GetNode(curPos).gameObject;
            lastPos = curPos;
        }
        GUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal();
        Vector3 tempPos = m_Config.Path[curPos].GetVector3();
        GUILayout.Label("x", GUILayout.Width(20f));
        tempPos.x = EditorGUILayout.FloatField(tempPos.x,GUILayout.Width(100));
        GUILayout.Label("y", GUILayout.Width(20f));
        tempPos.y = EditorGUILayout.FloatField(tempPos.y, GUILayout.Width(100));
        GUILayout.Label("z", GUILayout.Width(20f));
        tempPos.z = EditorGUILayout.FloatField(tempPos.z, GUILayout.Width(100));
        if(lastPosData != tempPos)
        {
            m_Config.Path[curPos].SetVector3(tempPos);
            helper.RefershPos(curPos,tempPos);
        }
        lastPosData = tempPos;
        
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加节点", GUILayout.Width(80f)))
        {
            Add();
        }
        if (GUILayout.Button("删除节点", GUILayout.Width(80f)))
        {
            Remove();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
    }

    void Add()
    {
        Common.Auto.ThriftVector3 pos = new Common.Auto.ThriftVector3();
        m_Config.Path.Insert(++curPos, pos);
        helper.AddObj(curPos,pos.GetVector3());
        RefreshDisplayString();
    }

    void Remove()
    {
        if(m_Config.Path.Count <= 1)
        {
            EditorUtility.DisplayDialog("错误", "只剩最后一个以上位置节点", "确定");
            return;
        }
        m_Config.Path.RemoveAt(curPos);
        helper.RemoveObj(curPos);
        if (curPos >= m_Config.Path.Count)
        {
            curPos--;
        }
        RefreshDisplayString();
    }

    void RefreshDisplayString()
    {
        displayStrings = new string[m_Config.Path.Count];
        for (int i = 0; i < m_Config.Path.Count; i++)
        {
            displayStrings[i] = i.ToString();
        }
    }

    void Update()
    {
        var go = Selection.activeGameObject;
        if(go != null)
        {
            var index = helper.GetIndex(go);
            if(index >= 0)
            {
                var pos = go.transform.position;
                 m_Config.Path[index].SetVector3(pos);
                if(index == curPos)
                {
                    lastPosData = m_Config.Path[index].GetVector3();
                    Repaint();
                }
            }
        }
        
    }
    void OnDestroy()
    {
        helper.Clear();
        Destroy(helper.gameObject);
        helper = null;
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
            m_Config = m_ActionFrameData.MoveTransformFrame;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new MoveTransformFrameConfig();
        }

        if(m_Config.Path == null || m_Config.Path.Count < 1)
        {
            m_Config.Path = new System.Collections.Generic.List<Common.Auto.ThriftVector3>();
            Common.Auto.ThriftVector3 origin = new Common.Auto.ThriftVector3();
            m_Config.Path.Add(origin);
        }
        RefreshDisplayString();

        var go = new GameObject();
        helper = go.AddComponent<MoveFrameHelper>();
        helper.Init(m_Config.Path);

        curPos = 0;
        lastPos = curPos;
        Selection.activeGameObject = helper.GetNode(curPos).gameObject;


        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
    }
    protected override void OnSave()
    {
        m_ActionFrameData.MoveTransformFrame = m_Config;
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
        m_Instance = EditorWindow.GetWindow<MoveFrameEdit>(false, "移动", true);
    }
    #endregion
}

