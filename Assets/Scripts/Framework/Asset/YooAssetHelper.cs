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
    }
    
    
}