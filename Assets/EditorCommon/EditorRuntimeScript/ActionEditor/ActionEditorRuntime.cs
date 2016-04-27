//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// NameSpace : Assets.Script.Framework.EditorRuntimeScript.ActionEditor
// FileName : ActionEditorMain
//
// Created by : LeoLi (742412055@qq.com) at 2015/10/29 16:49:30
//
//
//========================================================================

using Cache;
using Moudles.BaseMoudle.Converter;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters;
using System.Collections;

public class ActionEditorRuntime : SingletonTemplateMon<ActionEditorRuntime>
{
    //public float X_offset = 15f;
    //public float Y_offset = 300f;
    public Text CameraDistanceUI;

    //readonly
    private readonly string SCENE_CAMERANAME = "MainCamera";
    //
    private GameObject m_ObjMapInstance;
    private Camera m_SceneCamera;
    private Action m_ClearEditorWindowCallBack;
    private Action m_CloseEditorWindowCallBack;
    private Action<Vector3> m_RaycastCallBack;

    #region MonoBehavior
    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        TimeManager.Instance.Initialize();
        CacheManager.Init(Application.persistentDataPath + "/Cache");
        LogManager.Instance.Initialize(true, true);
        ResourceManager.Instance.Initialize();
        TickTaskManager.Instance.InitializeTickTaskSystem();
        ConverterManager.Instance.Initialize();
        WindowManager.Instance.Initialize();
        FuncMethodDef.InitFuncMethod();
        LimitMethodDef.InitLimitMethod();
        TargetMethodDef.InitTargetMethod();
        
        AssetUpdateManager.Instance.CheckUpdate(() =>
        {
            _instance = this;
        },false);
    }

    // Update is called once per frame
    void Update()
    {
        TickTaskManager.Instance.Update();

        if (m_ObjMapInstance != null && m_SceneCamera != null)
        {
            PlayerCharacter player = PlayerManager.Instance.GetPlayerInstance();
            if (player != null)
            {
                Vector3 playerPos = player.GetTransformData().GetPosition();
                CameraDistanceUI.text = Vector3.Magnitude(m_SceneCamera.transform.position - playerPos).ToString("f2");
            }
        }
        else
        {
            CameraDistanceUI.text = "null";
        }

        if (null != m_SceneCamera)
        {
            HandlerSceneCamera();
        }
    }
    #endregion

    #region Public Interface
    public void SetSceneCamera(GameObject instance)
    {
        m_ObjMapInstance = instance;
        m_SceneCamera = m_ObjMapInstance.GetComponentInChildren<Camera>();
        if (m_SceneCamera == null) {
            Debuger.LogError("Scene Camera Not Found");
            return;
        }
        if (EditorCameraMovement.Instance != null && m_SceneCamera.gameObject.name == SCENE_CAMERANAME)
        {
            EditorCameraMovement.Instance.SetSceneCamera(m_SceneCamera);
        }
    }

    public void SetClearWindow(Action clear)
    {
        m_ClearEditorWindowCallBack = clear;
    }
    public void SetCloseWindow(Action closeWindow)
    {
        m_CloseEditorWindowCallBack = closeWindow;
    }

    public void SetRaycastCallBack(Action<Vector3> onRaycastCallback)
    {
        m_RaycastCallBack = onRaycastCallback;
    }
    public void SetCameraDistanceUI (float distance)
    {
        CameraDistanceUI.text = distance.ToString("f2");
    }
    #endregion

    #region System Function
    private void OnApplicationQuit()
    {
        if (null != m_ClearEditorWindowCallBack)
        {
            m_ClearEditorWindowCallBack();
        }
        if (null != m_CloseEditorWindowCallBack)
        {
            m_CloseEditorWindowCallBack();
        }
    }
    private void HandlerSceneCamera()
    {
        if (null != m_RaycastCallBack && Input.GetMouseButtonDown(0))
        {
            Ray ray = m_SceneCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Terrain")))
            {
                m_RaycastCallBack(hitInfo.point);
                m_RaycastCallBack = null;
            }
        }
    }
    #endregion
}
