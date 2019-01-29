namespace Demo.Enums
{
    public enum WaveCallbackStrategy
    {
        /// <summary>
        /// 使用功能
        /// </summary>
        FunctionCallback,
        /// <summary>
        /// 创建一个新窗口（只应在GUI线程上完成）
        /// </summary>
        NewWindow,
        /// <summary>
        /// 使用现有的窗口句柄
        /// </summary>
        ExistingWindow,
        /// <summary>
        /// 使用事件句柄
        /// </summary>
        Event,
    }
}