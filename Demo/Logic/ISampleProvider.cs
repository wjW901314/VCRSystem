namespace Demo.Logic
{
    public interface ISampleProvider
    {
        /// <summary>
        /// 获取此示例提供程序的Wave格式。
        /// </summary>
        /// <value>The wave format.</value>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// 使用32位浮点样本填充指定的缓冲区
        /// </summary>
        /// <param name="buffer">填充样本的缓冲区</param>
        /// <param name="offset">偏移到缓冲区</param>
        /// <param name="count">要读取的样本数</param>
        /// <returns>写入缓冲区的样本数</returns>
        int Read(float[] buffer, int offset, int count);
    }
}