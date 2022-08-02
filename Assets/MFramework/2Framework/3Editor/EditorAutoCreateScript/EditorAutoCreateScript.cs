#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// 标题：脚本自动化生成工具
    /// 目的：解决手动获取场景对象实例需要繁琐且重复的操作
    /// 功能：选中菜单MFramework/脚本自动化工具，拖入根节点，设置脚本路径，点击生成，可自动获取根节点其下字段、属性、映射并生成脚本到指定路径 
    /// 规则：游戏对象的命名要规范，要以小写字母开头，不要使用创建时默认的名称会被屏蔽
    /// 作者：毛俊峰
    /// 时间：2022.08.01
    /// 版本：1.0
    /// </summary>
    public class EditorAutoCreateScript : ScriptableWizard
    {
        [MenuItem("MFramework/脚本自动化工具")]
        static void CreateWizard()
        {
            //创建CubChange对话窗，第一个参数为弹窗名，第二个参数为Create按钮名(不填参默认为Create)
            ScriptableWizard.DisplayWizard<EditorAutoCreateScript>("脚本自动化", "生成", "重置");
        }
        [Header("根节点")]
        public Transform rootTrans;
        [Header("脚本类名 格式：Xxx")]
        public string scriptName;
        [Header("脚本路径 格式：x:/xxx/xx/xxx/")]
        public string scriptPath;
        [Header("是否创建根节点旗下未激活的游戏对象字段")]
        public bool isCreateInactiveChildObj;
        [Header("是否覆盖同路径下的同类名脚本")]
        public bool isOverrideScript;

        private void OnEnable()
        {
            Reset();
        }
        private void OnWizardCreate()
        {
            AutoCreateScriptStructInfo autoCreateScriptStructInfo = new AutoCreateScriptStructInfo
            {
                rootTrans = rootTrans,
                scriptName = scriptName,
                scriptPath = scriptPath,
                isCreateHideObj = isCreateInactiveChildObj,
                isOverrideScript = isOverrideScript
            };
            AutoCreateScript.BuildUIScript(autoCreateScriptStructInfo);
        }
        private void OnWizardOtherButton()
        {
            Debug.Log("重置");
            Reset();
        }
        private void OnWizardUpdate()
        {
            helpString = null;
            errorString = null;

            if (Selection.gameObjects.Length > 0)
            {
                helpString = "选中：" + Selection.gameObjects[0];
            }
            else
            {
                if (!rootTrans)
                {
                    //给出错误提示
                    errorString = "请选择场景中的游戏对象作为根节点放入 rootTrans";
                }
                else if (string.IsNullOrEmpty(scriptName))
                {
                    errorString = "请填写脚本类名 ScriptName";
                }
            }
        }

        /// <summary>
        /// 当鼠标选中的物体发生改变时调用此方法
        /// </summary>
        private void OnSelectionChange()
        {
            OnWizardUpdate();
        }

        private void Reset()
        {
            rootTrans = null;
            scriptName = string.Empty;
            scriptPath = Application.dataPath + "/ScriptAuto/";
            isCreateInactiveChildObj = true;
            isOverrideScript = false;
        }
    }

    /// <summary>
    /// 脚本自动化结构体
    /// </summary>
    public struct AutoCreateScriptStructInfo
    {
        /// <summary>
        /// 场景目标父对象
        /// </summary>
        public Transform rootTrans;
        /// <summary>
        /// 脚本类名
        /// </summary>
        public string scriptName;
        /// <summary>
        /// 脚本路径
        /// </summary>
        public string scriptPath;
        /// <summary>
        /// 是否创建未激活的游戏对象字段
        /// </summary>
        public bool isCreateHideObj;
        /// <summary>
        /// 是否覆盖同路径下的同类名脚本
        /// </summary>
        public bool isOverrideScript;
    }


    public class AutoCreateScript
    {
        //拼接字段
        static string fieldContent = string.Empty;
        //拼接属性
        static string propertyContent = string.Empty;
        //拼接字段映射
        static string fieldMapContent = "public void InitMapField()\n\t{\n\t";
        [MenuItem("生成/根据字符串生成枚举类型")]
        public static void BuildUIScript(AutoCreateScriptStructInfo autoCreateScriptStructInfo)
        {
            string scriptPath = autoCreateScriptStructInfo.scriptPath;
            string scriptName = autoCreateScriptStructInfo.scriptName;
            if (!autoCreateScriptStructInfo.rootTrans)
            {
                Debug.LogError("脚本自动化失败 sceneGameObject is null");
                return;
            }
            if (string.IsNullOrEmpty(scriptName))//todo  后期通过正则表达式验证类名
            {
                Debug.LogError("脚本自动化失败 scriptName is null");
                return;
            }
            if (string.IsNullOrEmpty(scriptPath))
            {
                Debug.LogError("脚本自动化失败 scriptPath is null");
                return;
            }
            string scriptAllPath = scriptPath + scriptName + ".cs";

            AutoCreateScriptTemplate.rootTrans = autoCreateScriptStructInfo.rootTrans.transform;
            UnityComponent unityComponent = new UnityComponent();
            unityComponent.SaveAllUIComponentContainerOnly(autoCreateScriptStructInfo.rootTrans.transform, autoCreateScriptStructInfo.isCreateHideObj);
            //场景对象实例集合
            List<object> listContainer = unityComponent.GetListComponentContainer();
            foreach (var dic in listContainer)
            {
                if (dic.GetType() == typeof(Dictionary<string, Button>))
                {
                    Dictionary<string, Button> dicText = dic as Dictionary<string, Button>;
                    foreach (var item in dicText)
                    {
                        //string type = item.Value.GetType().ToString();
                        WriteScript(item.Key, "Button");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, Toggle>))
                {
                    Dictionary<string, Toggle> dicChild = dic as Dictionary<string, Toggle>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "Toggle");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, Text>))
                {
                    Dictionary<string, Text> dicChild = dic as Dictionary<string, Text>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "Text");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, Image>))
                {
                    Dictionary<string, Image> dicChild = dic as Dictionary<string, Image>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "Image");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, Slider>))
                {
                    Dictionary<string, Slider> dicChild = dic as Dictionary<string, Slider>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "Slider");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, ScrollRect>))
                {
                    Dictionary<string, ScrollRect> dicChild = dic as Dictionary<string, ScrollRect>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "ScrollRect");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, Scrollbar>))
                {
                    Dictionary<string, Scrollbar> dicChild = dic as Dictionary<string, Scrollbar>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "Scrollbar");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, InputField>))
                {
                    Dictionary<string, InputField> dicChild = dic as Dictionary<string, InputField>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "InputField");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, TMP_InputField>))
                {
                    Dictionary<string, TMP_InputField> dicChild = dic as Dictionary<string, TMP_InputField>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "TMP_InputField");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, TextMeshProUGUI>))
                {
                    Dictionary<string, TextMeshProUGUI> dicChild = dic as Dictionary<string, TextMeshProUGUI>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "TextMeshProUGUI");
                    }
                    continue;
                }
                if (dic.GetType() == typeof(Dictionary<string, Transform>))
                {
                    Dictionary<string, Transform> dicChild = dic as Dictionary<string, Transform>;
                    foreach (var item in dicChild)
                    {
                        WriteScript(item.Key, "Transform");
                    }
                    continue;
                }
            }
            fieldMapContent += "}";

            string classInfo = AutoCreateScriptTemplate.classTemplate;
            classInfo = classInfo.Replace("{类名}", scriptName);
            classInfo = classInfo.Replace("{字段}", fieldContent);
            classInfo = classInfo.Replace("{属性}", propertyContent);
            classInfo = classInfo.Replace("{方法}", fieldMapContent);

            //写入本地
            //判定文件夹
            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }
            if (!Directory.Exists(scriptPath))
            {
                Debug.LogError("脚本自动化失败 脚本路径不合法 scriptPath：" + scriptPath);
                return;
            }

            //脚本已存在
            if (File.Exists(scriptAllPath))
            {
                //不允许覆盖
                if (!autoCreateScriptStructInfo.isOverrideScript)
                {
                    Debug.LogError("脚本自动化失败 脚本已存在且不允许覆盖 scriptAllPath：" + scriptAllPath + "，isOverrideScript：" + autoCreateScriptStructInfo.isOverrideScript);
                    return;
                }
            }
            FileStream file = new FileStream(scriptAllPath, FileMode.Create); //FileMode.Create 新建新的文件 如果文件存在，则直接覆盖    FileMode.CreateNew 新建新的文件 如果文件存在，则产生异常
            StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
            fileW.Write(classInfo);
            fileW.Flush();
            fileW.Close();
            file.Close();
            Debug.Log("脚本自动化成功 scriptName：" + scriptName + "，scriptAllPath：" + scriptAllPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void WriteScript(string fieldName, string fieldType)
        {
            fieldContent += "public " + fieldType + " " + fieldName + ";\n\t";
            string propertyName = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
            propertyContent += AutoCreateScriptTemplate.propertyTemplate(fieldType, propertyName, fieldName);
            fieldMapContent += AutoCreateScriptTemplate.fieldMapTemplate(fieldName, fieldType);
        }
    }


    /// <summary>
    /// 自动生成代码模板
    /// </summary>
    public class AutoCreateScriptTemplate
    {
        public static Transform rootTrans;
        /// <summary>
        /// 头节点模板
        /// </summary>
        public static string classTemplate =
@"using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using MFramework;
/// <summary>
/// 以下代码都是通过脚本自动生成的
/// </summary>
public class {类名} : MonoBehaviour
{
    {字段}
    
    {属性}

    private void Awake()
    {
		InitMapField();
	}

    {方法}
}
";
        /// <summary>
        /// 属性模板
        /// </summary>
        /// <param name="returnStr"></param>
        /// <param name="PropertyStr"></param>
        /// <param name="fieldStr"></param>
        /// <returns></returns>
        public static string propertyTemplate(string returnStr, string PropertyStr, string fieldStr)
        {
            return "public " + returnStr + " " + PropertyStr + " { get => " + fieldStr + "; set => " + fieldStr + " = value; }\n\t";
        }

        /// <summary>
        /// 字段映射模板
        /// </summary>
        /// <param name="returnStr"></param>
        /// <param name="PropertyStr"></param>
        /// <param name="fieldStr"></param>
        /// <returns></returns>
        public static string fieldMapTemplate(string fieldName, string fieldType)
        {
            return "\t" + @fieldName + @" = transform.FindObject<" + fieldType + ">(\"" + fieldName + "\");\n\t";
        }
    }
}

#endif