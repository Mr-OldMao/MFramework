using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：UI窗体基类
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.12.29、2023.09.04
    /// </summary>
    public abstract class UIFormBase : MonoBehaviour, IUIFormBase
    {
        /// <summary>
        /// 当前窗体是否正在显示
        /// </summary>
        public bool IsShow { get; private set; }

        public UIFormState CurUIFormState { get; set; } = UIFormState.Unknow;

        /// <summary>
        /// UI窗体状态
        /// </summary>
        public enum UIFormState
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknow = 0,
            /// <summary>
            /// 已关闭
            /// </summary>
            Closed,
            /// <summary>
            /// 正在关闭
            /// </summary>
            Closing,
            /// <summary>
            /// 已打开
            /// </summary>
            Opened,
            /// <summary>
            /// 正在打开
            /// </summary>
            Opening,
        }

        /// <summary>
        /// 获取UI窗体层级
        /// </summary>
        public abstract UILayerType GetUIFormLayer { get; protected set; }

        protected string UIFormRootDir = UIFormConfig.UIFormRootDir;

        /// <summary>
        /// UI窗体预制体实体位置相对路径
        /// </summary>
        public abstract string UIFormEntitySubPath
        {
            get;
            protected set;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
            }
            IsShow = true;
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            IsShow = false;
        }

        /// <summary>
        /// 初始化字段映射
        /// </summary>
        protected abstract void InitMapField();

        /// <summary>
        /// 注册UI事件
        /// </summary>
        protected abstract void RegisterUIEvnet();

        protected virtual void OnEnable()
        {

        }
        protected virtual void Awake()
        {
            InitMapField();
            RegisterUIEvnet();
        }
        protected virtual void Start()
        {

        }
        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void OnDestroy()
        {

        }
        protected virtual void OnDisable()
        {

        }


    }
}
