using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：热更新路径配置
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.2022.110.10
    /// 版本：1.0
    /// </summary>
    public class HotUpdateConfig
    {
        /// <summary>
        /// 本地版本信息文件名
        /// </summary>
        public static string hotUpdateVersionFileName = "LocalVersion.json";
        /// <summary>
        /// 热更前本地版本信息文件根目录
        /// </summary>
        public static string localVersionRootPath = Application.streamingAssetsPath + "/Version";
        /// <summary>
        /// 热更后本地版本信息文件根目录
        /// </summary>
        public static string hotUpdatedLocalVersionRootPath = Application.persistentDataPath + "/Version";
    }
}