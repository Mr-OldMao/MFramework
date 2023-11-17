namespace MFramework
{
    /// <summary>
    /// 标题：资源加载管理类
    /// 功能：基于Addressable插件来管理资源的加载、释放、获取等功能
    /// 作者：毛俊峰
    /// 创建时间：2023.11.16
    /// </summary>
    public class LoadResManager : LoadRes<LoadResManager>
    {
        /// <summary>
        /// 获取当前所有资源状态
        /// </summary>
        public void ResStateInfo()
        {
            Debugger.Log("获取当前所有资源状态 resCount：" + base.dicCacheAssets.Count);
            foreach (var key in base.dicCacheAssets.Keys)
            {
                Debugger.Log("k：" + key + "，v：" + dicCacheAssets[key].res + "，useCount：" + dicCacheAssets[key].ResUseCount);
            }
        }
    }
}
