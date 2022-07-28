using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：消息系统
    /// 描述：基于事件的消息系统
    /// 目的：降低不同模块之间的相互调用的耦合性
    /// 功能：管理消息的注册、注销、调用
    /// 作者：毛俊峰
    /// 时间：2022.07.16
    /// 版本：1.0
    /// </summary>
    public class MsgEvent
    {
        /// <summary>
        /// 消息事件缓存容器（带参）  key-消息名  value-消息列表
        /// </summary>
        private static Dictionary<string, List<MsgEventParamInfo>> m_DicMsgEventParamContainer = new Dictionary<string, List<MsgEventParamInfo>>();
        public static Dictionary<string, List<MsgEventParamInfo>> GetDicMsgEventParamContainer
        {
            get => m_DicMsgEventParamContainer;
        }

        /// <summary>
        /// 消息事件缓存容器（无参）  key-消息名  value-消息列表
        /// </summary>
        private static Dictionary<string, List<MsgEventInfo>> m_DicMsgEventContainer = new Dictionary<string, List<MsgEventInfo>>();
        public static Dictionary<string, List<MsgEventInfo>> GetDicMsgEventContainer
        {
            get => m_DicMsgEventContainer;
        }

        /// <summary>
        /// 带参消息事件数据结构
        /// </summary>
        public class MsgEventParamInfo
        {
            /// <summary>
            /// 消息事件
            /// </summary>
            public Action<object> msgEvent;
            /// <summary>
            /// 当前消息事件描述
            /// </summary>
            public string msgEventDescript;
            /// <summary>
            /// 当前消息事件执行次数
            /// </summary>
            public int msgEventActiveCount;
        }
        /// <summary>
        /// 带参消息事件数据结构
        /// </summary>
        public class MsgEventInfo
        {
            /// <summary>
            /// 消息事件
            /// </summary>
            public Action msgEvent;
            /// <summary>
            /// 当前消息事件描述
            /// </summary>
            public string msgEventDescript;
            /// <summary>
            /// 当前消息事件执行次数
            /// </summary>
            public int msgEventActiveCount;
        }


        #region 消息注册
        /// <summary>
        /// 消息注册有参
        /// 调用时序：Action()、Start()
        /// </summary>
        /// <param name="msgName">消息名</param>
        /// <param name="msgEvent">消息事件</param>
        /// <param name="msgEventDescript">消息事件描述</param>
        public static void RegisterMsgEvent(string msgName, Action<object> msgEvent, string msgEventDescript = "")
        {
            if (m_DicMsgEventParamContainer.ContainsKey(msgName))
            {
                List<MsgEventParamInfo> msgEventParamInfoList = m_DicMsgEventParamContainer[msgName];
                MsgEventParamInfo msgEventParamInfo = new MsgEventParamInfo
                {
                    msgEvent = msgEvent,
                    msgEventActiveCount = 0,
                    msgEventDescript = msgEventDescript
                };
                msgEventParamInfoList.Add(msgEventParamInfo);
            }
            else
            {
                List<MsgEventParamInfo> msgEventParamInfoList = new List<MsgEventParamInfo>();
                MsgEventParamInfo msgEventParamStruct = new MsgEventParamInfo
                {
                    msgEvent = msgEvent,
                    msgEventDescript = msgEventDescript,
                    msgEventActiveCount = 0
                };
                msgEventParamInfoList.Add(msgEventParamStruct);
                m_DicMsgEventParamContainer.Add(msgName, msgEventParamInfoList);
            }
        }

        /// <summary>
        /// 消息注册（无参）
        /// 调用时序：Action()、Start()
        /// </summary>
        /// <param name="msgName">消息名</param>
        /// <param name="msgEvent">消息事件</param>
        /// <param name="msgEventDescript">消息事件描述</param>
        public static void RegisterMsgEvent(string msgName, Action msgEvent, string msgEventDescript = "")
        {
            if (m_DicMsgEventContainer.ContainsKey(msgName))
            {
                List<MsgEventInfo> msgEventInfoList = m_DicMsgEventContainer[msgName];
                MsgEventInfo msgEventParamInfo = new MsgEventInfo
                {
                    msgEvent = msgEvent,
                    msgEventActiveCount = 0,
                    msgEventDescript = msgEventDescript
                };
                msgEventInfoList.Add(msgEventParamInfo);
            }
            else
            {
                List<MsgEventInfo> msgEventInfoList = new List<MsgEventInfo>();
                MsgEventInfo msgEventParamStruct = new MsgEventInfo
                {
                    msgEvent = msgEvent,
                    msgEventDescript = msgEventDescript,
                    msgEventActiveCount = 0
                };
                msgEventInfoList.Add(msgEventParamStruct);
                m_DicMsgEventContainer.Add(msgName, msgEventInfoList);
            }
        }

        #endregion

        #region 消息注销
        /// <summary>
        /// 注销消息  注销消息名下的  所有消息（带参和无参）
        /// 一般用于注销 注册过lamda表达式的事件消息
        /// </summary>
        /// <param name="msgName"></param>
        public static void UnregisterMsgEventAll(string msgName)
        {
            if (m_DicMsgEventParamContainer.ContainsKey(msgName))
            {
                m_DicMsgEventParamContainer.Remove(msgName);
            }
            else if (m_DicMsgEventContainer.ContainsKey(msgName))
            {
                m_DicMsgEventContainer.Remove(msgName);
            }
            else
            {
                Debug.LogError("消息注销失败 请检查消息名下是否有消息 msgName：" + msgName);
            }
        }
        /// <summary>
        /// 注销消息  注销消息名下的 所有带参消息
        /// 一般用于注销 注册过lamda表达式的事件消息
        /// </summary>
        public static void UnregisterMsgEventParam(string msgName)
        {
            if (m_DicMsgEventParamContainer.ContainsKey(msgName))
            {
                m_DicMsgEventParamContainer.Remove(msgName);
            }
            else
            {
                Debug.LogError("消息注销失败 请检查消息名下是否有带参消息 msgName：" + msgName);
            }
        }
        /// <summary>
        /// 注销消息  注销消息名下的 所有无参消息
        /// 一般用于注销 注册了lamda表达式的事件消息
        /// </summary>
        public static void UnregisterMsgEventNotParam(string msgName)
        {
            if (m_DicMsgEventContainer.ContainsKey(msgName))
            {
                m_DicMsgEventContainer.Remove(msgName);
            }
            else
            {
                Debug.LogError("消息注销失败 请检查消息名下是否有无参消息 msgName：" + msgName);
            }
        }

        /// <summary>
        /// 注销消息  注销消息名下 指定带参消息
        /// 一般用于注销 注册过非lamda表达式的事件消息
        /// </summary>
        /// <param name="msgName"></param>
        public static void UnregisterMsgEvent(string msgName, Action<object> msgEvent)
        {
            if (m_DicMsgEventParamContainer.ContainsKey(msgName))
            {
                List<MsgEventParamInfo> msgEventParamInfos = m_DicMsgEventParamContainer[msgName];
                MsgEventParamInfo targetMsg = null;
                for (int i = 0; i < msgEventParamInfos.Count; i++)
                {
                    if (msgEventParamInfos[i].msgEvent == msgEvent)
                    {
                        targetMsg = msgEventParamInfos[i];
                        break;
                    }
                }
                if (targetMsg != null)
                {
                    msgEventParamInfos.Remove(targetMsg);
                    Debug.Log("消息注销成功");
                }
                else
                {
                    Debug.LogError("消息注销失败 未查询到该消息Action事件 msgName:" + msgName);
                }
            }
            else
            {
                Debug.LogError("消息注销失败 请检查消息名 msgName：" + msgName);
            }
        }
        /// <summary>
        /// 注销消息  注销消息名下 指定无参参消息
        /// 一般用于注销 注册过非lamda表达式的事件消息
        /// </summary>
        /// <param name="msgName"></param>
        public static void UnregisterMsgEvent(string msgName, Action msgEvent)
        {
            if (m_DicMsgEventContainer.ContainsKey(msgName))
            {
                List<MsgEventInfo> msgEventInfos = m_DicMsgEventContainer[msgName];
                MsgEventInfo targetMsg = null;
                for (int i = 0; i < msgEventInfos.Count; i++)
                {
                    if (msgEventInfos[i].msgEvent == msgEvent)
                    {
                        targetMsg = msgEventInfos[i];
                        break;
                    }
                }
                if (targetMsg != null)
                {
                    msgEventInfos.Remove(targetMsg);
                    Debug.Log("消息注销成功");
                }
                else
                {
                    Debug.LogError("消息注销失败 未查询到该消息Action事件 msgName:" + msgName);
                }
            }
            else
            {
                Debug.LogError("消息注销失败 请检查消息名 msgName：" + msgName);
            }
        }
        #endregion

        #region 消息调用
        /// <summary>
        /// 发送带参消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="param">消息事件参数</param>
        public static void SendMsg(string msgName, object param)
        {
            if (m_DicMsgEventParamContainer.ContainsKey(msgName))
            {
                List<MsgEventParamInfo> msgEventParamInfos = m_DicMsgEventParamContainer[msgName];
                foreach (MsgEventParamInfo item in msgEventParamInfos)
                {
                    item.msgEvent.Invoke(param);
                }
            }
            else
            {
                Debug.LogError("消息系统-发送消息失败 检查消息名称 msgName：" + msgName);
            }
        }

        /// <summary>
        /// 发送无参消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="param">消息事件参数</param>
        public static void SendMsg(string msgName)
        {
            if (m_DicMsgEventContainer.ContainsKey(msgName))
            {
                List<MsgEventInfo> msgEventInfos = m_DicMsgEventContainer[msgName];
                foreach (MsgEventInfo item in msgEventInfos)
                {
                    item.msgEvent.Invoke();
                }
            }
            else
            {
                Debug.LogError("消息系统-发送消息失败 检查消息名称 msgName：" + msgName);
            }
        }
        #endregion
    }
}