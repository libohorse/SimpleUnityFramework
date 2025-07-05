using System;

/// <summary>
/// 记录需要绑定的按钮
/// 编辑器绑定和运行时绑定公用这个标签
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class UIButton : Attribute
{
    public string Name { get; }

    public UIButton(string name)
    {
        Name = name;
    }
}