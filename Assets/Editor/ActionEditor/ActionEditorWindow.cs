//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ActionEditorWindow
//
// Created by : LeoLi at 2015/10/29 16:49:30
//========================================================================
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ActionEditor;
using TerrainEditor;


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
    private readonly float WINDOW_MIN_WIDTH = 1120f;
    private readonly float WINDOW_MIN_HIEGHT = 600f;
    private readonly float WINDOW_MAX_WIDTH = 1200f;
    private readonly float WINDOW_MAX_HIEGHT = 1000f;
    private readonly float ANIM_BAR_LENGTH = 1000f;
    private readonly float ANIM_BAR_REAL_LENGTH = 1000f - 64f;
    private readonly float ANIM_PERFRAME_LENGTH = 0.1f;
    private readonly float TIMELINE_START_X = 113f;
    private readonly float TIMELINE_START_Y = 242f;
    private readonly float TIMELINE_OFFSET_UP = -15f;
    private readonly float TIMELINE_OFFSET_DOWN = 15f;
    private readonly float TIMELINE_WIDTH = 5F;
    private readonly float TIMELINE_HIGHT = 20F;
    //Editor Info
    private static ActionEditorWindow m_MainWnd;
    private GUIStyle titleStyle;
    private GameObject m_ObjSceneRoot;
    public string[] m_szActionFrameName;
    private EActionFrameType m_eActionFrameType;
    private Vector2 m_EventScorllPos;
    private Texture m_KeyframeTex;
    //Editor State
    private bool m_bInitSceneCamera;
    private bool m_bPlay = false;
    private bool m_bIsCreateNew;
    private float m_fAniTimeLastValue = 0f;
    private int m_nAffectedObjectNum = 0;
    private int m_nAffectedObjectLastNum = 0;
    private int m_ActionId = -1;
    //Editor Data
    private GameObject m_ObjMap;
    private GameObject m_ObjMapInstance;
    private GameObject[] m_AffectedObjects = new GameObject[0];
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
    private Dictionary<float, KeyframeData> m_KeyFrameDataDict = new Dictionary<float, KeyframeData>();
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
        ActionEditorRuntime.Instance.SetCloseWindow(CloseWindow);

    }
    public void OnGUI()
    {
        GUILayout.Space(WINDOW_SPACE);
        EditorGUILayout.LabelField("方案:", titleStyle);
        #region 方案
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
        #endregion

        GUILayout.Space(WINDOW_VETICAL_SPACE);
        EditorGUILayout.LabelField("编辑:", titleStyle);
        #region 编辑
        GUILayout.Space(WINDOW_SPACE);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("播放", GUILayout.Width(100f)))
            {
                OnPlay();
            }
            if (GUILayout.Button("停止", GUILayout.Width(100f)))
            {
                OnStop();
            }
            if (GUILayout.Button("保存", GUILayout.Width(100f)))
            {
                OnSave();
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        if (m_bIsCreateNew)
        {
            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("场景:", titleStyle);
            #region 场景
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("地图选择:", GUILayout.Width(60f));
                m_ObjMap = (GameObject)EditorGUILayout.ObjectField(m_ObjMap, typeof(GameObject), true, GUILayout.Width(200f));

                EditorGUILayout.LabelField("剧情名称:", GUILayout.Width(60f));
                m_CurrentActionName = EditorGUILayout.TextField(m_CurrentActionName, GUILayout.Width(100f));

                EditorGUILayout.LabelField("剧情ID:", GUILayout.Width(60f));

                m_MapIDInputBuffer = EditorGUILayout.TextField(m_MapIDInputBuffer, GUILayout.Width(50f));

                int id = m_CurrentEditiongMapId;

                if (int.TryParse(m_MapIDInputBuffer, out id))
                {
                    m_CurrentEditiongMapId = id;
                }


                CheckMap();

                CheckCamera();
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("时间轴:", titleStyle);
            #region 时间轴
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(WINDOW_SPACE);

                m_DurationInputBuffer = EditorGUILayout.TextArea(m_DurationInputBuffer, GUILayout.Width(50f));
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
                    OnChangeActionTimeLine(m_fAniTimeValue);
                }

                if (GUILayout.Button(">", GUILayout.Width(20f)))
                {
                    m_fAniTimeValue += ANIM_PERFRAME_LENGTH;
                    if (m_fAniTimeValue > m_fActionDuration)
                    {
                        m_fAniTimeValue = m_fActionDuration;
                    }
                    OnChangeActionTimeLine(m_fAniTimeValue);
                }
                m_fAniTimeValue = EditorGUILayout.Slider(m_fAniTimeValue, 0, m_fActionDuration, GUILayout.Width(ANIM_BAR_LENGTH));
                if (!m_bPlay && m_fAniTimeValue != m_fAniTimeLastValue)
                {
                    OnChangeActionTimeLine(m_fAniTimeValue);
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            #region 时间轴节点
            if (m_KeyFrameDataDict != null)
            {
                int iIndex = 0;
                foreach (KeyValuePair<float, KeyframeData> unit in m_KeyFrameDataDict)
                {
                    float fTime = 0f;
                    float.TryParse(unit.Key.ToString("f2"), out fTime);
                    float fPercent = (float)unit.Key / m_fActionDuration;
                    if (null != unit.Value)
                    {
                        Rect rctA = new Rect(TIMELINE_START_X + ANIM_BAR_REAL_LENGTH * fPercent, TIMELINE_START_Y + (iIndex % 2 == 0 ? TIMELINE_OFFSET_UP : TIMELINE_OFFSET_DOWN), TIMELINE_WIDTH, TIMELINE_HIGHT);
                        bool bPress = GUI.Button(rctA, "");
                        GUI.DrawTexture(rctA, m_KeyframeTex);
                        if (bPress)
                        {
                            ActionKeyframeWindow.Instance.OpenWindow(unit.Key, unit.Value);
                        }

                    }
                    ++iIndex;
                }
            }
            #endregion

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("使用对象:", titleStyle);
            #region 使用对象
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("对象数量: ", GUILayout.Width(60f));
                m_nAffectedObjectNum = EditorGUILayout.IntField(m_nAffectedObjectNum, GUILayout.Width(60f));
                if (!m_bPlay && m_nAffectedObjectNum != m_nAffectedObjectLastNum)
                {
                    m_AffectedObjects = new GameObject[m_nAffectedObjectNum];
                }
                EditorGUILayout.BeginVertical();
                {
                    for (int i = 0; i < m_AffectedObjects.Length; i++)
                    {
                        m_AffectedObjects[i] = (GameObject)EditorGUILayout.ObjectField(m_AffectedObjects[i], typeof(GameObject), true, GUILayout.Width(200f));
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("节点:", titleStyle);
            #region 创建节点
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                m_eActionFrameType = (EActionFrameType)EditorGUILayout.Popup((int)m_eActionFrameType, m_szActionFrameName, GUILayout.Width(150f));
                if (GUILayout.Button("创建节点", GUILayout.Width(100f)))
                {
                    InsertFrame(m_eActionFrameType, null);
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("节点列表:", titleStyle);
            #region 节点列表
            GUILayout.Space(WINDOW_SPACE);
            m_EventScorllPos = EditorGUILayout.BeginScrollView(m_EventScorllPos, GUILayout.Height(200f));
            {
                if (m_FileData != null && m_FileData.FrameDatalist != null)
                {
                    List<ActionFrameData> lstTemp = m_FileData.FrameDatalist;
                    for (int i = 0; i < lstTemp.Count; i++)
                    {
                        if (null == lstTemp[i])
                        {
                            continue;
                        }
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField((i + 1).ToString() + ". 时间: " + lstTemp[i].Time.ToString("f2"), GUILayout.Width(150f));
                            GUILayout.FlexibleSpace();

                            EditorGUILayout.LabelField("类型: " + m_szActionFrameName[lstTemp[i].Type], GUILayout.Width(150f));
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("编辑节点", GUILayout.Width(100f)))
                            {
                                OnStop();
                                InsertFrame((EActionFrameType)lstTemp[i].Type, lstTemp[i]);
                                break;
                            }
                            GUILayout.Space(5);

                            if (GUILayout.Button("X", GUILayout.Width(20f)))
                            {
                                var option = EditorUtility.DisplayDialog("确定要删除节点吗？",
                                                                         "确定吗？确定吗？确定吗？确定吗？确定吗？",
                                                                         "确定", "取消");
                                if (option)
                                {
                                    OnStop();
                                    DelFrame(lstTemp[i]);
                                    Repaint();
                                    break;
                                }
                                else
                                {
                                    OnStop();
                                }

                            }
                            GUILayout.Space(5);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            #endregion

        }
        m_fAniTimeLastValue = m_fAniTimeValue;
        m_nAffectedObjectLastNum = m_nAffectedObjectNum;
    }
    private void OnChangeActionTimeLine(float fValue)
    {
        //Debug.Log(fValue.ToString());
    }
    private void Update()
    {
        if (!m_bPlay)
        {
            return;
        }

        ActionPlayer action = ActionManager.Instance.GetAction(m_ActionId);
        if (null == action || action.IsFinish())
        {
            m_bPlay = false;
            Repaint();
            return;
        }

        m_fAniTimeValue = action.GetActionRunTime();
        Repaint();
    }
    #endregion

    #region Public Interface
    public void SaveData(ActionFrameData data)
    {
        if (null == m_FileData)
        {
            return;
        }

        ActionHelper.AddFrame(m_FileData, data);
        m_KeyFrameDataDict = ActionHelper.ConvertKeyFrameData(m_FileData.FrameDatalist);
        Repaint();
    }
    public void OpenAction(ActionFileData data)
    {
        ClearData();

        //Action Data
        m_CurrentEditiongMapId = data.ID;
        m_fActionDuration = (float)data.Duration;
        m_CurrentMapName = data.MapResName;
        m_CurrentActionName = data.FileName;
        m_KeyFrameDataDict = ActionHelper.ConvertKeyFrameData(data.FrameDatalist);
        m_FileData = data;

        //Editor Data
        m_bIsCreateNew = true;
        m_DurationInputBuffer = m_fActionDuration.ToString();
        m_MapIDInputBuffer = m_CurrentEditiongMapId.ToString();
        m_ObjMap = ActionHelper.GetSceneMap(data.MapResName);

        Repaint();

    }
    public void InsertFrame(EActionFrameType eType, ActionFrameData frameData)
    {
        if (null != frameData)
        {
            m_fAniTimeValue = (float)frameData.Time;
            OnChangeActionTimeLine(m_fAniTimeValue);
        }

        switch (eType)
        {
            case EActionFrameType.SetCamera:
                SetCameraFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.MoveCamera:
                MoveCameraFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.PlayAudio:
                PlayAudioFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.AddNpc:

                break;
            case EActionFrameType.EditNpc:

                break;

        }
    }
    public void DelFrame(ActionFrameData data)
    {
        if (null == m_FileData)
        {
            return;
        }

        ActionHelper.DelFrame(m_FileData, data);
        m_KeyFrameDataDict = ActionHelper.ConvertKeyFrameData(m_FileData.FrameDatalist);
    }
    #endregion

    #region System Event
    private void ClearData()
    {
        // editor state
        m_bPlay = false;
        m_bIsCreateNew = false;
        m_bInitSceneCamera = false;
        m_fAniTimeLastValue = 0f;
        m_ActionId = -1;

        // editor data
        m_ObjMap = null;
        if (null != m_ObjMapInstance)
        {
            Object.Destroy(m_ObjMapInstance);
        }
        m_ObjMapInstance = null;
        m_DurationInputBuffer = null;
        m_MapIDInputBuffer = null;

        // action data
        m_fAniTimeValue = 0f;
        m_CurrentActionName = null; ;
        m_CurrentMapName = "";
        m_FileData = null;
        m_KeyFrameDataDict = null;

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

        m_szActionFrameName = new string[]{    "相机 / 定义摄像机",
                                               "相机 / 移动摄像机",
                                               "声音 / 添加声音播放",
                                               "角色 / 创建角色",
                                               "角色 / 修改角色",
                                               //"",
                                               //"",
                                               //"",
                                               //"",
                                                };
        m_KeyframeTex = ActionHelper.LoadEditorKeyframeTex();

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
                //create npc
                CreateNpc();
            }
        }
    }
    private void CheckCamera()
    {
        if (null != m_ObjMapInstance && !m_bInitSceneCamera)
        {
            // notice camera control
            ActionEditorRuntime.Instance.SetSceneCamera(m_ObjMapInstance);
            ActionEditorRuntime.Instance.SetClearWindow(ClearData);
            m_bInitSceneCamera = true;
        }
    }
    private static void CloseWindow()
    {
        m_MainWnd.ClearData();
        m_MainWnd.Close();
        m_MainWnd = null;

        //Close Windows
        ActionListWindow.CloseWindow();
        ActionKeyframeWindow.CloseWindow();
        SetCameraFrameEdit.CloseWindow();
        MoveCameraFrameEdit.CloseWindow();
    }
    private void OnPlay()
    {
        if (null == m_FileData)
        {
            return;
        }
        m_bPlay = true;
        m_ActionId = ActionManager.Instance.InsertAction(m_FileData.ID, m_FileData,m_AffectedObjects);
        ActionPlayer action = ActionManager.Instance.GetAction(m_ActionId);
        if (null == action || action.IsFinish())
        {
            return;
        }
        action.SetActionRunTime(m_fAniTimeValue);
    }
    private void OnStop()
    {
        if (m_ActionId >= 0)
        {
            ActionManager.Instance.RemoveAction(m_ActionId);
        }
        ResetTimeLine();
    }
    private void OnSave()
    {
        //Action Data
        if (m_FileData == null)
        {
            m_FileData = new ActionFileData();
        }
        m_FileData.ID = m_CurrentEditiongMapId;
        m_FileData.Duration = (double)m_fActionDuration;
        m_FileData.MapResName = m_CurrentMapName;
        m_FileData.FileName = m_CurrentActionName;
        //m_FileData.FrameDatalist = ActionHelper.ConvertActionFrameData(m_KeyFrameData);

        m_FileDataList = ActionHelper.GetActionEditFileList();
        ActionHelper.SaveActionEditFileList(m_FileDataList, m_FileData);
        EditorUtility.DisplayDialog("保存成功", "保存成功", "确定");
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
        if (ActionEditorRuntime.Instance == null)
        {
            return false;
        }
        return true;

    }
    private void ResetTimeLine()
    {
        m_bPlay = false;
        m_fAniTimeLastValue = 0f;
        m_ActionId = -1;

        m_fAniTimeValue = 0f;

    }
    private void CreateNpc() //Test
    {
        TerrainEditorData TerrainData = ConfigManager.Instance.GetTerrainEditorData(0);

        //init npc
        for (int i = 0; i < TerrainData.NpcDataList.Count; ++i)
        {
            TerrainNpcData elem = TerrainData.NpcDataList[i];

            if (null == TerrainData)
            {
                Debuger.LogError("error terrain !!!");
                return;
            }

            Npc newNpc = new Npc();
            newNpc.Initialize(20000001);
            newNpc.GetTransformData().SetPosition(elem.Pos.GetVector3());
            newNpc.GetTransformData().SetRotation(elem.Rot.GetVector3());
            newNpc.GetTransformData().SetScale(elem.Scale.GetVector3());
        }

    }
    #endregion

}
