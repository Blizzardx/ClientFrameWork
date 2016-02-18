using UnityEngine;
using System.Collections;
public enum GameStateType
{
    //logic
    none,
    LoginState,
    MainCityState,
    BattleState,
    ReConnectState,
    SelectSceneState,

    //node
    RegularityState,
    RegularityAlphaState,
	ArithmeticState,
    FireworkState,
	FireworkGuideState,
    Regularity2DStage,
	FlightState,
    MusicGameState,
    DrawGameState,
    RatioGameState,
    RunnerGameState,
    MessageTreeGameState,

    //world
    Copenhagen01State,
    Copenhagen02State,
    Copenhagen03State,
    ShaoLinsi01State,
    ShaoLinsi02State,
    ShaoLinsi03State,
    YuLin01Stage,
    DaBaoJiao01Stage,
    DiShini01Stage,
    DiShini02Stage,
    DaJieJuStage,
    TestPreFlightStage,
}
public class WindowID
{
    public const int Loading = 0;
    public const int WindowTest1 = 1;
    public const int WindowTest2 = 2;
    public const int WindowTest3 = 3;
    public const int WindowProject1 = 4;
    public const int Login = 5;
    public const int Register = 6;
    public const int Alert = 7;
    public const int AssetUpdate = 8;
    public const int CreateChar = 9;
    public const int AIAttach = 10;
    public const int Regularity = 11;
    public const int SelectScene = 12;
    public const int MissionDebugAttach = 13;
    public const int Firework = 14;
    public const int Regularity2D = 15;
	public const int Flight = 16;
    public const int ChangeScene = 17;
    public const int StoryBg = 18;
    public const int WinPanel = 19;
    public const int LosePanel = 20;
    public const int MsgTreeBuy = 21;
    public const int MsgTreeSell = 22;
    public const int MsgTreeSelectPanel = 23;
    public const int ChangeScene01 = 24;
    public const int UIHead= 25;
    public const int UIGM = 26;
    public const int ChangeScene02 = 27;
    public const int ChangeScene03 = 28;
public const int GuideMsgTreeBuy = 29;
    public const int GuideMsgTreeSell = 30;	
public const int FireworkGuide = 31;
}

public enum WindowLayer
{
    Window,
    Tip ,
    Max,
}
public enum GameLogicSceneType
{
    SceneEnd = 1,
    TalkEnd = 2,
    ActionEnd = 3,
    Login = 4,
    Register = 5,
    CreateChar = 6,
}
public class Definer
{
    public static void RegisterWindow()
    {
        WindowManager.Instance.RegisterWindow(WindowID.Loading, "Loading/UIWindow_Loading", WindowLayer.Window, typeof(UIWindowLoading));
        //WindowManager.Instance.RegisterWindow(WindowID.WindowTest1, "Window/UIWindow_test1", WindowLayer.Window, typeof(UIWindowTest1));
        //WindowManager.Instance.RegisterWindow(WindowID.WindowTest2, "Window/UIWindow_test2", WindowLayer.Window, typeof(UIWindowTest2));
        //WindowManager.Instance.RegisterWindow(WindowID.WindowTest3, "Window/UIWindow_test3", WindowLayer.Window, typeof(UIWindowTest3));
        WindowManager.Instance.RegisterWindow(WindowID.WindowProject1, "Project1/UIWindow_Scene1", WindowLayer.Window, typeof(UIWindowProject1));
        WindowManager.Instance.RegisterWindow(WindowID.Alert, "Tip/UIWindow_Alert", WindowLayer.Tip, typeof(UIWindowAlert));
        WindowManager.Instance.RegisterWindow(WindowID.AssetUpdate, "AssetUpdate/UIWindow_AssetUpdate", WindowLayer.Window, typeof(UIWindowAssetUdpate));
        WindowManager.Instance.RegisterWindow(WindowID.Login, "Login/UIWindow_Login", WindowLayer.Window, typeof(UIWindowLogin));
        WindowManager.Instance.RegisterWindow(WindowID.Register, "Login/UIWindow_Register", WindowLayer.Window, typeof(UIWindowRegister));
        WindowManager.Instance.RegisterWindow(WindowID.CreateChar, "Login/UIWindow_CreateChar", WindowLayer.Window, typeof(UIWindowCreateChar));
        WindowManager.Instance.RegisterWindow(WindowID.AIAttach, "AI/UIWindow_RuntimeAIAttack", WindowLayer.Window, typeof(UIWindowAIDebugAttach));
        WindowManager.Instance.RegisterWindow(WindowID.Regularity, "Regularity/UIWindow_Regularity", WindowLayer.Window, typeof(UIWindowRegularity));
        WindowManager.Instance.RegisterWindow(WindowID.SelectScene, "SelectScene/UIWindow_SelectScene", WindowLayer.Window, typeof(UIWindowSelectScene));
        WindowManager.Instance.RegisterWindow(WindowID.MissionDebugAttach, "Mission/UIWindow_MissionDebugAttach", WindowLayer.Window, typeof(UIWindowMissionDebugAttach));
        WindowManager.Instance.RegisterWindow(WindowID.Firework, "Firework/UIWindow_Firework", WindowLayer.Window, typeof(UIWindowFirework));
        WindowManager.Instance.RegisterWindow(WindowID.Regularity2D, "Regularity/UIWindow_Regularity2D", WindowLayer.Window, typeof(UIWindowRegularity2D));
        WindowManager.Instance.RegisterWindow(WindowID.ChangeScene, "ChangeScene/UIWindow_ChangeScene", WindowLayer.Window, typeof(UIWindowChangeScene));
        WindowManager.Instance.RegisterWindow(WindowID.StoryBg, "ChangeScene/UIWindow_StoryBg", WindowLayer.Window, typeof(UIWindowChangeScene));
        WindowManager.Instance.RegisterWindow(WindowID.WinPanel, "Common/UIWindow_Win", WindowLayer.Tip, typeof(UIWindowWin));
        WindowManager.Instance.RegisterWindow(WindowID.LosePanel, "Common/UIWindow_Lose", WindowLayer.Tip, typeof(UIWindowLose));
        WindowManager.Instance.RegisterWindow(WindowID.MsgTreeBuy, "MessageTree/UIWindow_MsgTreeBuy", WindowLayer.Window, typeof(UIWindowMsgTreeBuy));
        WindowManager.Instance.RegisterWindow(WindowID.MsgTreeSell, "MessageTree/UIWindow_MsgTreeSell", WindowLayer.Window, typeof(UIWindowMsgTreeSell));
        WindowManager.Instance.RegisterWindow(WindowID.MsgTreeSelectPanel, "MessageTree/UIWindow_MsgTreeSellerSellect", WindowLayer.Window, typeof(UIWindowMsgTreeSelectPanel));
        WindowManager.Instance.RegisterWindow(WindowID.GuideMsgTreeBuy, "MessageTree/UIWindow_GuideMsgTreeBuy", WindowLayer.Window, typeof(UIWindowGuideMsgTreeBuy));
        WindowManager.Instance.RegisterWindow(WindowID.GuideMsgTreeSell, "MessageTree/UIWindow_GuideMsgTreeSell", WindowLayer.Window, typeof(UIWindowguideMsgTreeSell));
        WindowManager.Instance.RegisterWindow(WindowID.ChangeScene01, "ChangeScene/UIWindow_ChangeScene01", WindowLayer.Window, typeof(UIWindowChangeScene));
        WindowManager.Instance.RegisterWindow(WindowID.ChangeScene02, "ChangeScene/UIWindow_ChangeScene02", WindowLayer.Window, typeof(UIWindowChangeScene));
        WindowManager.Instance.RegisterWindow(WindowID.ChangeScene03, "ChangeScene/UIWindow_ChangeScene03", WindowLayer.Window, typeof(UIWindowChangeScene));
        WindowManager.Instance.RegisterWindow(WindowID.UIHead, "ChangeScene/UIWindow_head", WindowLayer.Window, typeof(UIWindowChangeScene));
        WindowManager.Instance.RegisterWindow(WindowID.UIGM, "GM/UIWindow_GM", WindowLayer.Window, typeof(UIWindowGM));
		WindowManager.Instance.RegisterWindow(WindowID.FireworkGuide, "Firework/UIWindow_FireworkGuide", WindowLayer.Window, typeof(UIWindowFireworkGuide));
    }
    public static void RegisterStage()
    {
        //logic scene
        StageManager.Instance.RegisterStage(GameStateType.LoginState, "Login", typeof(LoginStage));
        StageManager.Instance.RegisterStage(GameStateType.ReConnectState, "Login", typeof(ReconnectStage));
        StageManager.Instance.RegisterStage(GameStateType.SelectSceneState, "SelectScene", typeof(SelectSceneStage));

        //node game scene
        StageManager.Instance.RegisterStage(GameStateType.RegularityState, "20_RegularityGame", typeof(RegularityStage)); 
        StageManager.Instance.RegisterStage(GameStateType.RegularityAlphaState, "20_RegularityGameAlpha", typeof(RegularityAlphaStage)); 
        StageManager.Instance.RegisterStage(GameStateType.ArithmeticState, "30_ArithmeticGame", typeof(ArithmeticStage));
        StageManager.Instance.RegisterStage(GameStateType.FireworkState, "FireworkGame", typeof(FireworkStage));
		StageManager.Instance.RegisterStage(GameStateType.FireworkGuideState, "FireworkGameGuide", typeof(FireworkGuideStage));
        StageManager.Instance.RegisterStage(GameStateType.Regularity2DStage, "20_Regularity2DGame", typeof(Regularity2DStage));
        StageManager.Instance.RegisterStage(GameStateType.FlightState, "40_FlightGames", typeof(FlightStage));
        StageManager.Instance.RegisterStage(GameStateType.MusicGameState, "60_MusicGame", typeof(MusicGameStage));
        StageManager.Instance.RegisterStage(GameStateType.DrawGameState, "DrawSomething", typeof(DrawGameStage));
        StageManager.Instance.RegisterStage(GameStateType.RatioGameState, "10_RatioGame", typeof(RatioGameStage));
        StageManager.Instance.RegisterStage(GameStateType.RunnerGameState, "RunnerMan", typeof(RunnerGameStage));
        StageManager.Instance.RegisterStage(GameStateType.MessageTreeGameState, "MessageTree", typeof(MessageTreeStage));

        //world scene
        StageManager.Instance.RegisterStage(GameStateType.Copenhagen01State, "Copenhagen 01", typeof(Copenhagen01Stage));
        StageManager.Instance.RegisterStage(GameStateType.Copenhagen02State, "Copenhagen 02", typeof(Copenhagen02Stage));
        StageManager.Instance.RegisterStage(GameStateType.Copenhagen03State, "Copenhagen 03", typeof(Copenhagen03Stage));
        StageManager.Instance.RegisterStage(GameStateType.ShaoLinsi01State, "ShaoLinSi01", typeof(ShaoLinsi01Stage));
        StageManager.Instance.RegisterStage(GameStateType.ShaoLinsi02State, "ShaoLinSi02", typeof(ShaoLinsi02Stage));
        StageManager.Instance.RegisterStage(GameStateType.ShaoLinsi03State, "ShaoLinSi03", typeof(ShaoLinsi03Stage));
        StageManager.Instance.RegisterStage(GameStateType.YuLin01Stage, "YuLin01", typeof(YuLinStage));
        StageManager.Instance.RegisterStage(GameStateType.DaBaoJiao01Stage, "DaBaoJiao01", typeof(DaBaoJiaoStage));
        StageManager.Instance.RegisterStage(GameStateType.DiShini01Stage, "DiSiNi01", typeof(DiShini01Stage));
        StageManager.Instance.RegisterStage(GameStateType.DiShini02Stage, "DiSiNi02", typeof(DiShini02Stage));
        StageManager.Instance.RegisterStage(GameStateType.DaJieJuStage, "Copenhagen 01", typeof(DaJieJuStage));
        StageManager.Instance.RegisterStage(GameStateType.TestPreFlightStage, "Copenhagen 01", typeof(TestPreFlightStage));
    }
    public static  void DoCollection()
    {
        if (StageManager.Instance.GetCurrentGameStage() == GameStateType.LoginState)
        {
            return;
            // do nothing
        }
        
        //release logic 
        
        
    }
}