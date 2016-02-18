//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.18444
// FileName : TransformContainer
//
// Created by : Baoxue at 2015/11/17 11:56:31
// Changed by : LeoLi at 2015/11/25 17:32:31
//
// Purpose : 
//========================================================================

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

//[RequireComponent(typeof(Animator))]
public class CharTransformContainer : TransformContainerBase
{
    [SerializeField]
    private Animator m_Animator;
    //[SerializeField]
    //private NavMeshAgent m_NavAgent;
    private bool m_bIsInsideMap;
    private NavMeshPath m_NavMeshPath;
    private int m_SpeedId;
    private NavMeshObstacle m_NavObs;
    //anim
    private string m_CurrentAnimName = "";
    private Action m_OnFinishAnimCallBack;
    // char setting
    private float m_MaxSpeed = 5f;

    public bool IsInsideMap
    {
        get
        {
            return m_bIsInsideMap;
        }
    }

    #region MonoBehavior
    private void Start()
    {
        m_SpeedId = Animator.StringToHash("Speed");

        LifeManager.AddToSceneObjList(gameObject);
        //Init Components
        m_NavMeshPath = new NavMeshPath();
        m_Animator = this.gameObject.GetComponent<Animator>();
        m_NavObs = this.gameObject.GetComponent<NavMeshObstacle>();
        //if (m_NavObs != null)
        //{
        //    m_NavObs.shape = NavMeshObstacleShape.Capsule;
        //    m_NavObs.radius = .5f;
        //}
        if (m_Animator == null)
        {
            Debuger.LogWarning("Animator Not Found");
        }
        //m_NavAgent = this.gameObject.GetComponentInChildren<NavMeshAgent>();
        //if (m_NavAgent == null)
        //{
        //    Debuger.LogError("NavMeshAgent Not Found");
        //    return;
        //}
        //reset char pos
        NavMeshHit hit;
        if (NavMesh.SamplePosition(this.gameObject.transform.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            this.gameObject.transform.position = hit.position;
        }

        CharSettingScript setting = gameObject.GetComponent<CharSettingScript>();
        if (setting != null)
        {
            m_MaxSpeed = setting.DefaultSpeed;
        }

    }
    private void Update()
    {
        for (int i = 0; i < m_NavMeshPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(m_NavMeshPath.corners[i], m_NavMeshPath.corners[i + 1], Color.red);
        }
    }
    private void OnAnimatorMove()
    {
        //if (m_NavAgent != null)

        if (m_Animator != null && m_Animator.HasState(0, Animator.StringToHash("Locomotion.Idle")))
        {
            //anim
            AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (m_CurrentAnimName != null && m_CurrentAnimName != "")
            {
                bool inCurrent = stateInfo.IsName(m_CurrentAnimName);
                if (!inCurrent)
                {
                    OnFinishCurrentAnim();
                }
            }

            //move
            AnimatorStateInfo state = m_Animator.GetCurrentAnimatorStateInfo(0);
            bool isInLocomotion = state.IsName("Locomotion.Idle") || state.IsName("Locomotion.TurnOnSpot") || state.IsName("Locomotion.WalkRun") ? true : false;

            if (m_Animator.GetFloat(m_SpeedId) < 0.01f && isInLocomotion)
            {
                if (m_NavObs != null && !m_NavObs.carving)
                {
                    Invoke("CaveObstacle", 0.5f);
                }
                //return;
            }
            else
            {
                transform.position = m_Animator.deltaPosition + transform.position;
            }
            transform.rotation = m_Animator.rootRotation;
            //ResetNavAgentPos();
        }
    }
    private void OnGUI()
    {
        //GUILayout.Label("desiredVelocity   " + m_NavAgent.desiredVelocity.ToString());
        //GUILayout.Label("actualVelocity   " + (Quaternion.Inverse(transform.rotation) * m_NavAgent.desiredVelocity).ToString());


        //NavMeshHit hit;
        //NavMesh.SamplePosition(this.gameObject.transform.position, out hit, 1.0f, NavMesh.AllAreas);
        //GUILayout.Label("Hit   " + hit.position.ToString());
        //GUILayout.Label("CHar  " + this.gameObject.transform.position.ToString());
    }
    #endregion

    #region Public Interface
    public void FindPath(Vector3 target, Action<List<Vector3>> callback)
    {
        if (m_NavObs != null)
        {
            m_NavObs.carving = false;
        }

        StartCoroutine(DelayToInvokeDo(() =>
        {
            NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, m_NavMeshPath);
            List<Vector3> result = new List<Vector3>();
            result.AddRange(m_NavMeshPath.corners);
            callback(result);
        }, 0.1f));
        //return result;
    }
    public void DirectPlayAnimation(string state, Action onFinishCallBack = null)
    {
        if (m_Animator != null)
        {
            m_Animator.Play(state);
            m_CurrentAnimName = state;
            m_OnFinishAnimCallBack = onFinishCallBack;
        }
    }
    public float GetMaxSpeed(float speed)
    {
        if (m_MaxSpeed < speed)
            return m_MaxSpeed;
        else
            return speed;
    }
    #endregion

    #region System Function
    private void ResetNavAgentPos()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(this.gameObject.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            //m_NavAgent.gameObject.transform.position = hit.position;
            if (!m_bIsInsideMap)
            {
                m_bIsInsideMap = true;
            }
            //保证NavAgent组件始终跟随NavAgent挂点
            //if (m_NavAgent.areaMask != NavMesh.AllAreas)
            //{
            //    m_NavAgent.areaMask = NavMesh.AllAreas;
            //}
        }
        else
        {
            Debuger.LogWarning("Charactor: " + this.gameObject.name + " Out of Range!");
            if (m_bIsInsideMap)
            {
                m_bIsInsideMap = false;
            }
            //保证NavAgent组件始终跟随NavAgent挂点
            //if (m_NavAgent.areaMask == NavMesh.AllAreas)
            //{
            //    m_NavAgent.areaMask = 0;
            //}
        }
    }
    private void CaveObstacle()
    {
        m_NavObs.carving = true;
    }
    private IEnumerator DelayToInvokeDo(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
    private void OnFinishCurrentAnim()
    {
        if (m_OnFinishAnimCallBack != null)
        {
            m_CurrentAnimName = "";
            m_OnFinishAnimCallBack();
        }
    }
    #endregion
}

