using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：Transform静态扩展
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.08.01
    /// 版本：1.0
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// 查找对象
        /// </summary>
        /// <typeparam name="T">查找的类型</typeparam>
        /// <param name="trans"></param>
        /// <param name="name">查找的对象名</param>
        /// <param name="includeInactive">是否包含不显示在场景中的对象</param>
        /// <returns></returns>
        public static T FindObject<T>(this Transform trans, string name, bool includeInactive = true) where T : Component
        {
            if (trans == null || name == null) return default(T);
            T[] ts = trans.GetComponentsInChildren<T>(includeInactive);
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i].name == name) return ts[i];
            }
            return default(T);
        }
    }
}