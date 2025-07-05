using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Manager
{
    public static class AAManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static Task Initialize()
        {
            // TODO: 热更新 配置加载之类的操作
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="key">address</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public static async Task<T> LoadAssetAsync<T>(string key)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;
            return handle.Result;
        }

        /// <summary>
        /// 回调方式加载资源
        /// </summary>
        /// <param name="key">address</param>
        /// <param name="callback">回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        public static void LoadAsset<T>(string key, Action<T> callback)
        {
            Addressables.LoadAssetAsync<T>(key).Completed += handle => { callback?.Invoke(handle.Result); };
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="asset">需要释放的资源</param>
        public static void ReleaseAsset(object asset)
        {
            Addressables.Release(asset);
        }

        #region UI
    
        /// <summary>
        /// 初始化实例
        /// 避免一些简单的类专门要继承MonoBehavior
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<GameObject> InstantiateAsync(string key)
        {
            var handle = Addressables.InstantiateAsync(key);
            await handle.Task;
            return handle.Result;
        }

        /// <summary>
        /// 因为UI加载用的比较多,封装了一个方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static async Task<GameObject> LoadUIAsync(Type type)
        {
            // 路径规则需要根据实际项目定义
            return await InstantiateAsync($"UIPrefabs/{type.Name}/{type.Name}.prefab");
        }

        /// <summary>
        /// 和LoadUIAsync成对出现
        /// </summary>
        /// <param name="ui"></param>
        public static void ReleaseUI(GameObject ui)
        {
            Addressables.Release(ui);
        }

        #endregion
    }
}