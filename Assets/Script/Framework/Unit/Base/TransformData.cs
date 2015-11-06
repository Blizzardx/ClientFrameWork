using UnityEngine;
using System.Collections.Generic;

public class TransformData
{
    private Vector3 m_vPos;
    private Vector3 m_vRotation;
    private Vector3 m_vScale;
    private GameObject m_ObjectInstance;
    private AnimationController m_Control;

    #region move
    private List<Vector3>   m_MovePath;
    private bool            m_bIsMoving;
    private float           m_fMoveSpeed;
    private int             m_iCurrentTargetIndex;
    private float           m_fStopDistance;
    private Vector3         m_vInitPos;
    private float           m_fInitTime;
    private float           m_fTimeSpace;
    private Vector3         m_vMoveForword;
    #endregion

    #region public interface
    virtual public void Initialize(string resourcePath,AssetType resourceType)
    {
        m_ObjectInstance = GameObject.Instantiate(ResourceManager.Instance.LoadBuildInResource<GameObject>(resourcePath, resourceType));
        if (null == m_ObjectInstance)
        {
            Debuger.LogError("Can't load resource " + resourcePath);
        }
        m_Control = new AnimationController();
        m_Control.Initialize(m_ObjectInstance);
        LifeTickTask.Instance.RegisterToUpdateList(Update);
    }
    virtual public void Distructor()
    {
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        GameObject.Destroy(m_ObjectInstance);
    }
    virtual public void PlayAnimation(string state)
    {
        m_Control.PlayAnimation(state);
    }
    virtual public void MoveTo(List<Vector3> path, float speed,float stopDistance)
    {
        m_bIsMoving = true;
        m_MovePath = path;
        m_fStopDistance = stopDistance;
        m_fMoveSpeed = speed;
        m_iCurrentTargetIndex = 0;
        InitMove(m_vPos);
    }
    virtual public void MoveTo(Vector3 targetPosition, float speed, float stopDistance)
    {
       
    }
    virtual public void Stop()
    {
        m_bIsMoving = false;
    }
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
    virtual public void Update()
    {
        if (m_bIsMoving)
        {
            Move();
        }
    }
    #endregion

    #region move
    private void InitMove(Vector3 pos)
    {
        float stopdistance = m_iCurrentTargetIndex == (m_MovePath.Count - 1) ? m_fStopDistance : 0.0f;
        m_vPos = pos;
        m_vInitPos = m_vPos;
        m_fInitTime = TimeManager.Instance.GetTime();
        m_fTimeSpace = ((m_MovePath[m_iCurrentTargetIndex] - m_vPos).magnitude - stopdistance) / m_fMoveSpeed;
        m_vMoveForword = (m_MovePath[m_iCurrentTargetIndex]-m_vPos).normalized;
        m_ObjectInstance.transform.forward = m_vMoveForword;
        m_ObjectInstance.transform.position = m_vPos;

    }
    private void Move()
    {
        float deltaTime = TimeManager.Instance.GetTime() - m_fInitTime;
        m_vPos = m_vMoveForword * m_fMoveSpeed * deltaTime + m_vInitPos;
        m_ObjectInstance.transform.position = m_vPos;
        m_vRotation = m_ObjectInstance.transform.eulerAngles;

        if (deltaTime >= m_fTimeSpace)
        {
            ++ m_iCurrentTargetIndex;
            if (m_iCurrentTargetIndex >= m_MovePath.Count)
            {
                Stop();
                return;
            }
            else
            {
                InitMove(m_vPos);
            }
        }
    }
    #endregion
}
