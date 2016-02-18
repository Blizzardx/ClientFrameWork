//========================================================================
// Copyright(C): CYTX
//
// FileName : GestureManager
// 
// Created by : LeoLi at 2015/12/23 16:03:39
//
// Purpose : 
//========================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class GestureManager : Singleton<GestureManager>
{
    private GameObject m_fingerGesture;
    private GameObject m_LineRender;
    private FingerGestures.Finger m_DraggingFinger = null;
    private GameObject m_CurrentSelection = null;
    private List<Gesture.EventHandler> m_lstEventHandler = new List<Gesture.EventHandler>();
    private Dictionary<GameObject, GameObject> m_LineRenderInstances = new Dictionary<GameObject, GameObject>();
    private List<DictatePositionScript> m_lstDictatePoint = new List<DictatePositionScript>();
    private PlayerCharacter m_Player;
    private bool m_bIsDictate = true;

    #region Public Interface
    public void Initialize()
    {
        // FingerGestures
        m_fingerGesture = GameObject.Find("FingerGestures");
        if (m_fingerGesture == null)
        {
            Debuger.LogError("FingerGestures not Found");
            return;
        }
        // LineRenderer
        m_fingerGesture.AddComponent<ScreenRaycaster>();
        m_LineRender = ResourceManager.Instance.LoadBuildInResource<GameObject>("LineRenderer", AssetType.Effect);
        if (m_LineRender == null)
        {
            Debuger.LogError("LineRender not Found");
            return;
        }
        // DictatePoints
        GameObject root = GameObject.Find("DictateRoot");
        if (root == null)
        {
            Debuger.LogWarning("DictateRoot not Found");
            return;
        }
        m_lstDictatePoint.AddRange(root.GetComponentsInChildren<DictatePositionScript>());
        if (m_lstDictatePoint.Count <= 0)
        {
            Debuger.LogWarning("Dictate Points not Found");
            return;
        }
        //else
        //{
        //    foreach (DictatePositionScript dic in m_lstDictatePoint)
        //    {
        //        Debug.Log(dic.gameObject.name);
        //    }
        //}
    }
    public void Clear()
    {
        if (m_fingerGesture == null)
            return;
        // Remove Recognizer
        Component[] recognizers = m_fingerGesture.GetComponents<GestureRecognizer>();
        foreach (GestureRecognizer recong in recognizers)
        {
            GameObject.Destroy(recong);
        }
        if (m_fingerGesture.GetComponent<ScreenRaycaster>())
        {
            GameObject.Destroy(m_fingerGesture.GetComponent<ScreenRaycaster>());
        }
        // Clear Event
        if (m_lstEventHandler != null && m_lstEventHandler.Count > 0)
        {
            foreach (Gesture.EventHandler handler in m_lstEventHandler)
            {
                FingerGestures.OnGestureEvent -= handler;
            }
            m_lstEventHandler.Clear();
        }
    }
    public void EnableMoveChar()
    {
        DragRecognizer dragRcong = m_fingerGesture.GetComponent<DragRecognizer>();

        if (dragRcong == null)
        {
            dragRcong = m_fingerGesture.AddComponent<DragRecognizer>();
        }
        dragRcong.UseSendMessage = false;
        if (dragRcong.Raycaster == null)
        {
            dragRcong.Raycaster = m_fingerGesture.GetComponent<ScreenRaycaster>();
        }

        LongPressRecognizer longpressRcong = m_fingerGesture.GetComponent<LongPressRecognizer>();
        if (longpressRcong == null)
        {
            longpressRcong = m_fingerGesture.AddComponent<LongPressRecognizer>();
        }
        longpressRcong.Duration = 0.2f;
        longpressRcong.UseSendMessage = false;
        if (longpressRcong.Raycaster == null)
        {
            longpressRcong.Raycaster = m_fingerGesture.GetComponent<ScreenRaycaster>();
        }

        FingerGestures.OnGestureEvent += OnSelectNpc;
        if (m_lstEventHandler != null && !m_lstEventHandler.Contains(OnSelectNpc))
            m_lstEventHandler.Add(OnSelectNpc);

        FingerGestures.OnGestureEvent += OnCharMove;
        if (m_lstEventHandler != null && !m_lstEventHandler.Contains(OnCharMove))
            m_lstEventHandler.Add(OnCharMove);
    }
    public void DisableMoveChar()
    {
        // Remove Recognizer
        if (m_fingerGesture.GetComponent<DragRecognizer>())
        {
            GameObject.Destroy(m_fingerGesture.GetComponent<DragRecognizer>());
        }
        if (m_fingerGesture.GetComponent<LongPressRecognizer>())
        {
            GameObject.Destroy(m_fingerGesture.GetComponent<LongPressRecognizer>());
        }

        // Clear Event
        if (m_lstEventHandler != null && m_lstEventHandler.Contains(OnCharMove))
        {
            FingerGestures.OnGestureEvent -= OnCharMove;
            m_lstEventHandler.Remove(OnCharMove);
        }
    }
    public void SetDictation(bool dictate)
    {
        m_bIsDictate = dictate;
    }
    #endregion

    #region System Event
    private void OnSelectNpc(Gesture gesture)
    {
        if (!(gesture is LongPressGesture))
        {
            return;
        }

        if (!m_bIsDictate)
        {
            return;
        }

        LongPressGesture longGest = (LongPressGesture)gesture;

        // Set npc selected
        GameObject selection = gesture.Selection;
        if (null == selection)
        {
            return;
        }
        CharTransformContainer container = selection.GetComponent<CharTransformContainer>();
        if (container == null)
        {
            //Debug.LogWarning("container not found");
            return;
        }
        var data = container.GetData();
        if (data is Npc)
        {
            Npc npc = (Npc)data;
            if (npc.IsInGroup)
            {
                CharTransformData charData = (CharTransformData)(npc.GetTransformData());
                charData.SetSelectedStatus(true);
            }
        }

    }
    private void OnCharMove(Gesture gesture)
    {
        // check gesture
        if (!(gesture is DragGesture))
        {
            return;
        }
        DragGesture dragGest = (DragGesture)gesture;
        // Start
        if (dragGest.Phase == ContinuousGesturePhase.Started)
        {
            m_CurrentSelection = gesture.Selection;
            if (null == gesture.Selection)
            {
                return;
            }
            Transform transform = null;
            if(GetMouseHitPlayer(out transform))
            {
                gesture.Selection = transform.gameObject;
                m_CurrentSelection = gesture.Selection;
            }
            // get char
            CharTransformContainer container = m_CurrentSelection.GetComponent<CharTransformContainer>();
            if (container == null)
            {
                //Debug.LogWarning("container not found");
                return;
            }
            var data = container.GetData();
            // player
            if (data is PlayerCharacter)
            {
                m_Player = (PlayerCharacter)data;
                CharTransformData charData = (CharTransformData)(m_Player.GetTransformData());

                m_DraggingFinger = gesture.Fingers[0];
                CreateLineRender();

            }
            // npc
            else if (data is Npc)
            {
                Npc npc = (Npc)data;
                CharTransformData charData = (CharTransformData)(npc.GetTransformData());
                // check npc selected
                if (charData.GetSelectedStatus())
                {
                    m_DraggingFinger = gesture.Fingers[0];
                    CreateLineRender();
                }
            }
        }
        // Update
        else if (dragGest.Phase == ContinuousGesturePhase.Updated)
        {
            // make sure this is the finger we started dragging with
            if (gesture.Fingers[0] != m_DraggingFinger)
                return;
            if (!m_CurrentSelection)
                return;
            if (m_LineRenderInstances.ContainsKey(m_CurrentSelection))
            {
                GameObject lineRenderer = m_LineRenderInstances[m_CurrentSelection];
                LineRenderer renderer = lineRenderer.GetComponent<LineRenderer>();
                if (renderer)
                {
                    renderer.SetPosition(0, m_CurrentSelection.transform.position);
                    Vector3 hitpoint = new Vector3();
                    if (GetMouseHitTerrainPos(out hitpoint))
                    {
                        renderer.SetPosition(1, hitpoint);
                        if (GestureManager.CheckNavmeshPoint(hitpoint))
                        {
                            renderer.SetColors(new Color(0, 1, 0), new Color(0, 1, 0));
                        }
                        else
                        {
                            renderer.SetColors(new Color(1, 0, 0), new Color(1, 0, 0));
                        }
                    }
                }
            }

        }
        // End
        else if (dragGest.Phase == ContinuousGesturePhase.Ended)
        {
            if (!m_CurrentSelection)
                return;

            ClearLineRender();

            // get char
            CharTransformContainer container = m_CurrentSelection.GetComponent<CharTransformContainer>();
            if (container == null)
            {
                //Debug.LogWarning("container not found");
                return;
            }
            var data = container.GetData();
            // player
            if (data is PlayerCharacter)
            {
                m_Player = (PlayerCharacter)data;
                MovePlayer();
            }
            // npc
            else if (data is Npc)
            {
                Npc npc = (Npc)data;
                CharTransformData charData = (CharTransformData)(npc.GetTransformData());
                if (charData.GetSelectedStatus())
                {
                    MoveNpc(npc);
                }
            }

        }

    }
    public static bool CheckNavmeshPoint(Vector3 input)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(input, out hit, 1.0f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region System Function
    private bool GetMouseHitTerrainPos(out Vector3 hitpoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        hitpoint = new Vector3();
        if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Terrain")))
        {
            hitpoint = hitInfo.point;
            return true;
        }
        return false;
    }
    private bool GetMouseHitPlayer(out Transform hitpoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("InputCollider")))
        {
            hitpoint = hitInfo.transform;
            return true;
        }
        hitpoint = null;
        return false;
    }
    private void CreateLineRender()
    {
        GameObject lineRenderer = GameObject.Instantiate(m_LineRender, m_CurrentSelection.transform.position, m_CurrentSelection.transform.rotation) as GameObject;
        lineRenderer.transform.parent = m_CurrentSelection.transform;
        if (m_LineRenderInstances.ContainsKey(m_CurrentSelection))
        {
            ClearLineRender();
        }
        m_LineRenderInstances.Add(m_CurrentSelection, lineRenderer);
    }
    private void ClearLineRender()
    {
        if (!m_CurrentSelection)
            return;

        if (m_LineRenderInstances.ContainsKey(m_CurrentSelection))
        {
            GameObject lineRenderer = m_LineRenderInstances[m_CurrentSelection];
            GameObject.Destroy(lineRenderer);
            m_LineRenderInstances.Remove(m_CurrentSelection);
        }

    }
    private void MovePlayer()
    {
        Vector3 hitpoint = new Vector3();
        if (GetMouseHitTerrainPos(out hitpoint))
        {
            m_Player.MoveTo(hitpoint);
        }
    }
    private void MoveNpc(Npc npc)
    {
        Vector3 hitpoint = new Vector3();
        if (GetMouseHitTerrainPos(out hitpoint))
        {
            if (m_Player == null)
                return;

            if (m_lstDictatePoint.Count <= 0)
            {
                // no dictate point
                npc.MoveTo(hitpoint);
                npc.IsPlayerControlled = true;
                npc.IsInGroup = false;
                return;
            }

            foreach (DictatePositionScript point in m_lstDictatePoint)
            {
                bool isOutOfCamera = !point.IsInCamera();
                bool isRightDirection = IsRightDirection(hitpoint, point.transform.position);
                if (isOutOfCamera && isRightDirection)
                {
                    // success
                    npc.MoveTo(point.transform.position, false, () =>
                    {
                        CharTransformData npcobj = (CharTransformData)(npc.GetTransformData());
                        //npcobj.GetGameObject().SetActive(false);
                    });
                    npc.IsPlayerControlled = true;
                    npc.IsInGroup = false;
                    return;
                }
            }

            //fail
        }
    }

    private bool IsRightDirection(Vector3 target, Vector3 dictatePoint)
    {
        Vector3 playerPos = m_Player.GetTransformData().GetPosition();

        Vector3 a = target - playerPos;
        Vector3 b = dictatePoint - playerPos;

        float angle = Mathf.Acos(Vector3.Dot(a.normalized, b.normalized)) * Mathf.Rad2Deg;

        return (angle < 45f);
    }
    #endregion
}

