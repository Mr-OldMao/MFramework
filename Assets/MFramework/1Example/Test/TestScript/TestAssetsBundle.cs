using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：测试加载卸载 AssetsBundle
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class TestAssetsBundle : MonoBehaviour
    {

#if UNITY_EDITOR
        public string buildABPath = Application.dataPath + "/BuildAssetBundle";

        private void Start()
        {
            BuildAssetBundle(buildABPath);


        }

        private void BuildAssetBundle(string buildPath)
        {
            Debug.Log("BuildAB  Path：" + buildPath);
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            UnityEditor.BuildPipeline.BuildAssetBundles(buildPath, UnityEditor.BuildAssetBundleOptions.None, UnityEditor.BuildTarget.StandaloneWindows);
        }

        private void LoadRes()
        {

        }

#endif

    }
}