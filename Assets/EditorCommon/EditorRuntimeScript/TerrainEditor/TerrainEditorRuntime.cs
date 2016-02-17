using System;
using System.Runtime.Serialization.Formatters;
using Cache;
using Moudles.BaseMoudle.Converter;
using UnityEngine;
using System.Collections;

public class TerrainEditorRuntime : SingletonTemplateMon<TerrainEditorRuntime>
{
    private GameObject m_ObjSceneCamera;
    private Action m_ClearEditorWindowCallBack;
    private Action m_CloseEditorWindowCallBack;
    private Action<Vector3> m_RaycastCallBack;
    private Action<Transform> m_SelectCallBack;
    private Camera m_SceneCamera;


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
        MessageManager.Instance.Initialize();
        ConverterManager.Instance.Initialize();
        AssetUpdateManager.Instance.CheckUpdate(() =>
        {
            _instance = this;
        }, false);
    }

    // Update is called once per frame
    void Update()
    {
        TickTaskManager.Instance.Update();
        if (null != m_SceneCamera)
        {
            HandlerSceneCamera();
        }
    }
    #endregion

    public void SetSceneCamera(GameObject sceneCamera)
    {
        m_SceneCamera = sceneCamera.GetComponentInChildren<Camera>();
        if (m_SceneCamera == null)
        {
            Debuger.LogError("Scene Camera Not Found");
            return;
        }

        if (EditorCameraMovement.Instance != null) { 
            EditorCameraMovement.Instance.SetSceneCamera(m_SceneCamera);
        }
    }
    public void SetRaycastCallBack(Action<Vector3> onRaycastCallback)
    {
        m_RaycastCallBack = onRaycastCallback;
    }

    public void SetSelectCallBack(Action<Transform> onSelected)
    {
        m_SelectCallBack = onSelected;
    }
    public void SetClearWindow(Action clear)
    {
        m_ClearEditorWindowCallBack = clear;
    }
    public void SetCloseWindow(Action closeWindow)
    {
        m_CloseEditorWindowCallBack = closeWindow;
    }
    private void HandlerSceneCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = m_SceneCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo ;
            if (Physics.Raycast(ray, out hitInfo,100.0f))
            {
                if (null != m_RaycastCallBack)
                {
                    m_RaycastCallBack(hitInfo.point);
                    m_RaycastCallBack = null;
                }
                if (null != m_SelectCallBack)
                {
                    m_SelectCallBack(hitInfo.transform);
                }
            }
        }
    }
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


}
