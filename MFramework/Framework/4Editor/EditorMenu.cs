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





        [MenuItem("MFramework/脚本自动化工具/1.一键生成游戏对象映射实体类", false, 1)]
        private static void EditorMenu_EditorAutoCreateScriptSceneObj()
        {
            EditorSceneObjMapEntity.CreateWizard();
        }

        [MenuItem("MFramework/脚本自动化工具/2.Json反序列化实体类自动生成工具", false, 2)]
        private static void EditorMenu_EditorAutoCreateScriptJsonMapClass()
        {
            EditorJsonMapEntity.CreateWizard();
        }

        [MenuItem("MFramework/脚本自动化工具/3.选中脚本(需继承ScriptableObject)生成.asset文件", false, 3)]
        private static void EditorMenu_EditorAutoCreateAssetFile()
        {
            //右键脚本（脚本需要继承ScriptableObject）自动生成.asset文件 
            AutoCreateAssetFile.Create();
        }



        #region AB包
        //[MenuItem("MFramework/AB/ExportAssetsBundle")]
        //private static void BuildAssetBundle()
        //{
        //    string buildPath = ABSetting.assetBundleBuildPath;
        //    if (!Directory.Exists(buildPath))
        //    {
        //        Directory.CreateDirectory(buildPath);
        //    }
        //    BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        //}

        [MenuItem("MFramework/AB/打印项目所有ABName", false, 1)]
        private static void PrintAllABName()
        {
            string[] abNameArr = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < abNameArr.Length; i++)
            {
                Debug.Log((i + 1) + "：" + abNameArr[i]);
            }
        }

        [MenuItem("MFramework/AB/一键标记资源", false, 2)]
        private static void OneKeyABTag()
        {
            ABTag.AutoTagAB();
        }

        [MenuItem("MFramework/AB/一键生成被标记的资源AB", false, 3)]
        private static void OneKeyBuildAB()
        {
            ABBuild.BuildAssetBundle();
        }

        [MenuItem("MFramework/AB/(打整包)一键标记、打包所有(GameMain\\AB)资源", false, 0)]
        public static void BuildAB()
        {
            if (Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.Delete(Application.streamingAssetsPath, true);
                Debug.Log("已清空StreamingAssets文件夹");
            }

            //一键标记资源
            ABTag.AutoTagAB();
            Debug.Log("已完成一键标记资源");

            //一键打包ab资源
            string buildPath = ABSetting.assetBundleBuildPath;
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            Debug.Log("已完成一键打包ab资源");


            ////写入资源热更json文件数据
            //ResHotUpdateData resHotUpdateData = new ResHotUpdateData()
            //{
            //    version = PlayerSettings.bundleVersion,
            //    assetBundleNames = AssetDatabase.GetAllAssetBundleNames()
            //};
            //HotUpdateManager.WriteResHotUpdateData(resHotUpdateData);
            //Debug.Log("已完成写入资源热更json文件数据");

            //写入AB资源MD5信息 一份写入StreamingAssets，一份写入本地磁盘作为热更文件列表对比使用
            HotUpdateTool.WriteMD5(HotUpdateTool.WriteFileType.XML, buildPath, Application.streamingAssetsPath + "/Data");
            HotUpdateTool.WriteMD5(HotUpdateTool.WriteFileType.Json, buildPath, HotUpdateSetting.folderFullPath_MD5);
            Debug.Log("已完成写入整包ab资源信息(版本号、md5信息)");

            //写入服务器热更配置表
            HotUpdateTool.WriteServerConfigFile(HotUpdateTool.WriteFileType.XML, buildPath, Application.streamingAssetsPath + "/Data");
            Debug.Log("已写入服务器热更配置表");
            AssetDatabase.Refresh();
        }
        #endregion

        #region 热更流程

        [MenuItem("MFramework/热更流程/热更打包器")]
        public static void BuildHotUpdateBundleMethod()
        {
            BuildHotUpdateBundle.BuildHotUpdateBundleMethod();
        }


        /// <summary>
        /// 导出AB包MD5信息文件(.json)
        /// </summary>
        [MenuItem("MFramework/热更流程/导出AB包MD5信息文件(.json)")]
        public static void ExportMD5InfoFile_Json()
        {
            string abRootDirectory = Application.streamingAssetsPath;
            HotUpdateTool.WriteMD5(HotUpdateTool.WriteFileType.Json, abRootDirectory, HotUpdateSetting.folderFullPath_MD5);
        }

        /// <summary>
        /// 导出AB包MD5信息文件(.json)
        /// </summary>
        [MenuItem("MFramework/热更流程/导出AB包MD5信息文件(.xml)")]
        public static void ExportMD5InfoFile_XML()
        {
            string abRootDirectory = Application.streamingAssetsPath;
            HotUpdateTool.WriteMD5(HotUpdateTool.WriteFileType.XML, abRootDirectory, HotUpdateSetting.folderFullPath_MD5);
        }

        /// <summary>
        /// 导出AB包MD5信息文件(.json)
        /// </summary>
        [MenuItem("MFramework/热更流程/导出AB包MD5信息文件(.bytes)")]
        public static void ExportMD5InfoFile_Binary()
        {
            string abRootDirectory = Application.streamingAssetsPath;
            HotUpdateTool.WriteMD5(HotUpdateTool.WriteFileType.Binary, abRootDirectory, HotUpdateSetting.folderFullPath_MD5);
        }
        #endregion

        #region 不常用
        [MenuItem("MFramework/不常用/创建工程目录结构")]
        public static void AutoCreateAssetsDirectory()
        {
            EditorCreateAssetsDirectory.AutoCreateAssetsDirectory();
        }

        [MenuItem("MFramework/不常用/Renderer/合并网格", false, 1)]
        private static void MeshCombine()
        {
            MergeMesh.GetInstance.MeshCombine();
        }

        [MenuItem("MFramework/不常用/Renderer/清空网格", false, 2)]
        private static void ClearMesh()
        {
            MergeMesh.GetInstance.ClearMesh();
        }

        [MenuItem("MFramework/不常用/自动打包框架UnityPackage包     %e")]
        private static void MenuClicked1()
        {
            string packageName = "MFramework_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".unitypackage";
            //准备导出unitypackage资源路径
            string exportPathName = "Assets/MFramework";
            EditorAutoBuildUnityPackage.ExportUnityPactage(packageName, exportPathName);
        }
        #endregion
    }
}

#endif