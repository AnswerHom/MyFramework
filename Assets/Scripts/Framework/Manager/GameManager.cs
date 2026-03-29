using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Framework.Manager
{
    /// <summary>
    /// 游戏管理器仓库，用于管理游戏内所有管理器
    /// </summary>
    public class GameManager
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                    _instance.Init();
                }

                return _instance;
            }
        }

        private readonly List<IManager> _managers = new();
        private readonly List<IUpdateManager> _updateList = new();
        private readonly List<ILateUpdateManager> _lateUpdateList = new();
        private readonly List<IFixedUpdateManager> _fixedUpdateList = new();

        /// <summary>
        /// 管理器驱动
        /// </summary>
        private ManagerDriver _driver;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (_driver == null)
                _driver = new GameObject("ManagerDriver").AddComponent<ManagerDriver>();
        }

        public void Add(IManager manager)
        {
            _managers.Add(manager);
            if (manager is IUpdateManager) _updateList.Add(manager as IUpdateManager);
            if (manager is IFixedUpdateManager) _fixedUpdateList.Add(manager as IFixedUpdateManager);
            if (manager is ILateUpdateManager) _lateUpdateList.Add(manager as ILateUpdateManager);
            manager.Init();
        }

        #region Mono生命周期执行函数

        public void OnUpdate(float deltaTime)
        {
            var list = _updateList;
            for (int i = 0; i < list.Count; i++)
            {
                var mgr = list[i];
                mgr.OnUpdate(deltaTime);
            }
        }

        public void OnLateUpdate()
        {
            var list = _lateUpdateList;
            for (int i = 0; i < list.Count; i++)
            {
                var mgr = list[i];
                mgr.OnLateUpdate();
            }
        }

        public void OnFixedUpdate()
        {
            var list = _fixedUpdateList;
            for (int i = 0; i < list.Count; i++)
            {
                var mgr = list[i];
                mgr.OnFixedUpdate();
            }
        }

        #endregion
    }
}