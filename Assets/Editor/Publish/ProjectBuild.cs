using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

class ProjectBuild : Editor{
	
	//在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
	static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();
		
		foreach(EditorBuildSettingsScene e in EditorBuildSettings.scenes)
		{
			if(e==null)
				continue;
			if(e.enabled)
				names.Add(e.path);
		}
		return names.ToArray();
	}
	
    //得到项目的名称
	public static string projectName
	{
		get
		{ 
			//在这里分析shell传入的参数， 
			foreach(string arg in System.Environment.GetCommandLineArgs()) {
				if(arg.StartsWith("project"))
				{
					return arg.Split("-"[0])[1];
				}
			}
			return "test";
		}
	}
	//shell脚本直接调用这个静态方法
	static void BuildForIPhone()
	{         
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "USE_SHARE");
        //这里就是构建xcode工程的核心方法了， 
        //参数1 需要打包的所有场景
        //参数2 需要打包的名子
        //参数3 打包平台
		BuildPipeline.BuildPlayer(GetBuildScenes(), projectName, BuildTarget.iOS, BuildOptions.None);
	}
	//shell脚本直接调用这个静态方法
	static void BuildForAndroid()
	{ 
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "USE_SHARE");
		//这里就是构建xcode工程的核心方法了， 
		//参数1 需要打包的所有场景
		//参数2 需要打包的名子
		//参数3 打包平台
		BuildPipeline.BuildPlayer(GetBuildScenes(), projectName, BuildTarget.Android, BuildOptions.None);
	}
}
