using UnityEngine;

namespace Framework
{
    [CreateAssetMenu(menuName = "Assets/BootConfig")]
    [System.Serializable]
    public class BootConfig : ScriptableObject
    {
        public string assetPackageName = "main";
        public string assetCDNUri = string.Empty;
    }
}