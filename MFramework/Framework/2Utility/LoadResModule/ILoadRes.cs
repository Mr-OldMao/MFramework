using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 标题：资源加载接口
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2023.11.16
    /// </summary>
    public interface ILoadRes
    {
        /// <summary>
        /// 批量资源异步加载根据标签
        /// </summary>
        /// <param name="lables"></param>
        /// <param name="callbackLoadedComplete"></param>
        void LoadResAsyncByLable(List<string> lables, Action callbackLoadedComplete = null);

        void LoadResAsyncByAssetPath<T>(string assetPath, Action<T> callbackLoadedComplete = null, bool autoInstantiate = true) where T : UnityEngine.Object;

        void LoadResAsyncByDirectory(string dirPath = "/GameMain/AB/", Action callbackLoadedComplete = null, Action<float> callbackLoadedProgress = null);


        //缓存资源
        void CacheRes(string key, ResInfo resInfo);

        //获取资源
        T GetRes<T>(string key, bool isAutoClone = true) where T : UnityEngine.Object;

        //释放资源
    }



    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResType
    {
        None = 0,
        /// <summary>
        /// 预制体 GameObject
        /// </summary>
        Prefab,
        /// <summary>
        /// 图片 png、jpg、tga
        /// </summary>
        Image,
        /// <summary>
        /// 材质
        /// </summary>
        Material,
        //TODO

    }


}