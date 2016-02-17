using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Run;

public class RunnerGameEditorWindow : EditorWindow
{
    static public RunnerGameEditorWindow Instance
    {
        get
        {
            if (null == m_MainWnd)
            {
                CreateWindow();
            }

            return m_MainWnd;
        }
    }

    private static RunnerGameEditorWindow m_MainWnd;
    //readonly
    private readonly float WINDOW_MIN_WIDTH = 1120f;
    private readonly float WINDOW_MIN_HIEGHT = 290f;
    //Editor Info

    //Editor State

    //Editor Data
    private string m_TrunkDataPath;
    //Trunk Data

    #region Public
    [MenuItem("Editors/RunnerGame/道路编辑器")]
    public static void CreateWindow()
    {
        m_MainWnd = EditorWindow.GetWindow<RunnerGameEditorWindow>(false, "道路编辑器", true);
        m_MainWnd.Init();
    }
    [MenuItem("Editors/RunnerGame/选择道路文件目录")]
    private static void OpenDataPath()
    {
        m_MainWnd.m_TrunkDataPath = EditorUtility.OpenFilePanel("选择道路文件", "", "xml");
    }
    #endregion

    #region Mono
    //private void  Update()
    //{

    //}
    private void OnGUI()
    {

    }
    #endregion

    #region Event
    private void Init()
    {
        // Check

        // SetUp
        m_MainWnd.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HIEGHT);
    }
    private void OnPlay()
    {
       
    }
    #endregion

    #region Method
    
    #endregion
}
