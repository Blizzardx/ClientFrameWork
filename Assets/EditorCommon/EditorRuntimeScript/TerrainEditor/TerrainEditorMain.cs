using System;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System.Collections;

public class TerrainEditorMain : SingletonTemplateMon<TerrainEditorMain>
{
    private GameObject m_ObjSceneCamera;
    private Action m_ClearEditorWindowCallBack;
    private Action m_CloseEditorWindowCallBack;
    private Action<Vector3> m_RaycastCallBack;
    private Camera m_SceneCamera;


    #region MonoBehavior
    void Awake()
    {
        _instance = this;
    }
    // Use this for initialization
    void Start()
    {
        ResourceManager.Instance.Initialize();
        LogManager.Instance.Initialize(true, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (null != m_ObjSceneCamera)
        {
            HandlerSceneCamera();
        }
    }
    #endregion

    public void SetSceneCamera(GameObject sceneCamera)
    {
        m_ObjSceneCamera = sceneCamera;
        m_SceneCamera = m_ObjSceneCamera.GetComponent<Camera>();
        if (EditorCameraMovement.Instance != null) { 
            EditorCameraMovement.Instance.SetSceneCamera(m_SceneCamera);
        }
    }
    public void SetRaycastCallBack(Action<Vector3> onRaycastCallback)
    {
        m_RaycastCallBack = onRaycastCallback;
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
        if (null != m_RaycastCallBack && Input.GetMouseButtonDown(0))
        {
            Ray ray = m_SceneCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo ;
            if (Physics.Raycast(ray, out hitInfo,100.0f))
            {
                m_RaycastCallBack(hitInfo.point);
                m_RaycastCallBack = null;
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
