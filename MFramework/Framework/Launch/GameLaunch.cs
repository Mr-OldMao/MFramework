using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：程序入口
    /// 功能：初始化游戏框架、游戏逻辑
    /// 作者：毛俊峰
    /// 时间：2022.12.28、2023.11.21
    /// </summary>
    public class GameLaunch : SingletonByMono<GameLaunch>
    {
        [Tooltip("项目运行模式")]
        [SerializeField]
        private LaunchModel m_LaunchModel = LaunchModel.EditorModel;
        /// <summary>
        /// 项目运行模式
        /// </summary>
        public LaunchModel LaunchModel { get => m_LaunchModel; }

        [Tooltip("绑定UI窗体预设体信息方式")]
        [SerializeField]
        private UIFormConfig.BindType bindType = UIFormConfig.BindType.Auto;

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
            if (m_LaunchModel == LaunchModel.BuilderModel)
            {
                DebuggerConfig.MaxCountCacheLogFile = 10;
                DebuggerConfig.CanSaveLogDataFile = true;
                DebuggerConfig.CanWriteDeviceHardwareData = false;
                //实时缓存日志到本地
                DebuggerConfig.CanChangeConsolePrintStyle = true;
                DebuggerConfig.CanPrintLogTagList = m_LaunchModel == LaunchModel.EditorModel ?
                    new List<LogTag> { LogTag.Temp, LogTag.MF, LogTag.Test, LogTag.Forever } : new List<LogTag> { LogTag.Forever };
                DebuggerConfig.CanPrintConsoleLog = true;
                DebuggerConfig.CanPrintConsoleLogError = true; 
            }
            DebuggerConfig.GetDebuggerConfigState();
            #endregion

        }

        private void InitGameLogic()
        {
            #region 加载UI窗体配置文件            
            UIFormConfig.GetInstance.Bind(bindType);
            #endregion

            #region 异步加载ab资源
            //bool loadResCompleted = false;
            //LoadResManager.GetInstance.LoadResAsyncByDirectory("/GameMain/AB/", () =>
            //{
            //    loadResCompleted = true;
            //});
            #endregion

            //等待所有异步资源加载完毕方可进入主逻辑
            UnityTool.GetInstance.DelayCoroutineWaitReturnTrue(() =>
            {
                //return UIFormConfig.GetInstance.IsBindComplete && loadResCompleted;
                return UIFormConfig.GetInstance.IsBindComplete;
            }, () =>
            {
                GameLogic.GetInstance.Init();
            });
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
