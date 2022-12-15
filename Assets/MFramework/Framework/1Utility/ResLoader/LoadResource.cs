using System;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源同步加载、异步加载、卸载、计数，对ResLoader进行封装
    /// 功能：对以下五类资源支持，
    ///       1.编辑器下的资源类型 UnityEditor.AssetDatabase.LoadAssetAtPath
    ///       2.AB包
    ///       3.AB包中资源
    ///       4.Resources目录下资源
    /// 作者：毛俊峰
    /// 时间：2022.09.29
    /// 版本：1.0
    /// </summary>
    public class LoadResource : SingletonByMono<LoadResource>
    {
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resPath">资源路径，具体格式根据资源类型ResType而定</param>
        /// <param name="goCloneReturn">T:GameObject 是否自动克隆并返回</param>
        /// <param name="resType"></param>
        /// <returns></returns>
        public static T LoadSync<T>(string resPath, ResType resType = ResType.Null, bool goCloneReturn = true) where T : UnityEngine.Object
        {
            string assetName = string.Empty;
            string parsedAssetPath = resPath;

            switch (resType)
            {
                case ResType.Null:
                    //编辑器下默认加载类型
#if UNITY_EDITOR
                    resType = ABSetting.resTypeDefaultEditor;
#else
                    resType = ABSetting.resTypeDefaultNotEditor;
                    parsedAssetPath = ParseAssetPath(resPath);
                    assetName = ParseAssetName(resPath);
#endif
                    break;
                case ResType.ResEditor:
                    break;
                case ResType.ResResources:
                    break;
                case ResType.ResAssetBundlePack:
                    parsedAssetPath = ParseAssetPath(resPath);
                    break;
                case ResType.ResAssetBundleAsset:
                    parsedAssetPath = ParseAssetPath(resPath);
                    assetName = ParseAssetName(resPath);
                    break;
                default:
                    break;
            }
            T asset = ResLoader.LoadSync<T>(resType, parsedAssetPath, assetName);
            if (typeof(T) == typeof(GameObject) && goCloneReturn)
            {
                if (asset != null)
                {
                    return Instantiate(asset);
                }
            }
            return asset;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resPath"></param>
        /// <param name="callback"></param>
        /// <param name="resType"></param>
        public static void LoadAsync<T>(string resPath, Action<T> callback, ResType resType = ResType.Null) where T : UnityEngine.Object
        {
            string assetName = string.Empty;
            string parsedAssetPath = resPath;

            switch (resType)
            {
                case ResType.Null:
#if UNITY_EDITOR
                    resType = ABSetting.resTypeDefaultEditor;
#else
                    resType = ABSetting.resTypeDefaultNotEditor;
                    parsedAssetPath = ParseAssetPath(resPath);
                    assetName = ParseAssetName(resPath);
#endif
                    break;
                case ResType.ResEditor:
                    break;
                case ResType.ResResources:
                    break;
                case ResType.ResAssetBundlePack:
                    parsedAssetPath = ParseAssetPath(resPath);
                    break;
                case ResType.ResAssetBundleAsset:
                    parsedAssetPath = ParseAssetPath(resPath);
                    assetName = ParseAssetName(resPath);
                    break;
                default:
                    break;
            }
            ResLoader.LoadAsync<T>(resType, callback, parsedAssetPath, assetName);
        }



        /// <summary>
        /// 解析目标资源的AB路径
        /// </summary>
        public static string ParseAssetPath(string path)
        {
            string[] pathSplitArr = path.Split('/', '.');
            //字符串转换 去头去后缀转小写,路径全为小写，且不允许有后缀 Assets/AssetsRes/ABRes/Prefab/Cube1.prefab =》assetsres/abres/prefab/cube1
            string abPath = path.Replace(pathSplitArr[0] + "/", "").Replace("." + pathSplitArr[pathSplitArr.Length - 1], "").ToLower();
            //Debug.Log("TAB abPath   " + abPath);
            return abPath;
        }

        /// <summary>
        /// 解析目标资源的AB名称
        /// </summary>
        private static string ParseAssetName(string path)
        {
            string[] pathSplitArr = path.Split('/', '.');
            //提取资源名称 Assets/AssetsRes/ABRes/Prefab/Cube1.prefab =》cube1
            string abName = pathSplitArr[pathSplitArr.Length - 2].ToLower();
            //Debug.Log("TAB abName   " + abName);
            return abName;
        }

    }
}