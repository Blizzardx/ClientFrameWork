using System;
using Pathfinding;
using UnityEngine;
using System.Collections.Generic;
public class TransformDataBase
{
    protected Vector3 m_vPos;
    protected Vector3 m_vRotation;
    protected Vector3 m_vScale;

    virtual public Vector3 GetPosition()
    {
        return m_vPos;
    }
    virtual public void SetPosition(Vector3 value)
    {
        m_vPos = value;
    }
    virtual public Vector3 GetRotation()
    {
        return m_vRotation;
    }
    virtual public void SetRotation(Vector3 value)
    {
        m_vRotation = value;
    }
    virtual public Vector3 GetScale()
    {
        return m_vScale;
    }
    virtual public void SetScale(Vector3 value)
    {
        m_vScale = value;
    }
}

// CharTransformData for A*

//public class CharTransformData : TransformDataBase
//{
//    protected GameObject            m_ObjectInstance;
//    protected AnimationController   m_Control;
//    protected int                   m_iInstanceId;
//    protected Ilife                 m_LifeData;
//    private TransformContainer      m_bodyMark;

//    #region move
//    private List<Vector3>   m_MovePath;
//    private bool            m_bIsMoving;
//    private float           m_fMoveSpeed;
//    private int             m_iCurrentTargetIndex;
//    private float           m_fStopDistance;
//    private Vector3         m_vInitPos;
//    private float           m_fInitTime;
//    private float           m_fTimeSpace;
//    private Vector3         m_vMoveForword;
//    private Seeker          m_Seeker;
//    private SimpleSmoothModifier m_simpleSmooth;
//    private Action          m_OnStopCallBack;
//    #endregion

//    #region public interface
//    virtual public void Initialize(Ilife lifeData,string resourcePath,AssetType resourceType)
//    {
//        m_LifeData = lifeData;
//        m_iInstanceId = lifeData.GetInstanceId();

//        m_ObjectInstance = GameObject.Instantiate(ResourceManager.Instance.LoadBuildInResource<GameObject>(resourcePath, resourceType));

//        //mark transform
//        m_bodyMark = m_ObjectInstance.AddComponent<TransformContainer>();
//        m_bodyMark.Initialize(lifeData.GetInstanceId(), lifeData);

//        if (null == m_ObjectInstance)
//        {
//            Debuger.LogError("Can't load resource " + resourcePath);
//        }
//        m_Control = new AnimationController();
//        m_Control.Initialize(m_ObjectInstance);
//    }
//    public void Distructor()
//    {
//        GameObject.Destroy(m_ObjectInstance);
//    }
//    virtual public void PlayAnimation(string state)
//    {
//        m_Control.PlayAnimation(state);
//    }
//    virtual public void MoveTo(List<Vector3> path, float speed,float stopDistance,Action onStopCallBack = null)
//    {
//        m_bIsMoving = true;
//        m_MovePath = path;
//        m_fStopDistance = stopDistance;
//        m_fMoveSpeed = speed;
//        m_iCurrentTargetIndex = 0;
//        m_OnStopCallBack = onStopCallBack;
//        InitMove(m_vPos);
//    }
//    virtual public void MoveTo(Vector3 targetPosition, float speed, float stopDistance,Action onStopCallBack = null)
//    {
//        m_fStopDistance = stopDistance;
//        m_fMoveSpeed = speed;
//        m_OnStopCallBack = onStopCallBack;
//        FindPath(targetPosition);
//    }
//    virtual public void Stop()
//    {
//        m_bIsMoving = false;
//        if (null != m_OnStopCallBack)
//        {
//            m_OnStopCallBack();
//        }
//    }
//    override public void SetPosition(Vector3 value)
//    {
//        m_vPos = value;
//        m_ObjectInstance.transform.position = m_vPos;
//    }
//    override public void SetRotation(Vector3 value)
//    {
//        m_vRotation = value;
//        m_ObjectInstance.transform.eulerAngles = m_vRotation;
//    }
//    override public void SetScale(Vector3 value)
//    {
//        m_vScale = value;
//        m_ObjectInstance.transform.localScale = m_vScale;
//    }
//    virtual public void Update()
//    {
//        if (m_bIsMoving)
//        {
//            Move();
//        }
//    }
//    public GameObject GetGameObject()
//    {
//        return m_ObjectInstance;
//    }
//    #endregion

//    #region move
//    private void InitMove(Vector3 pos)
//    {
//        float stopdistance = m_iCurrentTargetIndex == (m_MovePath.Count - 1) ? m_fStopDistance : 0.0f;
//        m_vPos = pos;
//        m_vInitPos = m_vPos;
//        m_fInitTime = TimeManager.Instance.GetTime();
//        m_fTimeSpace = ((m_MovePath[m_iCurrentTargetIndex] - m_vPos).magnitude - stopdistance) / m_fMoveSpeed;
//        m_vMoveForword = (m_MovePath[m_iCurrentTargetIndex]-m_vPos).normalized;
//        m_ObjectInstance.transform.forward = 1*m_vMoveForword;
//        m_ObjectInstance.transform.position = m_vPos;

//    }
//    private void Move()
//    {
//        float deltaTime = TimeManager.Instance.GetTime() - m_fInitTime;
//        m_vPos = m_vMoveForword * m_fMoveSpeed * deltaTime + m_vInitPos;
//        m_ObjectInstance.transform.position = m_vPos;
//        m_vRotation = m_ObjectInstance.transform.eulerAngles;

//        if (deltaTime >= m_fTimeSpace)
//        {
//            ++ m_iCurrentTargetIndex;
//            if (m_iCurrentTargetIndex >= m_MovePath.Count)
//            {
//                Stop();
//                return;
//            }
//            else
//            {
//                InitMove(m_vPos);
//            }
//        }
//    }
//    private void InitSeeker()
//    {
//        if (null == m_Seeker)
//        {
//            m_Seeker = m_ObjectInstance.GetComponent<Seeker>();
//        }
//        if (null == m_Seeker)
//        {
//            m_Seeker = m_ObjectInstance.AddComponent<Seeker>();
//        }
//        if (null == m_simpleSmooth)
//        {
//            m_simpleSmooth = m_ObjectInstance.GetComponent<SimpleSmoothModifier>();
//        }
//        if (null == m_simpleSmooth)
//        {
//            m_simpleSmooth = m_ObjectInstance.AddComponent<SimpleSmoothModifier>();
//        }
//    }
//    private void FindPath(Vector3 targetPos)
//    {
//        InitSeeker();
//        m_Seeker.StartPath(m_ObjectInstance.transform.position, targetPos, OnSeekerCallBack);
//    }
//    private void OnSeekerCallBack(Path p)
//    {
//        m_simpleSmooth.smoothType = SimpleSmoothModifier.SmoothType.Simple;
//        m_simpleSmooth.Apply(p, ModifierData.All);
//        var list = p.vectorPath;
//        MoveTo(list, m_fMoveSpeed, m_fStopDistance,m_OnStopCallBack);
//    }

//    #endregion
//}
