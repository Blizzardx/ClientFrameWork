using Assets.Scripts.Core.Utils;
using Common.Auto;
using Communication;
using Config;
using Config.Table;
using NetWork.Auto;
using TerrainEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class TriggerEditorData
{
    public GameObject m_SourceObj;
    public bool m_bIsSelected;
    public TerrainTriggerData TriggerData;
}

public class NpcEditorData
{
    public Npc m_Instance;
    public bool m_bIsSelected;
    public TerrainNpcData m_NpcData;
}
public class TerrainEditorWindow : EditorWindow
{
    private static TerrainEditorWindow m_MainWnd;
    private string m_strDataPath;
    private TerrainEditorDataArray m_DataList;

    //editor data
    private GameObject m_ObjMap;
    private GameObject m_ObjMapInstance;
    private bool m_bIsCreateNew;
    private Vector3 m_LastTimePos;
    private Vector3 m_LastTimeRot;
    private PlayerCharacter m_PlayerChar;
    private string[] m_PlayerTypeList;
    private int m_iSelectedPlayerId;
    private GameObject m_ObjSceneRoot;
    private GameObject m_ObjTriggerRoot;
    private GameObject m_ObjNpcRoot;
    private Dictionary<ETriggerAreaType, GameObject> m_TriggerTemplateMap;
    private ETriggerAreaType m_ETriggerAreaType;
    private string[] m_TriggerTypeList;
    private string[] m_NpcTypeList;
    private int m_nCurrentSettingPosTriggerIndex;
    private int m_nCurrentSettingPosNpcIndex;
    private System.Collections.Generic.List<TriggerEditorData> m_RemovingTriggerList;
    private System.Collections.Generic.List<TriggerEditorData> m_TriggerList;
    private System.Collections.Generic.List<NpcEditorData> m_RemovingNpcList;
    private System.Collections.Generic.List<NpcEditorData> m_NpcList;
    private int m_CurrentEditiongMapId;
    private string m_CurrentEditiongMapName;
    private string m_MapNameInputBuffer;
    private string m_CurrentMapResPath;
    private int m_iSelectedNpcId;
    private Npc m_CurrentSelectedNpc;
    private Vector2 m_tmp;

    // editor output data
    private TerrainEditorData m_OutputData;

    [MenuItem("Editors/Terrain/地图编辑器")]
    static void CreateWindow()
    {
        if (!CheckScene())
        {
            return;
        }
        m_MainWnd = EditorWindow.GetWindow<TerrainEditorWindow>(false, "地形编辑器", true);
        m_MainWnd.Init();
        TerrainEditorRuntime.Instance.SetCloseWindow(CloseWindow);
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
        if (TerrainEditorRuntime.Instance == null)
        {
            return false;
        }
        return true;

    }
    void Init()
    {
        //m_strDataPath = Application.persistentDataPath + "/EditorData/terrainConfig_txtpkg.bytes";
        m_strDataPath = Application.dataPath + "/EditorCommon/EditorResources/mmAdv/1.0/terrainConfig_txtpkg.bytes";

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

        m_RemovingNpcList = new System.Collections.Generic.List<NpcEditorData>();
        m_NpcList = new System.Collections.Generic.List<NpcEditorData>();
        m_TriggerList = new System.Collections.Generic.List<TriggerEditorData>();
        m_ETriggerAreaType = ETriggerAreaType.Sphere;
        m_RemovingTriggerList = new System.Collections.Generic.List<TriggerEditorData>();
        m_MapNameInputBuffer = string.Empty;
        m_iSelectedNpcId = 0;

        CharactorConfigTable playerConfigTable = ConfigManager.Instance.GetCharactorConfigTable();
        if (null == playerConfigTable)
        {
            EditorUtility.DisplayDialog("", "playerConfigTable 配置文件读取失败", "ok");

            return;
        }
        m_PlayerTypeList = new string[playerConfigTable.CharactorCofigMap.Count];
        tmpIndex = 0;
        foreach (var elem in playerConfigTable.CharactorCofigMap)
        {
            m_PlayerTypeList[tmpIndex++] = elem.Key.ToString();
        }
        TerrainEditorRuntime.Instance.SetSelectCallBack(OnClickCallBack);
        Debug.Log(" initialize terrain");
    }
    void OnGUI()
    {
        m_tmp = EditorGUILayout.BeginScrollView(m_tmp);
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
                var option = EditorUtility.DisplayDialog("警告", "重置面板将清除本地数据", "确定", "取消");
                if (option)
                {
                    ClearData();
                }
            }
            if (GUILayout.Button("同步地形方案", GUILayout.Width(150f)))
            {
                if (EditorUtility.DisplayDialog("警告", "本地在远程已存在方案将要被远程覆盖", "ok"))
                {
                    UpdateTerrainConfig();
                }
            }
            if (GUILayout.Button("合并地形方案", GUILayout.Width(150f)))
            {
                if (EditorUtility.DisplayDialog("警告", "远程在本地已存在方案将要被本地覆盖", "ok"))
                {
                    MergeTerrainConfig();
                }
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
                if (null != m_ObjMap && !FileUtils.IsEndof(m_ObjMap.name, "_map"))
                {
                    m_ObjMap = null;
                }
                CheckMap();
            }
            EditorGUILayout.EndHorizontal();

            //draw trigger
            DrawTrigger();

            //draw npc
            DrawNpc();

            //draw player 
            DrawCreatePlayerPos();
        }
        EditorGUILayout.EndScrollView();
    }
    public void OpenTerrain(TerrainEditorData data)
    {
        ClearData();

        m_bIsCreateNew = true;
        m_CurrentEditiongMapId = data.ID;
        m_CurrentEditiongMapName = data.MapName;
        m_MapNameInputBuffer = m_CurrentEditiongMapId.ToString();

        m_ObjMap = ResourceManager.Instance.LoadBuildInResource<GameObject>(data.MapResName, AssetType.Map);

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
        for (int i = 0; null != data.NpcDataList && i < data.NpcDataList.Count; ++i)
        {
            TerrainNpcData elem = data.NpcDataList[i];

            AddNpc(elem.Id, elem.Pos.GetVector3(), elem.Rot.GetVector3(), elem.Scale.GetVector3());
        }

        PlayerInitPosData pos = data.PlayerInitPos;
        if (null != pos)
        {
            CreatePlayerCharactor(pos.Id, pos.Pos.GetVector3(), pos.Rot.GetVector3());
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
                    AddTrigger(m_ETriggerAreaType, Vector3.zero, Vector3.zero, Vector3.one);
                }
            }

            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < m_TriggerList.Count; ++i)
            {
                TriggerEditorData data = m_TriggerList[i];

                GUIStyle textColor = new GUIStyle();
                textColor.normal.textColor = data.m_bIsSelected ? Color.red:Color.white;

                if (NGUIEditorTools.DrawHeader(m_TriggerList[i].TriggerData.AreaType.ToString()))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("调整触发器位置", textColor,GUILayout.Width(120f)))
                        {
                            m_nCurrentSettingPosTriggerIndex = i;
                            TerrainEditorRuntime.Instance.SetRaycastCallBack(SetTriggerPos);
                        }
                        if (GUILayout.Button("删除触发器", textColor, GUILayout.Width(120f)))
                        {
                            m_RemovingTriggerList.Add(m_TriggerList[i]);
                        }
                        if (GUILayout.Button("编辑触发器事件", textColor, GUILayout.Width(120f)))
                        {
                            TerrainTriggerNodeEditorWindow.Instance.OpenWindow(data.TriggerData);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        Vector3 tmpPos = data.m_SourceObj.transform.position;
                        GUILayout.TextArea("触发器位置", textColor, GUILayout.Width(120f));
                        GUILayout.Label(tmpPos.ToString(), GUILayout.Width(100f));

                        GUILayout.Label("x", GUILayout.Width(20f));
                        tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
                        GUILayout.Label("y", GUILayout.Width(20f));
                        tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
                        GUILayout.Label("z", GUILayout.Width(20f));
                        tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

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
            Vector3 scale = data.m_SourceObj.transform.localScale;
            float realRange = scale.x;

            GUILayout.TextArea("触发器大小", GUILayout.Width(120f));

            realRange = EditorGUILayout.Slider(realRange, 0f, 5f);
            scale = new Vector3(realRange, realRange, realRange);
            data.m_SourceObj.transform.localScale = scale;
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

                GUIStyle textColor = new GUIStyle();
                textColor.normal.textColor = data.m_bIsSelected ? Color.red : Color.white;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.TextField("NPCID: " + data.m_NpcData.Id, GUILayout.Width(300f));

                    if (GUILayout.Button("调整NPC位置", textColor,GUILayout.Width(120f)))
                    {
                        m_nCurrentSettingPosNpcIndex = i;
                        TerrainEditorRuntime.Instance.SetRaycastCallBack(SetNpcPos);
                    }
                    if (GUILayout.Button("NPC高亮", textColor, GUILayout.Width(120f)))
                    {
                        if (null != m_CurrentSelectedNpc)
                        {
                            ((CharTransformData) (m_CurrentSelectedNpc.GetTransformData())).SetSelectedStatus(false);
                        }
                        m_CurrentSelectedNpc = data.m_Instance;
                        ((CharTransformData)(m_CurrentSelectedNpc.GetTransformData())).SetSelectedStatus(true);
                    }
                    if (GUILayout.Button("删除NPC", textColor, GUILayout.Width(120f)))
                    {
                        m_RemovingNpcList.Add(data);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    Vector3 tmpPos = data.m_Instance.GetTransformData().GetPosition();
                    GUILayout.TextArea("NPC位置", textColor, GUILayout.Width(120f));
                    GUILayout.Label(tmpPos.ToString(), GUILayout.Width(100f));

                    GUILayout.Label("x", GUILayout.Width(20f));
                    tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
                    GUILayout.Label("z", GUILayout.Width(20f));
                    tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

                    if (m_LastTimePos != tmpPos)
                    {
                        data.m_Instance.GetTransformData().SetPosition(tmpPos);
                        data.m_NpcData.Pos.SetVector3(tmpPos);
                    }

                    m_LastTimePos = tmpPos;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    Vector3 tmpRot = data.m_Instance.GetTransformData().GetRotation();
                    GUILayout.TextArea("NPC朝向", GUILayout.Width(120f));
                    GUILayout.Label(tmpRot.y.ToString(), GUILayout.Width(100f));
                    GUILayout.Label("y", GUILayout.Width(20f)); 
                    tmpRot.y = EditorGUILayout.Slider(tmpRot.y, 0f, 359.9f);

                    if (data.m_Instance.GetTransformData().GetRotation() != tmpRot)
                    {
                        data.m_Instance.GetTransformData().SetRotation(tmpRot);
                        data.m_NpcData.Rot.SetVector3(tmpRot);
                    }

                }
                EditorGUILayout.EndHorizontal();
            }
            CheckRemovingNpc();
        }
    }
    void DrawCreatePlayerPos()
    {
        GUILayout.Space(5f);
        if (NGUIEditorTools.DrawHeader("角色出生点配置"))
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                m_iSelectedPlayerId = EditorGUILayout.Popup(m_iSelectedPlayerId, m_PlayerTypeList, GUILayout.Width(100f));
                if (GUILayout.Button("生成角色", GUILayout.Width(100f)))
                {
                    CreatePlayerCharactor(int.Parse(m_PlayerTypeList[m_iSelectedPlayerId]),Vector3.zero,Vector3.zero);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (null != m_PlayerChar)
            {
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("调整玩家位置", GUILayout.Width(120f)))
                {
                    TerrainEditorRuntime.Instance.SetRaycastCallBack(SetPlayerPos);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpPos = m_PlayerChar.GetTransformData().GetPosition();
                    EditorGUILayout.LabelField("玩家位置:", GUILayout.Width(80f));

                    GUILayout.Label("x", GUILayout.Width(20f));
                    tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
                    GUILayout.Label("z", GUILayout.Width(20f));
                    tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

                    if (m_PlayerChar.GetTransformData().GetPosition() != tmpPos)
                    {
                        m_PlayerChar.GetTransformData().SetPosition(tmpPos);
                    }

                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpRot = m_PlayerChar.GetTransformData().GetRotation();
                    EditorGUILayout.LabelField("玩家朝向:", GUILayout.Width(80f));

                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpRot.y = EditorGUILayout.Slider(tmpRot.y, 0f, 359.9f);

                    if (m_PlayerChar.GetTransformData().GetRotation() != tmpRot)
                    {
                        m_PlayerChar.GetTransformData().SetRotation(tmpRot);
                    }

                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    #endregion

    #region data handler
    private void AddTrigger(ETriggerAreaType TriggerAreaType, Vector3 position, Vector3 rotation, Vector3 scale, int targetId = 0, int enterLimitId = 0, int exitLimitId = 0, int enterFuncId = 0, int exitFuncId = 0)
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
        if (null != m_ObjMap)
        {
            if (null == m_ObjMapInstance)
            {
                // reset map
                m_ObjMapInstance = GameObject.Instantiate(m_ObjMap);
                ComponentTool.Attach(m_ObjSceneRoot.transform, m_ObjMapInstance.transform);
                //record map name
                m_CurrentMapResPath = m_ObjMap.name;

                TerrainEditorRuntime.Instance.SetSceneCamera(m_ObjMapInstance);
                TerrainEditorRuntime.Instance.SetClearWindow(ClearData);
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
            for (int i = 0; i < m_TriggerList.Count; ++i)
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
                m_NpcList[i].m_Instance.Distructor();
               
            }
            m_NpcList.Clear();
        }
        if (null != m_RemovingNpcList)
        {
            m_RemovingNpcList.Clear();
        }
        m_ObjMapInstance = null;
        //clear player
        if (null != m_PlayerChar)
        {
            m_PlayerChar.Distructor();
            m_PlayerChar = null;
        }
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
        m_NpcList[m_nCurrentSettingPosNpcIndex].m_Instance.GetTransformData().SetPosition(positin);
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

        if (IsExistNpcInScene(id))
        {
            EditorUtility.DisplayDialog("npc重复","场景中已经存在相同ID的npc，错误 id= " + id, "ok");
            return;
        }

        NpcEditorData elem = new NpcEditorData();
        elem.m_NpcData = new TerrainNpcData();
        elem.m_NpcData.Pos = new ThriftVector3();
        elem.m_NpcData.Rot = new ThriftVector3();
        elem.m_NpcData.Scale = new ThriftVector3();
        elem.m_NpcData.Id = id;

        elem.m_NpcData.Pos.SetVector3(position);
        elem.m_NpcData.Rot.SetVector3(rotation);
        elem.m_NpcData.Scale.SetVector3(scale);

        elem.m_Instance = new Npc();
        elem.m_Instance.Initialize(id);
        GameObject instance = ((CharTransformData)(elem.m_Instance.GetTransformData())).GetGameObject();
        ComponentTool.Attach(m_ObjNpcRoot.transform, instance.transform);
        ((CharTransformData)(elem.m_Instance.GetTransformData())).SetPhysicStatus(false);
        elem.m_Instance.GetTransformData().SetPosition(position);
        elem.m_Instance.GetTransformData().SetRotation(rotation);
        elem.m_Instance.GetTransformData().SetScale(scale);

        
        m_NpcList.Add(elem);
    }
    private bool IsExistNpcInScene(int id)
    {
        for (int i = 0; i < m_NpcList.Count; ++i)
        {
            if (m_NpcList[i].m_NpcData.Id == id)
            {
                return true;
            }
        }
        return false;
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
                m_RemovingNpcList[i].m_Instance.Distructor();
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
    private void CreatePlayerCharactor(int id,Vector3 pos,Vector3 rot)
    {
        if (null != m_PlayerChar)
        {
            //
            EditorUtility.DisplayDialog("错误", "只能有一个玩家在场景中", "ok");
            return;
        }
        CharactorConfig tmpConfig = ConfigManager.Instance.GetCharactorConfig(id);
        if (null == tmpConfig)
        {
            EditorUtility.DisplayDialog("id 错误", "请检查表中char id ，错误 id= " + id, "ok");
            return;
        }

        GameObject sourceObj = ResourceManager.Instance.LoadBuildInResource<GameObject>(tmpConfig.ModelResource,
            AssetType.Char);
        if (null == sourceObj)
        {
            EditorUtility.DisplayDialog("模型 id 错误", "请检查表中char id ，错误 id= " + tmpConfig.ModelResource, "ok");
            return;
        }

        var player = PlayerCharacter.Create(id);
        m_PlayerChar = player;
        ((CharTransformData) (m_PlayerChar.GetTransformData())).SetPhysicStatus(false);
        m_PlayerChar.GetTransformData().SetPosition(pos);
        m_PlayerChar.GetTransformData().SetRotation(rot);
    }
    private void SetPlayerPos(Vector3 positin)
    {
        m_PlayerChar.GetTransformData().SetPosition(positin);
    }
    private void SaveData()
    {
        if (null == m_TriggerList)
        {
            EditorUtility.DisplayDialog("保存失败","!!!", "ok");
            return;
        }
        if (string.IsNullOrEmpty(m_CurrentEditiongMapName) || string.IsNullOrEmpty(m_CurrentMapResPath) || m_CurrentEditiongMapId <= 0)
        {
            EditorUtility.DisplayDialog("保存失败", "地图信息不完整", "ok");
            return;
        }
        if (null == m_PlayerChar)
        {
            EditorUtility.DisplayDialog("保存失败","必须编辑玩家的出生位置", "ok");
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

        m_OutputData.NpcDataList = new System.Collections.Generic.List<TerrainNpcData>(m_NpcList.Count);
        for (int i = 0; i < m_NpcList.Count; ++i)
        {
            m_OutputData.NpcDataList.Add(m_NpcList[i].m_NpcData);
        }
        m_OutputData.PlayerInitPos = new PlayerInitPosData();
        m_OutputData.PlayerInitPos.Pos = new ThriftVector3();
        m_OutputData.PlayerInitPos.Rot = new ThriftVector3();
        m_OutputData.PlayerInitPos.Id = m_PlayerChar.GetInstanceId();
        m_OutputData.PlayerInitPos.Pos.SetVector3(m_PlayerChar.GetTransformData().GetPosition());
        m_OutputData.PlayerInitPos.Rot.SetVector3(m_PlayerChar.GetTransformData().GetRotation());
        m_DataList = GetTerrainEditFileList();

        SaveTerrainEditFileList(m_DataList, m_OutputData);

    }
    public void SaveTerrainEditFileList(TerrainEditorDataArray filedata, TerrainEditorData editorData)
    {
        if (null == filedata)
        {
            filedata = new TerrainEditorDataArray();
            filedata.DataList = new System.Collections.Generic.List<TerrainEditorData>();
        }

        if (null != editorData)
        {
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
        }

        byte[] data = ThriftSerialize.Serialize(filedata);
        FileUtils.WriteByteFile(m_strDataPath, data);

    }
    public TerrainEditorDataArray GetTerrainEditFileList()
    {
        ResourceManager.DecodePersonalDataTemplate(m_strDataPath, ref m_DataList);

        return m_DataList;
    }
    private void UpdateTerrainConfig()
    {
        TerrainEditorDataArray newList = new TerrainEditorDataArray();
        newList.DataList = new List<TerrainEditorData>();

        TerrainEditorDataArray remoteConfig = ConfigManager.Instance.GetTerrainEditorDataArray();
        TerrainEditorDataArray localConfig = GetTerrainEditFileList();

        if (null == remoteConfig || remoteConfig.DataList == null || remoteConfig.DataList.Count == 0)
        {
            return;
        }

        if (remoteConfig != null || remoteConfig.DataList != null)
        {
            foreach (var elemRemote in remoteConfig.DataList)
            {
                //add remote 
                newList.DataList.Add(elemRemote);
            }
        }

        if (null != localConfig && null != localConfig.DataList)
        {
            foreach (var elemLocal in localConfig.DataList)
            {
                bool isExistInRemote = false;
                foreach (var elemRemote in remoteConfig.DataList)
                {
                    if (elemLocal.ID == elemRemote.ID)
                    {
                        isExistInRemote = true;
                        break;
                    }
                }
                if (!isExistInRemote)
                {
                    //add local 
                    newList.DataList.Add(elemLocal);
                }
            }
        }

        byte[] data = ThriftSerialize.Serialize(newList);
        FileUtils.WriteByteFile(m_strDataPath, data);
    }
    private void MergeTerrainConfig()
    {
        TerrainEditorDataArray newList = new TerrainEditorDataArray();
        newList.DataList = new List<TerrainEditorData>();

        TerrainEditorDataArray remoteConfig = ConfigManager.Instance.GetTerrainEditorDataArray();
        TerrainEditorDataArray localConfig = GetTerrainEditFileList();

        if (null == remoteConfig || remoteConfig.DataList == null || remoteConfig.DataList.Count == 0)
        {
            return;
        }

        foreach (var elemLocal in localConfig.DataList)
        {
            //add loacl 
            newList.DataList.Add(elemLocal);
        }

        foreach (var elemRemote in remoteConfig.DataList)
        {
            bool isExistInLocal = false;
            foreach (var elemLocal in localConfig.DataList)
            {
                if (elemLocal.ID == elemRemote.ID)
                {
                    isExistInLocal = true;
                    break;
                }
            }
            if (!isExistInLocal)
            {
                //add local 
                newList.DataList.Add(elemRemote);
            }
        }

        byte[] data = ThriftSerialize.Serialize(newList);
        FileUtils.WriteByteFile(m_strDataPath, data);
    }
    private void OnClickCallBack(Transform obj)
    {
        foreach (var elem in m_TriggerList)
        {
            if (elem.m_SourceObj == obj.gameObject)
            {
                elem.m_bIsSelected = true;
            }
            else
            {
                elem.m_bIsSelected = false;
            }
        }
        foreach (var elem in m_NpcList)
        {
            GameObject npcobj = ((CharTransformData) (elem.m_Instance.GetTransformData())).GetGameObject();
            if (npcobj == obj.gameObject)
            {
                elem.m_bIsSelected = true;
            }
            else
            {
                elem.m_bIsSelected = false;
            }
        }
        EditorWindow.FocusWindowIfItsOpen(typeof(TerrainEditorWindow));
    }
    #endregion
}
