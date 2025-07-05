using System;
using Constant;

namespace Framework.Attribute
{
    /// <summary>
    /// 记录需要监听的消息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UIListener : System.Attribute
    {
        public UIEvent Name { get; }

        public UIListener(UIEvent name)
        {
            Name = name;
        }
    }
}