using System.Collections.Generic;

namespace MFramework
{
    /// <summary>
    /// 标题：日志系统配置
    /// 功能：
    /// 1.允许对指定标签打印
    /// 2.允许修改控制台日志显示样式
    /// 3.允许缓存实时日志信息到本地
    /// 作者：毛俊峰
    /// 时间：2022.10.19   2023.10.27
    /// 版本：1.1
    /// </summary>
    public class DebuggerConfig
    {
        #region 控制台日志打印
        private static bool m_CanPrintConsoleLog = UnityEngine.PlayerPrefs.GetInt("CanPrintConsoleLog", 1) == 1;
        private static bool m_CanPrintConsoleLogError = UnityEngine.PlayerPrefs.GetInt("CanPrintConsoleLogError", 1) == 1;
        private static bool m_CanSaveLogDataFile = UnityEngine.PlayerPrefs.GetInt("CanSaveLogDataFile", 1) == 1;

        /// <summary>
        /// 可打印的日志标签集合
        /// </summary>
        public static List<LogTag> CanPrintLogTagList = new List<LogTag>
        {
            LogTag.Temp,
            LogTag.Test,
            LogTag.Forever //当前标签不受约束，必然显示，可加可不加
        };

        /// <summary>
        /// 是否允许控制台打印日志标签中的 所有非错误日志
        /// </summary>
        public static bool CanPrintConsoleLog
        {
            get
            {
                return m_CanPrintConsoleLog;
            }
            set
            {
                UnityEngine.PlayerPrefs.SetInt("CanPrintConsoleLog", value ? 1 : 0);
                m_CanPrintConsoleLog = value;
            }
        }

        /// <summary>
        /// 允许控制台打印日志标签中的 错误日志
        /// </summary>
        public static bool CanPrintConsoleLogError
        {
            get
            {
                return m_CanPrintConsoleLogError;
            }
            set
            {
                UnityEngine.PlayerPrefs.SetInt("CanPrintConsoleLogError", value ? 1 : 0);
                m_CanPrintConsoleLogError = value;
            }
        }

        /// <summary>
        /// 允许改变控制台打印样式
        /// </summary>
        public static bool CanChangeConsolePrintStyle = true;
        #endregion


        #region 缓存日志文件到本地
        /// <summary>
        /// 允许日志文件到缓存本地
        /// </summary>
        public static bool CanSaveLogDataFile
        {
            get
            {
                return m_CanSaveLogDataFile;
            }
            set
            {
                UnityEngine.PlayerPrefs.SetInt("CanSaveLogDataFile", value ? 1 : 0);
                m_CanSaveLogDataFile = value;
            }
        }
        /// <summary>
        /// 允许写入硬件数据信息
        /// </summary>
        public static bool CanWriteDeviceHardwareData = true;
        /// <summary>
        /// 缓存历史日志文件最大数量
        /// </summary>
        public static uint MaxCountCacheLogFile = 10;
        #endregion

        /// <summary>
        /// 获取当日志系统配置状态
        /// </summary>
        public static void GetDebuggerConfigState()
        {
            string logTagStr = string.Empty;
            for (int i = 0; i < CanPrintLogTagList.Count; i++)
            {
                logTagStr += CanPrintLogTagList[i];
                if (i != CanPrintLogTagList.Count - 1)
                {
                    logTagStr += "、";
                }
            }
            UnityEngine.Debug.Log($"当前控制台日志状态：" +
                 $"\n1.日志标签集合：{logTagStr}" +
                 $"\n2.打印非错误异常日志：{(CanPrintConsoleLog ? "已开启" : "已关闭")}" +
                 $"\n3.打印错误异常日志：{(CanPrintConsoleLogError ? "已开启" : "已关闭")}" +
                 $"\n4.缓存日志信息到本地：{(CanSaveLogDataFile ? "已开启" : "已关闭")}");
        }
    }
}