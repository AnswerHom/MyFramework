using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Luban;
using UnityEngine;
using YooAsset;

namespace Framework.Config
{
    public class ConfigManager
    {
        private static cfg.Tables _tables;
        private static bool _isInitialized;
        private static readonly string ConfigPath = "Res/Data/Config/";

        // 缓存已加载的 bytes 数据，避免重复加载
        private static readonly Dictionary<string, byte[]> _cachedBytes = new Dictionary<string, byte[]>();

        public static bool IsInitialized => _isInitialized;

        /// <summary>
        /// 初始化配置系统（异步）
        /// </summary>
        public async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                return;
            }

            // 预加载所有配置数据到缓存
            await PreloadAllConfigsAsync();

            // 注册懒加载回调（从缓存读取，不再有IO操作）
            var offsetLoaders = new Dictionary<string, Func<ByteBuf>>();
            var byteBufLoaders = new Dictionary<string, Func<int, int, ByteBuf>>();

            // 注册 base_itemtable 的加载器
            offsetLoaders["base_itemtable"] = () => LoadOffsetFromCache("base_itemtable");
            byteBufLoaders["base_itemtable"] = (offset, length) => LoadByteBufFromCache("base_itemtable", offset, length);

            // 如果有其他表，在这里继续注册...

            _tables = new cfg.Tables(
                name => offsetLoaders[name](),
                (name, offset, length) => byteBufLoaders[name](offset, length)
            );

            _isInitialized = true;
        }

        /// <summary>
        /// 同步初始化（内部会异步执行）
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            InitializeAsync().Forget();
        }

        /// <summary>
        /// 获取 ItemTable
        /// </summary>
        public cfg.Base.ItemTable GetItemTable()
        {
            CheckInitialized();
            return _tables.ItemTable;
        }

        /// <summary>
        /// 通用获取方法
        /// </summary>
        public T Get<T>(Func<cfg.Tables, T> getter) where T : class
        {
            CheckInitialized();
            return getter(_tables);
        }

        private void CheckInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("ConfigManager is not initialized. Please call InitializeAsync() first.");
            }
        }

        /// <summary>
        /// 预加载所有配置文件到缓存
        /// </summary>
        private async UniTask PreloadAllConfigsAsync()
        {
            var tasks = new List<UniTask>();

            // base_itemtable
            tasks.Add(LoadBytesAsync($"{ConfigPath}base_itemtable"));
            tasks.Add(LoadBytesAsync($"{ConfigPath}base_itemtable_offset"));

            // 如果有其他表，在这里添加...

            await UniTask.WhenAll(tasks);
        }

        private async UniTask LoadBytesAsync(string path)
        {
            if (_cachedBytes.ContainsKey(path))
            {
                return;
            }

            var handle = YooAssets.LoadAssetAsync<TextAsset>(path);
            await handle;
            var textAsset = handle.AssetObject as TextAsset;
            if (textAsset == null)
            {
                throw new Exception($"Failed to load config file: {path}");
            }

            var bytes = textAsset.bytes;
            _cachedBytes[path] = bytes;
            handle.Release();
        }

        private ByteBuf LoadOffsetFromCache(string tableName)
        {
            var path = $"{ConfigPath}{tableName}_offset";
            if (!_cachedBytes.TryGetValue(path, out var bytes))
            {
                throw new Exception($"Offset file not found in cache: {path}");
            }
            return new ByteBuf(bytes);
        }

        private ByteBuf LoadByteBufFromCache(string tableName, int offset, int length)
        {
            var path = $"{ConfigPath}{tableName}";
            if (!_cachedBytes.TryGetValue(path, out var allBytes))
            {
                throw new Exception($"Data file not found in cache: {path}");
            }

            // 从完整数据中切片指定范围
            var slice = new byte[length];
            Array.Copy(allBytes, offset, slice, 0, length);
            return new ByteBuf(slice);
        }
    }
}
