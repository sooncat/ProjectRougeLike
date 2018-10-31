using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class BuildABUtils : Editor {

    [MenuItem("FWUtils/BuldAll")]
    public static void BuildAllTest()
    {
        BuildAllAssetbundle("Assets/StreamingAssets/AssetBundle");
        //BuildAllAssetbundle("Assets/StreamingAssets/AssetBundles_Uncompressed", BuildAssetBundleOptions.UncompressedAssetBundle);
        //BuildAllAssetbundle("Assets/StreamingAssets/AssetBundles_ChunkBased", BuildAssetBundleOptions.ChunkBasedCompression);

        Debug.Log("Build End");
        AssetDatabase.Refresh();
    }

    //[MenuItem("FWUtils/BuldSelectedTest")]
    //public static void BuildSelectTest()
    //{   
    //    BuildSelectedAssetBundle("Assets/StreamingAssets/AssetBundles", "res/wings");
    //    Debug.Log("Build End");
    //    AssetDatabase.Refresh();
    //}
    
    /// <summary>
    /// 将所有设置过AssetBundle属性的资源打包
    /// </summary>
    /// <param name="destPath"></param>
    /// <param name="options"></param>
    public static void BuildAllAssetbundle(string destPath, 
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None)
    {
        CheckDir(destPath);
        BuildPipeline.BuildAssetBundles(destPath, options, 
            EditorUserBuildSettings.activeBuildTarget);
    }

    /// <summary>
    /// 打包选中的资源
    /// </summary>
    /// <param name="destPath"></param>
    /// <param name="assetBundleName"></param>
    /// <param name="options"></param>
    public static void BuildSelectedAssetBundle(string destPath, 
        string assetBundleName,
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None)
    {
        CheckDir(destPath);

        Object[] selects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        string[] asset = new string[selects.Length];
        for (int i = 0; i < selects.Length; i++)
        {
            //获得选择 对象的路径
            asset[i] = AssetDatabase.GetAssetPath(selects[i]);
        }

        List<AssetBundleBuild> abBuilds = new List<AssetBundleBuild>();
        AssetBundleBuild abBuild = new AssetBundleBuild();
        abBuild.assetBundleName = assetBundleName;
        abBuild.assetNames = asset;
        abBuilds.Add(abBuild);

        BuildPipeline.BuildAssetBundles(destPath, abBuilds.ToArray(), options,
            EditorUserBuildSettings.activeBuildTarget);
    }

    /// <summary>
    /// 完全自定义的打包
    /// </summary>
    /// <param name="destPath"></param>
    /// <param name="abBuilds"></param>
    /// <param name="options"></param>
    public static void BuildCustomAssetBundel(string destPath,
        AssetBundleBuild[] abBuilds,
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None)
    {
        CheckDir(destPath);
        BuildPipeline.BuildAssetBundles(destPath, abBuilds, options,
            EditorUserBuildSettings.activeBuildTarget);
    }

    static void CheckDir(string destPath)
    {
        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
            Debug.Log("CreateDirectory " + destPath);
        }
    }

    static void LabelAllAssetbundle()
    {
        
    }
}
