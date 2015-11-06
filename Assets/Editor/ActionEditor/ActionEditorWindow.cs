//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ActionEditorWindow
//
// Created by : LeoLi (742412055@qq.com) at 2015/10/29 16:49:30
//
//
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;

public class ActionEditorWindow : EditorWindow
{
    #region Property
    static public ActionEditorWindow Instance
    {
        get
        {
            if (null == m_MainWnd)
            {
                CreateWindow();
            }

            return m_MainWnd;
        }
    }
    #endregion

    #region Field
    //readonly
    private readonly int WINDOW_TITLE_FONTSIZE = 20;
    private readonly float WINDOW_VETICAL_SPACE = 20f;
    private readonly float WINDOW_SPACE = 10f;
    private readonly float WINDOW_MIN_WIDTH = 1100f;
    private readonly float WINDOW_MIN_HIEGHT = 600f;
    private readonly float WINDOW_MAX_WIDTH = 1200f;
    private readonly float WINDOW_MAX_HIEGHT = 1000f;
    private readonly float ANIM_BAR_LENGTH = 1000f;
    private readonly float ANIM_PERFRAME_LENGTH = 0.1f;
    //Editor Info
    private static ActionEditorWindow m_MainWnd;
    private GUIStyle titleStyle;
    private GameObject m_ObjSceneRoot;
    private string[] m_szActionFrameName;
    private EActionFrameType m_eActionFrameType;
    //Editor State
    private bool m_bInitSceneCamera;
    private bool m_bPlay = false;
    private bool m_bIsCreateNew;
    private float m_fAniTimeLastValue = 0f;
    //Editor Data
    private GameObject m_ObjMap;
    private GameObject m_ObjMapInstance;
    private string m_MapIDInputBuffer;
    private string m_DurationInputBuffer;
    //Action Data
    private int m_CurrentEditiongMapId;
    private float m_fActionDuration = 20f;
    private float m_fAniTimeValue = 0f;
    private string m_CurrentMapName;
    private string m_CurrentActionName;
    private ActionFileData m_FileData;
    private ActionFileDataArray m_FileDataList;
    private Dictionary<float, ActionFrameData> m_KeyFrameData = new Dictionary<float, ActionFrameData>();
    #endregion

    #region MonoBehavior
    [MenuItem("Editors/Action")]
    static void CreateWindow()
    {
        if (!CheckScene())
        {
            return;
        }
        m_MainWnd = EditorWindow.GetWindow<ActionEditorWindow>(false, "剧情编辑器", true);
        m_MainWnd.Init();
        ActionEditorMain.Instance.SetCloseWindow(CloseWindow);

    }
    public void OnGUI()
    {

        GUILayout.Space(WINDOW_SPACE);
        EditorGUILayout.LabelField("方案:", titleStyle);
        GUILayout.Space(WINDOW_SPACE);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("选择剧情方案", GUILayout.Width(100f)))
            {
                ActionListWindow.Instance.OpenWindow();
            }
            if (GUILayout.Button("创建剧情方案", GUILayout.Width(100f)))
            {
                m_bIsCreateNew = true;
            }
            if (GUILayout.Button("重置面板", GUILayout.Width(100f)))
            {
                ClearData();
            }

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(WINDOW_VETICAL_SPACE);
        EditorGUILayout.LabelField("编辑:", titleStyle);
        GUILayout.Space(WINDOW_SPACE);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("播放", GUILayout.Width(100f)))
            {

            }
            if (GUILayout.Button("暂停", GUILayout.Width(100f)))
            {

            }
            if (GUILayout.Button("保存", GUILayout.Width(100f)))
            {
                SaveData();
            }
        }
        EditorGUILayout.EndHorizontal();


        if (m_bIsCreateNew)
        {
            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("场景:", titleStyle);
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("地图选择:", GUILayout.Width(50f));
                m_ObjMap = (GameObject)EditorGUILayout.ObjectField(m_ObjMap, typeof(GameObject),true);

                EditorGUILayout.LabelField("剧情名称:", GUILayout.Width(50f));
                m_CurrentActionName = EditorGUILayout.TextField(m_CurrentActionName);

                EditorGUILayout.LabelField("剧情ID:", GUILayout.Width(50f));

                m_MapIDInputBuffer = EditorGUILayout.TextField(m_MapIDInputBuffer);

                int id = m_CurrentEditiongMapId;

                if (int.TryParse(m_MapIDInputBuffer, out id))
                {
                    m_CurrentEditiongMapId = id;
                }


                CheckMap();

                CheckCamera();
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("时间轴:", titleStyle);

            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(WINDOW_SPACE);

                m_DurationInputBuffer = EditorGUILayout.TextArea(m_DurationInputBuffer);
                float duration = m_fActionDuration;
                if (float.TryParse(m_DurationInputBuffer, out duration))
                {
                    m_fActionDuration = duration;
                }

                if (GUILayout.Button("<", GUILayout.Width(20f)))
                {
                    m_fAniTimeValue -= ANIM_PERFRAME_LENGTH;
                    if (m_fAniTimeValue < 0f)
                    {
                        m_fAniTimeValue = 0f;
                    }
                    OnChangeAniTimeSlider(m_fAniTimeValue);
                }

                if (GUILayout.Button(">", GUILayout.Width(20f)))
                {
                    m_fAniTimeValue += ANIM_PERFRAME_LENGTH;
                    if (m_fAniTimeValue > m_fActionDuration)
                    {
                        m_fAniTimeValue = m_fActionDuration;
                    }
                    OnChangeAniTimeSlider(m_fAniTimeValue);
                }
                m_fAniTimeValue = EditorGUILayout.Slider(m_fAniTimeValue, 0, m_fActionDuration, GUILayout.Width(ANIM_BAR_LENGTH));
                if (!m_bPlay && m_fAniTimeValue != m_fAniTimeLastValue)
                {
                    OnChangeAniTimeSlider(m_fAniTimeValue);
                }
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("节点:", titleStyle);
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                m_eActionFrameType = (EActionFrameType)EditorGUILayout.Popup((int)m_eActionFrameType, m_szActionFrameName, GUILayout.Width(100f));
                if (GUILayout.Button("创建节点", GUILayout.Width(100f)))
                {
                    OnInsertEvent(m_eActionFrameType, null);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("节点列表:", titleStyle);
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("筛选:", GUILayout.Width(50f));
            }
            EditorGUILayout.EndHorizontal();
        }

        m_fAniTimeLastValue = m_fAniTimeValue;
    }
    private void OnChangeAniTimeSlider(float fValue)
    {
        //Debug.Log(fValue.ToString());
    }
    #endregion

    #region Public Interface
    public void OnInsertEvent(EActionFrameType eType, ActionFrameData frameData)
    {
        //if (null == m_SelectAniData || m_SelectAniData.Count < 1)
        //{
        //    return;
        //}

        if (null != frameData)
        {
            m_fAniTimeValue = (float)frameData.Time;
            OnChangeAniTimeSlider(m_fAniTimeValue);
        }

        switch (eType)
        {
            case EActionFrameType.SetCamera:
                SetCameraFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;

            case EActionFrameType.MoveCamera:
                MoveCameraFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;

            //case ESkillFrameType.ShakeCamera:
            //    ShakeCameraFrameEdit.CreateWindow(GetSelectAnimLength(), m_fAniTimeValue, eType, frameData, m_SrcUnit, m_lstUnitTargets);
            //    break;
        }
    }
    public void SaveData(ActionFrameData data)
    {
        if ( null == m_FileData)
        {
            return;
        }

        //MonoSkillDataScript.AddFrame(m_SkillFileData, m_EditSkill.Id, m_SelectAniData, data);
        //ResetTimeLine();
        //ReSetKeyFrameData();

        ActionHelper.AddFrame(m_FileData, data);
        
    }
    public void OpenAction(ActionFileData data)
    {
        ClearData();

        //Action Data
        m_CurrentEditiongMapId = data.ID;
        m_fActionDuration = (float)data.Duration;
        m_CurrentMapName = data.MapResName;
        m_CurrentActionName = data.FileName;
        m_KeyFrameData = ActionHelper.ConvertKeyFrameData(data.FrameDatalist);

        //Editor Data
        m_bIsCreateNew = true;
        m_DurationInputBuffer = m_fActionDuration.ToString();
        m_MapIDInputBuffer = m_CurrentEditiongMapId.ToString();
        m_ObjMap = ResourceManager.Instance.LoadBuildInResource<GameObject>(data.MapResName, AssetType.Map);

        Repaint();

    }
    #endregion

    #region System Event
    private void SaveData()
    {
        //Action Data
        m_FileData = new ActionFileData();
        m_FileData.ID = m_CurrentEditiongMapId;
        m_FileData.Duration = (double)m_fActionDuration;
        m_FileData.MapResName = m_CurrentMapName;
        m_FileData.FileName = m_CurrentActionName;
        m_FileData.FrameDatalist = ActionHelper.ConvertActionFrameData(m_KeyFrameData);

        m_FileDataList = ActionHelper.GetActionEditFileList();
        ActionHelper.SaveActionEditFileList(m_FileDataList, m_FileData);
    }
    private void ClearData()
    {
        // editor state
        m_bPlay = false;
        m_bIsCreateNew = false;
        m_bInitSceneCamera = false;
        m_fAniTimeLastValue = 0f;

        // editor data
        m_ObjMap = null;
        m_ObjMapInstance = null;
        if (null != m_ObjMapInstance)
        {
            Object.Destroy(m_ObjMapInstance);
        }
        m_DurationInputBuffer = null; ;
        m_MapIDInputBuffer = null; 

        // action data
        m_fAniTimeValue = 0f;
        m_CurrentActionName = null; ;
        m_CurrentMapName = "";
        m_FileData = null;
        m_KeyFrameData = null;

    }
    private void Init()
    {
        GameObject RootObj = GameObject.Find("ActionEditorRoot");
        m_ObjSceneRoot = GameObject.Find("SceneRoot");

        if (null == RootObj || null == m_ObjSceneRoot)
        {
            Debug.LogError("wrong scene");
        }

        m_MainWnd.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        m_MainWnd.maxSize = new Vector2(WINDOW_MAX_WIDTH, WINDOW_MAX_HIEGHT);

        m_szActionFrameName = new string[]{    "定义摄像机",
                                               "移动摄像机",
                                               //"",
                                               //"",
                                               //"",
                                               //"",
                                               //"",
                                               //"",
                                               //"",
                                                };

        titleStyle = new GUIStyle();
        titleStyle.fontSize = WINDOW_TITLE_FONTSIZE;
        titleStyle.normal.textColor = Color.white;
    }
    private void CheckMap()
    {
        if (null != m_ObjMap)
        {
            if (null == m_ObjMapInstance)
            {
                // reset map
                m_ObjMapInstance = GameObject.Instantiate(m_ObjMap);
                ComponentTool.Attach(m_ObjSceneRoot.transform, m_ObjMapInstance.transform);
                //record map name
                m_CurrentMapName = m_ObjMap.name;
                Debug.Log("map name : " + m_CurrentMapName);
                //reset Global Script
                GlobalScripts.Instance.Reset();
            }
        }
    }
    private void CheckCamera()
    {
        if (null != m_ObjMapInstance && !m_bInitSceneCamera)
        {
            // notice camera control
            ActionEditorMain.Instance.SetSceneCamera(m_ObjMapInstance);
            ActionEditorMain.Instance.SetClearWindow(ClearData);
            m_bInitSceneCamera = true;
        }
    }
    private static void CloseWindow()
    {
        m_MainWnd.Close();
        m_MainWnd = null;

        //Close Windows
        ActionListWindow.CloseWindow();
        SetCameraFrameEdit.CloseWindow();
        MoveCameraFrameEdit.CloseWindow();
    }
    #endregion

    #region System Function
    private static bool CheckScene()
    {
        var RootObj = GameObject.Find("ActionEditorRoot");
        var SceneRoot = GameObject.Find("SceneRoot");

        if (null == RootObj || null == SceneRoot)
        {
            return false;
        }
        if (ActionEditorMain.Instance == null)
        {
            return false;
        }
        return true;

    }
    #endregion

}
