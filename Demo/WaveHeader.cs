using System;
using Demo.Enums;

namespace Demo
{
    public class WaveHeader
    {
        /// <summary>
        /// 指向锁定数据缓冲区的指针(lpData)
        /// </summary>
        public IntPtr dataBuffer;
        /// <summary>
        /// 数据缓冲区长度 (dwBufferLength)
        /// </summary>
        public int bufferLength;
        /// <summary>
        /// 仅用于输入 (dwBytesRecorded)
        /// </summary>
        public int bytesRecorded;
        /// <summary>
        /// 供客户使用（dwUser）
        /// </summary>
        public IntPtr userData;
        /// <summary>
        /// assorted flags (dwFlags)
        /// </summary>
        public WaveHeaderFlags flags;
        /// <summary>
        /// 循环控制计数器（dwLoops）
        /// </summary>
        public int loops;
        /// <summary>
        ///PWaveHdr，为驱动程序保留（lpNext）
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// 驱动程序保留
        /// </summary>
        public IntPtr reserved;
    }
}