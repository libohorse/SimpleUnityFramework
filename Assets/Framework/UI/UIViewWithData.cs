namespace Framework.UI
{
    /// <summary>
    /// UI视图绑定数据的基类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public abstract class UIViewWithData<T> : UIView where T : UIData
    {
        protected T Data { get; set; }
    }
}