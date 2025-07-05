using System;
using Constant;

namespace Framework.Attribute
{
    /// <summary>
    /// UI元数据类
    /// 这里只是简单的设置了层级信息
    /// 可以大量的使用这个特性注入UI信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UIMetaData : System.Attribute
    {
        public readonly UILayer Layer;

        public UIMetaData(UILayer layer)
        {
            Layer = layer;
        }
    }
}