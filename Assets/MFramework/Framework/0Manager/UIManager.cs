using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// 标题：UI管理器
    /// 功能：UI资源的加载、卸载、面板层级配置、屏幕适配
    /// 作者：毛俊峰
    /// 时间：2022.07.16
    /// 版本：1.0
    /// </summary>
    public class UIManager
    {
        /// <summary>
        /// 缓存加载后的面板  key-面板ID（唯一标识）
        /// </summary>
        private static Dictionary<int, PanelInfo> m_DicUIPanelInfoContainer = new Dictionary<int, PanelInfo>();
        class PanelInfo
        {
            public GameObject panel;
        }

        private static GameObject m_UIRoot;
        public static GameObject UIRoot
        {
            get
            {
                if (!m_UIRoot)
                {
                    m_UIRoot = Object.Instantiate(Resources.Load<GameObject>("UI/Canvas/UIRoot"));
                }
                return m_UIRoot;
            }
        }
        /// <summary>
        /// UI层级
        /// </summary>
        public enum UILayerType
        {
            Bg,
            Common,
            Top
        }

        /// <summary>
        /// 设置Canvas分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="matchValue"></param>
        public static void SetResolution(float width, float height, float matchValue = 0)
        {
            CanvasScaler canvasScaler = UIRoot.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(width, height);
            canvasScaler.matchWidthOrHeight = matchValue;
        }

        /// <summary>
        /// 加载UI面板
        /// 返回：面板ID（唯一标识）
        /// </summary>
        /// <param name="resName">面板重命名 约定：</param>
        /// <param name="resPath">面板预设位置全路径</param>
        /// <param name="uILayerType">>面板层级</param>
        /// <returns></returns>
        public static int LoadPanel(string resName, string resPath, UILayerType uILayerType)
        {
            GameObject loadObj = Resources.Load<GameObject>(resPath);
            if (loadObj == null)
            {
                Debug.LogError("加载UI面板失败 检查Resources资源路径 resPath ：" + resPath);
                return -1;
            }
            GameObject clone = Object.Instantiate(loadObj);
            clone.name = resName;
            RectTransform cloneRect = clone.GetComponent<RectTransform>();
            Transform parent = UIRoot.transform.Find(uILayerType.ToString());
            cloneRect.SetParent(parent);
            cloneRect.offsetMax = Vector3.zero;
            cloneRect.offsetMin = Vector3.zero;
            cloneRect.anchoredPosition3D = Vector3.zero;
            cloneRect.anchorMin = Vector2.zero;
            cloneRect.anchorMax = Vector2.one;
            cloneRect.pivot = new Vector2(0.5f, 0.5f);

            int panelID = GetNewPanelID();
            m_DicUIPanelInfoContainer.Add(panelID, new PanelInfo() { panel = clone });
            return panelID;
        }

        /// <summary>
        /// 卸载面板
        /// </summary>
        /// <param name="panelID"></param>
        public static void UnloadPanel(int panelID)
        {
            Object.Destroy(GetPanelByID(panelID));
        }

        /// <summary>
        /// 获取面板实例根据面板ID
        /// </summary>
        /// <param name="panelID"></param>
        public static GameObject GetPanelByID(int panelID)
        {
            if (m_DicUIPanelInfoContainer.ContainsKey(panelID))
            {
                return m_DicUIPanelInfoContainer[panelID].panel;
            }
            else
            {
                Debug.LogError("获取面板实例失败 检查 panelID：" + panelID);
                return null;
            }
        }

        /// <summary>
        /// 获取新面板ID
        /// </summary>
        private static int GetNewPanelID()
        {
            int newPanelID = Random.Range(1, 1000);
            while (m_DicUIPanelInfoContainer.ContainsKey(newPanelID))
            {
                newPanelID = Random.Range(1, 1000);
            }
            return newPanelID;
        }
    }


}