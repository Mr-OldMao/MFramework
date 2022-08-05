#if UNITY_EDITOR
using MFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace MFramework_Editor
{
    /// <summary>
    /// 标题：Json自动映射成为实体类(基于Newtonsoft.Json) 脚本自动化生成工具
    /// 目的：解决前后端协议传输，获取后端Json数据需要手动创建对应的实体类的问题
    /// 功能：输入Json格式的字符串，指定实体类的路径，自动生成Json对应的实体类
    /// 作者：毛俊峰
    /// 时间：2022.08.05
    /// 版本：1.0
    /// </summary>
    public class EditorAutoCreateScriptJsonMapClass : ScriptableWizard
    {
        public static JsonMapClassStruct jsonMapClassStruct;
        //[MenuItem("MFramework/脚本自动化工具/1.Json自动映射实体类", false, 2)]
        public static void CreateWizard()
        {
            DisplayWizard<EditorAutoCreateScriptJsonMapClass>("Json自动映射实体类", "生成", "重置");
        }
        [Header("Json格式字符串")]
        public string jsonStr;
        [Header("脚本类名 格式：Xxx")]
        public string scriptName;
        [Header("脚本路径 格式：x:/xxx/xx/xxx/")]
        public string scriptPath;
        [Header("是否覆盖同路径下的同类名脚本")]
        public bool isOverrideScript;

        private void OnEnable()
        {
            Reset();
        }
        private void OnWizardCreate()
        {
            jsonMapClassStruct = new JsonMapClassStruct
            {
                jsonStr = jsonStr,
                scriptName = scriptName,
                scriptPath = scriptPath,
                isOverrideScript = isOverrideScript
            };
            try
            {
                AutoCreateScriptImpl.BuildScript();
            }
            catch (System.Exception)
            {
                Debug.Log("脚本自动化生成工具运行失败! Json数据 格式错误 请检查 jsonStr：" + jsonMapClassStruct.jsonStr);
            }
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
            if (string.IsNullOrEmpty(jsonStr))
            {
                //给出错误提示
                errorString = "请输入Json";
            }
            else
            {
                if (string.IsNullOrEmpty(scriptName))
                {
                    errorString = "请填写脚本类名";
                }
                else if (string.IsNullOrEmpty(scriptPath))
                {
                    errorString = "请填写脚本路径";
                }
                else
                {
                    helpString = "点击“生成”尝试创建Json实体类";
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
            jsonStr = string.Empty;
            scriptName = string.Empty;
            scriptPath = EditorMenu.autoCreateScriptDefaultPath + "JsonMapEntity/";
            isOverrideScript = false;
        }

        class AutoCreateScriptImpl
        {
            /// <summary>
            /// 映射内容  key-自定义类名  value-类结构
            /// </summary>
            private static Dictionary<string, ClassInfo> dicClassContent = new Dictionary<string, ClassInfo>();

            private class ClassInfo
            {
                public string classHead;
                public string classBody;
                public string classEnd;
            }

            public static void BuildScript()
            {
                JsonMapClassStruct jsonMapClassStruct = EditorAutoCreateScriptJsonMapClass.jsonMapClassStruct;
                DeserializeObject(jsonMapClassStruct.jsonStr, jsonMapClassStruct.scriptName);
                string scriptContent = "";
                foreach (var item in dicClassContent)
                {
                    string subContent = item.Value.classHead + item.Value.classBody + item.Value.classEnd;
                    scriptContent += subContent;
                    //Debug.Log("k: " + item.Key + "，v：" + subContent);
                }
                scriptContent = AutoCreateScriptTemplate.classTemplateHead(jsonMapClassStruct.scriptName, scriptContent);
                WriteFile(jsonMapClassStruct, scriptContent);
            }

            static int temp = 1;
            //反序列化json成为 Obj
            private static void DeserializeObject(string jsonStr, string className)
            {
                JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonStr);//将json字符串转化为JObject
                foreach (var obj in jObject)
                {
                    //Debug.Log("temp:" + temp + ",item.Key:" + obj.Key + " ,item.Value.Type:" + obj.Value.Type + ",item.Value : " + obj.Value);
                    if (obj.Value.Type == JTokenType.Integer)
                    {
                        //Debug.Log("temp:" + temp + ",type:" + obj.Value.Type + ",k:" + obj.Key + ",v:" + obj.Value);
                        SpliceStr(className, obj.Key, obj.Value.Type);
                    }
                    else if (obj.Value.Type == JTokenType.String)
                    {
                        //Debug.Log("temp:" + temp + ",type:" + obj.Value.Type + ",k:" + obj.Key + ",v:" + obj.Value);
                        SpliceStr(className, obj.Key, obj.Value.Type);
                    }
                    else if (obj.Value.Type == JTokenType.Array)
                    {
                        string subJson = obj.Value.ToString();
                        temp++;
                        //Debug.Log("temp:" + temp + ",type:" + obj.Value.Type + ",k:" + obj.Key + ",v:" + obj.Value + ",subJson：" + subJson);
                        //遍历数组
                        foreach (var arrItem in obj.Value)
                        {
                            //Debug.Log("temp:" + temp + ",arrItem  数组元素类型:" + arrItem.Type + "数组元素v:" + arrItem);
                            if (arrItem.Type == JTokenType.Object || arrItem.Type == JTokenType.Array)
                            {
                                temp++;
                                //Debug.Log("temp:" + temp + ",subJson " + subJson);
                                SpliceStr(className, obj.Key, obj.Value.Type);
                                DeserializeObject(arrItem.ToString(), obj.Key);
                            }
                            else
                            {
                                string typeStr = GetFieldTypeByJTokenType("", arrItem.Type) + "[]";
                                SpliceStr(className, obj.Key, JTokenType.None, typeStr);
                            }
                            break;//仅遍历一次
                        }
                    }
                    else if (obj.Value.Type == JTokenType.Object)
                    {
                        temp++;
                        string subJson = obj.Value.ToString();
                        //Debug.Log("temp:" + temp + ",type:" + obj.Value.Type + ",k:" + obj.Key + ",v:" + obj.Value + ",subJson " + subJson);
                        SpliceStr(className, obj.Key, obj.Value.Type);
                        DeserializeObject(subJson, obj.Key);
                    }
                }
            }
            /// <summary>
            /// 拼接字符串
            /// </summary>
            /// <param name="subClassName">类名</param>
            /// <param name="fieldName"></param>
            /// <param name="jTokenType"></param>
            /// <param name="customType">基本数据数组类型</param>
            private static void SpliceStr(string subClassName, string fieldName, JTokenType jTokenType, string baseTypeArray = "")
            {
                string dataType = string.IsNullOrEmpty(baseTypeArray) ? GetFieldTypeByJTokenType(fieldName, jTokenType) : baseTypeArray;

                if (!dicClassContent.ContainsKey(subClassName))
                {
                    ClassInfo classInfo = new ClassInfo();
                    if (subClassName == jsonMapClassStruct.scriptName)
                    {
                        classInfo.classHead = "";
                        classInfo.classEnd = "";
                    }
                    else
                    {
                        //类名首字母大写
                        string changeClassName = subClassName.StringFirstUpper(subClassName);  //StringFirstUpper(subClassName);
                        classInfo.classHead = "\n\tpublic class " + changeClassName + "\n\t{";
                        classInfo.classEnd = "\n\t}";
                    }
                    classInfo.classBody = "";
                    dicClassContent.Add(subClassName, classInfo);
                }
                //字段缩进格式   class缩进一次 subClass缩进两次
                string format = subClassName == jsonMapClassStruct.scriptName ? "\n\t" : "\n\t\t";
                dicClassContent[subClassName].classBody += (format + AutoCreateScriptTemplate.FieldTemplate(fieldName, dataType));
            }

            /// <summary>
            /// 获取字段类型 根据JTokenType
            /// </summary>
            /// <param name="jTokenType"></param>
            /// <returns></returns>
            private static string GetFieldTypeByJTokenType(string fieldName, JTokenType jTokenType)
            {
                string fieldTypeStr = "";
                switch (jTokenType)
                {
                    case JTokenType.None:
                        break;
                    case JTokenType.Object:
                        fieldTypeStr = fieldName.StringFirstUpper(fieldName);// StringFirstUpper(fieldName);
                        break;
                    case JTokenType.Array:
                        fieldTypeStr = fieldName.StringFirstUpper(fieldName) + "[]"; //StringFirstUpper(fieldName) + "[]";
                        break;
                    case JTokenType.Constructor:
                        break;
                    case JTokenType.Property:
                        break;
                    case JTokenType.Comment:
                        break;
                    case JTokenType.Integer:
                        fieldTypeStr = "int";
                        break;
                    case JTokenType.Float:
                        break;
                    case JTokenType.String:
                        fieldTypeStr = "string";
                        break;
                    case JTokenType.Boolean:
                        break;
                    case JTokenType.Null:
                        break;
                    case JTokenType.Undefined:
                        break;
                    case JTokenType.Date:
                        break;
                    case JTokenType.Raw:
                        break;
                    case JTokenType.Bytes:
                        break;
                    case JTokenType.Guid:
                        break;
                    case JTokenType.Uri:
                        break;
                    case JTokenType.TimeSpan:
                        break;
                    default:
                        break;
                }
                return fieldTypeStr;
            }

            /// <summary>
            /// 写入本地
            /// </summary>
            /// <param name="jsonMapClassStruct"></param>
            /// <param name="scriptContent"></param>
            private static void WriteFile(JsonMapClassStruct jsonMapClassStruct, string scriptContent)
            {
                string scriptAllPath = jsonMapClassStruct.scriptPath + jsonMapClassStruct.scriptName + ".cs";
                //写入本地
                //判定文件夹
                if (!Directory.Exists(jsonMapClassStruct.scriptPath))
                {
                    Directory.CreateDirectory(jsonMapClassStruct.scriptPath);
                }
                if (!Directory.Exists(jsonMapClassStruct.scriptPath))
                {
                    Debug.LogError("脚本自动化失败 脚本路径不合法 scriptPath：" + jsonMapClassStruct.scriptPath);
                    return;
                }
                //脚本已存在
                if (File.Exists(scriptAllPath))
                {
                    //不允许覆盖
                    if (!jsonMapClassStruct.isOverrideScript)
                    {
                        Debug.LogError("脚本自动化失败 脚本已存在且不允许覆盖 scriptAllPath：" + scriptAllPath + "，isOverrideScript：" + jsonMapClassStruct.isOverrideScript);
                        return;
                    }
                }
                FileStream file = new FileStream(scriptAllPath, FileMode.Create); //FileMode.Create 新建新的文件 如果文件存在，则直接覆盖    FileMode.CreateNew 新建新的文件 如果文件存在，则产生异常
                StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
                fileW.Write(scriptContent);
                fileW.Flush();
                fileW.Close();
                file.Close();
                Debug.Log("脚本自动化成功 scriptName：" + jsonMapClassStruct.scriptName + "，scriptAllPath：" + scriptAllPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }



        /// <summary>
        /// 自动生成代码模板
        /// </summary>
        public class AutoCreateScriptTemplate
        {
            /// <summary>
            /// 头节点模板
            /// </summary>
            /// <param name="className"></param>
            /// <param name="content"></param>
            /// <returns></returns>
            public static string classTemplateHead(string className, string content)
            {
                string res =
@"
/// <summary>
/// Json映射的实体类 以下代码自动生成
/// 接口地址：
/// 创建时间：" + System.DateTime.Now + @"
/// </summary>
public class " + className + "\n{  " + content + "\n}";
                return res;
            }

            /// <summary>
            /// 字段模板
            /// </summary>
            /// <param name="fieldName"></param>
            /// <param name="fieldType"></param>
            /// <returns></returns>
            public static string FieldTemplate(string fieldName, string fieldType)
            {
                return "public " + fieldType + " " + fieldName + ";";
            }
        }
        /// <summary>
        /// 脚本自动化结构体
        /// </summary>
        public struct JsonMapClassStruct
        {
            /// <summary>
            /// Json格式字符串
            /// </summary>
            public string jsonStr;
            /// <summary>
            /// 脚本类名
            /// </summary>
            public string scriptName;
            /// <summary>
            /// 脚本路径
            /// </summary>
            public string scriptPath;
            /// <summary>
            /// 是否覆盖同路径下的同类名脚本
            /// </summary>
            public bool isOverrideScript;
        }
    }


}
#endif