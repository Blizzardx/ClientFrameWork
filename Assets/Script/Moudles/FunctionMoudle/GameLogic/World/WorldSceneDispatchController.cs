using Cache;
using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainEditor;
using UnityEngine;

public class PlayerProcessData
{
    public class TransformData
    {
        public Vector3 pos;
        public Vector3 rot;
    }
    public GameStateType        m_Type;
    public GameStateType        m_TerrainDataType;
    public bool                 m_bIsResumeScene;
    public TransformData        m_PlayerTransform;
    public int                  m_ActionId;
    public int                  m_iExitSceneFuncId;
    public int                  m_iExitSceneStep2FuncId;
    public Dictionary<int, TransformData> m_NpcTransformList;
}
public class WorldSceneDispatchController : LogicBase<WorldSceneDispatchController>
{
    private bool            m_bIsDebugMode;
    private PlayerProcessData m_ProcessData;
    private PlayerProcessInfo m_ProcessInfo;

    public override void StartLogic()
    {
        m_bIsDebugMode = AppManager.Instance.m_bIsDebugMode;
        LoadParam();
        RegisterMsg();
    }
    public override void EndLogic()
    {
        UnRegisterMsg();
    }
    public void EnterWorldScene()
    {
        if (m_bIsDebugMode )
        {
            StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
        }
        else
        {
            //TEST CODE
            if (m_ProcessData.m_Type == GameStateType.none)
            {
                WindowManager.Instance.OpenWindow(WindowID.Loading);
                //m_ProcessData.m_Type = GameStateType.Copenhagen01State;
                MessageDispatcher.Instance.BroadcastMessage(new MessageObject(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, 0));
                //SaveParam();
            }
            else
            {
                StageManager.Instance.ChangeState(m_ProcessData.m_Type);
            }
        }
    }
    public PlayerProcessData GetParam()
    {
        return m_ProcessData;
    }
    public void SetExitFunId(int id,int step2FuncId)
    {
        m_ProcessData.m_iExitSceneFuncId = id;
        m_ProcessData.m_iExitSceneStep2FuncId = step2FuncId;
        SaveParam();
    }
    public void ExecuteExitNodeGame()
    {
        if(UIWindowSelectScene.m_iIndex != -1)
        {
            UIWindowSelectScene.OnExitFlightGame();
            return;
        }
        if(m_ProcessData.m_iExitSceneFuncId != 0)
        {
            HandleTarget target = HandleTarget.GetHandleTarget(PlayerManager.Instance.GetPlayerInstance());
            FuncMethods.HandleFuncExec(target, m_ProcessData.m_iExitSceneFuncId, null);
            //m_ProcessData.m_iExitSceneFuncId = 0;
        }      
        else
        {
            StageManager.Instance.ChangeState(GameStateType.SelectSceneState);
        }  
    }
    private void RegisterMsg()
    {
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_PLAY_ACTION, OnActionStart);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_CHANGE_TO_NODE_GAME, OnChangeToNodeGame);
        MessageDispatcher.Instance.RegistMessage(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, OnChangetoWorldGame);
    }
    private void UnRegisterMsg()
    {
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_START, OnActionStart);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_CHANGE_TO_NODE_GAME, OnChangeToNodeGame);
        MessageDispatcher.Instance.UnregistMessage(ClientCustomMessageDefine.C_CHANGE_TO_WORLD_GAME, OnChangetoWorldGame);
    }
    private void OnActionStart(MessageObject obj)
    {
        if (!(obj.msgValue is int))
        {
            return;
        }

        //get id
        int id = (int)(obj.msgValue);
        SaveSceneInfo(GameStateType.none, id, StageManager.Instance.GetCurrentGameStage() != GameStateType.SelectSceneState && StageManager.Instance.GetCurrentGameStage() != GameStateType.LoginState);
    }
    private void OnActionFinish(MessageObject msg)
    {
        SaveSceneInfo(GameStateType.none, -1, StageManager.Instance.GetCurrentGameStage() != GameStateType.SelectSceneState && StageManager.Instance.GetCurrentGameStage() != GameStateType.LoginState);
    }
    private void OnChangeToNodeGame(MessageObject msg)
    {
        if (!(msg.msgValue is int))
        {
            return;
        }
        GameStateType state = GameStateType.none;

        int id = (int)(msg.msgValue);
        switch (id)
        {
            case 0:
                state = GameStateType.FlightState;
                break;
            case 1:
                state = GameStateType.RunnerGameState;
                break;
            case 2:
                state = GameStateType.ArithmeticState;
                break;
            case 3:
                state = GameStateType.RegularityAlphaState;
                break;
            case 4:
                if(PlayerManager.Instance.GetCharCounterData().GetFlag(7))
                {
                    state = GameStateType.FireworkState;
                }
                else
                {
                    state = GameStateType.FireworkGuideState;
                    //state = GameStateType.FireworkState;
                }
                break;
            case 5:
                state = GameStateType.RatioGameState;
                break;
            case 6:
                state = GameStateType.DrawGameState;
                break;
            case 7:
                state = GameStateType.MusicGameState;
                break;
            case 8:
                state = GameStateType.MessageTreeGameState;
                break;
            case 9:
                state = GameStateType.SelectSceneState;
                break;
        }

        if (state != GameStateType.none)
        {
            SaveSceneInfo(state,-1, StageManager.Instance.GetCurrentGameStage() != GameStateType.SelectSceneState && StageManager.Instance.GetCurrentGameStage() != GameStateType.LoginState);            
            StageManager.Instance.ChangeState(state);
        }
    }
    private void OnChangetoWorldGame(MessageObject msg)
    {
        if (!(msg.msgValue is int))
        {
            return;
        }
        int id = (int)(msg.msgValue);
        GameStateType state = GameStateType.none;
        switch (id)
        {
            case 0:
                state = GameStateType.Copenhagen01State;
                break;
            case 1:
                state = GameStateType.Copenhagen02State;
                break;
            case 2:
                state = GameStateType.Copenhagen03State;
                break;
            case 3:
                state = GameStateType.DiShini01Stage;
                break;
            case 4:
                state = GameStateType.DiShini02Stage;
                break;
            case 5:
                state = GameStateType.ShaoLinsi01State;
                break;
            case 6:
                state = GameStateType.ShaoLinsi02State;
                break;
            case 7:
                state = GameStateType.ShaoLinsi03State;
                break;
            case 8:
                state = GameStateType.DaBaoJiao01Stage;
                break;
            case 9:
                state = GameStateType.YuLin01Stage;
                break;
            case 10:
                state = GameStateType.DaJieJuStage;
                break;
        }
        if (state != GameStateType.none)
        {
            SaveSceneInfo(state,-1, StageManager.Instance.GetCurrentGameStage() != GameStateType.SelectSceneState && StageManager.Instance.GetCurrentGameStage() != GameStateType.LoginState);
            StageManager.Instance.ChangeState(state);
        }
    }
    private void SaveSceneInfo(GameStateType state,int actionId,bool isResumeScene)
    {
        //test code
        if(UIWindowSelectScene.m_iIndex != -1)
        {
            return;
        }

        if (state != GameStateType.none)
        {
            m_ProcessData.m_Type = state;
        }
        m_ProcessData.m_ActionId = actionId;

        if (null != PlayerManager.Instance.GetPlayerInstance())
        {
            m_ProcessData.m_TerrainDataType = StageManager.Instance.GetCurrentGameStage();
            m_ProcessData.m_PlayerTransform.pos = PlayerManager.Instance.GetPlayerInstance().GetTransformData().GetPosition();
            m_ProcessData.m_PlayerTransform.rot = PlayerManager.Instance.GetPlayerInstance().GetTransformData().GetRotation();


            //set npc player
            m_ProcessData.m_NpcTransformList.Clear();
            if (null != TerrainManager.Instance.GetNpcList())
            {
                foreach (var elem in TerrainManager.Instance.GetNpcList())
                {
                    int id = elem.Key;
                    PlayerProcessData.TransformData npc = new PlayerProcessData.TransformData();

                    npc.pos = elem.Value.GetTransformData().GetPosition();
                    npc.rot = elem.Value.GetTransformData().GetRotation();

                    m_ProcessData.m_NpcTransformList.Add(elem.Key, npc);
                }
            }
        }
        m_ProcessData.m_bIsResumeScene = isResumeScene;
        if(m_ProcessData.m_bIsResumeScene )
        {
            //check type
            m_ProcessData.m_bIsResumeScene = state == m_ProcessData.m_TerrainDataType;
        }
        if(m_bIsDebugMode)
        {
            m_ProcessData.m_bIsResumeScene = false;
        }
        SaveParam();
    }
    private void LoadParam()
    {
        //load info from local cache        
        byte[] obj = PlayerManager.Instance.GetCharBaseData().CharDeatail;

        m_ProcessInfo = new PlayerProcessInfo();
        m_ProcessData = new PlayerProcessData();
       

        if (null != obj)
        {
            ThriftSerialize.DeSerialize(m_ProcessInfo, obj);
            ChangeInfoToData(m_ProcessInfo, m_ProcessData);
        }
        else
        {
            m_ProcessData.m_PlayerTransform = new PlayerProcessData.TransformData();
            m_ProcessData.m_NpcTransformList = new Dictionary<int, PlayerProcessData.TransformData>();
        }
        if (m_bIsDebugMode)
        {
            m_ProcessData.m_bIsResumeScene = false;
        }
    }
    private void SaveParam()
    {
        ChangeDataToInfo(m_ProcessInfo, m_ProcessData);
        //load info from local cache
        byte[] obj = ThriftSerialize.Serialize(m_ProcessInfo);
        PlayerManager.Instance.GetCharBaseData().CharDeatail = obj;
    }
    private void ChangeInfoToData(PlayerProcessInfo info, PlayerProcessData data)
    {
        data.m_Type = (GameStateType)(info.StageType);
        if(data.m_Type == GameStateType.none)
        {
            m_ProcessData.m_PlayerTransform = new PlayerProcessData.TransformData();
            m_ProcessData.m_NpcTransformList = new Dictionary<int, PlayerProcessData.TransformData>();
        }
        else
        {
            data.m_TerrainDataType = (GameStateType)(info.TerrainDataType);
            data.m_ActionId = info.ActionId;
            data.m_bIsResumeScene = info.IsResumeScene;
            data.m_iExitSceneFuncId = info.ExitSceneFuncId;
            data.m_PlayerTransform = new PlayerProcessData.TransformData();
            if(info.PlayerTransformInfo != null)
            {
                if (info.PlayerTransformInfo.Pos != null)
                {
                    data.m_PlayerTransform.pos = info.PlayerTransformInfo.Pos.GetVector3();
                }
                if (info.PlayerTransformInfo.Rot != null)
                {
                    data.m_PlayerTransform.rot = info.PlayerTransformInfo.Rot.GetVector3();
                }
            }
            m_ProcessData.m_NpcTransformList = new Dictionary<int, PlayerProcessData.TransformData>();
            foreach (var elem in m_ProcessInfo.NpcTransformInfoList)
            {
                PlayerProcessData.TransformData transformData = new PlayerProcessData.TransformData();
                transformData.pos = elem.Value.Pos.GetVector3();
                transformData.rot = elem.Value.Rot.GetVector3();
                m_ProcessData.m_NpcTransformList.Add(elem.Key, transformData);
            }
        }
        
    }
    private void ChangeDataToInfo(PlayerProcessInfo info, PlayerProcessData data)
    {
        info.StageType = (int)(data.m_Type);
        if(data.m_Type != GameStateType.none)
        {
        }
        info.TerrainDataType = (int)(data.m_TerrainDataType);
        info.ActionId = data.m_ActionId;
        info.IsResumeScene = data.m_bIsResumeScene;
        info.ExitSceneFuncId = data.m_iExitSceneFuncId;
        info.PlayerTransformInfo = new ThriftTransformData();
        info.PlayerTransformInfo.Pos = new Common.Auto.ThriftVector3();
        info.PlayerTransformInfo.Pos.SetVector3(data.m_PlayerTransform.pos);
        info.PlayerTransformInfo.Rot = new Common.Auto.ThriftVector3();
        info.PlayerTransformInfo.Rot.SetVector3(data.m_PlayerTransform.rot);

        info.NpcTransformInfoList = new Dictionary<int, ThriftTransformData>();
        foreach(var elem in data.m_NpcTransformList)
        {
            ThriftTransformData tmpData = new ThriftTransformData();
            tmpData.Pos = new Common.Auto.ThriftVector3();
            tmpData.Rot = new Common.Auto.ThriftVector3();
            tmpData.Pos.SetVector3(elem.Value.pos);
            tmpData.Rot.SetVector3(elem.Value.rot);
            info.NpcTransformInfoList.Add(elem.Key, tmpData);
        }
        
    }
}
