using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 标题：框架测试类基类
    /// 功能：可快速新增GUIBtn用于触发函数
    /// 作者：毛俊峰
    /// 日期：2023.11.22
    /// </summary>
    public class TestBase : MonoBehaviour
    {
        private GUISkin m_GUISkin;
        private GUIBtnInfo[] m_GUIBtnInfos;
        private float m_OnGUIBtnWidth;
        private float m_OnGUIBtnHeight;

        private void Awake()
        {
            m_GUISkin = Resources.Load<GUISkin>("CustomGUISkin");

        }

        public virtual void OnGUI()
        {
            if (m_GUISkin != null)
            {
                GUI.skin = Resources.Load<GUISkin>("CustomGUISkin");
            }
            if (m_GUIBtnInfos != null)
            {
                ParseOnGUIBtns();
            }
        }

        public void AddOnGUIBtns(float width, float height, params GUIBtnInfo[] gUIBtnInfos)
        {
            m_OnGUIBtnWidth = width;
            m_OnGUIBtnHeight = height;
            this.m_GUIBtnInfos = gUIBtnInfos;
        }

        private void ParseOnGUIBtns()
        {
            float xPos = 25;
            float yPos = 25;
            float spacing = 25;
            for (int i = 0; i < m_GUIBtnInfos.Length; i++)
            {
                if (GUI.Button(new Rect(xPos, yPos, m_OnGUIBtnWidth, m_OnGUIBtnHeight), m_GUIBtnInfos[i].name))
                {
                    Debugger.Log("Click OnGUIBtn , btn：" + m_GUIBtnInfos[i].name, LogTag.MF);
                    m_GUIBtnInfos[i].action?.Invoke();
                }
                yPos += m_OnGUIBtnHeight + spacing;
            }
        }

        public struct GUIBtnInfo
        {
            public string name;
            public Action action;
        }
    }
}
