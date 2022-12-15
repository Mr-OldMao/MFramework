using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：测试四类资源的同步加载、异步加载、卸载自动回收
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class TestResLoader : MonoBehaviour
    {
        private string pathCube1 = "Assets/AssetsRes/ABRes/Test/Prefab/Cube1.prefab";//不关心大小写，但资源必须要有后缀
        private string pathCube2 = "Assets/AssetsRes/ABRes/Test/Prefab/Cube2.prefab";
        private string pathCube3 = "Assets/AssetsRes/ABRes/Test/Prefab/Cube3.prefab";

        private string pathResouces = "TestRes/Obj/cubePrefab";
        private void Start()
        {
            Debug.Log("Q：同步加载编辑器资源，" +
                "A：异步加载编辑器资源，" +
                "Z：回收编辑器资源，" +
                "W：同步加载AB包，" +
                "S：异步加载AB包，" +
                "X：回收AB包资源，" +
                "E：同步加载AB包中具体资源，" +
                "D：异步加载AB包中具体资源，" +
                "C：回收AB包中具体资源" +
                "R：同步加载Reoueces资源，" +
                "F：异步加载Reoueces资源，" +
                "V：回收Reoueces资源");
        }
        private void Update()
        {
            #region Editor
            if (Input.GetKeyDown(KeyCode.Q))
            {
                LoadResource.LoadSync<GameObject>(pathCube1, ResType.ResEditor);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                LoadResource.LoadAsync<GameObject>(pathCube1, (go) =>
                {
                    Instantiate(go);
                }, ResType.ResEditor);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                ResLoader.UnLoadAssets(pathCube1, ResType.ResEditor);
            }
            #endregion

            #region AssetBundlePack
            if (Input.GetKeyDown(KeyCode.W))
            {
                LoadResource.LoadSync<AssetBundle>(pathCube1, ResType.ResAssetBundlePack);
                string abPath = LoadResource.ParseAssetPath(pathCube1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //异步加载AB包
                LoadResource.LoadAsync<AssetBundle>(pathCube1, (go) =>
                {
                    Instantiate(go);
                    string abPath = LoadResource.ParseAssetPath(pathCube1);
                }, ResType.ResAssetBundlePack);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                //卸载AB包 
                string abPath = LoadResource.ParseAssetPath(pathCube1);
                ResLoader.UnLoadAssets(abPath, ResType.ResAssetBundlePack);
            }
            #endregion

            #region AssetBundleAsset
            if (Input.GetKeyDown(KeyCode.E))
            {
                LoadResource.LoadSync<GameObject>(pathCube3, ResType.ResAssetBundleAsset);
                string abPath = LoadResource.ParseAssetPath(pathCube3);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                //异步加载AB包
                LoadResource.LoadAsync<GameObject>(pathCube3, (go) =>
                {
                    Instantiate(go);
                    string abPath = LoadResource.ParseAssetPath(pathCube3);
                }, ResType.ResAssetBundleAsset);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                string abPath = LoadResource.ParseAssetPath(pathCube3);
                ResLoader.UnLoadAssets(abPath, ResType.ResAssetBundleAsset);
            }
            #endregion

            #region Resources
            if (Input.GetKeyDown(KeyCode.R))
            {
                LoadResource.LoadSync<GameObject>(pathResouces, ResType.ResResources);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                LoadResource.LoadAsync<GameObject>(pathResouces, (go) =>
                {
                    Instantiate(go);
                }, ResType.ResResources);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                ResLoader.UnLoadAssets(pathResouces, ResType.ResResources);
            }
            #endregion

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResLoader.ShowResLogInfo();
                Resources.UnloadUnusedAssets();
            }
        }
    }
}