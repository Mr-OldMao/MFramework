using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源加载器
    /// 功能：管理Resource资源、AssetBundle资源的同步加载、异步加载、资源卸载
    /// 作者：毛俊峰
    /// 时间：2022.07.24
    /// 版本：1.0
    /// </summary>
    public class ResLoader
    {
        /// <summary>
        /// 缓存全局正在使用的资源容器    唯一标识：资源路径
        /// </summary>
        public static List<AbRes> resContainer = new List<AbRes>();

        ///// <summary>
        ///// AssetBundle文件缓存池
        ///// </summary>
        //public static Dictionary<string, AssetBundle> dicAssetBundle = new Dictionary<string, AssetBundle>();

        #region 加载资源

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <returns></returns>
        public static T LoadSync<T>(string assetAllPath, ResType resType = ResType.Resources) where T : UnityEngine.Object
        {
            AbRes resData = FindResInfoByResContainer(assetAllPath);
            if (resData != null)
            {
                resData.Release();
                return resData.Asset as T;
            }
            AbRes newResData = CreateAsset(assetAllPath, resType);//= new ResInfo(assetAllPath);
            newResData.LoadSync();
            newResData.Release();
            resContainer.Add(newResData);
            return newResData.Asset as T;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <returns></returns>
        public static void LoadASync<T>(string assetAllPath, Action<T> callback, ResType resType = ResType.Resources) where T : UnityEngine.Object
        {
            AbRes resData = FindResInfoByResContainer(assetAllPath);
            if (resData != null)
            {
                resData.Release();
                return;
            }
            AbRes newResData = CreateAsset(assetAllPath, resType);
            newResData.LoadAsync((AbRes p) => callback(p.Asset as T));
            newResData.Release();
            resContainer.Add(newResData);
        }

        /// <summary>
        /// 创建资源
        /// </summary>
        /// <param name="assetAllPath"></param>
        /// <param name="resType"></param>
        /// <returns></returns>
        private static AbRes CreateAsset(string assetAllPath, ResType resType)
        {
            AbRes newResData;
            if (resType == ResType.Resources)
            {
                newResData = new ResourcesRes(assetAllPath);
            }
            else
            {
                newResData = new AssetBundleRes(assetAllPath);
            }
            return newResData;
        }


        /// <summary>
        /// 在资源容器根据查找资源根据资源路径
        /// </summary>
        /// <returns></returns>
        private static AbRes FindResInfoByResContainer(string assetAllPath)
        {
            return resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath);
        }


        #endregion

        #region 卸载资源

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        public static void UnLoadAllAssets()
        {
            for (int i = 0; i < resContainer.Count; i++)
            {
                Resources.UnloadAsset(resContainer[i].Asset);
            }
            resContainer.Clear();
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        public static void UnLoadAssets(string assetAllPath)
        {
            for (int i = 0; i < resContainer.Count; i++)
            {
                if (resContainer[i].AssetAllPath == assetAllPath)
                {
                    resContainer[i].Retain();
                    break;
                }
            }
        }
        #endregion

        #region 获取资源信息
        /// <summary>
        /// 判定资源是否已被加载
        /// </summary>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        public static AbRes CheckResExist(string assetAllPath)
        {
            AbRes resData = resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath);
            return resData;
        }

        /// <summary>
        /// 判定资源是否已被加载
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <returns></returns>
        public static AbRes CheckResExist<T>(string assetAllPath) where T : UnityEngine.Object
        {
            AbRes resData = resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath && typeof(T) == loadedAsset.Asset.GetType());
            return resData;
        }

        /// <summary>
        /// 显示当前资源信息
        /// </summary>
        public static void ShowResLogInfo()
        {
            Debug.Log("显示当前资源信息");
            Debug.Log("资源总个数：" + resContainer.Count);
            foreach (AbRes resData in resContainer)
            {
                Debug.Log(string.Format("资源实例：{0}，位置{1}，引用次数{2}", resData.Asset, resData.AssetAllPath, resData.RefCount));
            }
        }
        #endregion
    }
    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResType
    {
        Resources,
        AssetBundle
    }
}