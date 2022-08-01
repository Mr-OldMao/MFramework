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
    public class UnityTool : SingletonByMono<UnityTool>
    {
        #region 定时工具 延时调用
        public void DelayCoroutine(float seconds, Action callback)
        {
            StartCoroutine(IEDelayCoroutine(seconds, callback));
        }
        /// <summary>
        /// 延时调用 等待Func返回true   执行回调
        /// </summary>
        /// <param name="func"></param>
        /// <param name="callback"></param>
        public void DelayCoroutineWaitReturnTrue(Func<bool> func, Action callback)
        {
            StartCoroutine(IEDelayCoroutineWaitReturnTrue(func, callback));
        }
        /// <summary>
        /// 延时调用 等待Func返回false   执行回调
        /// </summary>
        /// <param name="func"></param>
        /// <param name="callback"></param>
        public void DelayCoroutineWaitReturnFalse(Func<bool> func, Action callback)
        {
            StartCoroutine(IEDelayCoroutineWaitReturnFalse(func, callback));
        }
        private IEnumerator IEDelayCoroutine(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }
        private IEnumerator IEDelayCoroutineWaitReturnTrue(Func<bool> func, Action callback)
        {
            yield return new WaitUntil(func);
            callback?.Invoke();
        }
        private IEnumerator IEDelayCoroutineWaitReturnFalse(Func<bool> func, Action callback)
        {
            yield return new WaitWhile(func);
            callback?.Invoke();
        }
        #endregion


    }
}