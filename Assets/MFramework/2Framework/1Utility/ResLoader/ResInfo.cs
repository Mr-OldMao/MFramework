using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源信息数据结构
    /// 功能：缓存资源信息，对资源进行引用计数处理， 未引用资源自动回收
    /// 作者：毛俊峰
    /// 时间：2022.08.02
    /// 版本：1.0
    /// </summary>
    public class ResInfo : AbRefCounter
    {
        public ResInfo(string assetAllPath)
        {
            AssetAllPath = assetAllPath;
        }
        /// <summary>
        /// 资源实例
        /// </summary>
        public UnityEngine.Object Asset
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
        /// 同步加载资源
        /// </summary>
        public void LoadSync()
        {
            Asset = Resources.Load(AssetAllPath);
        }
        public void LoadAsync(Action<ResInfo> callback)
        {
            ResourceRequest rr = Resources.LoadAsync(AssetAllPath);
            rr.completed += (AsyncOperation ao) =>
            {
                Asset = rr.asset;
                callback?.Invoke(this);
            };
        }
        /// <summary>
        ///回收资源
        /// </summary>
        protected override void OnZeroRef()
        {
            Resources.UnloadAsset(Asset);
            ResLoader.resContainer.Remove(this);
            Asset = null;
            Debug.Log("已回收资源 Asset：" + Asset + "，AssetAllPath：" + AssetAllPath);
        }
    }
}