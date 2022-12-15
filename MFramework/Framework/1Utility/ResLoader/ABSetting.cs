using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：AB自动化标记、打包设置
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.09.28
    /// 版本：1.0
    /// </summary>
    public class ABSetting
    {
        /// <summary>
        /// 标记AB，对当前(Assets目录下)子路径下资源进行自动标记，
        /// </summary>
        public static string assetsSubPath = "AssetsRes/ABRes";
        /// <summary>
        /// 打包AB，AB包存放路径
        /// </summary>
        public static string assetBundleBuildPath = Application.streamingAssetsPath;
        /// <summary>
        /// 默认加载方式 编辑器模式下
        /// </summary>
        public static ResType resTypeDefaultEditor = ResType.ResEditor;
        /// <summary>
        /// 默认加载方式 非编辑器模式下
        /// </summary>
        public static ResType resTypeDefaultNotEditor = ResType.ResAssetBundleAsset;
    }
}