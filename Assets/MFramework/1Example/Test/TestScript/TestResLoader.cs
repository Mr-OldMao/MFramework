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

            Debug.Log("Q：同步加载Reoueces资源1，" +
                "W：回收Reoueces资源1，" +
                "E：异步加载Reoueces资源2，" +
                "R：回收Reoueces资源2，" +
                "A：同步加载AssetBundle资源3，" +
                "S：回收AssetBundle资源3，" +
                "D：异步加载AssetBundle资源4，" +
                "F：回收AssetBundle资源4");
        }
        private void Update()
        {
            #region Resources
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResLoader.LoadSync<AudioClip>("TestRes/Audio/bgm1", ResType.Resources);
                Debug.Log("同步加载Reoueces资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                ResLoader.UnLoadAssets("TestRes/Audio/bgm1");
                Debug.Log("回收Reoueces资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/bgm1")?.RefCount);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ResLoader.LoadASync<AudioClip>("TestRes/Audio/effJumpScene", (AudioClip res) =>
                {
                    Debug.Log("异步加载Reoueces资源 B资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/effJumpScene")?.RefCount);
                }, ResType.Resources);
                Debug.Log("异步加载Reoueces资源 A资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/effJumpScene")?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResLoader.UnLoadAssets("TestRes/Audio/effJumpScene");
                Debug.Log("回收Reoueces资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist("TestRes/Audio/effJumpScene")?.RefCount);
            }
            #endregion

            #region AssetBundle
            if (Input.GetKeyDown(KeyCode.A))
            {
                //加载AB包
                string path = Application.dataPath + "/MFramework/1Example/Test/TesAssets/BuildAssetBundle/abaudio";
                AssetBundle ab = ResLoader.LoadSync<AssetBundle>(path, ResType.AssetBundle);
                //加载AB包中资源
                ab.LoadAsset<AudioClip>("bgm1");
                Debug.Log("同步加载AssetBundle资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                string path = Application.dataPath + "/MFramework/1Example/Test/TesAssets/BuildAssetBundle/abaudio";
                ResLoader.UnLoadAssets(path);
                Debug.Log("回收AssetBundle资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                //加载AB包
                string path = Application.dataPath + "/MFramework/1Example/Test/TesAssets/BuildAssetBundle/abobj";
                ResLoader.LoadASync<AssetBundle>(path, (AssetBundle ab) =>
                {
                    //加载AB包中资源
                    ab.LoadAsset<GameObject>("cubePrefab");
                    Debug.Log("异步加载AssetBundle资源 B资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
                }, ResType.AssetBundle);
                Debug.Log("异步加载AssetBundle资源 A资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                string path = Application.dataPath + "/MFramework/1Example/Test/TesAssets/BuildAssetBundle/abobj";
                ResLoader.UnLoadAssets(path);
                Debug.Log("回收AssetBundle资源 资源池当前资源个数 " + ResLoader.resContainer.Count + " 当前资源引用次数:" + ResLoader.CheckResExist(path)?.RefCount);
            }
            #endregion
        }
    }
}