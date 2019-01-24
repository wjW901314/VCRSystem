using System;
using System.IO;
using System.Resources;
using System.Runtime.Remoting.Messaging;

namespace MergeWave
{
    public class WavMergeUtil
    {
        /// <summary>
        /// 合并Wave文件
        /// </summary>
        /// <param name="inputWaveFiles">需要合并的Wave文件数组</param>
        /// <param name="outputWaveFile">合并后的Wave文件</param>
        public static void MergeWav(string[] inputWaveFiles, string outputWaveFile)
        {
            if (inputWaveFiles.Length < 1)
            {
                return;
            }

            FileStream fsInput = new FileStream(inputWaveFiles[0], FileMode.Open, FileAccess.Read);
            FileStream fsOutput = new FileStream(outputWaveFile, FileMode.Create, FileAccess.Write);
            byte[] buffer = new byte[2048];
            int total = 0;
            int count;
            while ((count = fsInput.Read(buffer, 0, buffer.Length)) > 0)
            {
                fsOutput.Write(buffer, 0, count);
                total += count;
            }

            fsInput.Close();
            for (int i = 1; i < inputWaveFiles.Length; i++)
            {
                string file = inputWaveFiles[i];
                WaveHeaderInfo header = ResolveHeader(file);
                FileStream dataInputStream = header.DataInputStream;
                while ((count = dataInputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fsOutput.Write(buffer, 0, count);
                    total += count;
                }

                dataInputStream.Close();
            }

            fsOutput.Flush();
            fsOutput.Close();
            WaveHeaderInfo outputHeader = ResolveHeader(outputWaveFile);
            outputHeader.DataInputStream.Close();
            FileStream fs = new FileStream(outputWaveFile, FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(4, SeekOrigin.Begin);
            byte[] fileLen = IntToBytes(total + outputHeader.DataOffset - 8);
            fs.Write(fileLen, 0, 4);
            fs.Seek(outputHeader.DataSizeOffset, SeekOrigin.Begin);
            byte[] dataLen = IntToBytes(total);
            fs.Write(dataLen, 0, 4);
            fs.Close();
        }

        /// <summary>
        /// 解析头部，并获得文件指针指向数据开始位置的InputStreram
        /// </summary>
        /// <param name="wavFile">文件全路径名称</param>
        /// <returns></returns>
        private static WaveHeaderInfo ResolveHeader(string wavFile)
        {
            FileStream fs = new FileStream(wavFile, FileMode.Open, FileAccess.Read);
            byte[] byte4 = new byte[4];
            byte[] buffer = new byte[2048];
            int readCount = 0;
            WaveHeaderInfo waveHeader = new WaveHeaderInfo();
            fs.Read(byte4, 0, byte4.Length); //RIFF
            fs.Read(byte4, 0, byte4.Length);
            readCount += 8;
            waveHeader.FileSizeOffset = 4;
            waveHeader.FileSize = BytesToInt(byte4);
            fs.Read(byte4, 0, byte4.Length); //WAVE
            fs.Read(byte4, 0, byte4.Length); //fmt
            fs.Read(byte4, 0, byte4.Length);
            readCount += 12;
            int fmtLen = BytesToInt(byte4);
            fs.Read(buffer, 0, fmtLen);
            readCount += fmtLen;
            fs.Read(byte4, 0, byte4.Length); //data or fact
            readCount += 4;
            if (IsFmt(byte4, 0))
            {
                //包含fmt段
                fs.Read(byte4, 0, byte4.Length);
                int factLen = BytesToInt(byte4);
                fs.Read(buffer, 0, factLen);
                fs.Read(byte4, 0, byte4.Length); //data
                readCount += 8 + factLen;
            }

            fs.Read(byte4, 0, byte4.Length); //data size
            int dataLen = BytesToInt(byte4);
            waveHeader.DataSize = dataLen;
            waveHeader.DataSizeOffset = readCount;
            readCount += 4;
            waveHeader.DataOffset = readCount;
            waveHeader.DataInputStream = fs;
            return waveHeader;
        }
        /// <summary>
        /// 校验wave文件RIFF格式
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">位置</param>
        /// <returns></returns>
        private static bool IsRiff(byte[] bytes, int start)
        {
            if (bytes[start + 0] == 'R' && bytes[start + 1] == 'I' && bytes[start + 2] == 'F' &&
                bytes[start + 3] == 'F')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 校验wave文件FMT段
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">位置</param>
        /// <returns></returns>
        private static bool IsFmt(byte[] bytes, int start)
        {
            if (bytes[start + 0] == 'f' && bytes[start + 1] == 'm' && bytes[start + 2] == 't' &&
                bytes[start + 3] == ' ')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 校验wave文件Data
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">位置</param>
        /// <returns></returns>
        private static bool IsData(byte[] bytes, int start)
        {
            if (bytes[start + 0] == 'd' && bytes[start + 1] == 'a' && bytes[start + 2] == 't' &&
                bytes[start + 3] == 'a')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 整形转字节数组
        /// </summary>
        /// <param name="i">整形</param>
        /// <returns></returns>
        private static byte[] IntToBytes(int i)
        {
            return BitConverter.GetBytes(i);
        }

        /// <summary>
        /// 字节数组转整形
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        private static int BytesToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Wave文件头部信息
        /// </summary>
        public class WaveHeaderInfo
        {
            public int FileSize { get; set; }

            public int FileSizeOffset { get; set; }
            public int DataSize { get; set; }
            public int DataSizeOffset { get; set; }
            public int DataOffset { get; set; }
            public FileStream DataInputStream { get; set; }
        }
    }
}