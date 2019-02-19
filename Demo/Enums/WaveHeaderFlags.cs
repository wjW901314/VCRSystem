namespace Demo.Enums
{
    public enum WaveHeaderFlags
    {
        /// <summary>
        /// 这个缓冲区是循环中的第一个缓冲区。此标志仅用于输出缓冲区。WHDR_BEGINLOOP
        /// </summary>
        BeginLoop = 0x00000004,
        /// <summary>
        /// 由设备驱动程序设置，以指示缓冲区已完成并将其返回到应用程序。 WHDR_DONE
        /// </summary>
        Done = 0x00000001,
        /// <summary>
        /// WHDR_ENDLOOP
        /// </summary>
        EndLoop = 0x00000008,
        /// <summary>
        ///由Windows设置，指示缓冲区排队等待播放。WHDR_INQUEUE
        /// </summary>
        InQueue = 0x00000010,
        /// <summary>
        /// 由Windows设置，指示缓冲区已使用WaveInPrepareHeader或WaveOutPrepareHeader函数准备好。WHDR_PREPARED
        /// </summary>
        Prepared = 0x00000002
    }
}