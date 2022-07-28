using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MFramework_Editor
{
    public class EditorMenu : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("MFramework/01.自动打包框架UnityPackage包     %e")]
#endif
        private static void MenuClicked1()
        {
            string packageName = "MFramework_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".unitypackage";
            //准备导出unitypackage资源路径
            string exportPathName = "Assets/MFramework";
            ExportUnityPactage(packageName, exportPathName);
        }


#if UNITY_EDITOR
        [MenuItem("MFramework/02.判定当前视图 是否为16：9分辨率")]
#endif
        private static void MenuClicked2()
        {
            Debug.Log(CheckScreenRatio(16, 9));
        }


        /// <summary>
        /// 导出unitypackage格式项目资源
        /// 导出位置固定  导出完成自动打开unitypackage所在文件夹
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="exportPathName"></param>
        public static void ExportUnityPactage(string packageName, string exportPathName)
        {
            AssetDatabase.ExportPackage(exportPathName, packageName, ExportPackageOptions.Recurse);
            //打开本地文件夹  目录不允许有中文
            Application.OpenURL("file://" + Application.dataPath + "../../");
        }


        /// <summary>
        /// 检测设备分辨率   当前游戏场景分辨率与目标分辨率比对
        /// </summary>
        private static bool CheckScreenRatio(int width, int height)
        {
            //获取本机宽高比
            float ratioLocal = Screen.width > Screen.height ? (float)Screen.width / Screen.height : (float)Screen.height / Screen.width;
            //获取目标宽高比
            float ratioTarget = width > height ? (float)width / height : (float)height / width;
            //容错
            float value = 0.03f;
            Debug.Log(ratioLocal + " " + ratioTarget);
            return MathF.Abs(ratioLocal - ratioTarget) < value;
        }
    }
}

