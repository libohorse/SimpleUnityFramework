using UnityEngine;

/// <summary>
/// UI视图基类
/// </summary>
public abstract class UIView : UIObject
{
    /// <summary>
    /// 刷新视图
    /// </summary>
    public abstract void OnRefresh();
}