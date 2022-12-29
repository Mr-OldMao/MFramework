using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 标题：UI窗体基类
/// 功能：
/// 作者：毛俊峰
/// 时间：2022.12.29
/// </summary>
public class UIFormBase : MonoBehaviour
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
