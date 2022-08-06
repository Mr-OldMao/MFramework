using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：Resource资源加载、卸载
    /// 功能：Resource资源同步加载、异步加载、资源卸载具体实现
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class ResourcesRes : AbRes
    {
        public ResourcesRes(string assetAllPath) : base(assetAllPath)
        {
            base.AssetAllPath = assetAllPath;
        }

        public override bool LoadSync()
        {
            return Asset = Resources.Load(AssetAllPath);
        }

        public override void LoadAsync(Action<AbRes> callback)
        {
            ResourceRequest rr = Resources.LoadAsync(AssetAllPath);
            rr.completed += (AsyncOperation ao) =>
            {
                Asset = rr.asset;
                callback?.Invoke(this);
            };
        }

        protected override void OnReleaseRes()
        {
            Resources.UnloadAsset(Asset);
            ResLoader.resContainer.Remove(this);
            Asset = null;
            Debug.Log("已回收资源 Asset：" + Asset + "，AssetAllPath：" + AssetAllPath);
        }
    }
}