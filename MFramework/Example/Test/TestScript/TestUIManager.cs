using MFramework;
using UnityEngine;
using static MFramework.UIManager;

/// <summary>
/// 标题：测试UIManager
/// 功能：
/// 作者：毛俊峰
/// 时间：2022.
/// 版本：1.0
/// </summary>
public class TestUIManager : MonoBehaviour
{
    private void Start()
    {
        //TEST
        //设置屏幕分辨率
        SetResolution(1920, 1080, 0);

        UIManager.GetInstance.Show<UIFormMain>("Main/UIFormMain.prefab", UILayerType.Common);

        UIManager.GetInstance.GetPanelLogic<UIFormMain>().Test();

        UIManager.GetInstance.GetPanelEntity<UIFormMain>().name = "UIForm123";

        UIManager.GetInstance.Hide<UIFormMain>();

        UIManager.GetInstance.Show<UIFormMain>("Main/UIFormMain.prefab");
    }
}
