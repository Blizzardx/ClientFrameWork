using Config;
using RunnerGame;
using UnityEngine;
using System.Collections;
using NetWork.Auto;

public class CopenhagenLogic: LogicBase<CopenhagenLogic>
{
    private bool            m_bIsTouching;
    private Vector3         m_vInitCamPos;
    private Vector3         m_vInitPos;
    private Vector3         m_fDeltaDistance;
    private PlayerCharacter m_PlayerChar;
    private const int       m_iCurrentStageId = 1;
    private StageConfig     m_CurrentStage;

	public override void StartLogic()
	{
        WindowManager.Instance.HideAllWindow();

        CheckSceneStageConfig();

        PlayerManager.Instance.CreatePlayerChar();
	    m_PlayerChar = PlayerManager.Instance.GetPlayerInstance();

        TerrainManager.Instance.InitializeTerrain(m_CurrentStage.StageMapId, AppManager.Instance.m_bIsShowTerrainTrigger);
        m_PlayerChar.GetTransformData().SetPosition(TerrainManager.Instance.GetPlayerInitPos().Pos.GetVector3());
        m_PlayerChar.GetTransformData().SetRotation(TerrainManager.Instance.GetPlayerInitPos().Rot.GetVector3());

        //trigger enter scene
        DoEnterScene();

        LifeTickTask.Instance.RegisterToUpdateList(Update);
	    RegisterMsg();

        //Setup MainCam
        GlobalScripts.Instance.Initialize();
        GlobalScripts.Instance.mGameCamera.SetTarget(((CharTransformData)(m_PlayerChar.GetTransformData())).GetGameObject().transform);

        //FingureGeture
        GestureManager.Instance.Initialize();
        GestureManager.Instance.EnableMoveChar();
	}
	public override void EndLogic()
	{
        m_PlayerChar.Distructor();
        UnRegisterMsg();
        LifeTickTask.Instance.UnRegisterFromUpdateList(Update);
        //clear map instance
        TerrainManager.Instance.CloseTerrain();
        //FingureGeture
        GestureManager.Instance.Clear();
	}
    private void RegisterMsg()
    {
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_HIT_LIFE, OnHitLife);
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_HIT_TERRAIN, OnHitTerrain);
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_START, OnActionStart);
        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
    }
    private void UnRegisterMsg()
    {
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_HIT_LIFE, OnHitLife);
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_HIT_TERRAIN, OnHitTerrain);
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_START, OnActionStart);
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
    }
    private void OnHitLife(MessageObject msg)
    {
        var transform = msg.msgValue as Transform;
        var playerBody =
            ((CharTransformData) (PlayerManager.Instance.GetPlayerInstance().GetTransformData())).GetGameObject()
                .transform;
        if (transform == playerBody)
        {
            return;
        }

        //Vector3 pos = ((Transform) (msg.msgValue)).position;
        //Debuger.Log("Move to target npc");
        //m_PlayerChar.MoveTo(pos);
    }
    private void OnHitTerrain(MessageObject msg)
    {
        //Vector3 pos = (Vector3)(msg.msgValue);
        //Debuger.Log("Move to target pos");
        //m_PlayerChar.MoveTo(pos);
    }
    private void OnActionStart(MessageObject msg)
    {
        GestureManager.Instance.DisableMoveChar();
    }
    private void OnActionFinish(MessageObject msg)
    {
        GestureManager.Instance.EnableMoveChar();
    }
    private void Update()
    {
        /*if (Input.touchCount > 0)
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
                m_Camera.transform.position = m_vInitCamPos + m_fDeltaDistance * -0.01f;
            }
        }*/
        /*if (Input.touchCount > 1)
        {
            if (Input.touches[1].phase == TouchPhase.Began)
            {
                Ray ray = m_Camera.ScreenPointToRay(Input.touches[1].position);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    m_PlayerChar.MoveTo(hitInfo.point);
                }
            }
        }*/

#if UNITY_EDITOR
       /* if (Input.GetMouseButtonDown(0))
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
            m_fDeltaDistance.z = m_fDeltaDistance.y ;
            m_fDeltaDistance.y = 0;
            m_Camera.transform.position = m_vInitCamPos + m_fDeltaDistance * -0.01f;
        }*/
       /* if (Input.GetMouseButtonDown(1))
        {
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                m_PlayerChar.MoveTo(hitInfo.point);
            }

        }*/
#endif
    }
    private void DoEnterScene()
    {
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, m_CurrentStage.EnterFuncId, null);
    }
    private void OnSceneWin()
    {
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, m_CurrentStage.WinLimitId, null);
    }
    private void OnSceneLose()
    {
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, m_CurrentStage.LoseFuncId, null);
    }
    public bool CanEnterScene()
    {
        CheckSceneStageConfig();
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        return LimitMethods.HandleLimitExec(target, m_CurrentStage.EnterLimitId, null);
    }
    private void CheckSceneStageConfig()
    {
        if (null == m_CurrentStage)
        {
            m_CurrentStage = ConfigManager.Instance.GetStageConfig(m_iCurrentStageId);
        }
    }

 
}
