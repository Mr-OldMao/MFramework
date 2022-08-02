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
        /// <param name="transform"></param>
        /// <param name="targetName">查找的对象名</param>
        /// <param name="includeInactive">是否包含不显示在场景中的对象</param>
        /// <returns></returns>
        public static T Find<T>(this Transform transform, string targetName, bool includeInactive = true) where T : Component
        {
            T res = default;
            if (transform == null || string.IsNullOrEmpty(targetName))
            {
                res = null;
            }
            var targetArr = transform.GetComponentsInChildren<T>(includeInactive);
            for (int i = 0; i < targetArr.Length; i++)
            {
                if (targetArr[i].name == targetName)
                {
                    res = targetArr[i];
                    break;
                }
            }
            return res;
        }
    }
}