using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BuildScript
{
	static string kAssetBundlesOutputPath = "StreamingAssets\\AssetBundles";
    static string kAssetBundlesDestinationPath = "trunk\\Assets\\StreamingAssets\\AssetBundles";

    public static void BuildAssetBundles()
	{
		// Choose the output path according to the build target.
		string outputPath = Application.dataPath.ToLower () + "/StreamingAssets/AssetBundles/" + GetPlatformFolderForAssetBundles (EditorUserBuildSettings.activeBuildTarget);
		if (!Directory.Exists(outputPath) )
			Directory.CreateDirectory (outputPath);
		
		//SetVersionDirAssetName ("Resources");
		BuildPipeline.BuildAssetBundles (outputPath, 0, EditorUserBuildSettings.activeBuildTarget);

        // Copy AB Files To Dir
        CopyAssetBundlesToDir();
    }

#if UNITY_EDITOR
    public static string GetPlatformFolderForAssetBundles(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.WebPlayer:
                return "WebPlayer";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }
#endif

	public static string GetBuildTargetName(BuildTarget target)
	{
		switch(target)
		{
		case BuildTarget.Android :
			return "/test.apk";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "/test.exe";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "/test.app";
		case BuildTarget.WebPlayer:
		case BuildTarget.WebPlayerStreamed:
			return "";
			// Add more build targets for your own.
		default:
			Debug.Log("Target not implemented.");
			return null;
		}
	}

	static void CopyAssetBundlesToDir()
	{
		string outputFolder = GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget);

        var dir = new DirectoryInfo(Application.dataPath);    // 获取生成的资源Assets完整路径

        var source = System.IO.Path.Combine(Path.Combine(dir.FullName, kAssetBundlesOutputPath), outputFolder);
        if (!System.IO.Directory.Exists(source))
            Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

        dir = dir.Parent.Parent;                              // 获取客户端代码开始路径

        // Setup the destination folder for assetbundles.
        var destination = System.IO.Path.Combine(Path.Combine(dir.FullName, kAssetBundlesDestinationPath), outputFolder);
        if (System.IO.Directory.Exists(destination))
            FileUtil.DeleteFileOrDirectory(destination);

        FileUtil.CopyFileOrDirectory(source, destination);
    }
}
