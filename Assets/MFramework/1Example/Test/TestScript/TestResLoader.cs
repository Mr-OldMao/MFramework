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
    public class TestResLoader : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResLoader.LoadSync<AudioClip>("TestRes/Audio/bgm1");
                Debug.Log("同步加载资源 当前资源缓存个数 " + ResLoader.resContainer.Count + " count :" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                ResLoader.UnLoadAssets("TestRes/Audio/bgm1");
                Debug.Log("回收资源 当前资源缓存个数 " + ResLoader.resContainer.Count + " count :" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ResLoader.LoadASync<AudioClip>("TestRes/Audio/effJumpScene", resInfo =>
                {
                    Debug.Log("异步加载资源 B当前资源缓存个数 " + ResLoader.resContainer.Count + " count :" + ResLoader.CheckResExist("TestRes/Audio/effJumpScene")?.RefCount);
                });
                Debug.Log("异步加载资源 A当前资源缓存个数 " + ResLoader.resContainer.Count + " count :" + ResLoader.CheckResExist("TestRes/Audio/effJumpScene")?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ResLoader.UnLoadAssets("TestRes/Audio/effJumpScene");
                Debug.Log("回收资源 当前资源缓存个数 " + ResLoader.resContainer.Count + " count :" + ResLoader.CheckResExist("TestRes/Audio/effJumpScene")?.RefCount);
            }
        }
    }
}