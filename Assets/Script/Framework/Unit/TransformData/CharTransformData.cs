//========================================================================
// Copyright(C): CYTX
//
// FileName : CharTransformData
// CLR Version : 4.0.30319.42000
// 
// Created by : LeoLi at 2015/11/25 14:52:21
//
// Purpose : 角色控制类
//========================================================================

using System;
using UnityEngine;
using System.Collections.Generic;

public class CharTransformData : TransformDataBase
{

    //Component
    protected int m_iInstanceId;
    protected Ilife m_LifeData;
    protected GameObject m_ObjectInstance;
    protected AnimatorAgent m_AnimatorAgent;
    protected CharTransformContainer m_CharContainer;

    //mesh render
    private Material m_NormalMaterial;
    private Material m_HighlightMaterial;

    //Nav State
    private bool m_bIsMoving;
    private Action m_OnFinishMoveCallBack;
    private SkinnedMeshRenderer m_MeshRender;
    private bool m_bIsSelected;

    //Obj Move
    private bool m_bIsObjMove = false;
    private float m_fInitTime;
    private float m_fTimeSpace;
    private Vector3 m_vInitPos;
    private Vector3 m_vMoveForword;

    //Rotate
    private bool m_bIsRotating = false;

    //Nav Data
    private List<Vector3> m_lstMovePath;
    private int m_iCurrentTargetIndex;
    private float m_fMaxSpeed;
    private float m_fStopDistance;
    private Vector3 m_Direction;

    #region public interface
    virtual public void Initialize(Ilife lifeData, string resourcePath, AssetType resourceType)
    {
        m_LifeData = lifeData;
        m_iInstanceId = lifeData.GetInstanceId();

        m_ObjectInstance = GameObject.Instantiate(ResourceManager.Instance.LoadBuildInResource<GameObject>(resourcePath, resourceType));

        //load material
        string localpath = resourcePath.Substring(0, resourcePath.LastIndexOf('/'));
        m_NormalMaterial = ResourceManager.Instance.LoadBuildInResource<Material>(localpath + "/Normal", AssetType.Char);
        m_HighlightMaterial = ResourceManager.Instance.LoadBuildInResource<Material>(localpath + "/SelectedHighlight", AssetType.Char);
        m_MeshRender = ComponentTool.FindChildComponent<SkinnedMeshRenderer>("Body", m_ObjectInstance);
        if (null == m_NormalMaterial || null == m_HighlightMaterial || null == m_MeshRender)
        {
            Debuger.LogWarning("can't load mesh render or normal&highlight materials !");
        }

        //mark transform
        m_CharContainer = m_ObjectInstance.AddComponent<CharTransformContainer>();
        m_CharContainer.Initialize(lifeData.GetInstanceId(), lifeData);

        if (null == m_ObjectInstance)
        {
            Debuger.LogError("Can't load resource " + resourcePath);
        }
        m_AnimatorAgent = new AnimatorAgent(m_ObjectInstance);
    }
    virtual public void Update()
    {
        // Object Move
        if (m_bIsObjMove)
        {
            ObjMoving();
        }
        // Animator Move
        else
        {
            if (m_bIsMoving)
            {
                Moving();
            }
            else if (m_bIsRotating)
            {
                FinishRotate();
            }
            else
            {
                m_AnimatorAgent.DoLocomotion(0, 0);
            }
        }
    }
    virtual public void Distructor()
    {
        GameObject.Destroy(m_ObjectInstance);
    }
    override public void SetPosition(Vector3 value)
    {
        m_vPos = value;
        m_ObjectInstance.transform.position = m_vPos;
    }
    override public void SetRotation(Vector3 value)
    {
        m_vRotation = value;
        m_ObjectInstance.transform.eulerAngles = m_vRotation;
    }
    override public void SetScale(Vector3 value)
    {
        m_vScale = value;
        m_ObjectInstance.transform.localScale = m_vScale;
    }
    public override Vector3 GetPosition()
    {
        m_vPos = m_ObjectInstance.transform.position;
        return m_vPos;
    }
    public override Vector3 GetRotation()
    {
        m_vRotation = m_ObjectInstance.transform.eulerAngles;
        return m_vRotation;
    }
    public override Vector3 GetScale()
    {
        m_vScale = m_ObjectInstance.transform.localScale;
        return m_vScale;
    }
    public GameObject GetGameObject()
    {
        return m_ObjectInstance;
    }
    virtual public void DirectPlayAnimation(string state, Action onFinishCallBack = null)
    {
        m_CharContainer.DirectPlayAnimation(state, onFinishCallBack);
    }
    virtual public void MoveTo(Vector3 targetPosition, float maxSpeed, float stopDistance, Action onFinishMoveCallMoveBack = null, bool objMove = false)
    {
        //m_lstMovePath = m_CharContainer.FindPath(targetPosition);
        m_CharContainer.FindPath(targetPosition, (result) =>
        {
            m_lstMovePath = result;
            m_fMaxSpeed = maxSpeed;
            m_fStopDistance = stopDistance;
            MoveTo(m_lstMovePath, maxSpeed, stopDistance, onFinishMoveCallMoveBack, objMove);
        });
        m_OnFinishMoveCallBack = onFinishMoveCallMoveBack;
    }
    virtual public void MoveTo(List<Vector3> path, float maxSpeed, float stopDistance, Action onFinishMoveCallBack = null, bool objMove = false)
    {
        m_lstMovePath = path;
        m_fStopDistance = stopDistance;
        m_fMaxSpeed = maxSpeed;
        m_OnFinishMoveCallBack = onFinishMoveCallBack;

        m_iCurrentTargetIndex = 0;
        if (objMove)
        {
            m_bIsObjMove = true;
            InitMove(m_vPos);
            SetPhysicStatus(false);
        }
        else
        {
            m_bIsMoving = true;
        }
    }
    virtual public void StopMove()
    {
        m_bIsMoving = false;
    }
    virtual public void FinishMove()
    {
        m_bIsMoving = false;
        if (null != m_OnFinishMoveCallBack)
        {
            m_OnFinishMoveCallBack();
        }
    }
    public void SetSelectedStatus(bool status)
    {
        if (m_bIsSelected == status)
        {
            return;
        }
        if (status)
        {
            if (m_MeshRender)
                m_MeshRender.material = m_HighlightMaterial;
        }
        else
        {
            if (m_MeshRender)
                m_MeshRender.material = m_NormalMaterial;
        }
        m_bIsSelected = status;
    }
    public bool GetSelectedStatus()
    {
        return m_bIsSelected;
    }
    public void AddNavmeshObs()
    {
        if (m_ObjectInstance.GetComponent<NavMeshObstacle>() == null)
            m_ObjectInstance.AddComponent<NavMeshObstacle>();
    }
    public void CharRotate (float angle)
    {
        m_bIsRotating = true;
        //m_AnimatorAgent.DoLocomotion(0.01f, angle);

        //float local = m_ObjectInstance.transform.localRotation.y;
        float result = angle - m_ObjectInstance.transform.eulerAngles.y;
        if (result < -180f)
        {
            result += 360f;
        }
        else if (result > 180f)
        {
            result -= 360f;
        }
        m_AnimatorAgent.DoLocomotion(0.01f, result);
    }
    #endregion

    #region move
    private void Moving()
    {
        try
        {
            m_vPos = m_ObjectInstance.transform.position;

            if (m_lstMovePath == null || m_lstMovePath.Count <= 0)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(m_vPos, out hit, 10.0f, NavMesh.AllAreas))
                {
                    List<Vector3> path = new List<Vector3>();
                    path.Add(hit.position);
                    MoveTo(path, m_fMaxSpeed, m_fStopDistance, m_OnFinishMoveCallBack);
                }
                return;
            }

            //Ignore 1st Point
            if (m_iCurrentTargetIndex == 0)
            {
                if ((m_vPos - m_lstMovePath[m_iCurrentTargetIndex]).magnitude <= m_fStopDistance)
                {
                    ++m_iCurrentTargetIndex;
                    if (m_iCurrentTargetIndex >= m_lstMovePath.Count)
                    {
                        FinishMove();
                        return;
                    }
                }
            }

            //Update Animator
            m_Direction = (m_lstMovePath[m_iCurrentTargetIndex] - m_vPos).normalized;
            Vector3 moveDirection = Quaternion.Inverse(m_ObjectInstance.transform.rotation) * m_Direction;
            float angle = Mathf.Atan2(moveDirection.x, moveDirection.z) * 180.0f / 3.14159f;
            m_fMaxSpeed = m_CharContainer.GetMaxSpeed(m_fMaxSpeed);
            float speed = m_iCurrentTargetIndex == 0 ? m_fMaxSpeed / 5 : m_fMaxSpeed;
            m_AnimatorAgent.DoLocomotion(speed, angle);

            //Check Distance
            float distance = (m_vPos - m_lstMovePath[m_iCurrentTargetIndex]).magnitude;
            if (distance <= m_fStopDistance)
            {
                ++m_iCurrentTargetIndex;
                if (m_iCurrentTargetIndex >= m_lstMovePath.Count)
                {
                    FinishMove();
                    return;
                }
            }
        }
        catch (Exception)
        {
            Debuger.LogWarning("error on npc move");
            throw;
        }

    }
    private void ObjMoving()
    {
        if (m_lstMovePath == null || m_lstMovePath.Count <= 0)
        {
            return;
        }
        float deltaTime = TimeManager.Instance.GetTime() - m_fInitTime;
        m_vPos = m_vMoveForword * m_fMaxSpeed * deltaTime + m_vInitPos;
        m_ObjectInstance.transform.position = m_vPos;
        m_vRotation = m_ObjectInstance.transform.eulerAngles;

        if (deltaTime >= m_fTimeSpace)
        {
            ++m_iCurrentTargetIndex;
            if (m_iCurrentTargetIndex >= m_lstMovePath.Count)
            {
                // stop
                m_bIsObjMove = false;
                if (null != m_OnFinishMoveCallBack)
                {
                    m_OnFinishMoveCallBack();
                }
                SetPhysicStatus(true);
                return;
            }
            else
            {
                InitMove(m_vPos);
            }
        }

    }
    private void InitMove(Vector3 pos)
    {
        if (m_lstMovePath == null || m_lstMovePath.Count <= 0)
        {
            return;
        }
        float stopdistance = m_iCurrentTargetIndex == (m_lstMovePath.Count - 1) ? m_fStopDistance : 0.0f;
        m_vPos = pos;
        m_vInitPos = m_vPos;
        m_fInitTime = TimeManager.Instance.GetTime();
        m_fTimeSpace = ((m_lstMovePath[m_iCurrentTargetIndex] - m_vPos).magnitude - stopdistance) / m_fMaxSpeed;
        m_vMoveForword = (m_lstMovePath[m_iCurrentTargetIndex] - m_vPos).normalized;
        m_ObjectInstance.transform.forward = 1 * m_vMoveForword;
        m_ObjectInstance.transform.position = m_vPos;
    }
    private void FinishRotate ()
    {
        m_bIsRotating = false;
    }
    #endregion

    #region physics
    public void SetPhysicStatus(bool status)
    {
        Rigidbody rigidbody = m_ObjectInstance.GetComponent<Rigidbody>();
        if (null != rigidbody)
        {
            rigidbody.isKinematic = !status;
        }
    }
    #endregion
}

