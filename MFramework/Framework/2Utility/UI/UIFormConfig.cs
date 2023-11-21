using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 标题：UI窗体配置表
    /// 功能：绑定映射UI窗体实体与mono脚本
    /// 三种绑定方式
    /// 1.自动绑定，Webgl平台非编辑器下不可用
    /// 2.通过内部代码手动绑定
    /// 3.通过配置json表手动绑定，查询json配置表不存在时，在本地自动生成默认配置表
    /// 作者：毛俊峰
    /// 时间：2023.01.04、2023.09.04、2023.11.20
    /// </summary>
    public class UIFormConfig : SingletonByMono<UIFormConfig>
    {
        /// <summary>
        /// UIForm预设实体根目录
        /// </summary>
        public const string UIFormRootDir = "Assets/GameMain/UI/UIForms";
        /// <summary>
        /// json配置文件绝对路径
        /// </summary>
        private static string m_JsonFilePath = Application.streamingAssetsPath + "/MF/ConfigUIBind.json";
        /// <summary>
        /// UI窗体实体映射表   key-mono脚本名 value-UI实体信息路径
        /// </summary>
        private Dictionary<string, UIEntityConfigInfo> m_DictUIFormConfig = new Dictionary<string, UIEntityConfigInfo>();

        /// <summary>
        /// UI实体信息
        /// </summary>
        public class UIEntityConfigInfo
        {
            /// <summary>
            /// UI窗体资源路径
            /// </summary>
            public string uiFormEntityName;
            /// <summary>
            /// UI窗体层级
            /// </summary>
            public UILayerType uiLayerType;

            public UIEntityConfigInfo(string uiFormEntityName, UILayerType uiLayerType = UILayerType.Common)
            {
                this.uiFormEntityName = uiFormEntityName;
                this.uiLayerType = uiLayerType;
            }
        }

        /// <summary>
        /// 绑定UI窗体信息方式
        /// </summary>
        public enum BindType
        {
            /// <summary>
            /// 手动绑定
            /// </summary>
            Manual,
            /// <summary>
            /// 自动绑定
            /// </summary>
            Auto,
            /// <summary>
            /// 根据Json配置文件绑定
            /// </summary>
            Json,
        }

        public bool IsBindComplete { get; private set; }
        /// <summary>
        /// 绑定UI预制体信息
        /// </summary>
        public void Bind(BindType bindType)
        {
            if (IsBindComplete)
            {
                Debugger.LogWarning("已绑定UI预制体信息");
                return;
            }
            Debugger.Log("正在绑定UI预制体信息，BindType：" + bindType, LogTag.MF);
            switch (bindType)
            {
                case BindType.Manual:
                    ManualBindUiFormInfo();
                    IsBindComplete = true;
                    break;
                case BindType.Auto:
                    AutoBindUIFormInfo();
                    IsBindComplete = true;
                    break;
                case BindType.Json:
                    BindUIFormInfoByJson(m_JsonFilePath);
                    break;
            }
        }

        /// <summary>
        /// 手动绑定UI窗体信息
        /// </summary>
        private void ManualBindUiFormInfo()
        {
            //方法体内部新增代码手动绑定
            //Add<UIFormTest>(new UIEntityConfigInfo(UIFormRootDir + "/Main/UIFormTest.prefab", UILayerType.Common));
        }

        /// <summary>
        /// 自动绑定UI窗体信息，Webgl平台非编辑器下不可用
        /// 优点：自动化配置，外部无需手动配置
        /// 缺点：通过反射遍历所有Assembly程序集下的类，找到所有UIFormBase的派生类进行配置，可能存在性能问题
        /// 建议：开发阶段且非webgl平台下使用
        /// </summary>
        public void AutoBindUIFormInfo()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            Debugger.LogError("自动绑定UI窗体信息，Webgl平台非编辑器下不可用");
            return;
#endif
            Type[] typeArr = Assembly.GetCallingAssembly().GetTypes();
            Type targetType = typeof(UIFormBase);

            foreach (Type type in typeArr)
            {
                if (type.BaseType == null)
                {
                    continue;
                }
                if (type.BaseType?.Name == targetType.Name)
                {
                    UIFormBase uIForm = System.Activator.CreateInstance(type) as UIFormBase;
                    Add(type, new UIEntityConfigInfo(uIForm.UIFormEntitySubPath, uIForm.GetUIFormLayer));
                    Debugger.Log("Auto Bind UIForm，Name：" + type.Name, LogTag.MF);
                }
            }
        }

        /// <summary>
        /// 根据json配置表手动绑定UI窗体信息
        /// </summary>
        /// <param name="jsonFilePath"></param>
        private void BindUIFormInfoByJson(string jsonFilePath)
        {

            bool isWebgl = false;
#if UNITY_WEBGL
            isWebgl = true;
#else
            isWebgl = false;
#endif
            Debugger.Log("Try Read json ,isWebgl：" + isWebgl + "，jsonFilePath：" + jsonFilePath, LogTag.MF);

#if UNITY_EDITOR
            if (!File.Exists(jsonFilePath))
            {
                Debugger.LogError("未找到配置文件，即将自动生成默认配置文件。 jsonFilePath：" + jsonFilePath);
                CreateBindUIFormJsonConfigFile(jsonFilePath);
            }
#endif
            //读取json文件
            if (!isWebgl)//非Webgl平台 使用IO流读取
            {
                string[] jsonFilePathArr = jsonFilePath.Split('/');
                string fileName = jsonFilePathArr[jsonFilePathArr.Length - 1];
                string rootPath = jsonFilePath.Replace(fileName, "");
                string jsonStr = new FileIOTxt(rootPath, fileName).Read();
                ParseBindJson(jsonStr);
                IsBindComplete = true;
                Debugger.Log("UI Bind Completed By JsonFile ,isWebgl：", LogTag.MF);

            }
            else
            {
                //Webgl平台 使用UnityWebRequest来读取json文件，目的是为了兼容Webgl平台，前者平台不支持IO流
                //注意：当前为异步绑定窗体信息，所需需要等待绑定完成后，才能执行UIManager.GetInstance().Show(...)方法
                UnityTool.GetInstance.DownLoadAssetsByURL<string>(jsonFilePath, (jsonStr) =>
                {
                    ParseBindJson(jsonStr);
                    IsBindComplete = true;
                    Debugger.Log("UI Bind Completed By JsonFile ,isWebgl：", LogTag.MF);
                }, () =>
                {
                    Debugger.LogError("根据json配置表手动绑定UI窗体信息失败，解析失败，请检查json文件。jsonFilePath：" + jsonFilePath);
                    IsBindComplete = false;
                });
            }
        }

        /// <summary>
        /// 解析绑定UI窗体的json信息
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        private bool ParseBindJson(string jsonStr)
        {
            try
            {
                BindUIForm bindUIForm = JsonUtility.FromJson<BindUIForm>(jsonStr);
                foreach (var item in bindUIForm.uIFormInfos)
                {
                    Add(item.uiFormLogicName, new UIEntityConfigInfo(item.uiFormEntitySubPath, (UILayerType)item.uiLayerType));
                }
                return true;
            }
            catch (Exception e)
            {
                Debugger.LogError("根据json配置表手动绑定UI窗体信息失败，解析失败，请检查json文件。jsonStr：" + jsonStr + " , e ：" + e);
                return false;
            }
        }

        /// <summary>
        /// 生成绑定UI窗体的json默认配置文件
        /// </summary>
        public static void CreateBindUIFormJsonConfigFile(string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = m_JsonFilePath;
            }
            if (File.Exists(filePath))
            {
                Debugger.LogError("无法生成绑定UI窗体的json默认配置文件，文件已存在，filePath："+ filePath);
                return;
            }

            string json = JsonUtility.ToJson(new BindUIForm()
            {
                uiFormRootDir = UIFormRootDir,
                uIFormInfos = new List<BindUIForm.UIFormInfo>
                 {
                     new BindUIForm.UIFormInfo
                     {
                          uiFormLogicName="UIFormTest",
                          uiFormEntitySubPath="/Main/UIFormTest.prefab",
                          uiLayerType = 1
                     },
                      new BindUIForm.UIFormInfo
                     {
                          uiFormLogicName="UIFormXxx1",
                          uiFormEntitySubPath="/xx/xxx.prefab",
                          uiLayerType = 0
                     },
                      new BindUIForm.UIFormInfo
                     {
                          uiFormLogicName="UIFormXxx2",
                          uiFormEntitySubPath="/xx/xxx.prefab",
                          uiLayerType = 2
                     }
                 }
            }, true);
            string[] filePathArr = filePath.Split('/');

            string fileName = filePathArr[filePathArr.Length - 1];
            string rootPath = filePath.Replace(fileName, "");
            new FileIOTxt(rootPath, fileName).Write(json);
            Debugger.Log("已创建绑定UI窗体的默认配置文件， filePath：" + filePath + "，请编辑前者配置文件，用于UI窗体加载显示", LogTag.MF);
        }

        /// <summary>
        /// 新增UI预制体信息到容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiEntityConfigInfo"></param>
        private void Add<T>(UIEntityConfigInfo uiEntityConfigInfo) where T : UIFormBase
        {
            Add(typeof(T), uiEntityConfigInfo);
        }

        /// <summary>
        /// 新增UI预制体信息到容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiEntityConfigInfo"></param>
        private void Add(Type type, UIEntityConfigInfo uiEntityConfigInfo)
        {
            string uiFormLogicName = type.Name;
            if (!m_DictUIFormConfig.ContainsKey(uiFormLogicName))
            {
                m_DictUIFormConfig.Add(uiFormLogicName, uiEntityConfigInfo);
            }
            else
            {
                Debugger.LogError("UIFormConfig is Exist， uiFormLogicName：" + uiFormLogicName + "，uiFormEntityName：" + uiEntityConfigInfo.uiFormEntityName);
            }
        }
        private void Add(string uiFormLogicName, UIEntityConfigInfo uiEntityConfigInfo)
        {
            if (!m_DictUIFormConfig.ContainsKey(uiFormLogicName))
            {
                m_DictUIFormConfig.Add(uiFormLogicName, uiEntityConfigInfo);
            }
            else
            {
                Debugger.LogError("UIFormConfig is Exist， uiFormLogicName：" + uiFormLogicName + "，uiFormEntityName：" + uiEntityConfigInfo.uiFormEntityName);
            }
        }


        /// <summary>
        /// 获取UI窗体配置数据，根据UIFormBase派生类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UIEntityConfigInfo GetUIFormConfigData<T>() where T : UIFormBase
        {
            UIEntityConfigInfo res = null;
            if (m_DictUIFormConfig.ContainsKey(typeof(T).Name))
            {
                res = m_DictUIFormConfig[typeof(T).Name];
            }
            return res;
        }
    }

    /// <summary>
    /// 绑定UI窗体信息 数据结构
    /// </summary>
    [Serializable]
    public class BindUIForm
    {
        /// <summary>
        /// UI窗体预设体存放位置根目录 Assets/xxx/xx
        /// </summary>
        public string uiFormRootDir;
        public List<UIFormInfo> uIFormInfos;
        [Serializable]
        public class UIFormInfo
        {
            /// <summary>
            /// 脚本名称  UIFormXxx
            /// </summary>
            public string uiFormLogicName;
            /// <summary>
            /// UI窗体预设体存放相对路径  /.../xxx.prefab
            /// </summary>
            public string uiFormEntitySubPath;
            /// <summary>
            /// UI所在层级【0,2】, 映射UILayerType枚举类型
            /// </summary>
            public int uiLayerType;
        }
    }
}
