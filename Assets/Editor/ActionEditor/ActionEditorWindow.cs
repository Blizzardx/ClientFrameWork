//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : ActionEditorWindow
//
// Created by : LeoLi at 2015/10/29 16:49:30
//========================================================================

using Assets.Scripts.Core.Utils;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
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
    private readonly float WINDOW_MIN_HIEGHT = 290f;
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
    private readonly Color[] ACTIONCOLORDEFINE = new Color[1] { Color.red };
    //Editor Info
    private static ActionEditorWindow m_MainWnd;
    private GUIStyle titleStyle;
    private GameObject m_ObjSceneRoot;
    private Dictionary<EActionFrameType, string> m_mapActionFrameNameDict;
    public string[] m_szActionFrameName;
    private EActionFrameType m_eActionFrameType = EActionFrameType.Max;
    private Vector2 m_EventScorllPos;
    private Texture m_KeyframeTexRed;
    private Texture m_KeyframeTexBlue;
    //Editor State
    private bool m_bInitSceneCamera;
    private bool m_bPlay = false;
    private bool m_bIsPaused = true;
    private bool m_bIsCreateNew;
    private float m_fAniTimeLastValue = 0f;
    private int m_nAffectedObjectNum = 0;
    private int m_nAffectedObjectLastNum = 0;
    private int m_ActionId = -1;
    private List<ActionFrameData> m_lstCopyedData = new List<ActionFrameData>();
    private List<ActionFrameData> m_lstSelectedFrameData = null;
    //Editor Data
    private GameObject m_ObjMap;
    private GameObject m_ObjMapInstance;
    private GameObject[] m_AffectedObjects = new GameObject[0];
    private string m_MapIDInputBuffer;
    private string m_DurationInputBuffer;
    private int m_TerrainID = 1;
    private AERuntimeParam m_RuntimeParam;
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
    [MenuItem("Editors/Action/剧情编辑器")]
    private static void CreateWindow()
    {
        if (!CheckScene())
        {
            return;
        }
        m_MainWnd = EditorWindow.GetWindow<ActionEditorWindow>(false, "剧情编辑器", true);
        m_MainWnd.Init();
        ActionEditorRuntime.Instance.SetCloseWindow(CloseWindow);

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
    private void OnGUI()
    {
        GUILayout.Space(WINDOW_SPACE);
        EditorGUILayout.LabelField("<color=#00FFFF>方案:</color>", titleStyle);
        if (GUI.Button(new Rect(Screen.width - 20, 0, 20, 20), "X", titleStyle))
        {
            var option = EditorUtility.DisplayDialog("关闭编辑器", "未保存的数据将会丢失", "确定", "取消");
            if (option)
            {
                CloseWindow();
            }
        }
        #region 方案
        GUILayout.Space(WINDOW_SPACE);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("选择方案", GUILayout.Width(100f)))
            {
                ActionListWindow.Instance.OpenWindow();
            }
            if (GUILayout.Button("创建方案", GUILayout.Width(100f)))
            {
                m_bIsCreateNew = true;
            }
            if (GUILayout.Button("重置面板", GUILayout.Width(100f)))
            {
                var option = EditorUtility.DisplayDialog("警告", "重置面板将清除本地数据", "确定", "取消");
                if (option)
                {
                    ClearData();
                }
            }
            if (GUILayout.Button("取代方案", GUILayout.Width(100f)))
            {
                OnReplaceFile();
            }
            if (GUILayout.Button("同步方案", GUILayout.Width(100f)))
            {
                OnSyncFile();
            }
            if (GUILayout.Button("合并方案", GUILayout.Width(100f)))
            {
                OnMergeFile();
            }
            if (GUILayout.Button("备份方案", GUILayout.Width(100f)))
            {
                OnBackupFile();
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        if (m_bIsCreateNew)
        {
            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("编辑:", titleStyle);
            #region 编辑
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                if (m_bIsPaused)
                {
                    if (GUILayout.Button("播放", GUILayout.Width(100f)))
                    {
                        OnPlay();
                    }
                }
                else
                {
                    if (GUILayout.Button("暂停", GUILayout.Width(100f)))
                    {
                        OnPaused();
                    }
                }
                if (GUILayout.Button("停止", GUILayout.Width(100f)))
                {
                    OnStop();
                }
                string save = m_FileData == null ? "初始化" : "保存";
                if (GUILayout.Button(save, GUILayout.Width(100f)))
                {
                    OnSave();
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            EditorGUILayout.LabelField("场景:", titleStyle);
            #region 场景
            GUILayout.Space(WINDOW_SPACE);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("地图选择:", GUILayout.Width(60f));
                m_ObjMap = (GameObject)EditorGUILayout.ObjectField(m_ObjMap, typeof(GameObject), true, GUILayout.Width(200f));
                if (null != m_ObjMap && !FileUtils.IsEndof(m_ObjMap.name, "_map"))
                {
                    m_ObjMap = null;
                }
                EditorGUILayout.LabelField("剧情名称:", GUILayout.Width(60f));
                m_CurrentActionName = EditorGUILayout.TextField(m_CurrentActionName, GUILayout.Width(100f));

                EditorGUILayout.LabelField("剧情ID:", GUILayout.Width(60f));
                m_MapIDInputBuffer = EditorGUILayout.TextField(m_MapIDInputBuffer, GUILayout.Width(50f));

                EditorGUILayout.LabelField("地形ID:", GUILayout.Width(60f));
                m_TerrainID = EditorGUILayout.IntField(m_TerrainID, GUILayout.Width(50f));

                if (GUILayout.Button("刷新地形", GUILayout.Width(100f)))
                {
                    CloseTerrain();
                    OpenTerrain();
                }

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

            if (m_FileData == null)
                return;

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
                    OnChangePlayerTimeLine(m_fAniTimeValue);
                }

                if (GUILayout.Button(">", GUILayout.Width(20f)))
                {
                    m_fAniTimeValue += ANIM_PERFRAME_LENGTH;
                    if (m_fAniTimeValue > m_fActionDuration)
                    {
                        m_fAniTimeValue = m_fActionDuration;
                    }
                    OnChangePlayerTimeLine(m_fAniTimeValue);
                }
                m_fAniTimeValue = EditorGUILayout.Slider(m_fAniTimeValue, 0, m_fActionDuration, GUILayout.Width(ANIM_BAR_LENGTH));
                if (!m_bPlay && m_fAniTimeValue != m_fAniTimeLastValue)
                {
                    OnChangePlayerTimeLine(m_fAniTimeValue);
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
                        bool bPress = GUI.Button(rctA, "aaaa");
                        Texture tex = unit.Value.isSelected == true ? m_KeyframeTexRed : m_KeyframeTexBlue;
                        GUI.DrawTexture(rctA, tex);
                        if (Event.current.type == EventType.used && rctA.Contains(Event.current.mousePosition))
                        {
                            //Debug.Log("asdsd" + iIndex.ToString());
                        }

                        if (bPress)
                        {
                            ResetKeyFrameTex();
                            OnClickTimeFrameTex(unit);
                            //Debug.Log(Event.current.mousePosition.ToString());
                            //Debug.Log(rctA.ToString());
                            if (Event.current.button == 1)
                            {
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("设置当前节点时间"), false, OnChangeFrameTime, unit.Value);
                                menu.ShowAsContext();
                                Event.current.Use();
                            }
                        }

                    }
                    ++iIndex;
                }
            }
            #endregion

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            //EditorGUILayout.LabelField("使用对象:", titleStyle);
            if (NGUIEditorTools.DrawHeader("使用对象"))
            {
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
            }

            #region runtime param
            GUILayout.Space(WINDOW_VETICAL_SPACE);
            if (NGUIEditorTools.DrawHeader("运行时数据"))
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("运行时数据编辑", GUILayout.Width(200f)))
                    {
                        AERuntimeParamEditorWindow.OpenWndow(m_RuntimeParam);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion

            GUILayout.Space(WINDOW_VETICAL_SPACE);
            if (NGUIEditorTools.DrawHeader("节点"))
            {
                #region 创建节点
                GUILayout.Space(WINDOW_SPACE);
                EditorGUILayout.BeginHorizontal();
                {
                    m_eActionFrameType = (EActionFrameType)EditorGUILayout.Popup((int)m_eActionFrameType, m_szActionFrameName, GUILayout.Width(150f));
                    if (m_eActionFrameType != EActionFrameType.Max)
                    {
                        if (GUILayout.Button("创建节点", GUILayout.Width(100f)))
                        {
                            if (Event.current.button == 0)
                            {
                                InsertFrame(m_eActionFrameType, null);
                            }
                            else if (Event.current.button == 1 && m_lstCopyedData != null && m_lstCopyedData.Count > 0)
                            {
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("粘贴节点"), false, OnPasteFrameData);
                                menu.ShowAsContext();
                                Event.current.Use();
                            }
                        }
                    }
                    else
                    {
                        if (m_lstCopyedData != null && m_lstCopyedData.Count > 0)
                        {
                            if (GUILayout.Button("粘贴节点", GUILayout.Width(100f)))
                            {
                                OnPasteFrameData();
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                //EditorGUILayout.LabelField("节点列表:", titleStyle);
                #region 节点列表
                GUILayout.Space(WINDOW_SPACE);
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw line
                m_EventScorllPos = EditorGUILayout.BeginScrollView(m_EventScorllPos);
                {
                    if (m_FileData != null && m_FileData.FrameDatalist != null)
                    {
                        List<ActionFrameData> lstTemp = m_FileData.FrameDatalist;
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw line
                        // whether has selected frame
                        bool wetherSelected = false;
                        float selectedTime = 0f;
                        foreach (KeyframeData data in m_KeyFrameDataDict.Values)
                        {
                            if (data.isSelected)
                            {
                                wetherSelected = true;
                                selectedTime = (float)data.framedatalist[0].Time;
                            }
                        }
                        //
                        for (int i = 0; i < lstTemp.Count; i++)
                        {
                            if (null == lstTemp[i])
                            {
                                continue;
                            }
                            if (wetherSelected)
                            {
                                if (lstTemp[i].Time != selectedTime)
                                {
                                    continue;
                                }
                            }
                            else if (m_eActionFrameType != EActionFrameType.Max)
                            {
                                if ((EActionFrameType)lstTemp[i].Type != m_eActionFrameType)
                                {
                                    continue;
                                }
                            }
                            GUILayout.Space(5);
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUIStyle textColor = new GUIStyle();
                                textColor.normal.textColor = GetFrameTypeColor(m_szActionFrameName[lstTemp[i].Type]);
                                if (m_lstSelectedFrameData != null && m_lstSelectedFrameData.Count > 0)
                                {
                                    if (m_lstSelectedFrameData.Contains(lstTemp[i]))
                                    {
                                        textColor.normal.textColor = Color.magenta;
                                    }
                                }
                                textColor.alignment = TextAnchor.MiddleLeft;

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
                                if (GUILayout.Button("编辑节点", GUILayout.Width(100f)))
                                {
                                    OnStop();
                                    InsertFrame((EActionFrameType)lstTemp[i].Type, lstTemp[i]);
                                    break;
                                }
                                GUILayout.Space(15);

                                if (GUILayout.Button((i + 1).ToString() + ". 时间: " + lstTemp[i].Time.ToString("f2"), textColor, GUILayout.Width(100f), GUILayout.Height(20f)))
                                {
                                    if (Event.current.button == 0)
                                    {
                                        OnClickFrame(lstTemp[i]);
                                    }
                                    else if (Event.current.button == 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        menu.AddItem(new GUIContent("复制节点"), false, OnCopyFrameData, lstTemp[i]);
                                        if (m_lstSelectedFrameData != null && m_lstSelectedFrameData.Count > 0)
                                        {
                                            menu.AddItem(new GUIContent("复制选择的节点"), false, OnCopySelectedAction);
                                            menu.AddItem(new GUIContent("平移选择的节点"), false, OnChangeSelectedActionTime);
                                            menu.AddItem(new GUIContent("取消选择的节点"), false, OnCancelSelectedAction);
                                        }
                                        if (m_lstCopyedData != null && m_lstCopyedData.Count > 0)
                                            menu.AddItem(new GUIContent("粘贴节点在此时间位置"), false, OnPasteFrameData, (float)lstTemp[i].Time);
                                        menu.ShowAsContext();
                                        Event.current.Use();
                                    }
                                    else if (Event.current.button == 2)
                                    {
                                        if (m_lstSelectedFrameData == null)
                                            m_lstSelectedFrameData = new List<ActionFrameData>();
                                        m_lstSelectedFrameData.Add(lstTemp[i]);
                                        Debug.Log(lstTemp[i].ToString() + " added into SelectedFrameData");
                                    }
                                }
                                GUILayout.Space(15f);

                                if (GUILayout.Button("类型: " + m_szActionFrameName[lstTemp[i].Type], textColor, GUILayout.Width(150f), GUILayout.Height(20f)))
                                {
                                    if (Event.current.button == 0)
                                    {
                                        OnClickFrame(lstTemp[i]);
                                    }
                                    else if (Event.current.button == 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        menu.AddItem(new GUIContent("复制节点"), false, OnCopyFrameData, lstTemp[i]);
                                        if (m_lstSelectedFrameData != null && m_lstSelectedFrameData.Count > 0)
                                        {
                                            menu.AddItem(new GUIContent("复制选择的节点"), false, OnCopySelectedAction);
                                            menu.AddItem(new GUIContent("平移选择的节点"), false, OnChangeSelectedActionTime);
                                            menu.AddItem(new GUIContent("取消选择的节点"), false, OnCancelSelectedAction);
                                        }
                                        if (m_lstCopyedData != null && m_lstCopyedData.Count > 0)
                                            menu.AddItem(new GUIContent("粘贴节点在此时间位置"), false, OnPasteFrameData, (float)lstTemp[i].Time);
                                        menu.ShowAsContext();
                                        Event.current.Use();
                                    }
                                    else if (Event.current.button == 2)
                                    {
                                        if (m_lstSelectedFrameData == null)
                                            m_lstSelectedFrameData = new List<ActionFrameData>();
                                        m_lstSelectedFrameData.Add(lstTemp[i]);
                                        Debug.Log(lstTemp[i].ToString() + " added into SelectedFrameData");
                                    }
                                }
                                GUILayout.Space(15f);

                                if (GUILayout.Button("使用对象ID: ", textColor, GUILayout.Width(70f), GUILayout.Height(20f)))
                                {
                                    if (Event.current.button == 0)
                                    {
                                        OnClickFrame(lstTemp[i]);
                                    }
                                    else if (Event.current.button == 1)
                                    {
                                        GenericMenu menu = new GenericMenu();
                                        menu.AddItem(new GUIContent("复制节点"), false, OnCopyFrameData, lstTemp[i]);
                                        if (m_lstSelectedFrameData != null && m_lstSelectedFrameData.Count > 0)
                                        {
                                            menu.AddItem(new GUIContent("复制选择的节点"), false, OnCopySelectedAction);
                                            menu.AddItem(new GUIContent("平移选择的节点"), false, OnChangeSelectedActionTime);
                                            menu.AddItem(new GUIContent("取消选择的节点"), false, OnCancelSelectedAction);
                                        }
                                        if (m_lstCopyedData != null && m_lstCopyedData.Count > 0)
                                            menu.AddItem(new GUIContent("粘贴节点在此时间位置"), false, OnPasteFrameData, (float)lstTemp[i].Time);
                                        menu.ShowAsContext();
                                        Event.current.Use();
                                    }
                                    else if (Event.current.button == 2)
                                    {
                                        if (m_lstSelectedFrameData == null)
                                            m_lstSelectedFrameData = new List<ActionFrameData>();
                                        m_lstSelectedFrameData.Add(lstTemp[i]);
                                        Debug.Log(lstTemp[i].ToString() + " added into SelectedFrameData");
                                    }
                                }
                                if (lstTemp[i].TargetIDs != null && lstTemp[i].TargetIDs.Count > 0)
                                {
                                    System.Text.StringBuilder str = new System.Text.StringBuilder();
                                    List<int> tempList = lstTemp[i].TargetIDs;
                                    for (int index = 0; index < tempList.Count; index++)
                                    {
                                        str.Append(tempList[index].ToString());
                                        str.Append("  ");
                                    }
                                    EditorGUILayout.LabelField(str.ToString(), textColor, GUILayout.Height(20f));
                                }
                                GUILayout.FlexibleSpace();


                            }
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(5);
                            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); //draw line
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
                #endregion
            }

        }
        m_fAniTimeLastValue = m_fAniTimeValue;
        m_nAffectedObjectLastNum = m_nAffectedObjectNum;

        if (Event.current.type == EventType.mouseUp && EditorWindow.mouseOverWindow == this)
        {
            OnClickWindow();
        }
    }
    private void OnChangeFrameTime(object data)
    {
        if (data != null && data is KeyframeData)
        {
            foreach (ActionFrameData actionData in (data as KeyframeData).framedatalist)
            {
                actionData.Time = m_fAniTimeValue;
            }
            m_KeyFrameDataDict = ActionHelper.ConvertKeyFrameData(m_FileData.FrameDatalist);
        }
    }
    private void OnPasteFrameData()
    {
        if (m_lstCopyedData != null)
        {
            SaveData(m_lstCopyedData);
            m_lstCopyedData.Clear();
        }
    }
    private void OnPasteFrameData(object data)
    {
        if (m_lstCopyedData != null)
        {
            if (data != null)
            {
                float time = (float)data;
                if (time < m_fActionDuration && time > 0)
                {
                    foreach (ActionFrameData frame in m_lstCopyedData)
                    {
                        frame.Time = time;
                    }
                }
            }
            SaveData(m_lstCopyedData);
            m_lstCopyedData.Clear();
        }
    }
    private void OnCopyFrameData(object data)
    {
        if (data != null && data is ActionFrameData)
        {
            m_lstCopyedData.Clear();
            m_lstCopyedData.Add(data as ActionFrameData);
        }
    }
    private void OnClickWindow()
    {
        ResetKeyFrameTex();
        Repaint();
    }
    private void OnClickTimeFrameTex(KeyValuePair<float, KeyframeData> unit)
    {
        //m_KeyframeTexRed = ActionHelper.LoadEditorKeyframeTexBlue();
        unit.Value.isSelected = true;
        //ActionKeyframeWindow.Instance.OpenWindow(unit.Key, unit.Value);
    }
    private void OnClickFrame(ActionFrameData data)
    {
        ResetKeyFrameTex();
        m_KeyFrameDataDict = ActionHelper.ConvertKeyFrameData(m_FileData.FrameDatalist);
        KeyframeData framedata = m_KeyFrameDataDict[(float)data.Time];
        if (null != framedata)
        {
            framedata.isSelected = true;
        }
    }
    private void OnChangePlayerTimeLine(float fValue)
    {
        //Debug.Log(fValue.ToString());
    }
    private void OnChangeSelectedActionTime()
    {
        if (m_lstSelectedFrameData == null || m_lstSelectedFrameData.Count <= 0)
            return;
        ActionTimeChangeWindow.Instance.OpenWindow(m_lstSelectedFrameData, m_fActionDuration);
    }
    private void OnCancelSelectedAction()
    {
        m_lstSelectedFrameData = null;
    }
    private void OnCopySelectedAction()
    {
        if (m_lstSelectedFrameData == null || m_lstSelectedFrameData.Count <= 0)
            return;
        m_lstCopyedData.Clear();
        m_lstCopyedData.AddRange(m_lstSelectedFrameData);
    }
    #endregion

    #region System Event
    private void ClearData()
    {
        OnStop();
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
            UnityEngine.Object.Destroy(m_ObjMapInstance);

            // clear terrain
            CloseTerrain();

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


        //Close Windows
        var subTypeQuery = from t in Assembly.GetExecutingAssembly().GetTypes()
                           where IsSubClassOf(t, typeof(AbstractFrameEdit))
                           select t;
        foreach (var type in subTypeQuery)
        {
            MethodInfo method = type.GetMethod("CloseWindow");
            method.Invoke(null, null);
        }
        m_RuntimeParam.ClearData();
        AERuntimeParamEditorWindow.CloseWindow();
        ActionKeyframeWindow.CloseWindow();
        ActionTimeChangeWindow.CloseWindow();
        ActionListWindow.CloseWindow();
        ConflictSolveWindow.CloseWindow();
    }
    private void Init()
    {
        // Check Game Root
        GameObject RootObj = GameObject.Find("ActionEditorRoot");
        m_ObjSceneRoot = GameObject.Find("SceneRoot");
        if (null == RootObj || null == m_ObjSceneRoot)
        {
            Debug.LogError("wrong scene");
        }
        // SetUp Window
        m_MainWnd.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
        m_MainWnd.maxSize = new Vector2(WINDOW_MAX_WIDTH, WINDOW_MAX_HIEGHT);
        titleStyle = new GUIStyle();
        titleStyle.fontSize = WINDOW_TITLE_FONTSIZE;
        titleStyle.normal.textColor = Color.white;
        m_KeyframeTexRed = ActionHelper.LoadEditorKeyframeTexRed();
        m_KeyframeTexBlue = ActionHelper.LoadEditorKeyframeTexBlue();
        m_FileDataList = ActionHelper.GetActionEditFileList();
        // Init Frame Names
        InitFrameName();
        m_szActionFrameName = new string[m_mapActionFrameNameDict.Count];
        foreach (KeyValuePair<EActionFrameType, string> pair in m_mapActionFrameNameDict)
        {
            if (m_szActionFrameName[(int)pair.Key] == null)
            {
                m_szActionFrameName[(int)pair.Key] = pair.Value;
            }
            else
            {
                Debuger.LogError(pair.Key.ToString() + "Has Same Values !!!!");
            }
        }
        // Init Data
        m_RuntimeParam = new AERuntimeParam();
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
                OpenTerrain();
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
        if (null == m_MainWnd)
        {
            return;
        }
        m_MainWnd.ClearData();
        m_MainWnd.Close();
        m_MainWnd = null;
    }
    private void OnPlay()
    {
        m_bIsPaused = false;

        if (null == m_FileData)
        {
            return;
        }

        if (!m_bPlay)
        {
            m_bPlay = true;
            m_ActionId = ActionManager.Instance.InsertAction(m_FileData.ID, m_FileData, m_RuntimeParam.GetRuntimeActionParam(), m_AffectedObjects);
            m_RuntimeParam.GetRuntimeActionParam().Id = m_ActionId;
        }

        ActionPlayer action = ActionManager.Instance.GetAction(m_ActionId);
        if (null == action || action.IsFinish())
        {
            return;
        }
        action.SetActionRunTime(m_fAniTimeValue);
        action.Play();
    }

    private void OnPaused()
    {
        m_bIsPaused = true;
        ActionPlayer action = ActionManager.Instance.GetAction(m_ActionId);
        if (null == action || action.IsFinish())
        {
            return;
        }
        action.Pause();
    }
    private void OnStop()
    {
        if (m_ActionId >= 0)
        {
            ActionManager.Instance.RemoveAction(m_ActionId);
        }
        ResetTimeLine();
        OnPaused();
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
        ActionHelper.CombineActionEditFileList(m_FileDataList);
        EditorUtility.DisplayDialog("保存成功", "保存成功", "确定");
    }
    private void OnReplaceFile()
    {
        var option = EditorUtility.DisplayDialog("警告!!!", "取代方案将彻底覆盖本地数据", "确定", "取消");
        if (option)
        {
            m_FileDataList = ActionHelper.GetActionEditFileList();
            ActionHelper.ReplaceEditFileList(m_FileDataList);
        }
    }
    private void OnSyncFile()
    {
        var option = EditorUtility.DisplayDialog("警告!!!", "同步方案将覆盖本地数据", "确定", "取消");
        if (option)
        {
            m_FileDataList = ActionHelper.GetActionEditFileList();
            if (m_FileData != null)
            {
                ActionHelper.SaveActionEditFileList(m_FileDataList, m_FileData);
            }
            ActionHelper.SyncActionEditFileList(m_FileDataList);
        }
    }
    private void OnMergeFile()
    {
        m_FileDataList = ActionHelper.GetActionEditFileList();
        if (m_FileData != null)
        {
            ActionHelper.SaveActionEditFileList(m_FileDataList, m_FileData);
        }
        Dictionary<int, ActionFileData> conflictData;
        if (ActionHelper.CombineActionEditFileList(m_FileDataList, out conflictData))
        {
            Debug.LogWarning("<color=orange> Conflict Occurred !</color>");
            ConflictSolveWindow.Instance.OpenWindow(conflictData);
        }
        else
        {
            EditorUtility.DisplayDialog("合并成功", "合并成功", "确定");
        }
    }
    private void OnBackupFile()
    {
        string path = "D:/ActionBackup/";
        ActionHelper.BackupEditFileList(path);
        EditorUtility.DisplayDialog("备份成功", "路径：" + path, "确定");
        EditorUtility.OpenFilePanel("", path, "");
    }
    #endregion

    #region Public Interface
    public void SaveData(ActionFrameData data)
    {
        if (null == m_FileData)
        {
            return;
        }

        if (ActionHelper.AddFrame(m_FileData, data))
        {
            OnSave();
        }
        m_KeyFrameDataDict = ActionHelper.ConvertKeyFrameData(m_FileData.FrameDatalist);
        Repaint();
    }
    public void SaveData(List<ActionFrameData> dataList)
    {
        if (null == m_FileData || null == dataList || dataList.Count <= 0)
        {
            return;
        }
        if (ActionHelper.AddFrameList(m_FileData, dataList))
        {
            OnSave();
        }
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
            OnChangePlayerTimeLine(m_fAniTimeValue);
        }

        switch (eType)
        {
            case EActionFrameType.ShakeCamera:
                ShakeCameraFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.MoveCamera:
                MoveCameraFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.PlayAudio:
                PlayAudioFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.StopAudio:
                StopAudioFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.AddNpc:
                AddNpcFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.AnimChar:
                AnimCharFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.MoveObject:
                MoveFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.EnableObject:
                EnableObjFrameEidt.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.EnableMeshRender:
                EnableMeshRenderFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.ChangeColor:
                ChangeColorFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.Runtime_CreateEffect:
                Runtime_CreateEffectFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.Runtime_MoveEffect:
                Runtime_MoveEffectFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.Runtime_AttackExec:
                Runtime_AttackExecFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.Runtime_RemoveEffect:
                Runtime_RemoveEffectFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.AddStateEffect:
                AddStateEffectFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.Runtime_AddUI:
                AddUIFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.Runtime_RemoveUI:
                RemoveUIFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.MoveChar:
                MoveCharFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.EntityPlayAnimation:
                EntityPlayAnimFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.RotateChar:
                RotateCharFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.RotateCamera:
                RotateCameraFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.ObjTransform:
                ObjectTransformFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
                break;
            case EActionFrameType.FuncMethod:
                FuncMethodFrameEdit.Instance.OpenWindow(m_fActionDuration, m_fAniTimeValue, eType, frameData);
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
    private void ResetKeyFrameTex()
    {
        foreach (KeyValuePair<float, KeyframeData> unit in m_KeyFrameDataDict)
        {
            unit.Value.isSelected = false;
        }
    }
    private void CreateNpc() //Test
    {
        Npc newNpc = new Npc();
        newNpc.Initialize(20000001);
        newNpc.GetTransformData().SetPosition(new Vector3(0, 0, -8));
        newNpc.GetTransformData().SetRotation(new Vector3(0, 0, 0));
        newNpc.GetTransformData().SetScale(new Vector3(1, 1, 1));
    }
    private static bool IsSubClassOf(Type type, Type baseType)
    {
        var b = type.BaseType;
        while (b != null)
        {
            if (b.Equals(baseType))
            {
                return true;
            }
            b = b.BaseType;
        }
        return false;
    }
    private void EditRuntimeParam()
    {

    }
    private void InitFrameName()
    {
        m_mapActionFrameNameDict = new Dictionary<EActionFrameType, string>();
        m_mapActionFrameNameDict.Add(EActionFrameType.ShakeCamera, "镜头 / 摄像机震动");
        m_mapActionFrameNameDict.Add(EActionFrameType.MoveCamera, "镜头 / 摄像机移动");
        m_mapActionFrameNameDict.Add(EActionFrameType.PlayAudio, "声音 / 添加声音播放");
        m_mapActionFrameNameDict.Add(EActionFrameType.StopAudio, "声音 / 停止声音播放");
        m_mapActionFrameNameDict.Add(EActionFrameType.AddNpc, "单位 / 创建NPC");
        m_mapActionFrameNameDict.Add(EActionFrameType.AnimChar, "单位 / 单位动画");
        m_mapActionFrameNameDict.Add(EActionFrameType.MoveChar, "单位 / 移动单位");
        m_mapActionFrameNameDict.Add(EActionFrameType.MoveObject, "物体 / 物体移动");
        m_mapActionFrameNameDict.Add(EActionFrameType.EnableObject, "物体 / 物体开关");
        m_mapActionFrameNameDict.Add(EActionFrameType.EnableMeshRender, "物体 / 物体渲染开关");
        m_mapActionFrameNameDict.Add(EActionFrameType.ChangeColor, "物体 / 修改颜色");
        m_mapActionFrameNameDict.Add(EActionFrameType.EntityPlayAnimation, "物体 / 播放动画");
        m_mapActionFrameNameDict.Add(EActionFrameType.Runtime_CreateEffect, "动作/创建特效");
        m_mapActionFrameNameDict.Add(EActionFrameType.Runtime_MoveEffect, "动作/移动特效");
        m_mapActionFrameNameDict.Add(EActionFrameType.Runtime_RemoveEffect, "动作/删除特效");
        m_mapActionFrameNameDict.Add(EActionFrameType.Runtime_AttackExec, "动作/打击点");
        m_mapActionFrameNameDict.Add(EActionFrameType.AddStateEffect, "特效 / 静态创建特效");
        m_mapActionFrameNameDict.Add(EActionFrameType.Runtime_AddUI, "UI / 添加UI");
        m_mapActionFrameNameDict.Add(EActionFrameType.Runtime_RemoveUI, "UI / 删除UI");
        m_mapActionFrameNameDict.Add(EActionFrameType.RotateChar, "单位 / 旋转单位");
        m_mapActionFrameNameDict.Add(EActionFrameType.RotateCamera, "镜头 / 摄像机旋转");
        m_mapActionFrameNameDict.Add(EActionFrameType.ObjTransform, "物体 / 物体瞬移");
        m_mapActionFrameNameDict.Add(EActionFrameType.FuncMethod, "功能函数 / 执行功能函数");
        m_mapActionFrameNameDict.Add(EActionFrameType.Max, "全部");
    }
    private Color GetFrameTypeColor(string framename)
    {
        string type = framename.Substring(0, 4);

        if (type.Contains("镜头"))
        {
            return ColorDefine.Blue;
        }
        else if (type.Contains("声音"))
        {
            return ColorDefine.Yellow;
        }
        else if (type.Contains("单位"))
        {
            return ColorDefine.Orange;
        }
        else if (type.Contains("物体"))
        {
            return ColorDefine.LightGray;
        }
        else if (type.Contains("动作"))
        {
            return ColorDefine.Red;
        }
        else if (type.Contains("特效"))
        {
            return ColorDefine.Green;
        }
        else if (type.Contains("UI"))
        {
            return ColorDefine.LightBlue;
        }
        else if (type.Contains("功能"))
        {
            return ColorDefine.Gray;
        }

        return Color.white;
    }
    private void OpenTerrain()
    {
        TerrainManager.Instance.InitializeTerrain(m_TerrainID, true, false);
    }
    private void CloseTerrain()
    {
        TerrainManager.Instance.CloseTerrain();
    }
    #endregion

}
