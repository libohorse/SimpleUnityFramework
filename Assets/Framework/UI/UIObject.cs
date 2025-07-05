using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI基类
/// </summary>
public abstract class UIObject : MonoBehaviour
{
    private void Awake()
    {
        BindButton(this);
    }

    private void OnEnable()
    {
        BindEvent(this);
    }

    private void OnDisable()
    {
        UnbindEvent(this);
    }

    #region Event
    /// <summary>
    /// 绑定消息
    /// </summary>
    /// <param name="ui">ui对象</param>
    /// <exception cref="Exception"></exception>
    private static void BindEvent(UIObject ui)
    {
        // 筛选带有[UIListener]的函数
        var methods = ui.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .Where(item => Attribute.IsDefined(item, typeof(UIListener)));

        foreach (var method in methods)
        {
            // 主要是为了简化使用,不允许有参数,消息本身已经携带了信息
            if (method.GetParameters().Length != 0)
            {
                Debug.LogWarning($"{ui.GetType().Name}.{method.Name} method parameters more than one");
                continue;
            }

            // 允许一个方法绑定多个事件
            // 因为会频繁遇到刷新页面的需求
            // 例如一堆的货币数值改变消息统一绑定到一个刷新方法上
            var attributes = method.GetCustomAttributes<UIListener>();
            foreach (var attribute in attributes)
            {
                var callback = (Action)Delegate.CreateDelegate(typeof(Action), ui, method);
                // 生成唯一的事件ID,避免重复注册
                var eventId = $"{ui.GetType().Name}_{method.Name}_{attribute.Name}";
                UIManager.AddListener(eventId, attribute.Name, callback);
            }
        }
    }

    /// <summary>
    /// 解绑消息
    /// </summary>
    /// <param name="ui">ui对象</param>
    private static void UnbindEvent(UIObject ui)
    {
        // 筛选带有[UIListener]的函数
        var methods = ui.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .Where(item => Attribute.IsDefined(item, typeof(UIListener)));

        foreach (var method in methods)
        {
            if (method.GetParameters().Length != 0)
            {
                Debug.LogWarning($"{ui.GetType().Name}.{method.Name} method parameters more than one");
                continue;
            }

            var attributes = method.GetCustomAttributes<UIListener>();
            foreach (var attribute in attributes)
            {
                var eventId = $"{ui.GetType().Name}_{method.Name}_{attribute.Name}";
                UIManager.RemoveListener(eventId, attribute.Name);
            }
        }
    }
    
    #endregion
    
    #region Button

    /// <summary>
    /// 运行时绑定按钮事件,解绑事件依赖于unity的垃圾释放,所以不用UnbindButton
    /// </summary>
    /// <param name="ui">ui对象</param>
    private static void BindButton(UIObject ui)
    {
        // 筛选带有[UIButton]的函数
        var methods = ui.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .Where(item => Attribute.IsDefined(item, typeof(UIButton)));

        // 获取所有带Button组件的子物体
        var buttons = ui.gameObject.GetComponentsInChildren<Button>();

        foreach (var method in methods)
        {
            if (method.GetParameters().Length != 0)
            {
                Debug.LogWarning($"{ui.GetType().Name}.{method.Name} method parameters more than one");
                continue;
            }

            var attributes = method.GetCustomAttributes<UIButton>();
            foreach (var attribute in attributes)
            {
                var callback = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), ui, method);
                foreach (var button in buttons)
                {
                    if (button.name == attribute.Name)
                    {
                        button.onClick.AddListener(callback);
                    }
                }
            }
        }
    }

    #endregion
}