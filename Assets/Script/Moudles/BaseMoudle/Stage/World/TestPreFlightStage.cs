using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TestPreFlightStage : StageBase
{
    public TestPreFlightStage(GameStateType type)
    : base(type)
    {
    }
    public override void InitStage()
    {
        base.InitStage();

        PlayerManager.Instance.CreatePlayerChar();
        var m_PlayerChar = PlayerManager.Instance.GetPlayerInstance();

        TerrainManager.Instance.InitializeTerrain(101, AppManager.Instance.m_bIsShowTerrainTrigger);
        m_PlayerChar.GetTransformData().SetPosition(TerrainManager.Instance.GetPlayerInitPos().Pos.GetVector3());
        m_PlayerChar.GetTransformData().SetRotation(TerrainManager.Instance.GetPlayerInitPos().Rot.GetVector3());

        MessageManager.Instance.RegistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);

        //trigger enter scene
        DoEnterScene();
    }
    public override void StartStage()
    {
        WindowManager.Instance.CloseWindow(WindowID.Loading);
    }
    public override void EndStage()
    {
        MessageManager.Instance.UnregistMessage(ClientCustomMessageDefine.C_ACTION_FININSH, OnActionFinish);
        PlayerManager.Instance.ClearPlayer();
        //clear map instance
        TerrainManager.Instance.CloseTerrain();
    }
    protected void DoEnterScene()
    {
        Timer _timer = TimerCollection.GetInstance().Create(StartFuncMethod, true, null);
        _timer.Start(0.5f);
    }
    private void StartFuncMethod()
    {
       MessageManager.Instance.AddToMessageQueue(new MessageObject(ClientCustomMessageDefine.C_PLAY_ACTION, 1800001));
    }

    private void OnActionFinish(MessageObject obj)
    {
        StageManager.Instance.ChangeState(GameStateType.FlightState);
    }
}
