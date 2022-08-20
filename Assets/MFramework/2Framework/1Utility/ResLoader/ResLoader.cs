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



        /// <summary>
        /// AB包路径
        /// </summary>
        public static string Path_AB = Application.streamingAssetsPath + "/BuildAB";

        #region 加载资源

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">U3D资源类型</typeparam>
        /// <param name="resType">资源种类</param>
        /// <param name="assetAllPath">ab包资源、resources资源全路径  格式：xx/xxx/xxx</param>
        /// <param name="assetName">具体资源名称 仅ResType.Asset资源种类填写</param>
        /// <returns></returns>
        public static T LoadSync<T>(ResType resType, string assetAllPath, string assetName = "") where T : UnityEngine.Object
        {
            AbRes resData = FindResInfoByResContainer(resType, assetAllPath, assetName);
            if (resData != null)
            {
                //判断资源加载的状态
                switch (resData.ResState)
                {
                    case AbRes.ResStateType.Loading:
                        throw new Exception("当前资源正在加载中！注意：对于同一个资源，请不要在异步加载当前资源未加载完毕时，使用同步加载该资源  assetAllPath：" + assetAllPath);
                    case AbRes.ResStateType.Loaded:
                        resData.Release();
                        return resData.Asset as T;
                }
            }
            AbRes newResData = CreateAsset(resType, assetAllPath, assetName);
            newResData.LoadSync();
            newResData.Release();
            resContainer.Add(newResData);
            return newResData.Asset as T;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="resType">资源种类</param>
        /// <param name="callback">回调<U3D资源类型></param>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <param name="assetName">具体资源名称 仅ResType.Asset资源种类填写</param>
        /// <returns></returns>
        public static void LoadAsync<T>(ResType resType, Action<T> callback, string assetAllPath, string assetName = "") where T : UnityEngine.Object
        {
            AbRes resData = FindResInfoByResContainer(resType, assetAllPath, assetName);
            if (resData != null)
            {
                //判断资源加载的状态
                switch (resData.ResState)
                {
                    case AbRes.ResStateType.Loading:
                        //等待资源加载完毕 回调
                        Debug.Log("该资源正在异步加载中，等待资源加载完毕 回调");
                        UnityTool.GetInstance.DelayCoroutineWaitReturnTrue(() =>
                        {
                            return resData.ResState == AbRes.ResStateType.Loaded;
                        }, () =>
                        {
                            resData.Release();
                            callback(resData.Asset as T);
                        });
                        return;
                    case AbRes.ResStateType.Loaded:
                        resData.Release();
                        callback(resData.Asset as T);
                        return;
                }
            }
            AbRes newResData = CreateAsset(resType, assetAllPath, assetName);
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
        private static AbRes CreateAsset(ResType resType, string assetAllPath, string assetName)
        {
            AbRes newResData = null;
            switch (resType)
            {
                case ResType.Resources:
                    newResData = new ResourcesRes(assetAllPath);
                    break;
                case ResType.AssetBundle:
                    newResData = new AssetBundleRes(assetAllPath);
                    break;
                case ResType.Asset:
                    if (string.IsNullOrEmpty(assetName))
                    {
                        Debug.LogError("assetName is null !  ResType.Asset方式加载资源，需要填写具体资源名 assetName");
                        break;
                    }
                    newResData = new AssetRes(assetAllPath, assetName);
                    break;
                default:
                    break;
            }
            return newResData;
        }


        /// <summary>
        /// 在资源容器根据查找资源根据资源路径
        /// </summary>
        /// <returns></returns>
        private static AbRes FindResInfoByResContainer(ResType resType, string assetAllPath, string assetName = "")
        {
            if (resType == ResType.Asset)
            {
                assetAllPath = assetAllPath + "/" + assetName;
            }
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
                Debug.Log(string.Format("资源类型：{0} 资源实例{1}，位置{1}，引用次数{2}", resData.resType, resData.Asset, resData.AssetAllPath, resData.RefCount));
            }
        }
        #endregion
    }
    /// <summary>
    /// 资源种类
    /// </summary>
    public enum ResType
    {
        /// <summary>
        /// Resources资源
        /// </summary>
        Resources,
        /// <summary>
        /// ab包
        /// </summary>
        AssetBundle,
        /// <summary>
        /// ab包中的资源
        /// </summary>
        Asset
    }
}