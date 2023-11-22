using MFramework;
using UnityEngine;

/// <summary>
/// 标题：测试UIManager
/// 功能：UI窗体的显示、隐藏、获取UI窗体实体、获取UI窗体上所挂载的脚本
/// 作者：毛俊峰
/// 日期：2023.04.20、2023.11.21
/// </summary>
public class TestUIManager : TestBase
{
    private void Start()
    {
        Debugger.Log("演示UIMananger案例 1.需要导入Unity资源包后  2.导入Addressable插件 3.标记UIFormTest.prefab的Addresable标签 4.再取消注释后续代码    资源包位置:Assets/MFramework/Example/AssetsUnityPackage/ExampleAssetsUIManager.unitypackage", LogTag.MF);

        Test();
    }

    private void Test()
    {
        //若GameLaunch中调用了绑定方法则无需再次绑定Bind(...)
        //UIFormConfig.GetInstance.Bind(UIFormConfig.BindType.Auto);
        //UIFormConfig.GetInstance.Bind(UIFormConfig.BindType.Json);
        //UIFormConfig.GetInstance.Bind(UIFormConfig.BindType.Manual);

        LoadResManager.GetInstance.LoadResAsyncByAssetPath<GameObject>("Assets/GameMain/UI/UIForms/Main/UIFormTest.prefab", (p) =>
        {
            //显示UI窗体  规则：事先已经在UIFormConfig.RegisterUIForm()中注册了UI预制体所要挂载的同名脚本
            UIManager.GetInstance.Show<UIFormTest>();

            //隐藏UI窗体
            UIManager.GetInstance.Hide<UIFormTest>();

            //显示UI窗体带动画  规则：事先已经在UIFormConfig.RegisterUIForm()中注册了UI预制体所要挂载的同名脚本
            UIManager.GetInstance.Show<UIFormTest>(new UIManager.UIFormAnimConfig { animTime = 2f }, (p) =>
            {
                Debugger.Log("显示UI窗体淡入动画播放完毕" + p);
                UIManager.GetInstance.Hide<UIFormTest>(new UIManager.UIFormAnimConfig { animTime = 2f }, (p) =>
                {
                    Debugger.Log("隐藏UI窗体淡出动画播放完毕" + p);
                    UIManager.GetInstance.Show<UIFormTest>();
                });
            });

            //调用UI窗体所挂载的脚本中的方法
            UIManager.GetInstance.GetUIFormLogicScript<UIFormTest>().Test();

            Debugger.Log("获取UI窗体实体：" + UIManager.GetInstance.GetUIFormEntity<UIFormTest>());
            Debugger.Log("获取UI窗体所在层级：" + UIManager.GetInstance.GetUIFromLayer<UIFormTest>());
            Debugger.Log("获取UI窗体当前状态：" + UIManager.GetInstance.GetUIFormState<UIFormTest>());
            Debugger.Log("获取UI窗体逻辑脚本：" + UIManager.GetInstance.GetUIFormLogicScript<UIFormTest>());
        }, false);

        AddOnGUIBtns(300, 80, new GUIBtnInfo
        {
            name = "显示/隐藏UI窗体",
            action = () =>
            {
                if (UIManager.GetInstance.GetUIFormState<UIFormTest>() == UIFormBase.UIFormState.Closed || UIManager.GetInstance.GetUIFormState<UIFormTest>() == UIFormBase.UIFormState.Unknow)
                {
                    UIManager.GetInstance.Show<UIFormTest>();
                }
                else if (UIManager.GetInstance.GetUIFormState<UIFormTest>() == UIFormBase.UIFormState.Opened || UIManager.GetInstance.GetUIFormState<UIFormTest>() == UIFormBase.UIFormState.Unknow)
                {
                    UIManager.GetInstance.Hide<UIFormTest>();
                }
            }
        }, new GUIBtnInfo
        {
            name = "显示UI窗体",
            action = () =>
            {
                UIManager.GetInstance.Show<UIFormTest>();

            }
        }, new GUIBtnInfo
        {
            name = "关闭UI窗体",
            action = () =>
            {
                UIManager.GetInstance.Hide<UIFormTest>();
            }
        }, new GUIBtnInfo
        {
            name = "显示UI窗体带动画",
            action = () =>
            {
                UIManager.GetInstance.Show<UIFormTest>(new UIManager.UIFormAnimConfig { animTime = 1f });
            }
        }, new GUIBtnInfo
        {
            name = "关闭UI窗体带动画",
            action = () =>
            {
                UIManager.GetInstance.Hide<UIFormTest>(new UIManager.UIFormAnimConfig { animTime = 1f });
            }
        });
    }
}
