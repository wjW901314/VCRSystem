namespace Demo.Logic
{
    public interface IWaveProvider
    {
        /// <summary>
        /// 获取此Wave提供程序的Wave格式。
        /// </summary>
        /// <value>波形格式</value>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// 用波形数据填充指定的缓冲区。
        /// </summary>
        /// <param name="buffer">用于填充波形数据的缓冲区.</param>
        /// <param name="offset">偏移到缓冲区</param>
        /// <param name="count">要读取的字节数</param>
        /// <returns>写入缓冲区的字节数</returns>
        int Read(byte[] buffer, int offset, int count);
    }
}