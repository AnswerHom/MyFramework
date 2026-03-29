using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Framework;
using Framework.Asset;
using Framework.Config;
using Framework.Manager;
using UnityEngine;
using YooAsset;

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
            Debug.LogError($"YooAssetHelper.InitPackage初始化失败：{yooAssetTask.Error}");
            return;
        }

        var versionTask = YooAssetHelper.RequestPackageVersion(bootConfig.assetPackageName);
        await versionTask.ToUniTask();
        if (versionTask.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"YooAssetHelper.RequestPackageVersion失败：{versionTask.Error}");
            return;
        }
        
        var mainfestTask = YooAssetHelper.UpdatePackageManifest(bootConfig.assetPackageName,versionTask.PackageVersion);
        await mainfestTask.ToUniTask();
        if (mainfestTask.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"YooAssetHelper.UpdatePackageManifest：{mainfestTask.Error}");
            return;
        }
        
        GameManager.Instance.Add(new ConfigManager());

        // Debug.Log(ConfigManager.Tables.ItemTable.Get(1001).Name);
    }

    // Update is called once per frame
    void Update()
    {
    }
}