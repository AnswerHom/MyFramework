namespace Framework.UI
{
    public struct UIConfig
    {
        /// <summary>
        /// UI代码名字
        /// </summary>
        public string CodeName { get; set; }
        
        /// <summary>
        /// UI类
        /// </summary>
        public System.Type DialogType { get; set; }
        
        /// <summary>
        /// 组件名
        /// </summary>
        public string ComponentName { get; set; }
        
        /// <summary>
        /// 加载的包名
        /// </summary>
        public string[] PackageName { get; set; }
        
        /// <summary>
        /// 遮罩类型
        /// </summary>
        public int MaskType { get; set; }
        
        /// <summary>
        /// 是否允许多开
        /// </summary>
        public bool AllowMultiple { get; set; }
    }
}