using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源加载器管理器
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class ResManager : SingletonByMono<ResManager>
    {
        public T LoadSync<T>(string resPath, ResType resType = ResType.Null, bool goCloneReturn = true) where T : UnityEngine.Object
        {
            return LoadResource.LoadSync<T>(resPath, resType, goCloneReturn);
        }
        public void LoadAsync<T>(string resPath, Action<T> callback, ResType resType = ResType.Null) where T : UnityEngine.Object
        {
            LoadResource.LoadAsync<T>(resPath, callback, resType);
        }
    }
}