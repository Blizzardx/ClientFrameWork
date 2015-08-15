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
    ReConnect,
}
public enum WindowID
{
    Loading,
    WindowTest1,
    WindowTest2,
    WindowTest3,
    Max,
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
        WindowManager.Instance.RegisterWindow(WindowID.WindowTest1, "Loading/UIWindow_test1", WindowLayer.Window, typeof(UIWindowTest1));
        WindowManager.Instance.RegisterWindow(WindowID.WindowTest2, "Loading/UIWindow_test2", WindowLayer.Window, typeof(UIWindowTest2));
        WindowManager.Instance.RegisterWindow(WindowID.WindowTest3, "Loading/UIWindow_test3", WindowLayer.Window, typeof(UIWindowTest3));
    }
    public static void RegisterStage()
    {
        StageManager.Instance.RegisterStage(GameStateType.LoginState, "Login",typeof(LoginLogic));
        StageManager.Instance.RegisterStage(GameStateType.MainCityState, "MainCity", typeof(MaincityStage));
        StageManager.Instance.RegisterStage(GameStateType.BattleState, "Battle", typeof(BattleStage));
        StageManager.Instance.RegisterStage(GameStateType.ReConnect, "Login", typeof(BattleStage));
    }
    public static  void DoCollection()
    {
        if (StageManager.Instance.GetCurrentGameStage() == GameStateType.LoginState)
        {
            return;
            // do nothing
        }

        

        //clear network layer
        NetWorkManager.Instance.RestSocketStatus();

        // clear common logic
        WindowManager.Instance.CloseAllWindow();

        //release assets
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}