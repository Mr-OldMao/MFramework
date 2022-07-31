using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：测试资源的加载卸载 
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class TestReourcesLoader : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("获取资源");
                ReourcesLoader.LoadAssets<AudioClip>("TestRes/Audio/bgm1");
            }


            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("回收资源");
                ReourcesLoader.UnLoadAssets("TestRes/Audio/bgm1");
            }
        }
    }
}