using System;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：日志系统
    /// 功能：
    /// 1.打印控制台指定标签
    /// 2.控制台日志的显示样式
    /// 3.编辑器运行状态下，开关日志打印
    /// 4.各个标签的打印状态本地数据持久化
    /// 作者：毛俊峰
    /// 时间：2022.10.19   2023.10.27
    /// 版本：1.1
    /// </summary>
    public class Debugger : SingletonByMono<Debugger>
    {
        /// <summary>
        /// 日志回调(1日志index 2日志内容 3日志类型 4日志标签 5日志堆栈信息)
        /// </summary>
        public static Action<int, object, LogType, LogTag, string> logCallback;
        private static int m_CurLogIndex = 1;

        #region 对外接口
        public static void Log(object message, LogTag logTag = LogTag.Temp)
        {
            LogHandle(message, logTag, LogType.Log);
        }

        public static void LogWarning(object message, LogTag logTag = LogTag.Temp)
        {
            LogHandle(message, logTag, LogType.Warning);
        }

        public static void LogError(object message)
        {
            LogHandle(message, LogTag.Forever, LogType.Error);
        }

        #endregion

        /// <summary>
        /// 日志处理
        /// </summary>
        /// <param name="logMsg"></param>
        /// <param name="logTag"></param>
        /// <param name="logType"></param>
        private static void LogHandle(object logMsg, LogTag logTag, LogType logType)
        {
            bool canPrint = false;
            if (logTag == LogTag.Forever)
            {
                canPrint = true;
            }
            else if (DebuggerConfig.CanPrintLogTagList != null && DebuggerConfig.CanPrintLogTagList.Contains(logTag))
            {
                switch (logType)
                {
                    case LogType.Log:
                    case LogType.Assert:
                    case LogType.Warning:
                        canPrint = DebuggerConfig.CanPrintConsoleLog && DebuggerConfig.CanPrintLogTagList.Contains(logTag);
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        canPrint = DebuggerConfig.CanPrintConsoleLogError && DebuggerConfig.CanPrintLogTagList.Contains(logTag);
                        break;
                }
            }
            if (canPrint)
            {
                if (DebuggerConfig.CanSaveLogDataFile && !SaveLogData.IsListeneringWriteLog)
                {
                    SaveLogData.GetInstance.ListenerWriteLog();
                }
                if (DebuggerConfig.CanChangeConsolePrintStyle)
                {
                    ChangeStyle(ref logMsg, logTag, logType);
                }
                Debug.unityLogger.Log(logType, logMsg);
                logCallback?.Invoke(m_CurLogIndex++, logMsg, logType, logTag, StackTraceUtility.ExtractStackTrace());
            }
        }

        /// <summary>
        /// 控制台打印样式调整
        /// </summary>
        /// <param name="logMsg"></param>
        /// <param name="logTag"></param>
        private static void ChangeStyle(ref object logMsg, LogTag logTag, LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                    switch (logTag)
                    {
                        case LogTag.Temp:
                            logMsg = "<color=#FF5656>" + logMsg + "</color>";
                            break;
                        case LogTag.Test:
                            logMsg = "<B><color=#FF5656>" + logMsg + "</color></B>";
                            break;
                        case LogTag.Forever:
                            logMsg = "<B><color=red>" + logMsg + "</color></B>";
                            break;
                    }
                    break;
                case LogType.Warning:
                    switch (logTag)
                    {
                        case LogTag.Temp:
                            logMsg = "<color=#FFF556>" + logMsg + "</color>";
                            break;
                        case LogTag.Test:
                            logMsg = "<B><color=#FFF556>" + logMsg + "</color></B>";
                            break;
                        case LogTag.Forever:
                            logMsg = "<B><color=yellow>" + logMsg + "</color></B>";
                            break;
                    }
                    break;
                case LogType.Log:
                    switch (logTag)
                    {
                        case LogTag.Temp:
                            logMsg = "<color=#AFFFFF>" + logMsg + "</color>";
                            break;
                        case LogTag.Test:
                            logMsg = "<B><color=#97FFFF>" + logMsg + "</color></B>";
                            break;
                        case LogTag.Forever:
                            logMsg = "<B><color=#00FFFF>" + logMsg + "</color></B>";
                            break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 日志标签
    /// </summary>
    public enum LogTag
    {
        /// <summary>
        /// 临时标签
        /// </summary>
        Temp,
        /// <summary>
        /// 关键节点调试标签
        /// </summary>
        Test,
        /// <summary>
        /// 常驻日志标签，不受外部打印开启与否的限制
        /// </summary>
        Forever
    }
}