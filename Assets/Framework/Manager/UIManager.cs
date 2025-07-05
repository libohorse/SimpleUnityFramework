using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class UIManager
{
    // Canvas预制体地址,对应Addressable Groups地址
    private const string CanvasPath = "Canvas";

    #region 弹窗管理

    // UI层级枚举
    private static readonly Dictionary<UILayer, Transform> UILayers = new();

    // UI视图缓存
    private static readonly Dictionary<string, GameObject> UIViews = new();

    // UI弹窗队列
    private static readonly Queue<UIInfo> UIViewQueue = new();

    /// <summary>
    /// 初始化UI管理器
    /// </summary>
    public static async Task Initialize()
    {
        // 确保场景中有Canvas
        var mainCanvas = Object.FindFirstObjectByType<Canvas>();
        if (!mainCanvas)
        {
            var canvasGo = new GameObject("Canvas");
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            mainCanvas = canvasGo.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        try
        {
            // 初始化UI层级
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var go = await AAManager.InstantiateAsync(CanvasPath);
                go.transform.SetParent(mainCanvas.transform);

                var canvas = go.GetComponent<Canvas>();
                canvas.sortingOrder = (int)layer * 100;
                canvas.name = layer.ToString();
                UILayers.Add(layer, canvas.transform);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// 插入弹窗到队列
    /// </summary>
    /// <param name="uiData">弹窗数据</param>
    /// <typeparam name="T">弹窗类型</typeparam>
    public static void AppendView<T>(UIData uiData = null) where T : UIView
    {
        UIViewQueue.Enqueue(new UIInfo { UIType = typeof(T), UIData = uiData });
        if (UIViewQueue.Count == 1)
        {
            OpenView(typeof(T), uiData);
        }
    }

    /// <summary>
    /// 打开弹窗
    /// </summary>
    /// <param name="uiData">弹窗数据</param>
    /// <typeparam name="T">弹窗类型</typeparam>
    public static void OpenView<T>(UIData uiData = null) where T : UIView
    {
        OpenView(typeof(T), uiData);
    }

    /// <summary>
    /// 管理器内部统一的打开弹窗方法
    /// </summary>
    /// <param name="type">弹窗类型</param>
    /// <param name="uiData">弹窗数据</param>
    private static async void OpenView(Type type, UIData uiData = null)
    {
        try
        {
            var uiName = type.Name;
            var metaData = GetMetaData(type);
            // 为了避免很多麻烦,不允许重复打开同一个弹窗
            // 有这种需求的可以使用AppendView方法或者修改设计方案
            if (!UIViews.TryAdd(uiName, null)) throw new Exception($"{uiName} already opened");
            var go = await AAManager.LoadUIAsync(type);
            go.transform.SetParent(UILayers[metaData.Layer], false);
            go.transform.SetAsLastSibling();
            UIViews[uiName] = go;
            var ui = go.GetComponent<UIView>();
            // 预制体上没有绑定UIView的派生类
            if (!ui) throw new Exception($"Not found ${uiName} Component");
            BindData(ui, uiData);
            // BindButton(ui);
            // BindEvent(ui);
            ui.OnRefresh();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// 关闭弹窗
    /// </summary>
    /// <typeparam name="T">弹窗类型</typeparam>
    public static void CloseView<T>() where T : UIView
    {
        CloseView(typeof(T));
    }

    /// <summary>
    /// 关闭弹窗
    /// </summary>
    /// <param name="uiView">弹窗类型</param>
    public static void CloseView(UIView uiView)
    {
        CloseView(uiView.GetType());
    }

    /// <summary>
    /// 管理器内部统一的关闭弹窗方法
    /// </summary>
    /// <param name="type">弹窗类型</param>
    private static void CloseView(Type type)
    {
        var uiName = type.Name;
        if (!UIViews.Remove(uiName, out var view)) return;
        // UnbindEvent(type);
        AAManager.ReleaseUI(view);

        // 弹窗队列判断
        if (!UIViewQueue.TryDequeue(out var uiItem)) return;
        if (uiItem.UIType != type) return;
        if (UIViewQueue.TryPeek(out var newUIItem))
        {
            OpenView(newUIItem.UIType, newUIItem.UIData);
        }
    }

    /// <summary>
    /// 绑定数据到UIView
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="uiData"></param>
    private static void BindData(UIView ui, UIData uiData)
    {
        // 找到Data属性并赋值给它
        if (uiData == null) return;
        var type = ui.GetType();
        var property = type.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance);
        if (property == null) return;
        property.SetValue(ui, uiData);
    }

    /// <summary>
    /// 获取弹窗元数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static UIMetaData GetMetaData(Type type)
    {
        var metaData = type.GetCustomAttribute(typeof(UIMetaData), true) as UIMetaData;
        if (metaData == null) throw new Exception($"Not found UILayer attribute in ${type.Name}");
        return metaData;
    }

    #endregion

    #region Event

    // UI事件回调缓存
    private static readonly Dictionary<string, Action> UIEvents = new();

    // UI事件监听器缓存
    private static readonly Dictionary<UIEvent, Action> UIListeners = new();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="eventId">事件id</param>
    /// <param name="eventName">事件名</param>
    /// <param name="listener">监听器</param>
    public static void AddListener(string eventId, UIEvent eventName, Action listener)
    {
        if (!UIEvents.TryAdd(eventId, listener))
        {
            throw new Exception($"{eventId} message cannot be registered twice");
        }

        if (!UIListeners.ContainsKey(eventName))
        {
            UIListeners[eventName] = delegate { };
        }

        UIListeners[eventName] += listener;
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="eventId">事件id</param>
    /// <param name="eventName">事件名</param>
    public static void RemoveListener(string eventId, UIEvent eventName)
    {
        if (!UIEvents.TryGetValue(eventId, out var listener)) return;
        if (UIListeners.ContainsKey(eventName))
        {
            UIListeners[eventName] -= listener;
        }
    }

    /// <summary>
    /// 触发消息
    /// </summary>
    /// <param name="eventName">事件名</param>
    public static void TriggerEvent(UIEvent eventName)
    {
        if (UIListeners.TryGetValue(eventName, out var action))
        {
            action?.Invoke();
        }
    }

    #endregion


    // UI信息类,用于存储队列弹窗类型和数据
    private class UIInfo
    {
        // 弹窗类型
        public Type UIType;

        // 弹窗数据
        public UIData UIData;
    }
}