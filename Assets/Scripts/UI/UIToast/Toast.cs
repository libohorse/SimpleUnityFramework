namespace UI.UIToast
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// 对Toast进行简单封装
    /// </summary>
    public static class Toast
    {
        /// <summary>
        /// 插入Toast
        /// </summary>
        /// <param name="message"></param>
        public static void AppendToast(string message)
        {
            UIToast.Instance?.AppendToast(message);
        }
    }
}