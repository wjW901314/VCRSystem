using System;

namespace Demo.Logic
{
    public interface IWaveIn : IDisposable
    {
        /// <summary>
        /// Recording WaveFormat
        /// </summary>
        WaveFormat WaveFormat { get; set; }

        /// <summary>
        /// 开始录制
        /// </summary>
        void StartRecording();

        /// <summary>
        /// 停止录制
        /// </summary>
        void StopRecording();

        /// <summary>
        /// 表示记录的数据可用
        /// </summary>
        event EventHandler<WaveInEventArgs> DataAvailable;

        /// <summary>
        /// 表示所有记录的数据现在都已接收
        /// </summary>
        event EventHandler<StoppedEventArgs> RecordingStopped;
    }
}