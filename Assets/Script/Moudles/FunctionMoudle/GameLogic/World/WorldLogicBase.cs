using Config;
using RunnerGame;
using UnityEngine;
using System.Collections;
using NetWork.Auto;
using System;

public abstract class WorldLogicBase
{
    protected PlayerCharacter   m_PlayerChar;
    protected StageConfig       m_CurrentStage;


    public virtual void InitLogic()
    {
        if(WorldSceneDispatchController.Instance.GetParam().m_bIsResumeScene)
        {
            ResumeScene();
        }
        else
        {
            InitScene();
        }
    }
    public virtual void EndLogic()
    {
        CloseScene();
    }

    public virtual void StartLogic()
    {
        Debug.Log("close loading window");
        WindowManager.Instance.CloseWindow(WindowID.Loading);
    }
    abstract public int GetSceneId();
    abstract public GameStateType GetSceneStageId();
    public bool CanEnterScene()
    {
        CheckSceneStageConfig();
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        return LimitMethods.HandleLimitExec(target, m_CurrentStage.EnterLimitId, null);
    }
    virtual protected void InitScene()
    {
        CheckSceneStageConfig();

        PlayerManager.Instance.CreatePlayerChar();
        m_PlayerChar = PlayerManager.Instance.GetPlayerInstance();

        TerrainManager.Instance.InitializeTerrain(m_CurrentStage.StageMapId, AppManager.Instance.m_bIsShowTerrainTrigger);
        m_PlayerChar.GetTransformData().SetPosition(TerrainManager.Instance.GetPlayerInitPos().Pos.GetVector3());
        m_PlayerChar.GetTransformData().SetRotation(TerrainManager.Instance.GetPlayerInitPos().Rot.GetVector3());

        //trigger enter scene
        DoEnterScene();

        RegisterMsg();

        //Setup MainCam
        GlobalScripts.Instance.Initialize();
        GlobalScripts.Instance.mGameCamera.SetTarget(((CharTransformData)(m_PlayerChar.GetTransformData())).GetGameObject().transform);

        //FingureGeture
        GestureManager.Instance.Initialize();
        GestureManager.Instance.EnableMoveChar();
    }
    virtual protected void CloseScene()
    {
        PlayerManager.Instance.ClearPlayer();
        UnRegisterMsg();
        //clear map instance
        TerrainManager.Instance.CloseTerrain();
        //FingureGeture
        GestureManager.Instance.Clear();
    }
    virtual protected void ResumeScene()
    {
        CheckSceneStageConfig();

        PlayerManager.Instance.CreatePlayerChar();
        m_PlayerChar = PlayerManager.Instance.GetPlayerInstance();

        TerrainManager.Instance.InitializeTerrain(m_CurrentStage.StageMapId, AppManager.Instance.m_bIsShowTerrainTrigger);
        m_PlayerChar.GetTransformData().SetPosition(WorldSceneDispatchController.Instance.GetParam().m_PlayerTransform.pos);
        m_PlayerChar.GetTransformData().SetRotation(WorldSceneDispatchController.Instance.GetParam().m_PlayerTransform.rot);

        //set npc player
        foreach(var elem in WorldSceneDispatchController.Instance.GetParam().m_NpcTransformList)
        {
            int id = elem.Key;
            Npc npc = null;
            if(TerrainManager.Instance.GetNpcList().TryGetValue(id,out npc))
            {
                if(null != npc)
                {
                    npc.GetTransformData().SetPosition(elem.Value.pos);
                    npc.GetTransformData().SetRotation(elem.Value.rot);
                }
            }
        }

        RegisterMsg();

        //Setup MainCam
        GlobalScripts.Instance.Initialize();
        GlobalScripts.Instance.mGameCamera.SetTarget(((CharTransformData)(m_PlayerChar.GetTransformData())).GetGameObject().transform);

        //FingureGeture
        GestureManager.Instance.Initialize();
        GestureManager.Instance.EnableMoveChar();

        if(WorldSceneDispatchController.Instance.GetParam().m_ActionId != -1)
        {
            //trigger to play action
            MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_PLAY_ACTION, WorldSceneDispatchController.Instance.GetParam().m_ActionId));
        }

        DoResumeScene();
    }
    protected void RegisterMsg()
    {
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_START, OnActionStart);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish); 
    }
    protected void UnRegisterMsg()
    {
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_START, OnActionStart);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
    }
    protected void OnActionStart(MessageObject msg)
    {
        GestureManager.Instance.DisableMoveChar();
    }
    protected void OnActionFinish(MessageObject msg)
    {
        GestureManager.Instance.EnableMoveChar();
    }
    protected void DoEnterScene()
    {
        //HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        //FuncMethods.HandleFuncExec(target, m_CurrentStage.EnterFuncId, null);
        Timer _timer = TimerCollection.GetInstance().Create(StartFuncMethod, true, null);
        _timer.Start(0.5f);
    }
    protected void DoResumeScene()
    {
        Timer _timer = TimerCollection.GetInstance().Create(StartResumeFuncMethod, true, null);
        _timer.Start(0.5f);
    }
    protected void OnSceneWin()
    {
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, m_CurrentStage.WinLimitId, null);
    }
    protected void OnSceneLose()
    {
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, m_CurrentStage.LoseFuncId, null);
    }
    protected void CheckSceneStageConfig()
    {
        if (null == m_CurrentStage)
        {
            m_CurrentStage = ConfigManager.Instance.GetStageConfig(GetSceneId());
        }
    }

    private void StartFuncMethod () 
    {
        //WindowManager.Instance.HideAllWindow();
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, m_CurrentStage.EnterFuncId, null);
    }
    private void StartResumeFuncMethod()
    {
        //WindowManager.Instance.HideAllWindow();
        HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
        FuncMethods.HandleFuncExec(target, WorldSceneDispatchController.Instance.GetParam().m_iExitSceneStep2FuncId, null);
    }
}