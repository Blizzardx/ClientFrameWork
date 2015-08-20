using UnityEngine;
using System.Collections;
public enum GameStateType
{
    none,
    /// <summary>
    /// 游戏场景状态  登录状态，
    /// </summary>
    LoginState,
    /// <summary>
    /// 主城状态 
    /// </summary>
    MainCityState,
    /// <summary>
    /// 战斗状态
    /// </summary>
    BattleState,
    /// <summary>
    /// 游戏重连状态
    /// </summary>
    TestProject1,
    TestProject2,
    ReConnect,
}
public class WindowID
{
    public const int Loading = 0;
    public const int WindowTest1 = 1;
    public const int WindowTest2 = 2;
    public const int WindowTest3 = 3;
    public const int Max = 4;
}
public enum WindowLayer
{
    Window,
    Tip ,
    Max,
}
public class Definer
{
    public static void RegisterWindow()
    {
        WindowManager.Instance.RegisterWindow(WindowID.Loading, "Loading/UIWindow_Loading", WindowLayer.Window, typeof(UIWindowLoading));
        WindowManager.Instance.RegisterWindow(WindowID.WindowTest1, "Window/UIWindow_test1", WindowLayer.Window, typeof(UIWindowTest1));
        WindowManager.Instance.RegisterWindow(WindowID.WindowTest2, "Window/UIWindow_test2", WindowLayer.Window, typeof(UIWindowTest2));
        WindowManager.Instance.RegisterWindow(WindowID.WindowTest3, "Window/UIWindow_test3", WindowLayer.Window, typeof(UIWindowTest3));
    }
    public static void RegisterStage()
    {
        StageManager.Instance.RegisterStage(GameStateType.LoginState, "Login", typeof(LoginStage));
        StageManager.Instance.RegisterStage(GameStateType.MainCityState, "MainCity", typeof(MaincityStage));
        StageManager.Instance.RegisterStage(GameStateType.BattleState, "Battle", typeof(BattleStage));
        StageManager.Instance.RegisterStage(GameStateType.ReConnect, "Login", typeof(ReconnectStage));
        StageManager.Instance.RegisterStage(GameStateType.TestProject1, "Main", typeof(TestProject1Stage));
        StageManager.Instance.RegisterStage(GameStateType.TestProject2, "Main", typeof(TestProject2Stage));
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