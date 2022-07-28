using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：定时工具
    /// 功能：延时调用
    /// 作者：毛俊峰
    /// 时间：2022.07.16
    /// 版本：1.0
    /// </summary>
    public class DelayTool : SingletonByMono<DelayTool>
    {
        public void Delay(float seconds, Action callback)
        {
            StartCoroutine(DelayCoroutine(seconds, callback));
        }

        private IEnumerator DelayCoroutine(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }
    }
}