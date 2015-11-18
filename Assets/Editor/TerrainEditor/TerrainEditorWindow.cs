using Assets.Scripts.Core.Utils;
using Common.Auto;
using Communication;
using Config;
using Config.Table;
using TerrainEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TriggerEditorData
{
    public GameObject m_SourceObj;
    public List<string> m_Tmp;
    public TerrainTriggerData TriggerData;
}

public class NpcEditorData
{
    public GameObject m_SourceObj;
    public List<string> m_Tmp;
    public TerrainNpcData m_NpcData;
}
public class TerrainEditorWindow : EditorWindow
{ 
    private static TerrainEditorWindow  m_MainWnd;
    private string                      m_strDataPath;
    private TerrainEditorDataArray      m_DataList;

    //editor data
    private GameObject                                  m_ObjMap;
    private GameObject                                  m_ObjMapInstance;
    private GameObject                                  m_ObjSceneCamera;
    private bool                                        m_bIsCreateNew;
    //private bool                                        m_bInitSceneCamera;
    private GameObject                                  m_ObjSceneRoot;
    private GameObject                                  m_ObjTriggerRoot;
    private GameObject                                  m_ObjNpcRoot;
    private Dictionary<ETriggerAreaType, GameObject>    m_TriggerTemplateMap;
    private ETriggerAreaType                            m_ETriggerAreaType;
    private string[]                                    m_TriggerTypeList;
    private string[]                                    m_NpcTypeList;
    private int                                         m_nCurrentSettingPosTriggerIndex;
    private int                                         m_nCurrentSettingPosNpcIndex;
    private List<TriggerEditorData>                     m_RemovingTriggerList;
    private List<TriggerEditorData>                     m_TriggerList;
    private List<NpcEditorData>                         m_RemovingNpcList;
    private List<NpcEditorData>                         m_NpcList;
    private int                                         m_CurrentEditiongMapId;
    private string                                      m_CurrentEditiongMapName;
    private string                                      m_MapNameInputBuffer;
    private string                                      m_CurrentMapResPath;
    private int m_iSelectedNpcId;

    // editor output data
    private TerrainEditorData                           m_OutputData;

    [MenuItem("Editors/Terrain")]
    static void CreateWindow()
    {
        if (!CheckScene())
        {
            return;
        }
        m_MainWnd = EditorWindow.GetWindow<TerrainEditorWindow>(false, "地形编辑器", true);
        m_MainWnd.Init();
        TerrainEditorMain.Instance.SetCloseWindow(CloseWindow);
    }
    static public TerrainEditorWindow Instance
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
    void OnEnable()
    {
    }
    private static bool CheckScene()
    {
        var RootObj = GameObject.Find("TerrainEditorRoot");
        var a = GameObject.Find("SceneRoot");
        var b = GameObject.Find("TriggerRoot");
        var TriggerTemplateRoot = GameObject.Find("TriggerTemplateRoot");

        if (null == RootObj || null == a || null == b || null == TriggerTemplateRoot)
        {
            return false;
        }
        if (TerrainEditorMain.Instance == null)
        {
            return false;
        }
        return true;

    }
    void Init()
    {
        m_strDataPath = Application.persistentDataPath + "/EditorData/terrainConfig_txtpkg.bytes";

        GameObject RootObj = GameObject.Find("TerrainEditorRoot");
        m_ObjSceneRoot = GameObject.Find("SceneRoot");
        m_ObjTriggerRoot = GameObject.Find("TriggerRoot");
        var TriggerTemplateRoot = GameObject.Find("TriggerTemplateRoot");
        m_ObjNpcRoot = GameObject.Find("NpcRoot");
        
        if (null == RootObj || null == m_ObjSceneRoot || null == m_ObjTriggerRoot || null == TriggerTemplateRoot)
        {
            Debug.LogError("wrong scene");
        }

        // init trigger 
        int triggrCount = TriggerTemplateRoot.transform.childCount;
        m_TriggerTemplateMap = new Dictionary<ETriggerAreaType, GameObject>(triggrCount);
        m_TriggerTypeList = new string[triggrCount];

        for (int i = 0; i < triggrCount; ++i)
        {
            ETriggerAreaType type = (ETriggerAreaType)(i);
            GameObject elem = TriggerTemplateRoot.transform.GetChild(i).gameObject;
            m_TriggerTemplateMap.Add(type, elem);
            m_TriggerTypeList[i] = type.ToString();
        }

        NpcConfigTable npcConfigTable = ConfigManager.Instance.GetNpcTable();
        if (null == npcConfigTable)
        {
            EditorUtility.DisplayDialog("", "npc配置文件读取失败", "ok");
            ClearData();
            CloseWindow();
            return;
        }

        m_NpcTypeList = new string[npcConfigTable.NpcCofigMap.Count];
        int tmpIndex = 0;
        foreach (var elem in npcConfigTable.NpcCofigMap)
        {
            m_NpcTypeList[tmpIndex++] = elem.Key.ToString();
        }

        m_RemovingNpcList = new List<NpcEditorData>();
        m_NpcList = new List<NpcEditorData>();
        m_TriggerList = new List<TriggerEditorData>();
		m_ETriggerAreaType = ETriggerAreaType.Sphere;
        m_RemovingTriggerList = new List<TriggerEditorData>();
        m_MapNameInputBuffer = string.Empty;
        m_iSelectedNpcId = 0;
        
        Debug.Log(" initialize terrain");
    }
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("地形方案列表", GUILayout.Width(100f)))
            {
                TerrainListWindow.Instance.OpenWindow();
            }

            if (GUILayout.Button("新建地形方案", GUILayout.Width(100f)))
            {
                m_bIsCreateNew = true;
            }
            if (GUILayout.Button("重置面板", GUILayout.Width(100f)))
            {
                ClearData();
            }
        }
        EditorGUILayout.EndHorizontal();


        GUILayout.Space(20f);
        if (GUILayout.Button("保存地形方案", GUILayout.Width(100f)))
        {
            SaveData();
        }
        if (m_bIsCreateNew)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("地图名称:", GUILayout.Width(50f));
                m_CurrentEditiongMapName = EditorGUILayout.TextArea(m_CurrentEditiongMapName);

                EditorGUILayout.LabelField("地图id:", GUILayout.Width(50f));
                
                m_MapNameInputBuffer = EditorGUILayout.TextArea(m_MapNameInputBuffer);
                int id = m_CurrentEditiongMapId;
                if (int.TryParse(m_MapNameInputBuffer, out id))
                {
                    m_CurrentEditiongMapId = id;
                }

                EditorGUILayout.LabelField("地图:", GUILayout.Width(50f));
                m_ObjMap = (GameObject)EditorGUILayout.ObjectField(m_ObjMap, typeof(GameObject));
                CheckMap();

                EditorGUILayout.LabelField("场景摄像机:", GUILayout.Width(50f));
                m_ObjSceneCamera = (GameObject)EditorGUILayout.ObjectField(m_ObjSceneCamera, typeof(GameObject));
            }
            EditorGUILayout.EndHorizontal();
            
            //draw trigger
            DrawTrigger();

            //draw npc
            DrawNpc();
        }
    }
    public void OpenTerrain(TerrainEditorData data)
    {
        ClearData();

        m_bIsCreateNew = true;
        m_CurrentEditiongMapId = data.ID;
        m_CurrentEditiongMapName = data.MapName;
        m_MapNameInputBuffer = m_CurrentEditiongMapId.ToString();

        m_ObjMap = ResourceManager.Instance.LoadBuildInResource<GameObject>(data.MapResName, AssetType.Map) ;
        
        for (int i = 0; data.TriggerDataList != null && i < data.TriggerDataList.Count; ++i)
        {
            TerrainTriggerData elem = data.TriggerDataList[i];

            AddTrigger
                (elem.AreaType,
                elem.Pos.GetVector3(),
                elem.Rot.GetVector3(),
                elem.Scale.GetVector3(),
                elem.TargetMethodId,
                elem.EnterLimitMethodId,
                elem.ExitLimitMethodId,
                elem.EnterFuncMethodId,
                elem.ExitFuncMethodId
                );
        }
        for (int i = 0;null != data.NpcDataList && i < data.NpcDataList.Count; ++i)
        {
            TerrainNpcData elem = data.NpcDataList[i];

            AddNpc(elem.Id,elem.Pos.GetVector3(),elem.Rot.GetVector3(),elem.Scale.GetVector3());
        }
    }

    #region draw ui
    void DrawTrigger()
    {
        if (NGUIEditorTools.DrawHeader("触发器配置"))
        {
            EditorGUILayout.BeginHorizontal();
            {
                m_ETriggerAreaType = (ETriggerAreaType)EditorGUILayout.Popup((int)m_ETriggerAreaType, m_TriggerTypeList, GUILayout.Width(100f));
                if (GUILayout.Button("添加一个触发器", GUILayout.Width(120f)))
                {
                    AddTrigger(  m_ETriggerAreaType, Vector3.zero,Vector3.zero,Vector3.one );
                }
            }
            
            EditorGUILayout.EndHorizontal();
            for(int i=0;i<m_TriggerList.Count;++i)
            {
                TriggerEditorData data = m_TriggerList[i];

                if (NGUIEditorTools.DrawHeader(m_TriggerList[i].TriggerData.AreaType.ToString()))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("调整触发器位置", GUILayout.Width(120f)))
                        {
                            m_nCurrentSettingPosTriggerIndex = i;
                            TerrainEditorMain.Instance.SetRaycastCallBack(SetTriggerPos);
                        }
                        if (GUILayout.Button("删除触发器", GUILayout.Width(120f)))
                        {
                            m_RemovingTriggerList.Add(m_TriggerList[i]);
                        }
                        if (GUILayout.Button("编辑触发器事件", GUILayout.Width(120f)))
                        {
                            TerrainTriggerNodeEditorWindow.Instance.OpenWindow(data.TriggerData);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        Vector3 tmpPos = data.m_SourceObj.transform.position;
                        GUILayout.TextArea("触发器位置", GUILayout.Width(120f));
                        GUILayout.Label(tmpPos.ToString(), GUILayout.Width(100f));

                        GUILayout.Label("x", GUILayout.Width(20f));
                        data.m_Tmp[1] = GUILayout.TextArea(data.m_Tmp[1]);
                        GUILayout.Label("y", GUILayout.Width(20f));
                        data.m_Tmp[2] = GUILayout.TextArea(data.m_Tmp[2]);
                        GUILayout.Label("z", GUILayout.Width(20f));
                        data.m_Tmp[3] = GUILayout.TextArea(data.m_Tmp[3]);

                        float tmpData = tmpPos.x;
                        if (float.TryParse(data.m_Tmp[1], out tmpData))
                        {
                            tmpPos.x = tmpData;
                        }
                        tmpData = tmpPos.y;
                        if (float.TryParse(data.m_Tmp[2], out tmpData))
                        {
                            tmpPos.y = tmpData;
                        }
                        tmpData = tmpPos.z;
                        if (float.TryParse(data.m_Tmp[3], out tmpData))
                        {
                            tmpPos.z = tmpData;
                        }
                        data.m_SourceObj.transform.position = tmpPos;
                        data.TriggerData.Pos.SetVector3(data.m_SourceObj.transform.position);
                    }
                    EditorGUILayout.EndHorizontal();
                    switch (data.TriggerData.AreaType)
                    {
                        case ETriggerAreaType.Cube:
                            DrawCubeTrigger(data);
                            break;
                        case ETriggerAreaType.Sphere:
                            DrawCircleTrigger(data);
                            break;
                        default:
                            break;
                    }
                }
            }
            CheckRemovingTrigger();
            
        }
    }
    private void DrawCubeTrigger(TriggerEditorData data)
    {
        
    }
    private void DrawCircleTrigger(TriggerEditorData data)
    {
        EditorGUILayout.BeginHorizontal();
        {
            Vector3 scale = data.TriggerData.Scale.GetVector3();
            GUILayout.TextArea("触发器大小", GUILayout.Width(120f));
            GUILayout.Label(scale.x.ToString(), GUILayout.Width(120f));

            data.m_Tmp[0] = GUILayout.TextArea(data.m_Tmp[0], GUILayout.Width(120f));
            float realRange = scale.x;
            if (float.TryParse(data.m_Tmp[0], out realRange))
            {
                scale = new Vector3(realRange, realRange, realRange);
                data.m_SourceObj.transform.localScale = scale;
            }
            data.TriggerData.Scale.SetVector3(scale);
        }
        EditorGUILayout.EndHorizontal();
    }
    void DrawNpc()
    {
        if (NGUIEditorTools.DrawHeader("NPC配置"))
        {
            EditorGUILayout.BeginHorizontal();
            {
                m_iSelectedNpcId = EditorGUILayout.Popup(m_iSelectedNpcId, m_NpcTypeList, GUILayout.Width(100f));
                if (GUILayout.Button("添加一个NPC", GUILayout.Width(120f)))
                {
                    AddNpc(int.Parse(m_NpcTypeList[m_iSelectedNpcId]), Vector3.zero, Vector3.zero, Vector3.one);
                }
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < m_NpcList.Count; ++i)
            {
                NpcEditorData data = m_NpcList[i];

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("调整NPC位置", GUILayout.Width(120f)))
                        {
                            m_nCurrentSettingPosNpcIndex = i;
                            TerrainEditorMain.Instance.SetRaycastCallBack(SetNpcPos);
                        }
                        if (GUILayout.Button("删除NPC", GUILayout.Width(120f)))
                        {
                            m_RemovingNpcList.Add(data);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        Vector3 tmpPos = data.m_SourceObj.transform.position;
                        GUILayout.TextArea("NPC位置", GUILayout.Width(120f));
                        GUILayout.Label(tmpPos.ToString(), GUILayout.Width(100f));

                        GUILayout.Label("x", GUILayout.Width(20f));
                        data.m_Tmp[1] = GUILayout.TextArea(data.m_Tmp[1]);
                        GUILayout.Label("y", GUILayout.Width(20f));
                        data.m_Tmp[2] = GUILayout.TextArea(data.m_Tmp[2]);
                        GUILayout.Label("z", GUILayout.Width(20f));
                        data.m_Tmp[3] = GUILayout.TextArea(data.m_Tmp[3]);

                        float tmpData = tmpPos.x;
                        if (float.TryParse(data.m_Tmp[1], out tmpData))
                        {
                            tmpPos.x = tmpData;
                        }
                        tmpData = tmpPos.y;
                        if (float.TryParse(data.m_Tmp[2], out tmpData))
                        {
                            tmpPos.y = tmpData;
                        }
                        tmpData = tmpPos.z;
                        if (float.TryParse(data.m_Tmp[3], out tmpData))
                        {
                            tmpPos.z = tmpData;
                        }
                        data.m_SourceObj.transform.position = tmpPos;
                        data.m_NpcData.Pos.SetVector3(data.m_SourceObj.transform.position);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        Vector3 tmpRot = data.m_SourceObj.transform.eulerAngles;
                        GUILayout.TextArea("NPC朝向", GUILayout.Width(120f));
                        GUILayout.Label(tmpRot.y.ToString(), GUILayout.Width(100f));

                        GUILayout.Label("y", GUILayout.Width(20f));
                        data.m_Tmp[4] = GUILayout.TextArea(data.m_Tmp[4]);

                        float tmpData = tmpRot.y;
                        if (float.TryParse(data.m_Tmp[4], out tmpData))
                        {
                            tmpRot.y = tmpData;
                        }

                        data.m_SourceObj.transform.eulerAngles = tmpRot;
                        data.m_NpcData.Rot.SetVector3(data.m_SourceObj.transform.eulerAngles);
                    }
                    EditorGUILayout.EndHorizontal();
            }
            CheckRemovingNpc();
        }
    }
    #endregion

    #region data handler
    private void AddTrigger(ETriggerAreaType TriggerAreaType, Vector3 position, Vector3 rotation, Vector3 scale, int targetId = 0, int enterLimitId = 0, int exitLimitId=0,int enterFuncId = 0, int exitFuncId = 0)
    {
        GameObject sourceObj = m_TriggerTemplateMap[TriggerAreaType];
        if (null == sourceObj)
        {
            Debug.Log(TriggerAreaType.ToString() + " " + m_TriggerTemplateMap.Count);
        }
        else 
        { 
            GameObject instance = GameObject.Instantiate(sourceObj);
            ComponentTool.Attach(m_ObjTriggerRoot.transform, instance.transform);


            TriggerEditorData elem = new TriggerEditorData();
            elem.TriggerData = new TerrainTriggerData();
            elem.TriggerData.Pos = new ThriftVector3();
            elem.TriggerData.Rot = new ThriftVector3();
            elem.TriggerData.Scale = new ThriftVector3();

            elem.TriggerData.Pos.SetVector3(position);
            elem.TriggerData.Rot.SetVector3(rotation);
            elem.TriggerData.Scale.SetVector3(scale);
            elem.TriggerData.AreaType = TriggerAreaType;
            
            elem.m_Tmp = new System.Collections.Generic.List<string>();
            for (int i = 0; i < 4; ++i)
            {
                elem.m_Tmp.Add(string.Empty);
            }

            elem.TriggerData.TargetMethodId = targetId;
            elem.TriggerData.EnterLimitMethodId = enterLimitId;
            elem.TriggerData.ExitLimitMethodId = exitLimitId;
            elem.TriggerData.EnterFuncMethodId = enterFuncId;
            elem.TriggerData.ExitFuncMethodId = exitFuncId;

            elem.m_SourceObj = instance;
            elem.m_SourceObj.transform.position = position;
            elem.m_SourceObj.transform.eulerAngles = rotation;
            elem.m_SourceObj.transform.localScale = scale;

            m_TriggerList.Add(elem);
        }
    }
    private void CheckMap()
    {
        if( null != m_ObjMap)
        {
            if( null == m_ObjMapInstance )
            {
                // reset map
                m_ObjMapInstance = GameObject.Instantiate(m_ObjMap);
                ComponentTool.Attach(m_ObjSceneRoot.transform, m_ObjMapInstance.transform);
                //record map name
                m_CurrentMapResPath = m_ObjMap.name;
                m_ObjSceneCamera = ComponentTool.FindChild("SceneCamera", m_ObjMapInstance);
                if (null == m_ObjSceneCamera)
                {
                    Debug.LogError("Can't load scene camera !!!");
                }

                TerrainEditorMain.Instance.SetSceneCamera(m_ObjSceneCamera);
                TerrainEditorMain.Instance.SetClearWindow(ClearData);
                Debug.Log("map name : " + m_CurrentMapResPath);
            }
        }
    }
    private void ClearData()
    {
        m_bIsCreateNew = false;
        m_ObjMap = null;
        // destroy map
        if (null != m_ObjMapInstance)
        {
            Object.Destroy(m_ObjMapInstance);
        }
        //clear trigger
        if (null != m_RemovingTriggerList)
        {
            m_RemovingTriggerList.Clear();
        }
        if (null != m_TriggerList)
        {
            for(int i=0;i<m_TriggerList.Count;++i)
            {
                GameObject.Destroy(m_TriggerList[i].m_SourceObj);
            }

            m_TriggerList.Clear();
        }
        //clear npc
        if (null != m_NpcList)
        {
            for (int i = 0; i < m_NpcList.Count; ++i)
            {
                GameObject.Destroy(m_NpcList[i].m_SourceObj);
            }
            m_NpcList.Clear();
        }
        if (null != m_RemovingNpcList)
        {
            m_RemovingNpcList.Clear();
        }
        m_ObjMapInstance = null;
    }
    private void SetTriggerPos(Vector3 positin)
    {
        if (m_nCurrentSettingPosTriggerIndex < 0 || m_nCurrentSettingPosTriggerIndex >= m_TriggerList.Count)
        {
            return;
        }
        m_TriggerList[m_nCurrentSettingPosTriggerIndex].TriggerData.Pos.SetVector3(positin);
        m_TriggerList[m_nCurrentSettingPosTriggerIndex].m_SourceObj.transform.position = positin;
    }
    private void SetNpcPos(Vector3 positin)
    {
        if (m_nCurrentSettingPosNpcIndex < 0 || m_nCurrentSettingPosNpcIndex >= m_NpcList.Count)
        {
            return;
        }
        m_NpcList[m_nCurrentSettingPosNpcIndex].m_NpcData.Pos.SetVector3(positin);
        m_NpcList[m_nCurrentSettingPosNpcIndex].m_SourceObj.transform.position = positin;
    }
    private void AddNpc(int id, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        NpcConfig tmpConfig = ConfigManager.Instance.GetNpcConfig(id);
        if (null == tmpConfig)
        {
            EditorUtility.DisplayDialog("id 错误", "请检查表中npc id ，错误 id= " + id, "ok");
            return;
        }

        GameObject sourceObj = ResourceManager.Instance.LoadBuildInResource<GameObject>(tmpConfig.ModelResource,
            AssetType.Char);
        if (null == sourceObj)
        {
            EditorUtility.DisplayDialog("模型 id 错误", "请检查表中npc id ，错误 id= " + tmpConfig.ModelResource, "ok");
            return;
        }
        GameObject instance = GameObject.Instantiate(sourceObj);
        ComponentTool.Attach(m_ObjNpcRoot.transform, instance.transform);

        NpcEditorData elem = new NpcEditorData();
        elem.m_NpcData = new TerrainNpcData();
        elem.m_NpcData.Pos = new ThriftVector3();
        elem.m_NpcData.Rot = new ThriftVector3();
        elem.m_NpcData.Scale = new ThriftVector3();
        elem.m_NpcData.Id = id;

        elem.m_NpcData.Pos.SetVector3(position);
        elem.m_NpcData.Rot.SetVector3(rotation);
        elem.m_NpcData.Scale.SetVector3(scale);

        elem.m_Tmp = new System.Collections.Generic.List<string>();
        for (int i = 0; i < 5; ++i)
        {
            elem.m_Tmp.Add(string.Empty);
        }
        
        elem.m_SourceObj = instance;
        elem.m_SourceObj.transform.position = position;
        elem.m_SourceObj.transform.eulerAngles = rotation;
        elem.m_SourceObj.transform.localScale = scale;

        m_NpcList.Add(elem);
    }
    private void CheckRemovingTrigger()
    {
        if (m_RemovingTriggerList.Count > 0)
        {
            m_nCurrentSettingPosTriggerIndex = -1;
            for (int i = 0; i < m_RemovingTriggerList.Count; ++i)
            {
                GameObject.Destroy(m_RemovingTriggerList[i].m_SourceObj);
                m_TriggerList.Remove(m_RemovingTriggerList[i]);
            }
            m_RemovingTriggerList.Clear();
        }
    }
    private void CheckRemovingNpc()
    {
        if (m_RemovingNpcList.Count > 0)
        {
            m_nCurrentSettingPosNpcIndex = -1;
            for (int i = 0; i < m_RemovingNpcList.Count; ++i)
            {
                GameObject.Destroy(m_RemovingNpcList[i].m_SourceObj);
                m_NpcList.Remove(m_RemovingNpcList[i]);
            }
            m_RemovingNpcList.Clear();
        }
    }
    private static void CloseWindow()
    {
        m_MainWnd.Close();
        m_MainWnd = null;
        TerrainTriggerNodeEditorWindow.CloseWindow();
        TerrainTriggerNodeEditorWindow.CloseWindow();
        TerrainListWindow.CloseWindow();
    }
    private void SaveData()
    {
        if (null == m_TriggerList)
        {
            return;
        }
        m_OutputData = new TerrainEditorData();
        m_OutputData.ID = m_CurrentEditiongMapId;
        m_OutputData.MapName = m_CurrentEditiongMapName;
        m_OutputData.MapResName = m_CurrentMapResPath;

        m_OutputData.TriggerDataList = new System.Collections.Generic.List<TerrainTriggerData>(m_TriggerList.Count);

        for (int i = 0; i < m_TriggerList.Count; ++i)
        {
            m_OutputData.TriggerDataList.Add(m_TriggerList[i].TriggerData);
        }

        m_OutputData.NpcDataList = new List<TerrainNpcData>(m_NpcList.Count);
        for (int i = 0; i < m_NpcList.Count; ++i)
        {
            m_OutputData.NpcDataList.Add(m_NpcList[i].m_NpcData);
        }

        m_DataList = GetTerrainEditFileList();

        SaveTerrainEditFileList(m_DataList, m_OutputData);

    }
    public void SaveTerrainEditFileList(TerrainEditorDataArray filedata,TerrainEditorData editorData)
    {
        if (null == filedata )
        {
            filedata = new TerrainEditorDataArray();
            filedata.DataList = new List<TerrainEditorData>();
        }

        bool bIsNeedAddNew = true;
        for (int i = 0; i < filedata.DataList.Count; ++i)
        {
            if (filedata.DataList[i].ID == editorData.ID)
            {
                filedata.DataList[i] = editorData;
                bIsNeedAddNew = false;
                break;
            }
        }
        if (bIsNeedAddNew)
        {
            filedata.DataList.Add(editorData);
        }

        byte[] data = ThriftSerialize.Serialize(filedata);
        FileUtils.WriteByteFile(m_strDataPath, data);
        
    }
    public TerrainEditorDataArray GetTerrainEditFileList()
    {
        ResourceManager.Instance.DecodePersonalDataTemplate(m_strDataPath, ref m_DataList);
        
        return m_DataList;
    }
    #endregion
}
