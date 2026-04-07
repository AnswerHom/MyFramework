using System;
using System.Collections;
using System.Collections.Generic;
using cfg.Base;
using Cysharp.Threading.Tasks;
using Framework;
using Framework.Asset;
using Framework.Config;
using Framework.Manager;
using Framework.UI;
using UnityEngine;
using YooAsset;
using LogType = Framework.LogType;

public class Boot : MonoBehaviour
{
    public EPlayMode wantAssetMode = EPlayMode.EditorSimulateMode;
    public BootConfig bootConfig;

    private void Start()
    {
        InitAync().Forget();
    }

    // Start is called before the first frame update
    async UniTask InitAync()
    {
        YooAssetHelper.Init();

        var yooAssetTask =
            YooAssetHelper.InitPackage(bootConfig.assetPackageName, wantAssetMode, true, bootConfig.assetCDNUri);
        await yooAssetTask.ToUniTask();

        if (yooAssetTask.Status != EOperationStatus.Succeed)
        {
            Log.Dev($"YooAssetHelper.InitPackage初始化失败：{yooAssetTask.Error}", LogType.Error);
            return;
        }

        var versionTask = YooAssetHelper.RequestPackageVersion(bootConfig.assetPackageName);
        await versionTask.ToUniTask();
        if (versionTask.Status != EOperationStatus.Succeed)
        {
            Log.Dev($"YooAssetHelper.RequestPackageVersion失败：{versionTask.Error}", LogType.Error);
            return;
        }

        var mainfestTask =
            YooAssetHelper.UpdatePackageManifest(bootConfig.assetPackageName, versionTask.PackageVersion);
        await mainfestTask.ToUniTask();
        if (mainfestTask.Status != EOperationStatus.Succeed)
        {
            Log.Dev($"YooAssetHelper.UpdatePackageManifest：{mainfestTask.Error}", LogType.Error);
            return;
        }


        RegisterManager();
        await PreloadConfig();
        
    }

    /// <summary>
    /// 注册管理器
    /// </summary>
    void RegisterManager()
    {
        Game.Instance.Add<ConfigManager>().Init();
        Game.Instance.Add<UIDialogManager>();
    }

    /// <summary>
    /// 预加载数值
    /// </summary>
    /// <returns></returns>
    UniTask PreloadConfig()
    {
        var task = new List<UniTask>();
        var mgr = Game.Instance.Get<ConfigManager>();
        task.Add(mgr.Load("base"));
        task.Add(mgr.Load("battle"));
        
        return UniTask.WhenAll(task);
    }
}