using UnityEngine;
using System.Collections;
using NetWork.Auto;

public class GameTestLogic: LogicBase<GameTestLogic>
{
    private bool m_bIsTouching;
    private Vector3 m_vInitCamPos;
    private Vector3 m_vInitPos;
    private Vector3 m_fDeltaDistance;
    private Camera m_Camera;
    private PlayerCharacter m_Leader;
    private Npc m_Npc;
	public override void StartLogic()
	{
	    /*GameObject map =
	        GameObject.Instantiate(ResourceManager.Instance.LoadBuildInResource<GameObject>("Scene", AssetType.Map));
	    ComponentTool.Attach(null, map.transform);*/

        m_Camera = ComponentTool.FindChild("SceneCamera", null).GetComponent<Camera>();
        WindowManager.Instance.HideAllWindow();
        //WindowManager.Instance.OpenWindow(WindowID.WindowProject1);

        CharBaseInfo charBaseInfo = new CharBaseInfo();
        charBaseInfo.CharId = 10000001;
        charBaseInfo.CharName = "sdfsd";



        m_Leader = PlayerCharacter.Create(charBaseInfo, new CharCounterInfo());

	    TerrainManager.Instance.InitializeTerrain(0);

        LifeTickTask.Instance.RegisterToUpdateList(Update);
	    RegisterMsg();
	}
	public override void EndLogic()
	{
        m_Leader.Distructor();
        UnRegisterMsg();
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        //clear map instance
        TerrainManager.Instance.CloseTerrain();
	}

    private void RegisterMsg()
    {
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_HIT_LIFE, OnHitLife);
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_HIT_TERRAIN, OnHitTerrain);
    }
    private void UnRegisterMsg()
    {
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_HIT_LIFE, OnHitLife);
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_HIT_TERRAIN, OnHitTerrain);
    }
    private void OnHitLife(MessageObject msg)
    {
        Vector3 pos = ((Transform) (msg.msgValue)).position;
        Debuger.Log("Move to target npc");
        m_Leader.MoveTo(pos);
    }
    private void OnHitTerrain(MessageObject msg)
    {
        Vector3 pos = (Vector3)(msg.msgValue);
        Debuger.Log("Move to target pos");
        m_Leader.MoveTo(pos);
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                m_bIsTouching = true;
                m_vInitPos = Input.touches[0].position;
                m_vInitCamPos = m_Camera.transform.position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {
                m_bIsTouching = false;

            }
            if (m_bIsTouching)
            {
                Vector3 currentPos = Input.touches[0].position;
                m_fDeltaDistance = (currentPos - m_vInitPos);
                m_fDeltaDistance.z = m_fDeltaDistance.y;
                m_fDeltaDistance.y = 0;
                m_Camera.transform.position = m_vInitCamPos + m_fDeltaDistance * 0.01f;
            }
        }
        /*if (Input.touchCount > 1)
        {
            if (Input.touches[1].phase == TouchPhase.Began)
            {
                Ray ray = m_Camera.ScreenPointToRay(Input.touches[1].position);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    m_Leader.MoveTo(hitInfo.point);
                }
            }
        }*/

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            m_bIsTouching = true;
            m_vInitPos = Input.mousePosition;
            m_vInitCamPos = m_Camera.transform.position;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_bIsTouching = false;

        }
        if (m_bIsTouching)
        {
            Vector3 currentPos = Input.mousePosition;
            m_fDeltaDistance = (currentPos - m_vInitPos);
            m_fDeltaDistance.z = m_fDeltaDistance.y;
            m_fDeltaDistance.y = 0;
            m_Camera.transform.position = m_vInitCamPos + m_fDeltaDistance * 0.01f;
        }
       /* if (Input.GetMouseButtonDown(1))
        {
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                m_Leader.MoveTo(hitInfo.point);
            }

        }*/
#endif
    }

    /// ... to do : 
}
