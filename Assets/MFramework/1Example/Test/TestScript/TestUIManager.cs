using MFramework;
using System.Collections;
using System.Collections.Generic;
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

        //加载面板
        Debug.Log(LoadPanel("GamePanel_Common1", "TestRes/UI/Panel/GamePanel", UILayerType.Common));
        Debug.Log(LoadPanel("GamePanel_Common2", "TestRes/UI/Panel/GamePanel", UILayerType.Common));
        int panelID = LoadPanel("BgPanel_Bg", "TestRes/UI/Panel/BgPanel", UILayerType.Bg);

        //获取面板
        Debug.Log(GetPanelByID(LoadPanel("HidePanel_Top", "TestRes/UI/Panel/HidePanel", UILayerType.Top)));

        //延时卸载面板
        UnityTool.GetInstance.DelayCoroutine(5, () => UnloadPanel(panelID));

    }
}
