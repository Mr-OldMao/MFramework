using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：测试Resource资源、AssetBundle资源的加载卸载 
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class TestResLoader : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Q：同步加载Reoueces资源，" +
                "W：异步加载Reoueces资源，" +
                "E：回收Reoueces资源，" +
                "A：同步加载AB包，" +
                "S：异步加载AB包，" +
                "D：回收AB包资源2，" +
                "Z：同步加载AB包中具体资源，" +
                "X：异步加载AB包中具体资源，" +
                "C：回收AB包中具体资源");
        }
        private void Update()
        {
            #region Resources
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResLoader.LoadSync<AudioClip>(ResType.Resources, "TestRes/Audio/bgm1");
                Debug.Log("同步加载Reoueces资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                ResLoader.LoadAsync<AudioClip>(ResType.Resources, (AudioClip res) =>
                {
                    Debug.Log("异步加载Reoueces资源 B资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
                }, "TestRes/Audio/bgm1");
                Debug.Log("异步加载Reoueces资源 A资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                ResLoader.UnLoadAssets("TestRes/Audio/bgm1");
                Debug.Log("回收Reoueces资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
            }
            #endregion

            #region AssetBundle
            if (Input.GetKeyDown(KeyCode.A))
            {
                string path = ResLoader.Path_AB + "/abcube";
                //同步加载AB包
                AssetBundle ab = ResLoader.LoadSync<AssetBundle>(ResType.AssetBundle, path);
                Instantiate(ab.LoadAsset("cubePrefab"));
                Debug.Log("同步加载AssetBundle资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //异步加载AB包
                string path = ResLoader.Path_AB + "/abcube";
                ResLoader.LoadAsync<AssetBundle>(ResType.AssetBundle, (AssetBundle ab) =>
                {
                    Instantiate(ab.LoadAsset("cubePrefab"));
                    Debug.Log("异步加载AssetBundle资源 B资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
                }, path);
                Debug.Log("异步加载AssetBundle资源 A资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                //卸载AB包
                string path = ResLoader.Path_AB + "/abcube";
                ResLoader.UnLoadAssets(path);
                Debug.Log("回收AssetBundle资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                //同步加载ab包中具体资源
                string abPath = ResLoader.Path_AB + "/abplane";
                GameObject obj = ResLoader.LoadSync<GameObject>(ResType.Asset, abPath, "planePrefab");
                Instantiate(obj);
                Debug.Log("同步加载AssetBundle资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(abPath + "/planePrefab")?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                //异步加载ab包中具体资源
                string abPath = ResLoader.Path_AB + "/abplane";
                ResLoader.LoadAsync<GameObject>(ResType.Asset, (GameObject obj) =>
                {
                    Instantiate(obj);
                    Debug.Log("异步加载AssetBundle资源 B资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(abPath + "/planePrefab")?.RefCount);
                }, abPath, "planePrefab");
                Debug.Log("异步加载AssetBundle资源 A资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(abPath + "/planePrefab")?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                string path = ResLoader.Path_AB + "/abplane/planePrefab";
                ResLoader.UnLoadAssets(path);
                Debug.Log("回收AssetBundle资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResLoader.ShowResLogInfo();
            }
            #endregion
        }
    }
}