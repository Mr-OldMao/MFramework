using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源信息数据结构抽象类
    /// 功能：缓存资源信息，对资源进行引用计数处理， 未引用资源自动回收
    /// 作者：毛俊峰
    /// 时间：2022.08.02
    /// 版本：1.0
    /// </summary>
    public abstract class AbRes : AbRefCounter
    {
        /// <summary>
        /// 资源实例路径
        /// </summary>
        /// <param name="assetAllPath"></param>
        public AbRes(string assetAllPath) { AssetAllPath = assetAllPath; }
        /// <summary>
        /// 资源实例
        /// </summary>
        public UnityEngine.Object Asset { get; set; }
        /// <summary>
        /// 资源Rousources下的全路径 xx/xxx/xxx
        /// </summary>
        public string AssetAllPath { get; set; }
        /// <summary>
        /// 同步加载资源
        /// </summary>
        public abstract bool LoadSync();
        public abstract void LoadAsync(Action<AbRes> callback);
        /// <summary>
        /// 卸载资源
        /// </summary>
        protected abstract void OnReleaseRes();
        /// <summary>
        ///回收资源 资源引用次数为0自动调用
        /// </summary>
        protected override void OnZeroRef()
        {
            OnReleaseRes();
        }
    }
}