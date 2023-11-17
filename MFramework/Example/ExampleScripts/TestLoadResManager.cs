using MFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 标题：测试资源加载功能基于Addressable
    /// 功能：
    /// 作者：毛俊峰
    /// 创建时间：2023.11.17
    /// </summary>
    public class TestLoadResManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debugger.Log("测试资源加载前，请确定以下几点。1.已导入Addressable插件。2.已导入AB资源到Assets/GameMain/AB/目录下，3.对导入的AB资源勾选Addresable寻址标记，可添加指定Lable");
        }

        private void Load1()
        {
            //异步加载资源,对指定文件夹下的所有资源文件进行自动加载识别、缓存(需要对资源文件进行Addressable标记)
            LoadResManager.GetInstance.LoadResAsyncByDirectory("/GameMain/AB/", () =>
            {
                Debugger.Log("资源异步加载完成回调");
                TestGetRes();
                LoadResManager.GetInstance.ResStateInfo();

            });
        }
        private void Load2()
        {
            LoadResManager.GetInstance.LoadResAsyncByAssetPath<GameObject>("Assets/GameMain/AB/Prefab/TestPrefab2.prefab", (p) =>
            {
                Debugger.Log("单个资源加载完成");
            });
            LoadResManager.GetInstance.LoadResAsyncByAssetPath<AudioClip>("Assets/GameMain/AB/Audio/TestAudio1.wav", (p) =>
            {
                Debugger.Log("单个资源加载完成");
                AudioSource audioSource = new GameObject("TestAudio").AddComponent<AudioSource>();
                audioSource.clip = p;
                audioSource.Play();
            });
        }
        private void Load3()
        {
            LoadResManager.GetInstance.LoadResAsyncByLable(new List<string> { "Prefab" });
        }

        private void TestGetRes()
        {
            Debugger.Log("测试获取单个资源");
            GameObject clone = LoadResManager.GetInstance.GetRes<GameObject>("TestPrefab1");
            clone.GetComponent<Renderer>().material = LoadResManager.GetInstance.GetRes<Material>("TestMat3");
            Texture texture = LoadResManager.GetInstance.GetRes<Texture>("TestImg1");
            clone.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);

            //获取批量资源
            Debugger.Log("获取批量资源");
            List<GameObject> goList = LoadResManager.GetInstance.GetResByResType<GameObject>();
            foreach (GameObject go in goList)
            {
                Debugger.Log("go：" + go);
            }

            Debugger.Log("获取错误类型资源类型");
            LoadResManager.GetInstance.GetRes<Material>("TestPrefab1");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Load1();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Load2();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Load3();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LoadResManager.GetInstance.ResStateInfo();
            }
        }


    }
}
