using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MFramework.UIFormBase;

namespace MFramework
{
    /// <summary>
    /// 标题：UI管理器
    /// 功能：
    /// UI窗体的显示与隐藏，内置有淡入淡出动画
    /// 根据脚本类型可获取指定UI窗体的状态、所在层级、UI窗体实体
    /// 作者：毛俊峰
    /// 日期：2022.04.20、203.11.22
    /// 版本：1.0
    /// </summary>
    public class UIManager : SingletonByMono<UIManager>
    {
        /// <summary>
        /// 缓存加载后的面板  key-面板名称（唯一标识） value-UI窗体数据
        /// </summary>
        private Dictionary<string, UIFormInfo> m_DicUIPanelInfoContainer = new Dictionary<string, UIFormInfo>();

        /// <summary>
        /// 全屏透明遮罩
        /// </summary>
        private static Transform m_CvsFullMask;

        /// <summary>
        /// UI窗体信息
        /// </summary>
        class UIFormInfo
        {
            /// <summary>
            /// UI窗体实体
            /// </summary>
            public GameObject uiFormEntity;
            /// <summary>
            /// UI窗体所挂载的逻辑脚本
            /// </summary>
            public UIFormBase uiFormLogicScript;
            public UIFormInfo(GameObject uiForm, UIFormBase uiFormLogicScript)
            {
                this.uiFormEntity = uiForm;
                this.uiFormLogicScript = uiFormLogicScript;
            }
        }

        /// <summary>
        /// UI窗体动画配置
        /// </summary>
        public struct UIFormAnimConfig
        {
            public float animTime;
        }

        private static GameObject m_UIRoot;
        public static GameObject UIRoot
        {
            get
            {
                if (!m_UIRoot)
                {
                    if (GameObject.Find("UIRoot"))
                    {
                        m_UIRoot = GameObject.Find("UIRoot");
                    }
                    else
                    {
                        m_UIRoot = Instantiate(Resources.Load<GameObject>("UI/Canvas/UIRoot"));
                        m_CvsFullMask = m_UIRoot?.transform.Find("CvsFullMask")?.transform;
                    }
                }
                return m_UIRoot;
            }
        }

        #region Show
        /// <summary>
        /// 显示UI窗体,
        /// UI窗体预制体资源事前已加载完毕
        /// </summary>
        /// <typeparam name="T">UIFormBase的派生类</typeparam>
        /// <returns></returns>
        public T Show<T>() where T : UIFormBase
        {
            if (m_DicUIPanelInfoContainer.ContainsKey(typeof(T).Name))
            {
                UIFormState uIFormState = m_DicUIPanelInfoContainer[typeof(T).Name].uiFormLogicScript.CurUIFormState;
                if (uIFormState == UIFormState.Opening || uIFormState == UIFormState.Opened)
                {
                    Debugger.LogError("显示UI窗体失败，当前UI窗体状态：" + uIFormState);
                    return null;
                }
            }

            UIFormConfig.UIEntityConfigInfo uiEntityConfigInfo = UIFormConfig.GetInstance.GetUIFormConfigData<T>();
            if (uiEntityConfigInfo != null)
            {
                return Show<T>(uiEntityConfigInfo.uiFormEntityName, uiEntityConfigInfo.uiLayerType);
            }
            else
            {
                Debugger.LogError("UIForm Show Fail ，uiFormEntity is Null， Type：" + typeof(T).Name);
                return null;
            }
        }

        /// <summary>
        /// 显示UI窗体(带淡入动画),
        /// UI窗体预制体资源事前已加载完毕
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIFormAnimConfig">ui窗体动画配置</param>
        /// <param name="callbackAnimComplete">动画播放完毕回调</param>
        public void Show<T>(UIFormAnimConfig uIFormAnimConfig, Action<T> callbackAnimComplete = null) where T : UIFormBase
        {
            UIFormState uIFormState = GetUIFormState<T>();
            if (uIFormState == UIFormState.Opening || uIFormState == UIFormState.Opened)
            {
                Debugger.LogError("显示UI窗体(带淡入动画)失败，当前UI窗体状态：" + uIFormState);
                return;
            }

            T res = Show<T>();
            UpdateUIFormState<T>(UIFormState.Opening);
            ShowFullMask(true);

            //播放淡入动画
            if (!res.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = res.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
            Coroutine coroutine = null;
            coroutine = UnityTool.GetInstance.DelayCoroutineTimer((curTIme) =>
            {
                canvasGroup.alpha = curTIme / uIFormAnimConfig.animTime;
                if (canvasGroup.alpha > 0.98f)
                {
                    canvasGroup.alpha = 1;
                    UpdateUIFormState<T>(UIFormState.Opened);
                    ShowFullMask(false);
                    StopCoroutine(coroutine);
                    callbackAnimComplete?.Invoke(res);
                }
                return canvasGroup.alpha < 1;
            }, 0.05f);
        }

        /// <summary>
        /// 显示UI窗体,窗体预制体资源事前已加载完毕
        /// </summary>
        /// <typeparam name="T">UIFormBase的派生类</typeparam>
        /// <param name="uiFormName">UI窗体路径："Assets/xxx/xx.prefab"</param>
        /// <returns></returns>
        private T Show<T>(string uiFormName, UILayerType uILayerType) where T : UIFormBase
        {
            if (!UIFormConfig.GetInstance.IsBindComplete)
            {
                Debugger.LogError("UIForm show fail，need init bind UIFormInfo ，UIFormConfig.GetInstance.Bind(...)");
                return default;
            }
            if (string.IsNullOrEmpty(uiFormName))
            {
                Debugger.LogError("UIFormName is null");
                return default;
            }
            if (!uiFormName.EndsWith(".prefab"))
            {
                Debugger.LogError("UIFormName Not Exist .prefab ，Path：" + uiFormName);
                return default;
            }
            T UIFormLogicScript = null;
            if (m_DicUIPanelInfoContainer.ContainsKey(typeof(T).Name))
            {
                UIFormLogicScript = m_DicUIPanelInfoContainer[typeof(T).Name].uiFormLogicScript as T;
            }
            string[] UIFormNameArr = uiFormName.Split('/', '.');
            string name = UIFormNameArr[UIFormNameArr.Length - 2];
            if (UIFormLogicScript == null)
            {
                //获取UI窗体资源实体
                GameObject UIForm = LoadResManager.GetInstance.GetRes<GameObject>(name, true);
                //修改UI窗体名称
                UIForm.name = name;
                //设置UI窗体位置
                RectTransform cloneRect = UIForm.GetComponent<RectTransform>();
                Transform parent = UIRoot.transform.Find(uILayerType.ToString());
                cloneRect.SetParent(parent);
                //cloneRect.offsetMax = Vector3.zero;
                //cloneRect.offsetMin = Vector3.zero;
                //cloneRect.anchoredPosition3D = Vector3.zero;
                //cloneRect.anchorMin = Vector2.zero;
                //cloneRect.anchorMax = Vector2.one;
                //cloneRect.pivot = new Vector2(0.5f, 0.5f);
                //为UI窗体添加指定UIFormBase的派生类
                UIFormLogicScript = UIForm.AddComponent<T>();
                //缓存至资源池
                m_DicUIPanelInfoContainer.Add(name, new UIFormInfo(UIForm, UIFormLogicScript));
            }
            UpdateUIFormState<T>(UIFormState.Opening);
            UIFormLogicScript.Show();
            UpdateUIFormState<T>(UIFormState.Opened);
            return UIFormLogicScript;
        }

        #endregion

        #region Hide
        /// <summary>
        /// 隐藏UI窗体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Hide<T>() where T : UIFormBase
        {
            UIFormState uIFormState = GetUIFormState<T>();
            if (uIFormState == UIFormState.Closing || uIFormState == UIFormState.Closed)
            {
                Debugger.LogError("隐藏UI窗体失败，当前UI窗体状态：" + uIFormState);
                return;
            }

            UpdateUIFormState<T>(UIFormState.Closing);
            GetUIFormLogicScript<T>()?.Hide();
            UpdateUIFormState<T>(UIFormState.Closed);
        }

        /// <summary>
        /// 隐藏UI窗体(带淡出动画),
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIFormAnimConfig"></param>
        /// <param name="callbackAnimComplete"></param>
        public void Hide<T>(UIFormAnimConfig uIFormAnimConfig, Action<T> callbackAnimComplete = null) where T : UIFormBase
        {
            UIFormState uIFormState = GetUIFormState<T>();
            if (uIFormState == UIFormState.Closing || uIFormState == UIFormState.Closed)
            {
                Debugger.LogError("隐藏UI窗体(带淡出动画)失败，当前UI窗体状态：" + uIFormState);
                return;
            }
            T res = GetUIFormLogicScript<T>();
            UpdateUIFormState<T>(UIFormState.Closing);
            ShowFullMask(true);
            //播放淡出动画
            if (!res.gameObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = res.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1f;
            Coroutine coroutine = null;
            coroutine = UnityTool.GetInstance.DelayCoroutineTimer((curTIme) =>
            {
                canvasGroup.alpha = 1 - (curTIme / uIFormAnimConfig.animTime);
                if (canvasGroup.alpha < 0.09f)
                {
                    canvasGroup.alpha = 0;
                    res?.Hide();
                    UpdateUIFormState<T>(UIFormState.Closed);
                    ShowFullMask(false);
                    StopCoroutine(coroutine);
                    callbackAnimComplete?.Invoke(res);
                }
                return canvasGroup.alpha > 0;
            }, 0.05f);
        }
        #endregion

        #region Other
        /// <summary>
        /// 获取UI窗体当前状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UIFormState GetUIFormState<T>() where T : UIFormBase
        {
            UIFormState uIFormState = UIFormState.Unknow;
            T uiformScript = GetUIFormLogicScript<T>();
            if (m_DicUIPanelInfoContainer.ContainsKey(typeof(T).Name))
            {
                uIFormState = uiformScript.CurUIFormState;
            }
            else
            {
                Debugger.LogError("当前UI窗体还未被加载过，无法获取UI窗体状态信息");
            }
            return uIFormState;
        }

        /// <summary>
        /// 获取UI窗体所在层级
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UILayerType GetUIFromLayer<T>() where T : UIFormBase
        {
            T uiFormScript = GetUIFormLogicScript<T>();
            if (uiFormScript != null)
            {
                return GetUIFormLogicScript<T>().GetUIFormLayer;
            }
            else
            {
                Debugger.LogError("GetUIFromLayer Fail!");
                return UILayerType.Common;
            }
        }

        /// <summary>
        /// 获取UI窗体实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public GameObject GetUIFormEntity<T>() where T : UIFormBase
        {
            if (m_DicUIPanelInfoContainer.ContainsKey(typeof(T).Name))
            {
                return m_DicUIPanelInfoContainer[typeof(T).Name].uiFormEntity;
            }
            else
            {
                Debugger.LogError("GetUIFormEntity Fail，name：" + typeof(T).Name);
                return null;
            }
        }

        /// <summary>
        /// 获取UI窗体的逻辑脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUIFormLogicScript<T>() where T : UIFormBase
        {
            if (m_DicUIPanelInfoContainer.ContainsKey(typeof(T).Name))
            {
                return m_DicUIPanelInfoContainer[typeof(T).Name].uiFormLogicScript as T;
            }
            else
            {
                Debugger.LogError("GetPanelLogic Fail，name：" + typeof(T).Name);
                return null;
            }
        }

        /// <summary>
        /// 更新UI窗体状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIFormState"></param>
        private void UpdateUIFormState<T>(UIFormState uIFormState) where T : UIFormBase
        {
            T uiformScript = GetUIFormLogicScript<T>();
            if (uiformScript == null)
            {
                Debugger.LogError("当前UI窗体还未被加载过，无法更新UI窗体状态信息");
            }
            else
            {
                uiformScript.CurUIFormState = uIFormState;
            }
        }

        /// <summary>
        /// 显示全屏透明遮罩
        /// </summary>
        /// <param name="isShow"></param>
        private void ShowFullMask(bool isShow)
        {
            m_CvsFullMask?.SetActive(isShow);
        }
        #endregion
    }
}