using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：程序入口
    /// 功能：初始化游戏框架、游戏逻辑
    /// 作者：毛俊峰
    /// 时间：2022.12.28
    /// </summary>
    public class GameLaunch : SingletonByMono<GameLaunch>
    {
        [SerializeField]
        private LaunchModel m_LaunchModel = LaunchModel.BuilderModel;
        /// <summary>
        /// 项目运行模式
        /// </summary>
        public LaunchModel LaunchModel { get => m_LaunchModel; }


        private void Awake()
        {
            //初始化游戏框架
            this.InitFramework();
            //检查资源更新
            this.CheckHotUpdate();
            //初始化游戏逻辑
            this.InitGameLogic();
        }


        private void CheckHotUpdate()
        {
            //获取服务器资源 脚本代码版本

            //拉去下载列表

            //下载更新资源到本地

        }
        private void InitFramework()
        {
            #region Init Log
            //日志打印
            DebuggerConfig.MaxCountCacheLogFile = 10;
            DebuggerConfig.CanWriteDeviceHardwareData = true;
            //实时缓存日志到本地
            DebuggerConfig.CanChangeConsolePrintStyle = true;
            DebuggerConfig.CanPrintLogTagList = m_LaunchModel == LaunchModel.EditorModel ?
                new List<LogTag> { LogTag.Temp, LogTag.Test, LogTag.Forever } : new List<LogTag> { LogTag.Forever };
            DebuggerConfig.CanPrintConsoleLog = m_LaunchModel == LaunchModel.EditorModel;
            DebuggerConfig.CanPrintConsoleLogError = m_LaunchModel == LaunchModel.BuilderModel;     
            #endregion
        }
        private void InitGameLogic()
        {
            //this.gameObject.AddComponent<GameLogic>().Init();
            GameLogic.GetInstance.Init();
        }
    }

    public enum LaunchModel
    {
        /// <summary>
        /// 引擎调试模式
        /// </summary>
        EditorModel,
        /// <summary>
        /// 真机打包模式
        /// </summary>
        BuilderModel,
    }
}
