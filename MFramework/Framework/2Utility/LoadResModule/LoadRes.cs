using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.AddressableAssets.Addressables;
using UnityEngine.AddressableAssets;
using System.IO;
namespace MFramework
{
    /// <summary>
    /// 标题：基于Addressable进行ab包的异步加载
    /// 功能：资源批量加载，单个加载，
    /// 描述：
    /// 加载时机：
    /// 1.饿汉式加载，程序启动时资源预加载
    /// 2.懒汉式加载，使用资源时加载
    /// 加载方式：
    /// 1.批量加载根据标签Addressables Lable
    /// 2.单个加载根据资源路径(Assets/GamMain/Ab/Prefab/...)
    /// 3.批量加载根据编辑器目录
    /// 作者：毛俊峰
    /// 时间：2023.11.16
    /// </summary>
    public class LoadRes<T> : SingletonByMono<T>, ILoadRes where T : Component
    {
        /// <summary>
        /// 缓存实体资源  k-资源名称 v-资源实体信息
        /// </summary>
        public Dictionary<string, ResInfo> dicCacheAssets;

        private void Awake()
        {
            dicCacheAssets = new Dictionary<string, ResInfo>();
        }

        /// <summary>
        /// 异步加载批量资源,对指定文件夹下的所有资源文件进行自动识别、缓存。可获取资源加载的进度
        /// 注意：需要对资源文件进行Addressable标记。
        /// 使用场景：一般在场景加载中进行，大规模批量加载缓存美术资源。属于恶汉式加载
        /// </summary>
        /// <param name="dirPath">Assets下的路径 格式 /xxx/xxx/xx</param>
        /// <param name="callbackLoadedComplete">所有资源全部加载完毕后回调</param>
        /// <param name="callbackLoadedProgress">所有资源未全部加载完毕时，实时获取资源加载的进度</param>
        public void LoadResAsyncByDirectory(string dirPath = "/GameMain/AB/", Action callbackLoadedComplete = null, Action<float> callbackLoadedProgress = null)
        {
            dirPath = Application.dataPath + dirPath;
            List<string> assetsPathArr = new List<string>();
            GetAllAssetsPath(dirPath, ref assetsPathArr);

            int curLoadedCount = 0;
            UnityTool.GetInstance.DelayCoroutineWaitReturnTrue(() => { return curLoadedCount == assetsPathArr.Count; }, () =>
            {
                Debugger.Log("LoadAssetsAsyncByDirectory callbackLoadedComplete");
                callbackLoadedComplete?.Invoke();
            });
            UnityTool.GetInstance.DelayCoroutineWaitReturnTrue(() => { return curLoadedCount <= assetsPathArr.Count; }, () =>
            {
                float progress = ((float)curLoadedCount / assetsPathArr.Count) * 100;
                Debugger.Log("Loading... Progress：" + (progress.ToString("#0.0")));
                callbackLoadedProgress?.Invoke(progress);
            });

            for (int i = 0; i < assetsPathArr.Count; i++)
            {
                //解析文件路径
                string assetPath = assetsPathArr[i].Split(@"\Assets\")[1].Replace('\\', '/');
                assetPath = "Assets/" + assetPath;
                //自动解析资源类型
                if (assetsPathArr[i].EndsWith(".prefab"))
                {
                    LoadResAsyncByAssetPath<GameObject>(assetPath, (p) => curLoadedCount++, false); ;
                }
                else if (assetsPathArr[i].EndsWith(".mat"))
                {
                    LoadResAsyncByAssetPath<Material>(assetPath, (p) => curLoadedCount++, false);
                }
                else if (assetsPathArr[i].EndsWith(".png") || assetsPathArr[i].EndsWith(".jpg") || assetsPathArr[i].EndsWith(".tga"))
                {
                    LoadResAsyncByAssetPath<Texture>(assetPath, (p) => curLoadedCount++, false);
                }
                else if (assetsPathArr[i].EndsWith(".mp3") || assetsPathArr[i].EndsWith(".wav"))
                {
                    LoadResAsyncByAssetPath<AudioClip>(assetPath, (p) => curLoadedCount++, false);
                }
                //else if (assetsPathArr[i].EndsWith(".TODO"))  // 新增解析方式
                //{

                //}
                else
                {
                    Debugger.LogError("自动解析资源类型失败，请新增解析方式");
                }
            }
        }

        /// <summary>
        /// 异步加载单个资源根据Addressable资源路径。
        /// 注意：需要对资源文件进行Addressable标记。
        /// 使用场景：一般用到某个资源才会调用此方法。属于懒汉式加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resPath">Assets/GamMain/Ab/xxx.xxx</param>
        /// <param name="callbackLoadedComplete"></param>
        /// <param name="autoInstantiate">是否自动实例化资源，做为回调参数传出，Texture类型资源不支持</param>
        public void LoadResAsyncByAssetPath<T>(string resPath, Action<T> callbackLoadedComplete = null, bool autoInstantiate = true) where T : UnityEngine.Object
        {
            LoadAssetAsync<T>(resPath).Completed
             += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<T> obj) =>
             {
                 CacheRes(obj.Result.name, new ResInfo(obj.Result));
                 T targetObject = obj.Result;
                 if (autoInstantiate && typeof(T) != typeof(Texture))
                 {
                     targetObject = Instantiate(obj.Result);
                 }
                 callbackLoadedComplete?.Invoke(targetObject);
             };
        }

        /// <summary>
        /// 异步加载批量资源根据Addressable标签。
        /// 注意：需要对资源文件进行Addressable标记
        /// </summary>
        /// <param name="lables"></param>
        /// <param name="callbackLoadedComplete"></param>
        public void LoadResAsyncByLable(List<string> lables, Action callbackLoadedComplete = null)
        {
            if (lables == null || lables.Count == 0)
            {
                return;
            }
            Debugger.Log("Loading Assets ...");

            bool loadMatComplete = false;
            bool loadPrefabComplete = false;
            bool loadAudioClipComplete = false;
            bool loadTextureClipComplete = false;

            Addressables.LoadAssetsAsync<GameObject>(lables, (p) =>
            {
                CacheRes(p.name, new ResInfo(p));
            }, MergeMode.Union, true).Completed += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<GameObject>> obj) =>
            {
                loadPrefabComplete = true;
                Debugger.Log("Load Prefab Complete");
            };

            Addressables.LoadAssetsAsync<Material>(lables, (p) =>
            {
                CacheRes(p.name, new ResInfo(p));
            }, MergeMode.Union, true).Completed += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<Material>> obj) =>
            {
                loadMatComplete = true;
                Debugger.Log("Load Material Complete");
            };

            Addressables.LoadAssetsAsync<Texture>(lables, (p) =>
            {
                CacheRes(p.name, new ResInfo(p));
            }, MergeMode.Union, true).Completed += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<Texture>> obj) =>
            {
                loadTextureClipComplete = true;
                Debugger.Log("Load Prefab Texture");
            };

            Addressables.LoadAssetsAsync<AudioClip>(lables, (p) =>
            {
                CacheRes(p.name, new ResInfo(p));
            }, MergeMode.Union, true).Completed += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<AudioClip>> obj) =>
            {
                loadAudioClipComplete = true;
                Debugger.Log("Load AudioClip Complete");
            };

            UnityTool.GetInstance.DelayCoroutineWaitReturnTrue(() => { return loadMatComplete && loadPrefabComplete && loadTextureClipComplete && loadAudioClipComplete; }, () =>
            {
                Debugger.Log("Load All Assets Complete");
                callbackLoadedComplete?.Invoke();
            });

        }

        /// <summary>
        /// 缓存资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resInfo"></param>
        public void CacheRes(string key, ResInfo resInfo)
        {
            if (!dicCacheAssets.ContainsKey(key))
            {
                dicCacheAssets.Add(key, resInfo);
            }
            else
            {
                Debugger.LogError("资源加载，缓存资源，key值重复，key：" + key);
            }
        }

        /// <summary>
        /// 获取单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="autoInstantiate">自动实例化资源 Texture类型资源不可用</param>
        /// <returns></returns>
        public T GetRes<T>(string key, bool autoInstantiate = true) where T : UnityEngine.Object
        {
            T res = null;
            if (dicCacheAssets.ContainsKey(key))
            {
                ResInfo resInfo = dicCacheAssets[key];
                if (resInfo != null)
                {
                    if (resInfo.res is T)
                    {
                        res = resInfo.res as T;
                        if (autoInstantiate && typeof(T) != typeof(Texture))
                        {
                            res = Instantiate(res);
                        }
                        resInfo.AddResUserCount();
                    }
                    else
                    {
                        Debugger.LogError($"获取资源失败!，目标资源类型与缓存资源类型不符！key：{key}，targetRes：{typeof(T)}，res：{resInfo.res}");
                    }
                }
                else
                {
                    Debugger.LogError("获取资源失败！资源信息为空，key：" + key);
                }
            }
            else
            {
                Debugger.LogError("获取资源失败！key 不存在，key：" + key);
            }
            return res;
        }

        /// <summary>
        /// 获取多个资源 根据资源类型
        /// </summary>
        /// <returns></returns>
        public List<T> GetResByResType<T>() where T : UnityEngine.Object
        {
            if (typeof(T) == typeof(Material) || typeof(T) == typeof(AudioClip) || typeof(T) == typeof(GameObject) || typeof(T) == typeof(Texture))
            {
                List<T> resList = new List<T>();
                foreach (ResInfo resInfo in dicCacheAssets.Values)
                {
                    if (resInfo.res.GetType() == typeof(T))
                    {
                        resList.Add(resInfo.res as T);
                    }
                }
                return resList;
            }
            else
            {
                Debugger.LogError("暂时不支持当前资源类型的获取  " + typeof(T));
                return null;
            }
        }

        /// <summary>
        /// 获取文件夹下所有资源路径
        /// </summary>
        /// <param name="forderPath">目录路径</param>
        /// <param name="assetsName"></param>
        private void GetAllAssetsPath(string forderPath, ref List<string> assetsName)
        {
            DirectoryInfo direction = new DirectoryInfo(forderPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                int tempI = i;
                if (files[tempI].Name.EndsWith(".meta"))
                {
                    continue;
                }
                if (!assetsName.Contains(files[tempI].FullName))
                {
                    assetsName.Add(files[tempI].FullName);
                }
                else
                {
                    Debugger.LogError("assets exist，assetsName：" + files[tempI].FullName);
                }
            }
            //下一层所有文件夹
            //文件夹下一层的所有文件夹
            DirectoryInfo[] folders = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < folders.Length; i++)
            {
                int tempI = i;
                GetAllAssetsPath(folders[tempI].FullName, ref assetsName);
            }
        }
    }
    /// <summary>
    /// 资源信息
    /// </summary>
    public class ResInfo
    {
        /// <summary>
        /// 资源对象
        /// </summary>
        public UnityEngine.Object res;
        /// <summary>
        /// 资源使用的次数
        /// </summary>
        public uint ResUseCount { get; private set; }
        public void AddResUserCount()
        {
            ResUseCount++;
        }
        public ResInfo(UnityEngine.Object res)
        {
            this.res = res;
        }
    }
}
