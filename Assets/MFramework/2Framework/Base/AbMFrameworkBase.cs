using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：框架抽象基类
    /// 功能：消息系统：封装消息事件 注册、发送，销毁脚本自动注销事件
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public abstract partial class AbMFrameworkBase : MonoBehaviour
    {
        /// <summary>
        /// 缓存当前脚本消息事件名称
        /// </summary>
        private List<string> m_DicMsgEventName;
        /// <summary>
        /// 消息注册
        /// </summary>
        /// <param name="msgEvent"></param>
        public void RegisterMsgEvent(string msgName, Action<object> msgEvent)
        {
            MsgEvent.RegisterMsgEvent(msgName, msgEvent);
            // 缓存当前脚本消息事件名称
            if (m_DicMsgEventName == null)
            {
                m_DicMsgEventName = new List<string>();
            }
            if (!m_DicMsgEventName.Contains(msgName))
            {
                m_DicMsgEventName.Add(msgName);
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgEvent"></param>
        public void SendMsg(string msgName, object param)
        {
            MsgEvent.SendMsg(msgName, param);
        }





        /// <summary>
        /// 销毁脚本前调用
        /// </summary>
        public void OnDestroyBefore()
        {
            //自动注销事件
            Debug.Log("自动注销消息事件");
            if (m_DicMsgEventName != null)
            {
                foreach (string msgEventName in m_DicMsgEventName)
                {
                    MsgEvent.UnregisterMsgEventAll(msgEventName);
                }
            }
            Debug.Log(MsgEvent.GetDicMsgEventContainer);

        }
        public virtual void OnDestroy()
        {
            OnDestroyBefore();
        }
    }
}