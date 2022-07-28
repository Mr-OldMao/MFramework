using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源管理
    /// 功能：资源的加载与卸载 
    /// 作者：毛俊峰
    /// 时间：2022..07.24
    /// 版本：1.0
    /// </summary>
    public class ReourcesLoader
    {
        /// <summary>
        /// 缓存正在使用的资源容器
        /// </summary>
        public static List<ResData> resourcesContainer = new List<ResData>();

        /// <summary>
        /// 资源数据结构
        /// </summary>
        public class ResData : AbRefCounter
        {
            public ResData(Object asset, string assetAllPath)
            {
                Asset = asset;
                AssetAllPath = assetAllPath;
            }
            public Object Asset
            {
                get;
                private set;
            }
            /// <summary>
            /// 资源Rousources下的全路径 xx/xxx/xxx
            /// </summary>
            public string AssetAllPath
            {
                get;
                private set;
            }
            /// <summary>
            ///回收资源
            /// </summary>
            protected override void OnZeroRef()
            {
                Resources.UnloadAsset(Asset);
                Asset = null;
                resourcesContainer.Remove(this);
                Debug.Log("已回收资源 Asset：" + Asset + "，AssetAllPath：" + AssetAllPath);
            }
        }

        #region 加载资源
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <returns></returns>
        public static T LoadAssets<T>(string assetAllPath) where T : Object
        {
            ResData resData = resourcesContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath);
            if (resData != null)
            {
                resData.Release();
                return resData.Asset as T;
            }
            T res = Resources.Load<T>(assetAllPath);
            ResData newResData = new ResData(res, assetAllPath);
            newResData.Release();
            resourcesContainer.Add(newResData);
            return res;
        }
        #endregion

        #region 卸载资源

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        public static void UnLoadAllAssets()
        {
            for (int i = 0; i < resourcesContainer.Count; i++)
            {
                Resources.UnloadAsset(resourcesContainer[i].Asset);
            }
            resourcesContainer.Clear();
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        public static void UnLoadAssets(string assetAllPath)
        {
            for (int i = 0; i < resourcesContainer.Count; i++)
            {
                if (resourcesContainer[i].AssetAllPath == assetAllPath)
                {
                    resourcesContainer[i].Retain();
                    break;
                }
            }
        }
        #endregion
    }
}