using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using Framework.Manager;
using Luban;
using UnityEngine;
using YooAsset;

namespace Framework.Config
{
    public class ConfigManager:IManager,IConfigLoader
    {
        public string Name => typeof(ConfigManager).Name;
        
        private static cfg.Tables _tables;
        public static cfg.Tables Tables => _tables;

        //数值offset数据
        private readonly Dictionary<string, ByteBuf> _offset = new(32);
        //数值内容文件
        private readonly Dictionary<string, ByteBuf> _bytes = new(32);
        
        
        /// <summary>
        /// 同步初始化（内部会异步执行）
        /// </summary>
        public void Init()
        {
            _tables = new Tables(this);
        }

        /// <summary>
        /// 通过分组加载
        /// </summary>
        /// <param name="groupName"></param>
        public async UniTask<bool> Load(string groupName)
        {
            var assets = YooAssets.GetAssetInfos(new[] { "data" });
            
            //获得前缀资源
            var prefix = $"{groupName}_";
            AssetInfo targetInfo = null;
            for (int i = 0; i < assets.Length; i++)
            {
                var info = assets[i];
                if (assets[i].Address.StartsWith(prefix))
                {
                    targetInfo = info;
                    break;
                }
            }
            
            //加载资源包
            var handle = YooAssets.LoadAllAssetsSync(targetInfo);
            await handle.ToUniTask();

            if (handle.Status != EOperationStatus.Succeed)
            {
                Log.Dev($"[ConfigManager] 加载数值分组{groupName}失败，原因：{handle.LastError}", LogType.Error);
                return false;
            }
            
            //加载分组内所有资源到字典中
            foreach (var obj in handle.AllAssetObjects)
            {
                var asset = obj as TextAsset;
                string fileName = asset.name;
                if (fileName.EndsWith("_offset"))
                {
                    _offset[fileName] = new ByteBuf(asset.bytes);
                }
                else
                {
                    _bytes[fileName] = new ByteBuf(asset.bytes);
                }
            }

            handle.Release();
            return true;
        }
        
        public ByteBuf LoadOffset(string fileName)
        {
            if (_offset.TryGetValue(fileName, out var buf))
                return buf;
            Log.Dev($"Cannot find config offset for fileName '{fileName}'", LogType.Error);
            return null;
        }

        public ByteBuf LoadByteBuf(string fileName, int offset, int length)
        {
            if (_bytes.TryGetValue(fileName, out var buf) == false)
            {
                Log.Dev($"Cannot find config byte for fileName '{fileName}'", LogType.Error);
                return null;
            }
            buf.ReaderIndex = offset;
            return buf;
        }
        
        public void Dispose()
        {
            
        }
    }
}
