#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MFramework.Editor
{
    /// <summary>
    /// 标题：Unity编辑器菜单
    /// 作者：毛俊峰
    /// 时间：2022.08.05
    /// 版本：1.0
    /// </summary>
    public class EditorMenu
    {
        /// <summary>
        /// 脚本自动化默认路径
        /// </summary>
        public static string autoCreateScriptDefaultPath = Application.dataPath + "/ScriptAuto/";

        [MenuItem("MFramework/自动打包框架UnityPackage包     %e")]
        private static void MenuClicked1()
        {
            string packageName = "MFramework_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".unitypackage";
            //准备导出unitypackage资源路径
            string exportPathName = "Assets/MFramework";
            EditorAutoBuildUnityPackage.ExportUnityPactage(packageName, exportPathName);
        }

        [MenuItem("MFramework/AssetsBundle/ExportAssetsBundle")]
        private static void BuildAssetBundle()
        {
            string buildPath = ABSetting.assetBundleBuildPath;
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }

        [MenuItem("MFramework/脚本自动化工具/1.场景游戏对象映射实体类", false, 1)]
        private static void EditorMenu_EditorAutoCreateScriptSceneObj()
        {
            EditorSceneObjMapEntity.CreateWizard();
        }

        [MenuItem("MFramework/脚本自动化工具/2.Json反序列化实体类自动生成工具", false, 2)]
        private static void EditorMenu_EditorAutoCreateScriptJsonMapClass()
        {
            EditorJsonMapEntity.CreateWizard();
        }
    }
}

#endif