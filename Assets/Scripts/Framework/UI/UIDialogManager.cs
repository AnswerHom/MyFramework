using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using FairyGUI;
using Framework.Manager;
using UnityEngine;

namespace Framework.UI
{
    public class UIDialogManager : IManager, IUpdateManager
    {
        public string Name => typeof(UIDialogManager).Name;

        private bool _isInit = false;

        /// <summary>
        /// UI类
        /// CodeName -> Class
        /// </summary>
        private readonly Dictionary<string, Type> _dialogTypes = new Dictionary<string, Type>();

        public void Init(Assembly assembly)
        {
            if (_isInit) return;
            _isInit = true;
            //注册UIObjectFactory
            UIPackageItemAttribute.RegisterAll(assembly);
            //注册UI类
            RegisterAllDialog(assembly);
        }

        /// <summary>
        /// 注册UI类
        /// </summary>
        /// <param name="assembly"></param>
        private void RegisterAllDialog(System.Reflection.Assembly assembly)
        {
            var baseType = typeof(UIDialog);

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsClass)
                    continue;

                if (type.IsAbstract || type.IsNotPublic || type.IsNested)
                    continue;

                if (!baseType.IsAssignableFrom(type))
                    continue;

                _dialogTypes[type.Name] = type;
            }
        }


        public async UniTask<UIDialog> Open(UIConfig config, object args)
        {
            //找对应的类
            var type = config.DialogType;
            if (type == null)
            {
                if (!_dialogTypes.TryGetValue(config.CodeName, out type))
                {
                    Log.Dev($"[UIDialogManager] Open Error : {config.CodeName} type not found !");
                    return null;
                }

                config.DialogType = type;
            }

            UIDialog dialog;

            dialog = await CreateDialogAsync(type, config, args);
            
            
            return dialog;
        }

        private async UniTask<UIDialog> CreateDialogAsync(System.Type type, UIConfig config, object args)
        {
            var dialog = Activator.CreateInstance(type) as UIDialog;
            
            if (args != null)
            {
                dialog.OnCreate(args);
            }
            
            dialog.SetConfig(config);

            //并发准备该做的事。eg:加载表格，网络通信等
            var readyTask = dialog.WaitReady();

            await readyTask;
            
            //加载资源包
            

            return dialog;
        }


        public void OnUpdate(float deltaTime)
        {
        }

        public void Dispose()
        {
            _isInit = false;
            _dialogTypes.Clear();
        }
    }
}