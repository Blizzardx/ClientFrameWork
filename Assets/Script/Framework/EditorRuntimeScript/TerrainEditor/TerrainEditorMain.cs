using System;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System.Collections;

public class TerrainEditorMain : SingletonTemplateMon<TerrainEditorMain>
{
    private GameObject m_ObjSceneCamera;
    public float m_fSpeed = 0.1f;
    public float m_fRotate = 0.1f;
    private Vector3 m_vDelta;
    private Vector3 m_vTmp;
    private Vector3 m_vInitPos;
    private Vector3 m_vCamInitRotate;
    private Action m_ClearEditorWindowCallBack;
    private Action m_CloseEditorWindowCallBack;
    private Action<Vector3> m_RaycastCallBack;
    private Camera m_SceneCamera;

    void Awake()
    {
        _instance = this;
    }

    public void SetSceneCamera(GameObject sceneCamera)
    {
        m_ObjSceneCamera = sceneCamera;
        m_SceneCamera = m_ObjSceneCamera.GetComponent<Camera>();
    }

    public void SetRaycastCallBack(Action<Vector3> onRaycastCallback)
    {
        m_RaycastCallBack = onRaycastCallback;
    }
    public void SetClearWindow(Action clear)
    {
        m_ClearEditorWindowCallBack = clear;
    }
	// Use this for initialization
	void Start () 
    {
	    ResourceManager.Instance.Initialize();
	    LogManager.Instance.Initialize(true, true);
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (null != m_ObjSceneCamera)
	    {
	        HandlerSceneCamera();
	    }
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
        if (Input.GetMouseButtonDown(1))
        {
            m_vInitPos = Input.mousePosition;
            m_vCamInitRotate = m_ObjSceneCamera.transform.eulerAngles;
        }
        if (Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.W))
            {
                m_ObjSceneCamera.transform.position += m_fSpeed*m_ObjSceneCamera.transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                m_ObjSceneCamera.transform.position -= m_fSpeed * m_ObjSceneCamera.transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_ObjSceneCamera.transform.position -= m_fSpeed * m_ObjSceneCamera.transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                m_ObjSceneCamera.transform.position += m_fSpeed * m_ObjSceneCamera.transform.right;
            }

            m_vDelta = Input.mousePosition - m_vInitPos;
            m_vTmp.x = -m_vDelta.y;
            m_vTmp.y = m_vDelta.x;
            m_vTmp *= m_fRotate;
            m_vTmp.z = m_ObjSceneCamera.transform.eulerAngles.z;
            m_vCamInitRotate.z = 0.0F;
            m_ObjSceneCamera.transform.eulerAngles = m_vCamInitRotate + m_vTmp;
        }
        if (Input.GetMouseButtonUp(1))
        {
            
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

    public void SetCloseWindow(Action closeWindow)
    {
        m_CloseEditorWindowCallBack = closeWindow;
    }
}
