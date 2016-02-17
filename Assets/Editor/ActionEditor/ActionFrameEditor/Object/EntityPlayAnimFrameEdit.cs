using System.IO.Pipes;
using System.Linq.Expressions;
using ActionEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


class EntityPlayAnimFrameEdit : AbstractFrameEdit
{
    static public EntityPlayAnimFrameEdit Instance
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

    private string[] m_TargetTypePopList = new[] { "摄像机", "npc", "玩家", };

    //readonly
    private float WINDOW_MIN_WIDTH = 650f;
    private float WINDOW_MIN_HIEGHT = 300f;

    private static EntityPlayAnimFrameEdit m_Instance;
    private string m_strResourceName;
    private EntityPlayAnimationConfig m_Config;


    private void OnGUI()
    {
        DrawBaseInfo();
        GUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("动画文件:", GUILayout.Width(150f));
        m_strResourceName = GUILayout.TextField(m_strResourceName);
        m_Config.EntityType =
            (EntityType) (EditorGUILayout.Popup((int) m_Config.EntityType, m_TargetTypePopList, GUILayout.Width(100f)));

        if (m_Config.EntityType == EntityType.Npc)
        {
            m_Config.CharId = EditorGUILayout.IntField(m_Config.CharId);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5f);
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
        m_Instance = EditorWindow.GetWindow<EntityPlayAnimFrameEdit>(false, "物体动画编辑", true);
    }
    protected override void OnPlay()
    {
        GameObject obj = null;
        switch (m_Config.EntityType)
        {
            case EntityType.Camera:
                obj = GlobalScripts.Instance.mGameCamera.transform.parent.gameObject;
                break;
            case EntityType.Npc:
                Ilife npc = LifeManager.GetLife(m_Config.CharId);
                if (null == npc || (!(npc is Npc)))
                {
                    Debuger.LogError("EntityPlayanim : can't load npc by id " + m_Config.CharId);
                    return;
                }
                obj = ((CharTransformData) ((Npc) (npc)).GetTransformData()).GetGameObject();
                break;
            case EntityType.Player:
                if (null == PlayerManager.Instance.GetPlayerInstance())
                {
                    Debuger.LogError("EntityPlayanim : can't load npc by id " + m_Config.CharId);
                    return;
                }
                obj =
                    ((CharTransformData) (PlayerManager.Instance.GetPlayerInstance().GetTransformData())).GetGameObject();
                break;
        }
        Animation desAnim = obj.GetComponent<Animation>();
        if (null == desAnim)
        {
            desAnim = obj.AddComponent<Animation>();
        }
        GameObject animsource = ResourceManager.Instance.LoadBuildInResource<GameObject>("AnimationStore", AssetType.Animation);
        if (null == animsource)
        {
            Debuger.LogError("EntityPlayanim : can't load animation source store");
            return;
        }
        Animation sourceAnim = animsource.GetComponent<Animation>();
        if (null == sourceAnim)
        {
            Debuger.LogError("EntityPlayanim : can't load animation source store");
            return;
        }
        AnimationClip clip = sourceAnim.GetClip(m_Config.AnimName);
        if (null == clip)
        {
            Debuger.LogError("EntityPlayanim : can't load target clip on source anim store");
            return;
        }
        desAnim.clip = clip;
        desAnim.Play();
    }
    protected override void OnSave()
    {
        //Save Data
        m_Config.AnimName = m_strResourceName;

        m_ActionFrameData.EntityPlayAnim = m_Config;
        ActionEditorWindow.Instance.SaveData(m_ActionFrameData);

        //Close Window
        m_Instance.Close();
    }
    private void Init()
    {
        m_Instance.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);

        //Update Info
        if (null != m_ActionFrameData)
        {
            m_fTime = (float)m_ActionFrameData.Time;
            m_Config = m_ActionFrameData.EntityPlayAnim;
            m_strResourceName = m_Config.AnimName;
        }
        else
        {
            m_ActionFrameData = new ActionFrameData();
            m_Config = new EntityPlayAnimationConfig();
            m_strResourceName = string.Empty;
        }
    }
}