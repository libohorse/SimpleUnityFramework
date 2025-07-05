using System;

/// <summary>
/// 记录需要监听的消息
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class UIListener : Attribute
{
    public UIEvent Name { get; }

    public UIListener(UIEvent name)
    {
        Name = name;
    }
}