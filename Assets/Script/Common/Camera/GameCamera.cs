//========================================================================
// Copyright(C): ***********************
//
// CLR Version : 4.0.30319.42000
// FileName : GameCamera
//
// Created by : LeoLi (742412055@qq.com) at 2015/11/4 15:01:25
//
//
//========================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameCamera : MonoBehaviour
{
    #region Property
    public float m_fDistance = 15f;
    public float m_fHeight = 9f;
    public float m_fOffsetHeight = 0f;
    public float m_fDistanceDamping = 2f;
    public float m_fHeightDamping = 2f;
    public float m_fOffsetHeightDamping = 2f;

    public static bool OpenClick
    {
        get;
        set;
    }
    public bool LockCam
    {
        set { m_bLock = value; }
        get { return m_bLock; }
    }
    #endregion

    #region Field
    private Transform m_LookTarget;
    private Vector3 m_LastLookTargetPos;
    private bool m_bLock = true;
    private float m_fInitDistance;
    private float m_fInitHeight;
    private float m_fInitOffsetHeigh;
    private float m_fInitDistanceDamping;
    private float m_fInitHeightDamping;
    private float m_fInitOffsetHeightDamping;
    private Vector3 m_vLastLookAtPos;
    private float m_fCurrDistance;
    private float m_fCurrHeightOffset;
    #endregion

    #region MonoBehavior
    void Start()
    {
        DontDestroyOnLoad(transform.parent);
        m_fInitDistance = m_fDistance;
        m_fInitHeight = m_fHeight;
        m_fInitHeightDamping = m_fHeightDamping;
        m_fInitDistanceDamping = m_fDistanceDamping;
        m_fInitOffsetHeigh = m_fOffsetHeight;
        m_fInitOffsetHeightDamping = m_fOffsetHeightDamping;
    }
    void LateUpdate()
    {
        if (OpenClick)
        {
            CheckClick();
        }

        if (LockCam && null != m_LookTarget)
        {
            float fWantedHeight = m_LookTarget.position.y + m_fHeight;
            float fWantedTargetHeight = m_LookTarget.position.y + m_fOffsetHeight;

            float fCurrHeight = transform.parent.position.y;

            fCurrHeight = Mathf.Lerp(fCurrHeight, fWantedHeight, m_fHeightDamping * Time.deltaTime);
            m_fCurrHeightOffset = Mathf.Lerp(m_fCurrHeightOffset, fWantedTargetHeight, m_fInitOffsetHeightDamping * Time.deltaTime);
            m_fCurrDistance = Mathf.Lerp(m_fCurrDistance, m_fDistance, m_fDistanceDamping * Time.deltaTime);

            transform.parent.position = m_LookTarget.position;
            transform.parent.position -= Vector3.forward * m_fCurrDistance;
            transform.parent.position = new Vector3(transform.parent.position.x, fCurrHeight, transform.position.z);
            Vector3 vLookAtPos = new Vector3(m_LookTarget.position.x, fWantedTargetHeight, m_LookTarget.position.z);
            transform.parent.LookAt(vLookAtPos);
        }
    }
    #endregion

    #region Public Interface
    public void SetTarget(Transform trans, bool bRightNow = false)
    {
        m_LookTarget = trans;
        if (null != m_LookTarget && bRightNow)
        {
            m_fCurrDistance = m_fDistance;
            m_fCurrHeightOffset = m_LookTarget.position.y + m_fOffsetHeight;
            transform.parent.position = new Vector3(m_LookTarget.position.x, m_LookTarget.position.y + m_fHeight, m_LookTarget.position.z);
            transform.parent.position -= Vector3.forward * m_fCurrDistance;
            transform.parent.LookAt(new Vector3(m_LookTarget.position.x, m_fCurrHeightOffset, m_LookTarget.position.z));
        }
    }
    public void ResetCam()
    {
        m_fDistance = m_fInitDistance;
        m_fHeight = m_fInitHeight;
        m_fOffsetHeight = m_fInitOffsetHeigh;
        m_fDistanceDamping = m_fInitDistanceDamping;
        m_fHeightDamping = m_fInitHeightDamping;
        m_fOffsetHeightDamping = m_fInitOffsetHeightDamping;
        LockCam = true;
    }
    public void ShakeCamera(float fTime, Vector3 vAmount)
    {
        Hashtable hash = new Hashtable();
        hash.Add("time", fTime);
        hash.Add("amount", vAmount);
        hash.Add("islocal", true);
        iTween.ShakePosition(gameObject, hash);
    }
    public void SetCameraPos(Vector3 vPos)
    {
        transform.parent.position = vPos;
    }
    public void SetCameraRot(Quaternion rot)
    {
        transform.parent.rotation = rot;
    }
    #endregion

    #region System Functions
    private void CheckClick()
    {
        if (null == UICamera.hoveredObject && Input.GetMouseButtonDown(0))
        {
            //LayerMask Lifelayer = 1 << LayerMask.NameToLayer("Life");
            //Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, 100f, Lifelayer.value))
            //{
            //    LogicEventMgr.Instance.ExecuteLogicEvent(ELoigc_Event.Camera_HitTarget, hit.transform);
            //    //BattleLogic.Instance.OnHitTargetEventHandler( hit.transform );
            //    return;
            //}

            //LayerMask Triggerlayer = 1 << LayerMask.NameToLayer("Trigger");
            //if (Physics.Raycast(ray, out hit, 100f, Triggerlayer.value))
            //{
            //    LogicEventMgr.Instance.ExecuteLogicEvent(ELoigc_Event.Camera_HitTrigger, hit.transform);
            //    //BattleLogic.Instance.OnHitMoveEventhandler( hit.point );
            //    return;
            //}

            //LayerMask Terrainlayer = 1 << LayerMask.NameToLayer("Terrain");
            //if (Physics.Raycast(ray, out hit, 100f, Terrainlayer.value))
            //{
            //    LogicEventMgr.Instance.ExecuteLogicEvent(ELoigc_Event.Camera_HitMove, hit.point);
            //    //BattleLogic.Instance.OnHitMoveEventhandler( hit.point );
            //    return;
            //}
        }
    }
    #endregion
}

