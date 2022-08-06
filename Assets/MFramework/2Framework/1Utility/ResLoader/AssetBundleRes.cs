using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：AssetBundle资源加载、卸载
    /// 功能：AssetBundle资源同步加载、异步加载、资源卸载具体实现
    /// 作者：毛俊峰
    /// 时间：2022.08.06
    /// 版本：1.0
    /// </summary>
    public class AssetBundleRes : AbRes
    {
        public AssetBundleRes(string assetAllPath) : base(assetAllPath)
        {
            base.AssetAllPath = assetAllPath;
        }

        private AssetBundle m_AssetBundle
        {
            get
            {
                return base.Asset as AssetBundle;
            }
            set
            {
                base.Asset = value;
            }
        }

        public override bool LoadSync()
        {
            //加载AB文件
            m_AssetBundle = AssetBundle.LoadFromFile(AssetAllPath);
            // todo直接加载AB文件中的资源  m_AssetBundle.LoadAsset("资源名")
            return m_AssetBundle;
           
        }

        public override void LoadAsync(Action<AbRes> callback)
        {
            AssetBundleCreateRequest rr = AssetBundle.LoadFromFileAsync(base.AssetAllPath);
            rr.completed += (AsyncOperation ao) =>
            {
                m_AssetBundle = rr.assetBundle;
                callback?.Invoke(this);
            };
        }

        protected override void OnReleaseRes()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;
            }
            ResLoader.resContainer.Remove(this);
            Debug.Log("已回收资源 Asset：" + Asset + "，AssetAllPath：" + AssetAllPath);
        }
    }
}