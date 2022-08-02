using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：资源加载器管理器
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class ResManager : MonoBehaviour
    {
        private void Start()
        {
          
        }


        private void OnGUI()
        {
           
            if (Input.GetKey(KeyCode.F2))
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("11111111111");
                GUILayout.EndVertical();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("获取资源");
                ResLoader.LoadSync<AudioClip>("Audio/bgm1");
                ResLoader.ShowResLogInfo();
                Debug.Log("测试1 " + ResLoader.CheckResExist("Audio/bgm1"));
                Debug.Log("测试2 " + ResLoader.CheckResExist<GameObject>("Audio/bgm1"));
                Debug.Log("测试3 " + ResLoader.CheckResExist<AudioClip>("Audio/bgm1"));
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("回收资源");
                ResLoader.UnLoadAssets("Audio/bgm1");
                ResLoader.ShowResLogInfo();

                Debug.Log("测试1 " + ResLoader.CheckResExist("Audio/bgm1"));
                Debug.Log("测试2 " + ResLoader.CheckResExist<GameObject>("Audio/bgm1"));
                Debug.Log("测试3 " + ResLoader.CheckResExist<AudioClip>("Audio/bgm1"));

            }
        }
    }
}