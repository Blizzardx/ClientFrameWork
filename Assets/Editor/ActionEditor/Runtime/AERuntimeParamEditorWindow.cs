using System;
using System.Collections.Generic;
using Config;
using Config.Table;
using NetWork.Auto;
using UnityEditor;
using UnityEngine;

public class AERuntimeParamEditorWindow : EditorWindow
{
    private static AERuntimeParamEditorWindow m_MainWnd ;

    private int                 m_iSelectedNpcId;
    private int                 m_iSelectedPlayerId;
    private GameObject          m_ObjNpcRoot;
    private List<Npc>           m_CreatedNpcList;
    private string[]            m_NpcTypeList;
    private string[]            m_PlayerTypeList;
    private int                 m_CurrentSettingPosIndex;
    private AERuntimeParam      m_Param;

    public static void OpenWndow(AERuntimeParam param)
    {
        m_MainWnd = EditorWindow.GetWindow<AERuntimeParamEditorWindow>(false, "运行时参数编辑器", true);
        m_MainWnd.m_Param = param;
        m_MainWnd.Init();
    }
    public static void CloseWindow()
    {
        if (null == m_MainWnd)
        {
            return;
        }
        m_MainWnd.Close();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("NPC位置:", GUILayout.Width(80f));
        }
        EditorGUILayout.EndHorizontal();

        DrawNpc();

        DrawPlayer();
    }
    void DrawNpc()
    {
        GUILayout.Space(5f);
        if (NGUIEditorTools.DrawHeader("NPC配置"))
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                m_iSelectedNpcId = EditorGUILayout.Popup(m_iSelectedNpcId, m_NpcTypeList, GUILayout.Width(100f));
                if (GUILayout.Button("生成NPC", GUILayout.Width(100f)))
                {
                    AddNpc(int.Parse(m_NpcTypeList[m_iSelectedNpcId]));
                }
            }
            EditorGUILayout.EndHorizontal();

            for(int i=0;i<m_CreatedNpcList.Count;++i)
            {
                var instance = m_CreatedNpcList[i];

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("调整NPC位置", GUILayout.Width(120f)))
                {
                    m_CurrentSettingPosIndex = i;
                    ActionEditorRuntime.Instance.SetRaycastCallBack(SetNpcPos);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpPos = instance.GetTransformData().GetPosition();
                    EditorGUILayout.LabelField("NPC位置:", GUILayout.Width(80f));

                    GUILayout.Label("x", GUILayout.Width(20f));
                    tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
                    GUILayout.Label("z", GUILayout.Width(20f));
                    tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

                    if (instance.GetTransformData().GetPosition() != tmpPos)
                    {
                        instance.GetTransformData().SetPosition(tmpPos);
                    }

                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpRot = instance.GetTransformData().GetRotation();
                    EditorGUILayout.LabelField("NPC朝向:", GUILayout.Width(80f));

                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpRot.y = EditorGUILayout.Slider(tmpRot.y, 0f, 359.9f);


                    if (instance.GetTransformData().GetRotation() != tmpRot)
                    {
                        instance.GetTransformData().SetRotation(tmpRot);
                    }

                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    private void DrawPlayer()
    {
        GUILayout.Space(5f);
        if (NGUIEditorTools.DrawHeader("玩家角色配置"))
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                m_iSelectedPlayerId = EditorGUILayout.Popup(m_iSelectedPlayerId, m_PlayerTypeList, GUILayout.Width(100f));
                if (GUILayout.Button("生成角色", GUILayout.Width(100f)))
                {
                    CreatePlayerCharactor(int.Parse(m_PlayerTypeList[m_iSelectedPlayerId]));
                }
                if (m_CreatedNpcList != null)
                {

                }
            }
            EditorGUILayout.EndHorizontal();

            if( null != PlayerManager.Instance.GetPlayerInstance())
            {
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("调整玩家位置", GUILayout.Width(120f)))
                {
                    ActionEditorRuntime.Instance.SetRaycastCallBack(SetPlayerPos);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpPos = PlayerManager.Instance.GetPlayerInstance().GetTransformData().GetPosition();
                    EditorGUILayout.LabelField("玩家位置:", GUILayout.Width(80f));

                    GUILayout.Label("x", GUILayout.Width(20f));
                    tmpPos.x = EditorGUILayout.FloatField(tmpPos.x);
                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpPos.y = EditorGUILayout.FloatField(tmpPos.y);
                    GUILayout.Label("z", GUILayout.Width(20f));
                    tmpPos.z = EditorGUILayout.FloatField(tmpPos.z);

                    if (PlayerManager.Instance.GetPlayerInstance().GetTransformData().GetPosition() != tmpPos)
                    {
                        PlayerManager.Instance.GetPlayerInstance().GetTransformData().SetPosition(tmpPos);
                    }

                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5f);
                    Vector3 tmpRot = PlayerManager.Instance.GetPlayerInstance().GetTransformData().GetRotation();
                    EditorGUILayout.LabelField("玩家朝向:", GUILayout.Width(80f));

                    GUILayout.Label("y", GUILayout.Width(20f));
                    tmpRot.y = EditorGUILayout.Slider(tmpRot.y, 0f, 359.9f);

                    if (PlayerManager.Instance.GetPlayerInstance().GetTransformData().GetRotation() != tmpRot)
                    {
                        PlayerManager.Instance.GetPlayerInstance().GetTransformData().SetRotation(tmpRot);
                    }

                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    private void Init()
    {
        NpcConfigTable npcConfigTable = ConfigManager.Instance.GetNpcTable();
        if (null == npcConfigTable)
        {
            EditorUtility.DisplayDialog("", "npc配置文件读取失败", "ok");
           
            return;
        }
        m_NpcTypeList = new string[npcConfigTable.NpcCofigMap.Count];
        int tmpIndex = 0;
        foreach (var elem in npcConfigTable.NpcCofigMap)
        {
            m_NpcTypeList[tmpIndex++] = elem.Key.ToString();
        }

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

        m_ObjNpcRoot = GameObject.Find("NpcRoot");
        m_CreatedNpcList = m_Param.GetNpcList();
    }
    private void CreatePlayerCharactor(int id)
    {
        if (null != PlayerManager.Instance.GetPlayerInstance())
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

        PlayerManager.Instance.CreatePlayerChar();
        m_Param.SetPlayer(PlayerManager.Instance.GetPlayerInstance());
    }
    private void AddNpc(int id)
    {
        if (!CheckCanCreateNpc(id))
        {
            EditorUtility.DisplayDialog("id 错误", "场景中已存在该id的npc ，id= " + id, "ok");
            return;
        }

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

        Npc elem = new Npc();
        elem.Initialize(id);

        m_CreatedNpcList.Add(elem);

    }
    private void SetNpcPos(Vector3 positin)
    {
        m_CreatedNpcList[m_CurrentSettingPosIndex].GetTransformData().SetPosition(positin);
    }
    private void SetPlayerPos(Vector3 positin)
    {
        PlayerManager.Instance.GetPlayerInstance().GetTransformData().SetPosition(positin);
    }
    private bool CheckCanCreateNpc(int id)
    {
        for (int i = 0; i < m_CreatedNpcList.Count; ++i)
        {
            if (m_CreatedNpcList[i].GetInstanceId() == id)
            {
                return false;
            }
        }
        return true;
    }
    
}

