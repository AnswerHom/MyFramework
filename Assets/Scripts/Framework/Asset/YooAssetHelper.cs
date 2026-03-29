using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace Framework.Asset
{
    public static class YooAssetHelper
    {
        public static void Init()
        {
            // 初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);
#if UNITY_WEBGL
			YooAssets.SetCacheSystemDisableCacheOnWebGL();
#endif
        }
        
        /// <summary>
        /// 初始化资源包
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="playMode"></param>
        /// <param name="isDefault"></param>
        /// <param name="assetHostUri"></param>
        /// <returns></returns>
        public static InitializationOperation InitPackage(string packageName, EPlayMode playMode, bool isDefault, string assetHostUri)
		{
			var package = YooAssets.TryGetPackage(packageName);
			if (package == null)
			{
				package = YooAssets.CreatePackage(packageName);
				if (isDefault)
					YooAssets.SetDefaultPackage(package);
			}

			InitializationOperation initializationOperation = null;

			if (playMode == EPlayMode.EditorSimulateMode)
			{
				// 编辑器模拟模式
				var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
				var packageRoot = buildResult.PackageRootDirectory;
				var fileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);

				var createParameters = new EditorSimulateModeParameters();
				createParameters.EditorFileSystemParameters = fileSystemParams;

				initializationOperation = package.InitializeAsync(createParameters);
			}
			
			return initializationOperation;
		}

        /// <summary>
        /// 获取资源版本
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
		public static RequestPackageVersionOperation RequestPackageVersion(string packageName)
		{
			var package = YooAssets.GetPackage(packageName);
			return package.RequestPackageVersionAsync();
		}
        
        /// <summary>
        /// 更新资源清单
        /// </summary>
        /// <param name="packageVersion"></param>
        /// <returns></returns>
		public static UpdatePackageManifestOperation UpdatePackageManifest(string packageName , string packageVersion)
		{
			var package = YooAssets.GetPackage(packageName);
			var operation = package.UpdatePackageManifestAsync(packageVersion);
			return operation;
		}
    }
}