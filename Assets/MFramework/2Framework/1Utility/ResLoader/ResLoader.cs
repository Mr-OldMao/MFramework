using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源管理
    /// 功能：资源的加载与卸载 
    /// 作者：毛俊峰
    /// 时间：2022.07.24
    /// 版本：1.0
    /// </summary>
    public class ResLoader
    {
        /// <summary>
        /// 缓存全局正在使用的资源容器    唯一标识：资源路径
        /// </summary>
        public static List<ResInfo> resContainer = new List<ResInfo>();

        #region 加载资源

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <returns></returns>
        public static T LoadSync<T>(string assetAllPath) where T : UnityEngine.Object
        {
            ResInfo resData = FindResInfoByResContainer(assetAllPath);
            if (resData != null)
            {
                resData.Release();
                return resData.Asset as T;
            }
            ResInfo newResData = new ResInfo(assetAllPath);
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
        public static void LoadASync<T>(string assetAllPath, Action<ResInfo> callback) where T : UnityEngine.Object
        {
            ResInfo resData = FindResInfoByResContainer(assetAllPath);
            if (resData != null)
            {
                resData.Release();
                return;
            }
            ResInfo newResData = new ResInfo(assetAllPath);
            newResData.LoadAsync(callback);
            newResData.Release();
            resContainer.Add(newResData);
        }

        /// <summary>
        /// 在资源容器根据查找资源根据资源路径
        /// </summary>
        /// <returns></returns>
        private static ResInfo FindResInfoByResContainer(string assetAllPath)
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
        public static ResInfo CheckResExist(string assetAllPath)
        {
            ResInfo resData = resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath);
            return resData;
        }

        /// <summary>
        /// 判定资源是否已被加载
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <returns></returns>
        public static ResInfo CheckResExist<T>(string assetAllPath) where T : UnityEngine.Object
        {
            ResInfo resData = resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath && typeof(T) == loadedAsset.Asset.GetType());
            return resData;
        }

        /// <summary>
        /// 显示当前资源信息
        /// </summary>
        public static void ShowResLogInfo()
        {
            Debug.Log("显示当前资源信息");
            Debug.Log("资源总个数：" + resContainer.Count);
            foreach (ResInfo resData in resContainer)
            {
                Debug.Log(string.Format("资源实例：{0}，位置{1}，引用次数{2}", resData.Asset, resData.AssetAllPath, resData.RefCount));
            }
        }
        #endregion
    }
}