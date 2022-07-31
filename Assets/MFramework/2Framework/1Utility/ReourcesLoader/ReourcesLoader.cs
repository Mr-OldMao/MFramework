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
        /// 缓存正在使用的资源容器    唯一标识：资源路径
        /// </summary>
        private static List<ResData> resContainer = new List<ResData>();

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
                resContainer.Remove(this);
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
            ResData resData = resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath);
            if (resData != null)
            {
                resData.Release();
                return resData.Asset as T;
            }
            T res = Resources.Load<T>(assetAllPath);
            ResData newResData = new ResData(res, assetAllPath);
            newResData.Release();
            resContainer.Add(newResData);
            return res;
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
        public static ResData CheckResExist(string assetAllPath)
        {
            ResData resData = resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath);
            return resData;
        }

        /// <summary>
        /// 判定资源是否已被加载
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <param name="assetAllPath">资源 Resources下的全路径  格式：xx/xxx/xxx</param>
        /// <returns></returns>
        public static ResData CheckResExist<T>(string assetAllPath) where T : Object
        {
            ResData resData = resContainer.Find(loadedAsset => loadedAsset.AssetAllPath == assetAllPath && typeof(T) == loadedAsset.Asset.GetType());
            return resData;
        }


        /// <summary>
        /// 显示当前资源信息
        /// </summary>
        public static void ShowResLogInfo()
        {
            Debug.Log("显示当前资源信息");
            Debug.Log("资源总个数：" + resContainer.Count);
            foreach (ResData resData in resContainer)
            {
                Debug.Log(string.Format("资源实例：{0}，位置{1}，引用次数{2}", resData.Asset, resData.AssetAllPath, resData.RefCount));
            }
        }
        #endregion
    }
}