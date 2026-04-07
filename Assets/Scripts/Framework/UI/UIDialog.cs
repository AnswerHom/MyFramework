using Cysharp.Threading.Tasks;

namespace Framework.UI
{
    public enum State
    {
        None = 0,
        Close,//关闭
        Hide,//隐藏
        LoadingPkg,//正在加载UI包
        UIReady, //在该阶段表明， FairyGUI已经就绪，只等数据相关操作完成
        Visible,//已经打开
    }
    
    public class UIDialog
    {
        public State State { get; private set; }
        
        public UIConfig Params { get; set; }
        
        public string CodeName { get; protected set; }
        
        public UIDialog()
        {
            State = State.LoadingPkg;
        }

        internal void SetConfig(UIConfig openParams)
        {
            Params = openParams;
            CodeName = Params.CodeName;
        }
        
        //在UI准备好之前，可以并发的事情 eg:异步加载表格，异步网络通信等.
        //仅当UI准备好，以及本函数返回值为真，才显示界面，进入OnOpen
        public virtual async UniTask<bool> WaitReady()
        {
            return true;
        }
        
        /// <summary>
        /// UIDialog实例被创建
        /// </summary>
        /// <param name="args">界面参数</param>
        public virtual void OnCreate(object args)
        {
        }
    }
}