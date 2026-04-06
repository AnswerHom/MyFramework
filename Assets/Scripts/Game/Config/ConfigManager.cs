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
    /// <summary>
    /// 数值管理器
    /// </summary>
    public class ConfigManager : ConfigLoader, IManager
    {
        public string Name => typeof(ConfigManager).Name;

        private static cfg.Tables _tables;
        public static cfg.Tables Tables => _tables;

        /// <summary>
        /// 已加载的数值分组
        /// </summary>
        private HashSet<string> _loadedGroups = new();

        /// <summary>
        /// 正在加载中的任务
        /// </summary>
        private Dictionary<string, UniTask<bool>> _loadingTasks = new();

        /// <summary>
        /// 同步初始化（内部会异步执行）
        /// </summary>
        public void Init()
        {
            _tables = new Tables(this);
        }

        /// <summary>
        /// 加载数值
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async UniTask<bool> Load(string groupName)
        {
            //是否已经加载
            if (_loadedGroups.Contains(groupName)) return true;
            //是否正在加载任务列表中
            UniTask<bool> task;
            if (_loadingTasks.TryGetValue(groupName, out task))
            {
                return await task;
            }

            //开始加载任务
            task = LoadGroup(groupName);
            _loadingTasks[groupName] = task;

            try
            {
                return await task;
            }
            finally
            {
                _loadingTasks.Remove(groupName);
                _loadedGroups.Add(groupName);
            }
        }


        public void Dispose()
        {
            Clear();
            _loadedGroups.Clear();
            _loadingTasks.Clear();
            _tables = null;
        }
    }
}