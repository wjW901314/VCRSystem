using System;
using System.IO;

namespace MergeWave
{
    public class WAVFile
    {
        /// <summary>
        /// 枚举该类指定的文件操作模式，读、写、读写
        /// </summary>
        public enum WAVFileMode
        {
            READ,
            WRITE,
            READ_WRITE
        }

        static WAVFile()
        {
            mDataStartPos = 44;
        }

        public WAVFile()
        {
            InitMembers();
        }

        /// <summary>
        /// 析构 确保文件被关闭
        /// </summary>
        ~WAVFile()
        {
            Close();
        }

        /// <summary>
        /// 打开一个WAV文件，并读取文件头和音频信息。
        /// </summary>
        /// <param name="pFilename">要打开的文件名</param>
        /// <param name="pMode">打开模式，读、读写，如果只想写入的话，去看Create().</param>
        /// <returns>返回信息</returns>
        public String Open(String pFilename, WAVFileMode pMode)
        {
            mFilename = pFilename;
            return (Open(pMode));
        }

        /// <summary>
        /// 打开 mFilename 指定的WAV文件，并读取文件头和音频信息，但不读取音频数据。
        /// </summary>
        /// <param name="pMode">The file opening mode.  Only READ and READ_WRITE are valid.  If you want to write only, then use Create().</param>
        /// <param name="pMode">打开模式，读、读写，如果只想写入的话，去看Create().</param>
        /// <returns>返回信息</returns>
        public String Open(WAVFileMode pMode)
        {
            if (mFileStream != null)
            {
                mFileStream.Close();
                mFileStream.Dispose();
                mFileStream = null;
            }

            String filenameBackup = mFilename;
            InitMembers();

            if ((pMode != WAVFileMode.READ) && (pMode != WAVFileMode.READ_WRITE))
                throw new WAVFileException("File mode not supported: " + WAVFileModeStr(pMode), "WAVFile.Open()");

            if (!File.Exists(filenameBackup))
                return ("File does not exist: " + filenameBackup);

            if (!IsWaveFile(filenameBackup))
                return ("File is not a WAV file: " + filenameBackup);

            mFilename = filenameBackup;

            String retval = "";

            try
            {
                mFileStream = File.Open(mFilename, FileMode.Open);
                mFileMode = pMode;

                // RIFF chunk (共12字节)
                // 文件头 (前4字节)
                byte[] buffer = new byte[4];
                mFileStream.Read(buffer, 0, 4);
                buffer.CopyTo(mWAVHeader, 0);
                // 读文件大小 (4字节)
                mFileStream.Read(buffer, 0, 4);
                //mFileSizeBytes = BitConverter.ToInt32(buffer, 0);
                // 读RIFF类型 (4字节)
                mFileStream.Read(buffer, 0, 4);
                buffer.CopyTo(mRIFFType, 0);

                // Format chunk (共24字节)
                mFileStream.Read(buffer, 0, 4);
                mFileStream.Read(buffer, 0, 4);
                mFileStream.Read(buffer, 0, 2);
                mFileStream.Read(buffer, 2, 2);
                mNumChannels = (BitConverter.IsLittleEndian ? buffer[2] : buffer[3]);
                mFileStream.Read(buffer, 0, 4);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                mSampleRateHz = BitConverter.ToInt32(buffer, 0);
                mFileStream.Read(buffer, 0, 4);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                mBytesPerSec = BitConverter.ToInt32(buffer, 0);
                mFileStream.Read(buffer, 2, 2);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer, 2, 2);
                mBytesPerSample = BitConverter.ToInt16(buffer, 2);
                mFileStream.Read(buffer, 2, 2);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer, 2, 2);
                mBitsPerSample = BitConverter.ToInt16(buffer, 2);

                // Data chunk
                mFileStream.Read(buffer, 0, 4);
                mFileStream.Read(buffer, 0, 4);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                mDataSizeBytes = BitConverter.ToInt32(buffer, 0);

                // 前面就是文件头，如果没有，直接从下面开始
                if (mDataSizeBytes != FileSizeBytes - 36)
                    mDataSizeBytes = (int) (FileSizeBytes - 36);

                // 文件的其余部分是音频数据，
                // 可以读连续调用到NextSample（）。

                mNumSamplesRemaining = NumSamples;
            }
            catch (Exception exc)
            {
                retval = exc.Message;
            }

            return (retval);
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close()
        {
            if (mFileStream != null)
            {
                // 写和读写模式下，写入文件头
                if ((mFileMode == WAVFileMode.WRITE) || (mFileMode == WAVFileMode.READ_WRITE))
                {
                    mFileStream.Seek(4, 0);

                    int size = (int) FileSizeBytes - 8;
                    byte[] buffer = BitConverter.GetBytes(size);
                    if (!BitConverter.IsLittleEndian)
                        Array.Reverse(buffer);
                    mFileStream.Write(buffer, 0, 4);
                    mFileStream.Seek(40, 0);
                    //mFileStream.Write(BitConverter.GetBytes(mDataBytesWritten), 0, 4);
                    size = (int) (FileSizeBytes - mDataStartPos);
                    buffer = BitConverter.GetBytes(size);
                    if (!BitConverter.IsLittleEndian)
                        Array.Reverse(buffer);
                    mFileStream.Write(buffer, 0, 4);
                }

                mFileStream.Close();
                mFileStream.Dispose();
                mFileStream = null;
            }

            // 重置
            InitMembers();
        }

        public byte[] GetNextSample_ByteArray()
        {
            byte[] audioSample = null;

            if (mFileStream == null)
                throw new WAVFileReadException("Read attempted with null internal file stream.",
                    "WAVFile.GetNextSample_ByteArray()");

            if ((mFileMode != WAVFileMode.READ) && (mFileMode != WAVFileMode.READ_WRITE))
                throw new WAVFileReadException("Read attempted in incorrect mode: " + WAVFileModeStr(mFileMode),
                    "WAVFile.GetNextSample_ByteArray()");

            try
            {
                int numBytes = mBitsPerSample / 8;
                audioSample = new byte[numBytes];
                mFileStream.Read(audioSample, 0, numBytes);
                --mNumSamplesRemaining;
            }
            catch (Exception exc)
            {
                audioSample = null;
                throw new WAVFileReadException(exc.Message, "WAVFile.GetNextSample_ByteArray()");
            }

            return audioSample;
        }

        public byte GetNextSample_8bit()
        {
            if (mBitsPerSample != 8)
                throw new WAVFileReadException("Attempted to retrieve an 8-bit sample when audio is not 8-bit.",
                    "WAVFile.GetNextSample_8bit()");

            byte sample8 = 0;

            byte[] sample = GetNextSample_ByteArray();
            if (sample != null)
                sample8 = sample[0];

            return sample8;
        }

        public short GetNextSample_16bit()
        {
            if (mBitsPerSample != 16)
                throw new WAVFileReadException("Attempted to retrieve a 16-bit sample when audio is not 16-bit.",
                    "WAVFile.GetNextSample_16bit()");

            short sample16 = 0;

            byte[] sample = GetNextSample_ByteArray();
            if (sample != null)
                sample16 = BitConverter.ToInt16(sample, 0);

            return sample16;
        }

        public short GetNextSampleAs16Bit()
        {
            short sample_16bit = 0;

            if (mBitsPerSample == 8)
                sample_16bit = ScaleByteToShort(GetNextSample_8bit());
            else if (mBitsPerSample == 16)
                sample_16bit = GetNextSample_16bit();

            return sample_16bit;
        }

        public byte GetNextSampleAs8Bit()
        {
            byte sample_8bit = 0;

            if (mBitsPerSample == 8)
                sample_8bit = GetNextSample_8bit();
            else if (mBitsPerSample == 16)
                sample_8bit = ScaleShortToByte(GetNextSample_16bit());

            return sample_8bit;
        }

        public void AddSample_ByteArray(byte[] pSample)
        {
            if (pSample != null)
            {
                if ((mFileMode != WAVFileMode.WRITE) && (mFileMode != WAVFileMode.READ_WRITE))
                    throw new WAVFileWriteException("Write attempted in incorrect mode: " + WAVFileModeStr(mFileMode),
                        "WAVFile.AddSample_ByteArray()");

                if (mFileStream == null)
                    throw new WAVFileWriteException("Write attempted with null internal file stream.",
                        "WAVFile.AddSample_ByteArray()");

                if (pSample.GetLength(0) != (mBitsPerSample / 8))
                    throw new WAVFileWriteException("Attempt to add an audio sample of incorrect size.",
                        "WAVFile.AddSample_ByteArray()");

                try
                {
                    int numBytes = pSample.GetLength(0);
                    mFileStream.Write(pSample, 0, numBytes);
                    //mDataBytesWritten += numBytes;
                }
                catch (Exception exc)
                {
                    throw new WAVFileWriteException(exc.Message, "WAVFile.AddSample_ByteArray()");
                }
            }
        }

        public void AddSample_8bit(byte pSample)
        {
            if (mBitsPerSample != 8)
                throw new WAVFileWriteException("Attempted to add an 8-bit sample when audio file is not 8-bit.",
                    "WAVFile.AddSample_8bit()");

            byte[] sample = new byte[1];
            sample[0] = pSample;
            AddSample_ByteArray(sample);
        }

        public void AddSample_16bit(short pSample)
        {
            if (mBitsPerSample != 16)
                throw new WAVFileWriteException("Attempted to add a 16-bit sample when audio file is not 16-bit.",
                    "WAVFile.AddSample_16bit()");

            byte[] buffer = BitConverter.GetBytes(pSample);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            AddSample_ByteArray(buffer);
        }

        /// <summary>
        /// 生成一个新的wav文件.
        /// </summary>
        /// <param name="pFilename">文件名</param>
        /// <param name="pStereo">是否为立体声，如果为false，则输出单声道文件</param>
        /// <param name="pSampleRate">采样率</param>
        /// <param name="pBitsPerSample">样本的比特率</param>
        /// <param name="pOverwrite">判断文件是否存在，如果存在就抛出System.IO.IOException</param>
        public void Create(String pFilename, bool pStereo, int pSampleRate, short pBitsPerSample, bool pOverwrite)
        {
            // 如果文件已经打开，就关闭
            Close();

            // 采样率如果不支持，抛出异常
            if (!SupportedSampleRate(pSampleRate))
                throw new WAVFileSampleRateException("Unsupported sample rate: " + pSampleRate.ToString(),
                    "WAVFile.Create()", pSampleRate);
            // 比特率如果不支持，抛出异常
            if (!SupportedBitsPerSample(pBitsPerSample))
                throw new WAVFileBitsPerSampleException(
                    "Unsupported number of bits per sample: " + pBitsPerSample.ToString(), "WAVFile.Create()",
                    pBitsPerSample);

            try
            {
                //创建该文件。如果pOverwrite为TRUE，使用FileMode.Create覆盖该文件
                //否则，使用FileMode.CreateNew生成新文件，否则，将抛出一个System.IO.IOException异常。
                if (pOverwrite)
                    mFileStream = File.Open(pFilename, FileMode.Create);
                else
                    mFileStream = File.Open(pFilename, FileMode.CreateNew);

                mFileMode = WAVFileMode.WRITE;

                mNumChannels = pStereo ? (byte) 2 : (byte) 1;
                mSampleRateHz = pSampleRate;
                mBitsPerSample = pBitsPerSample;

                // 写文件头.

                byte[] buffer = StrToByteArray("RIFF");
                mFileStream.Write(buffer, 0, 4);
                if (mWAVHeader == null)
                    mWAVHeader = new char[4];
                "RIFF".CopyTo(0, mWAVHeader, 0, 4);
                Array.Clear(buffer, 0, buffer.GetLength(0));
                mFileStream.Write(buffer, 0, 4);
                buffer = StrToByteArray("WAVE");
                mFileStream.Write(buffer, 0, 4);
                if (mRIFFType == null)
                    mRIFFType = new char[4];
                "WAVE".CopyTo(0, mRIFFType, 0, 4);

                buffer = StrToByteArray("fmt ");
                mFileStream.Write(buffer, 0, 4);
                Array.Clear(buffer, 0, buffer.GetLength(0));
                buffer[0] = 16;
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                mFileStream.Write(buffer, 0, 4);

                Array.Clear(buffer, 0, buffer.GetLength(0));
                buffer[0] = 1;
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer, 0, 2);
                mFileStream.Write(buffer, 0, 2);

                Array.Clear(buffer, 0, buffer.GetLength(0));
                buffer[0] = mNumChannels;
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer, 0, 2);
                mFileStream.Write(buffer, 0, 2);

                buffer = BitConverter.GetBytes(mSampleRateHz);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                mFileStream.Write(buffer, 0, 4);

                short bytesPerSample = 0;
                if (pStereo)
                    bytesPerSample = (short) ((mBitsPerSample / 8) * 2);
                else
                    bytesPerSample = (short) (mBitsPerSample / 8);

                mBytesPerSec = mSampleRateHz * bytesPerSample;
                buffer = BitConverter.GetBytes(mBytesPerSec);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                mFileStream.Write(buffer, 0, 4);

                byte[] buffer_2bytes = BitConverter.GetBytes(bytesPerSample);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer_2bytes);
                mFileStream.Write(buffer_2bytes, 0, 2);

                buffer_2bytes = BitConverter.GetBytes(mBitsPerSample);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(buffer_2bytes);
                mFileStream.Write(buffer_2bytes, 0, 2);

                buffer = StrToByteArray("data");
                mFileStream.Write(buffer, 0, 4);

                Array.Clear(buffer, 0, buffer.GetLength(0));
                mFileStream.Write(buffer, 0, 4);
                mDataSizeBytes = 0;
            }
            catch (Exception exc)
            {
                throw new WAVFileException(exc.Message, "WAVFile.Create()");
            }
        }

        /// <summary>
        /// 生成一个wav文件，上面Create（）的重载
        /// </summary>
        public void Create(String pFilename, bool pStereo, int pSampleRate, short pBitsPerSample)
        {
            Create(pFilename, pStereo, pSampleRate, pBitsPerSample, true);
        }

        /// <summary>
        /// 判断两个文件格式是否相同
        /// </summary>
        /// <param name="pWAVFile">另一个WAV文件</param>
        /// <returns></returns>
        public bool FormatMatches(WAVFile pWAVFile)
        {
            bool retval = false;

            if (pWAVFile != null)
                retval = ((mNumChannels == pWAVFile.mNumChannels) &&
                          (mSampleRateHz == pWAVFile.mSampleRateHz) &&
                          (mBitsPerSample == pWAVFile.mBitsPerSample));

            return retval;
        }

        /// <summary>
        /// 返回一个WAV文件的WAVFormat结构，包含音频WAV文件的格式信息（＃通道，采样率，比特率）。
        /// </summary>
        public static WAVFormat GetAudioFormat(String pFilename)
        {
            WAVFormat format = new WAVFormat(); 

            WAVFile audioFile = new WAVFile();
            if (audioFile.Open(pFilename, WAVFileMode.READ) == "")
            {
                format.BitsPerSample = audioFile.mBitsPerSample;
                format.NumChannels = audioFile.mNumChannels;
                format.SampleRateHz = audioFile.mSampleRateHz;

                audioFile.Close();
            }

            return (format);
        }

        /// <summary>
        /// 判断一个文件是否是WAV文件.
        /// </summary>
        /// <param name="pFilename">文件名</param>
        /// <returns>如果是WAV文件，返回真</returns>
        public static  bool IsWaveFile(String pFilename)
        {
            bool retval = false;

            if (File.Exists(pFilename))
            {
                try
                {
                    FileStream fileStream = File.Open(pFilename, FileMode.Open);

                    char[] fileID = new char[4];
                    char[] RIFFType = new char[4];

                    byte[] buffer = new byte[4];

                    fileStream.Read(buffer, 0, 4);
                    buffer.CopyTo(fileID, 0);

                    fileStream.Read(buffer, 0, 4);

                    fileStream.Read(buffer, 0, 4);
                    buffer.CopyTo(RIFFType, 0);

                    fileStream.Close();

                    String fileIDStr = new String(fileID);
                    String RIFFTypeStr = new String(RIFFType);

                    retval = ((fileIDStr == "RIFF") && (RIFFTypeStr == "WAVE"));
                }
                catch (Exception exc)
                {
                    throw new WAVFileException(exc.Message, "WAVFile.IsWaveFile()");
                }
            }

            return retval;
        }

        /// <summary>
        /// 返回的WAV音频文件最高的采样值
        /// </summary>
        public static byte[] HighestSampleValue(String pFilename, out short pBitsPerSample)
        {
            pBitsPerSample = 0;
            byte[] highestSampleValue = null;

            WAVFile audioFile = new WAVFile();
            try
            {
                if (audioFile.Open(pFilename, WAVFileMode.READ) == "")
                {
                    pBitsPerSample = audioFile.mBitsPerSample;

                    if (audioFile.mBitsPerSample == 8)
                    {
                        byte sample = 0;
                        byte highestSample = 0;
                        for (int i = 0; i < audioFile.NumSamples; ++i)
                        {
                            sample = audioFile.GetNextSample_8bit();
                            if (sample > highestSample)
                                highestSample = sample;
                        }

                        highestSampleValue = new byte[1];
                        highestSampleValue[0] = highestSample;
                    }
                    else if (audioFile.mBitsPerSample == 16)
                    {
                        short sample = 0;
                        short highestSample = 0;
                        for (int i = 0; i < audioFile.NumSamples; ++i)
                        {
                            sample = audioFile.GetNextSample_16bit();
                            if (sample > highestSample)
                                highestSample = sample;
                        }

                        highestSampleValue = BitConverter.GetBytes(highestSample);
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(highestSampleValue);
                    }

                    audioFile.Close();
                }
            }
            catch (Exception)
            {
            }

            return (highestSampleValue);
        }

        public static byte HighestSampleValue_8bit(String pFilename)
        {
            byte highestSampleVal = 0;

            short bitsPerSample;
            byte[] highestSample = HighestSampleValue(pFilename, out bitsPerSample);
            if (highestSample != null)
            {
                if (bitsPerSample == 8)
                    highestSampleVal = highestSample[0];
                else
                    throw new WAVFileReadException(
                        "Attempt to get largest 8-bit sample from audio file that is not 8-bit.",
                        "WAVFile.HighestSampleValue_8bit()");
            }

            return highestSampleVal;
        }

        public static byte HighestSampleValue_8bit(String[] pFilenames)
        {
            byte sampleValue = 0;
            byte highestSampleValue = 0;

            foreach (String filename in pFilenames)
            {
                sampleValue = HighestSampleValue_8bit(filename);
                if (sampleValue > highestSampleValue)
                    highestSampleValue = sampleValue;
            }

            return highestSampleValue;
        }

        public static short HighestSampleValueAs16Bit(String pFilename)
        {
            short highestSample = 0;

            try
            {
                WAVFile audioFile = new WAVFile();
                if (audioFile.Open(pFilename, WAVFileMode.READ) == "")
                {
                    if (audioFile.BitsPerSample == 8)
                    {
                        short sample = 0;
                        for (int i = 0; i < audioFile.NumSamples; ++i)
                        {
                            sample = ScaleByteToShort(audioFile.GetNextSample_8bit());
                            if (sample > highestSample)
                                highestSample = sample;
                        }
                    }
                    else if (audioFile.BitsPerSample == 16)
                    {
                        short sample = 0;
                        for (int i = 0; i < audioFile.NumSamples; ++i)
                        {
                            sample = audioFile.GetNextSample_16bit();
                            if (sample > highestSample)
                                highestSample = sample;
                        }
                    }

                    audioFile.Close();
                }
            }
            catch (Exception)
            {
            }

            return highestSample;
        }

        public static short HighestSampleValueAs16Bit(String[] pFilenames)
        {
            short highestSampleOverall = 0;

            short highestSample = 0;
            foreach (String filename in pFilenames)
            {
                highestSample = HighestSampleValueAs16Bit(filename);
                if (highestSample > highestSampleOverall)
                    highestSampleOverall = highestSample;
            }

            return highestSampleOverall;
        }

        public static short HighestBitsPerSample(String[] pFilenames)
        {
            short bitsPerSample = 0;

            if (pFilenames != null)
            {
                WAVFile audioFile = new WAVFile();
                String retval = "";
                foreach (String filename in pFilenames)
                {
                    try
                    {
                        retval = audioFile.Open(filename, WAVFileMode.READ);
                        if (retval == "")
                        {
                            if (audioFile.BitsPerSample > bitsPerSample)
                                bitsPerSample = audioFile.BitsPerSample;
                            audioFile.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return bitsPerSample;
        }

        public static byte HighestNumChannels(String[] pFilenames)
        {
            byte numChannels = 0;

            if (pFilenames != null)
            {
                WAVFile audioFile = new WAVFile();
                String retval = "";
                foreach (String filename in pFilenames)
                {
                    try
                    {
                        retval = audioFile.Open(filename, WAVFileMode.READ);
                        if (retval == "")
                        {
                            if (audioFile.NumChannels > numChannels)
                                numChannels = audioFile.NumChannels;
                            audioFile.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return numChannels;
        }

        /// <summary>
        /// 音量调整
        /// </summary>
        public static void AdjustVolume_Copy(String pSrcFilename, String pDestFilename, double pMultiplier)
        {
            // 空文件返回错误.
            if (pSrcFilename == "")
                throw new WAVFileReadException("Blank filename specified.", "WAVFile.AdjustVolume_Copy()");
            if (pDestFilename == "")
                throw new WAVFileWriteException("Blank filename specified.", "WAVFile.AdjustVolume_Copy()");

            // 打开一个源文件
            WAVFile srcFile = new WAVFile();
            String retval = srcFile.Open(pSrcFilename, WAVFileMode.READ);
            if (retval == "")
            {
                // 检查以确保输入文件的比特/采样和采样率合适。如果不合适，则抛出一个异常.
                if (!SupportedBitsPerSample(srcFile.BitsPerSample))
                {
                    WAVFileBitsPerSampleException exc = new WAVFileBitsPerSampleException(pSrcFilename +
                                                                                          " has unsupported bits/sample ("
                                                                                          + srcFile.BitsPerSample
                                                                                              .ToString() + ")",
                        "WAVFile.AdjustVolume_Copy()", srcFile.BitsPerSample);
                    srcFile.Close();
                    throw exc;
                }

                // 打开并开始调整音量
                WAVFile destFile = new WAVFile();
                destFile.Create(pDestFilename, srcFile.IsStereo, srcFile.SampleRateHz, srcFile.BitsPerSample);
                if (srcFile.BitsPerSample == 8)
                {
                    byte sample = 0;
                    for (int i = 0; i < srcFile.NumSamples; ++i)
                    {
                        sample = srcFile.GetNextSample_8bit();
                        if (pMultiplier != 1.0)
                            sample = (byte) ((double) sample * pMultiplier);
                        destFile.AddSample_8bit(sample);
                    }
                }
                else if (srcFile.BitsPerSample == 16)
                {
                    short sample = 0;
                    for (int i = 0; i < srcFile.NumSamples; ++i)
                    {
                        sample = srcFile.GetNextSample_16bit();
                        if (pMultiplier != 1.0)
                            sample = (short) ((double) sample * pMultiplier);
                        destFile.AddSample_16bit(sample);
                    }
                }

                srcFile.Close();
                destFile.Close();
            }
            else
                throw new WAVFileReadException(retval, "WAVFile.AdjustVolume_Copy()");
        }

        public static void AdjustVolumeInPlace(String pFilename, double pMultiplier)
        {
            if (pMultiplier == 1.0)
                return;

            WAVFile audioFile = new WAVFile();
            String retval = audioFile.Open(pFilename, WAVFileMode.READ_WRITE);
            if (retval == "")
            {
                if (!SupportedBitsPerSample(audioFile.BitsPerSample))
                {
                    short bitsPerSample = audioFile.BitsPerSample;
                    audioFile.Close();
                    throw new WAVFileBitsPerSampleException(pFilename + " has unsupported bits/sample ("
                                                                      + bitsPerSample.ToString() + ")",
                        "WAVFile.AdjustVolumeInPlace()", bitsPerSample);
                }

                if (!SupportedSampleRate(audioFile.SampleRateHz))
                {
                    int sampleRate = audioFile.SampleRateHz;
                    audioFile.Close();
                    throw new WAVFileSampleRateException(pFilename + " has unsupported sample rate ("
                                                                   + sampleRate.ToString() + ")",
                        "WAVFile.AdjustVolumeInPlace()", sampleRate);
                }

                if (audioFile.BitsPerSample == 8)
                {
                    byte sample = 0;
                    for (int sampleNum = 0; sampleNum < audioFile.NumSamples; ++sampleNum)
                    {
                        sample = (byte) ((double) audioFile.GetNextSample_8bit() * pMultiplier);
                        audioFile.SeekToAudioSample(sampleNum);
                        audioFile.AddSample_8bit(sample);
                    }
                }
                else if (audioFile.BitsPerSample == 16)
                {
                    short sample = 0;
                    for (int sampleNum = 0; sampleNum < audioFile.NumSamples; ++sampleNum)
                    {
                        sample = (short) ((double) audioFile.GetNextSample_16bit() * pMultiplier);
                        audioFile.SeekToAudioSample(sampleNum);
                        audioFile.AddSample_16bit(sample);
                    }
                }

                audioFile.Close();
            }
            else
                throw new WAVFileReadException(retval, "WAVFile.AdjustVolumeInPlace()");
        }

        public static void AdjustVolume_Copy_8BitTo16Bit(String pSrcFilename, String pDestFilename, double pMultiplier)
        {
            if (pSrcFilename == "")
                throw new WAVFileReadException("Blank filename specified.", "WAVFile.AdjustVolume_Copy_8BitTo16Bit()");
            if (pDestFilename == "")
                throw new WAVFileWriteException("Blank filename specified.", "WAVFile.AdjustVolume_Copy_8BitTo16Bit()");

            WAVFile srcFile = new WAVFile();
            String retval = srcFile.Open(pSrcFilename, WAVFileMode.READ);
            if (retval == "")
            {
                if (srcFile.BitsPerSample != 8)
                {
                    WAVFileBitsPerSampleException exc = new WAVFileBitsPerSampleException(pSrcFilename +
                                                                                          ": 8 bits per sample required, and the file has " +
                                                                                          srcFile.BitsPerSample
                                                                                              .ToString() +
                                                                                          " bits per sample.",
                        "WAVFile.AdjustVolume_Copy_8BitTo16Bit()",
                        srcFile.BitsPerSample);
                    srcFile.Close();
                    throw exc;
                }

                WAVFile destFile = new WAVFile();
                destFile.Create(pDestFilename, srcFile.IsStereo, srcFile.SampleRateHz, 16, true);

                short sample_16bit = 0;
                while (srcFile.NumSamplesRemaining > 0)
                {
                    sample_16bit = ScaleByteToShort(srcFile.GetNextSample_8bit());

                    if (pMultiplier != 1.0)
                        sample_16bit = (short) ((double) sample_16bit * pMultiplier);

                    destFile.AddSample_16bit(sample_16bit);
                }

                srcFile.Close();
                destFile.Close();
            }
            else
                throw new WAVFileReadException(retval, "WAVFile.AdjustVolume_Copy_8BitTo16Bit()");
        }

        public static void Convert_8BitTo16Bit_Copy(String pSrcFilename, String pDestFilename)
        {
            AdjustVolume_Copy_8BitTo16Bit(pSrcFilename, pDestFilename, 1.0);
        }

        /// <summary>
        /// 转换WAV文件的比特/采样和采样率，通道等到另一个WAV文件. 重载
        /// </summary>
        /// <param name="pSrcFilename">要转换的文件名</param>
        /// <param name="pDestFilename">目标文件</param>
        /// <param name="pBitsPerSample">目标文件的比特/采样和采样率</param>
        /// <param name="pStereo">目标文件是否立体声</param>
        public static void CopyAndConvert(String pSrcFilename, String pDestFilename, short pBitsPerSample, bool pStereo)
        {
            CopyAndConvert(pSrcFilename, pDestFilename, pBitsPerSample, pStereo, 1.0);
        }

        /// <summary>
        /// 转换WAV文件的比特/采样和采样率，通道等到另一个WAV文件.
        /// </summary>
        /// <param name="pSrcFilename">要转换的文件名</param>
        /// <param name="pDestFilename">目标文件</param>
        /// <param name="pBitsPerSample">目标文件的比特/采样和采样率</param>
        /// <param name="pStereo">目标文件是否立体声</param>
        /// <param name="pVolumeMultiplier">目标是否要调整音量</param>
        public static void CopyAndConvert(String pSrcFilename, String pDestFilename, short pBitsPerSample, bool pStereo,
            double pVolumeMultiplier)
        {
            WAVFile srcFile = new WAVFile();
            String retval = srcFile.Open(pSrcFilename, WAVFileMode.READ);
            if (retval != "")
                throw new WAVFileException(retval, "WAVFile.Convert_Copy()");

            WAVFile destFile = new WAVFile();
            destFile.Create(pDestFilename, pStereo, srcFile.SampleRateHz, pBitsPerSample);
            if ((srcFile.BitsPerSample == 8) && (pBitsPerSample == 8))
            {
                byte sample = 0;
                if (srcFile.IsStereo && !pStereo)
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = (byte) ((short) ((short) srcFile.GetNextSample_8bit() +
                                                  (short) srcFile.GetNextSample_8bit()) / 2);
                        if (pVolumeMultiplier != 1.0)
                            sample = (byte) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_8bit(sample);
                    }
                }
                else if ((srcFile.IsStereo && pStereo) || (!srcFile.IsStereo && !pStereo))
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = srcFile.GetNextSample_8bit();
                        if (pVolumeMultiplier != 1.0)
                            sample = (byte) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_8bit(sample);
                    }
                }
                else if (!srcFile.IsStereo && pStereo)
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = srcFile.GetNextSample_8bit();
                        if (pVolumeMultiplier != 1.0)
                            sample = (byte) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_8bit(sample);
                        destFile.AddSample_8bit(sample);
                    }
                }
            }
            else if ((srcFile.BitsPerSample == 8) && (pBitsPerSample == 16))
            {
                short sample = 0;
                if (srcFile.IsStereo && !pStereo)
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = (short) ((int) ((int) srcFile.GetNextSampleAs16Bit() +
                                                 (int) srcFile.GetNextSampleAs16Bit()) / 2);
                        if (pVolumeMultiplier != 1.0)
                            sample = (short) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_16bit(sample);
                    }
                }
                else if ((srcFile.IsStereo && pStereo) || (!srcFile.IsStereo && !pStereo))
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = srcFile.GetNextSampleAs16Bit();
                        if (pVolumeMultiplier != 1.0)
                            sample = (short) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_16bit(sample);
                    }
                }
                else if (!srcFile.IsStereo && pStereo)
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = srcFile.GetNextSampleAs16Bit();
                        if (pVolumeMultiplier != 1.0)
                            sample = (short) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_16bit(sample);
                        destFile.AddSample_16bit(sample);
                    }
                }
            }
            else if ((srcFile.BitsPerSample == 16) && (pBitsPerSample == 8))
            {
                byte sample = 0;
                if (srcFile.IsStereo && !pStereo)
                {
                    short sample_16bit = 0;
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample_16bit = (short) ((int) srcFile.GetNextSample_16bit() +
                                                (int) srcFile.GetNextSample_16bit() / 2);
                        if (pVolumeMultiplier != 1.0)
                            sample_16bit = (short) ((double) sample_16bit * pVolumeMultiplier);
                        sample = ScaleShortToByte(sample_16bit);
                        destFile.AddSample_8bit(sample);
                    }
                }
                else if ((srcFile.IsStereo && pStereo) || (!srcFile.IsStereo && !pStereo))
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = ScaleShortToByte(srcFile.GetNextSample_16bit());
                        if (pVolumeMultiplier != 1.0)
                            sample = (byte) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_8bit(sample);
                    }
                }
                else if (!srcFile.IsStereo && pStereo)
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = ScaleShortToByte(srcFile.GetNextSample_16bit());
                        if (pVolumeMultiplier != 1.0)
                            sample = (byte) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_8bit(sample);
                        destFile.AddSample_8bit(sample);
                    }
                }
            }
            else if ((srcFile.BitsPerSample == 16) && (pBitsPerSample == 16))
            {
                short sample = 0;
                if (srcFile.IsStereo && !pStereo)
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = (short) ((int) ((int) srcFile.GetNextSample_16bit() +
                                                 (int) srcFile.GetNextSample_16bit()) / 2);
                        if (pVolumeMultiplier != 1.0)
                            sample = (short) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_16bit(sample);
                    }
                }
                else if ((srcFile.IsStereo && pStereo) || (!srcFile.IsStereo && !pStereo))
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = srcFile.GetNextSample_16bit();
                        if (pVolumeMultiplier != 1.0)
                            sample = (short) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_16bit(sample);
                    }
                }
                else if (!srcFile.IsStereo && pStereo)
                {
                    while (srcFile.NumSamplesRemaining > 0)
                    {
                        sample = srcFile.GetNextSample_16bit();
                        if (pVolumeMultiplier != 1.0)
                            sample = (short) ((double) sample * pVolumeMultiplier);
                        destFile.AddSample_16bit(sample);
                        destFile.AddSample_16bit(sample);
                    }
                }
            }

            destFile.Close();
            srcFile.Close();
        }

        /// <summary>
        /// 采样率是否支持
        /// </summary>
        public static bool SupportedSampleRate(int pSampleRateHz)
        {
            return ((pSampleRateHz == 8000) || (pSampleRateHz == 11025) ||
                    (pSampleRateHz == 16000) || (pSampleRateHz == 18900) ||
                    (pSampleRateHz == 22050) || (pSampleRateHz == 32000) ||
                    (pSampleRateHz == 37800) || (pSampleRateHz == 44056) ||
                    (pSampleRateHz == 44100) || (pSampleRateHz == 48000));
        }

        /// <summary>
        /// 比特率是否支持.
        /// </summary>
        public static bool SupportedBitsPerSample(short pBitsPerSample)
        {
            return ((pBitsPerSample == 8) || (pBitsPerSample == 16));
        }

        /// <summary>
        /// 合并音乐文件
        /// </summary>
        /// <param name="pFileList">要合并的文件的数组</param>
        /// <param name="pOutputFilename">输出文件名</param>
        /// <param name="pTempDir">用来存储文件的临时目录，如果没有则创建，用完后删除</param>
        public static void MergeAudioFiles(String[] pFileList, String pOutputFilename, String pTempDir)
        {
            if (pFileList == null)
                return;
            if (pFileList.GetLength(0) == 0)
                return;

            // 确保所有的音频文件采样率相同。如果采样率不匹配，则抛出一个异常。
            if (!SampleRatesEqual(pFileList))
                throw new WAVFileAudioMergeException("The sample rates of the audio files differ.",
                    "WAVFile.MergeAudioFiles()");

            // 1、检查是否格式相同
            WAVFormat firstFileAudioFormat = GetAudioFormat(pFileList[0]);
            if (!SupportedBitsPerSample(firstFileAudioFormat.BitsPerSample))
                throw new WAVFileBitsPerSampleException(
                    "Unsupported number of bits per sample: " + firstFileAudioFormat.BitsPerSample.ToString(),
                    "WAVFile.MergeAudioFiles()", firstFileAudioFormat.BitsPerSample);
            if (!SupportedSampleRate(firstFileAudioFormat.SampleRateHz))
                throw new WAVFileSampleRateException(
                    "Unsupported sample rate: " + firstFileAudioFormat.SampleRateHz.ToString(),
                    "WAVFile.MergeAudioFiles()", firstFileAudioFormat.SampleRateHz);

            // 2、创建临时目录
            bool tempDirExisted = Directory.Exists(pTempDir);
            if (!tempDirExisted)
            {
                try
                {
                    Directory.CreateDirectory(pTempDir);
                    if (!Directory.Exists(pTempDir))
                        throw new WAVFileAudioMergeException(
                            "Unable to create temporary work directory (" + pTempDir + ")",
                            "WAVFile.MergeAudioFiles()");
                }
                catch (System.Exception exc)
                {
                    throw new WAVFileAudioMergeException("Unable to create temporary work directory (" + pTempDir +
                                                         "): "
                                                         + exc.Message, "WAVFile.MergeAudioFiles()");
                }
            }

            // 3. 找到所有文件的采样值，并计算基数
            int numTracks = pFileList.GetLength(0);
            double multiplier = 0.0;
            short highestSampleValue = 0;
            short highestBitsPerSample = HighestBitsPerSample(pFileList);
            bool outputStereo = (HighestNumChannels(pFileList) > 1);
            if (highestBitsPerSample == 8)
            {
                //获取所有文件的采样值，8位
                byte highestSample = HighestSampleValue_8bit(pFileList);
                highestSampleValue = (short) highestSample;

                byte difference = (byte) (highestSample - (byte.MaxValue / (byte) numTracks));
                multiplier = 1.0 - ((double) difference / (double) highestSample);
            }
            else if (highestBitsPerSample == 16)
            {
                //获取所有文件的采样值，16位
                highestSampleValue = HighestSampleValueAs16Bit(pFileList);

                short difference = (short) (highestSampleValue - (short.MaxValue / (short) numTracks));
                multiplier = 1.0 - ((double) difference / (double) highestSampleValue);
            }

            if (double.IsInfinity(multiplier) || (multiplier == 0.0))
            {
                // 如果临时目录存在，删除
                if (!tempDirExisted)
                    DeleteDir(pTempDir);
                throw new WAVFileAudioMergeException("Could not calculate first volume multiplier.",
                    "WAVFile.MergeAudioFiles()");
            }

            if (multiplier < 0.0)
                multiplier = -multiplier;

            // 4. 缩减源文件的音频水平，并输出保存在临时目录中。
            WAVFile[] scaledAudioFiles = new WAVFile[pFileList.GetLength(0)];
            String filename = "";
            WAVFile inputFile = new WAVFile();
            WAVFile outputFile = new WAVFile();
            for (int i = 0; i < pFileList.GetLength(0); ++i)
            {
                // pFileList[i] 包含文件的全路径名.
                filename = pTempDir + "\\" + Path.GetFileName(pFileList[i]);

                // 复制文件到临时目录，调整比特率、采样及通道。
                CopyAndConvert(pFileList[i], filename, highestBitsPerSample, outputStereo, multiplier);

                // 创建在scaledAudioFiles序列中的WAVFile的对象，并打开。
                scaledAudioFiles[i] = new WAVFile();
                scaledAudioFiles[i].Open(filename, WAVFileMode.READ);
            }

            // 5、 现在生成音频混音的文件输出
            outputFile.Create(pOutputFilename, outputStereo, firstFileAudioFormat.SampleRateHz,
                highestBitsPerSample);

            // 6. 合并
            if (highestBitsPerSample == 8)
            {
                byte sample = 0;
                while (SamplesRemain(scaledAudioFiles))
                {
                    sample = 0;
                    for (int i = 0; i < scaledAudioFiles.GetLength(0); ++i)
                    {
                        if (scaledAudioFiles[i].NumSamplesRemaining > 0)
                            sample += scaledAudioFiles[i].GetNextSample_8bit();
                    }

                    sample /= (byte) (scaledAudioFiles.GetLength(0));
                    outputFile.AddSample_8bit(sample);
                }
            }
            else if (highestBitsPerSample == 16)
            {
                short sample = 0;
                while (SamplesRemain(scaledAudioFiles))
                {
                    sample = 0;
                    for (int i = 0; i < scaledAudioFiles.GetLength(0); ++i)
                    {
                        if (scaledAudioFiles[i].NumSamplesRemaining > 0)
                            sample += scaledAudioFiles[i].GetNextSampleAs16Bit();
                    }

                    sample /= (short) (scaledAudioFiles.GetLength(0));
                    outputFile.AddSample_16bit(sample);
                }
            }

            outputFile.Close();

            // 7. 删除输入文件，释放磁盘空间
            foreach (WAVFile audioFile in scaledAudioFiles)
            {
                filename = audioFile.Filename;
                audioFile.Close();
                File.Delete(filename);
            }

            // 8. 调整输出文件的音量
            if (highestBitsPerSample == 8)
            {
                byte highestSampleVal = WAVFile.HighestSampleValue_8bit(pOutputFilename);
                byte maxValue = byte.MaxValue / 4 * 3;
                multiplier = (double) maxValue / (double) highestSampleVal;
            }
            else if (highestBitsPerSample == 16)
            {
                short finalMixFileHighestSample = WAVFile.HighestSampleValueAs16Bit(pOutputFilename);
                multiplier = (double) short.MaxValue / (double) finalMixFileHighestSample;
            }

            if (multiplier < 0.0)
                multiplier = -multiplier;

            AdjustVolumeInPlace(pOutputFilename, multiplier);

            // 如果临时目录存在，删除它
            if (!tempDirExisted)
            {
                String retval = DeleteDir(pTempDir);
                if (retval != "")
                    throw new WAVFileAudioMergeException(
                        "Unable to remove temp directory (" + pTempDir + "): " + retval,
                        "WAVFile.MergeAudioFiles()");
            }
        }

        private static bool SupportedBitsPerSample(object bitsPerSample)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 移动指针，返回音频数据的开始。
        /// </summary>
        public void SeekToAudioDataStart()
        {
            SeekToAudioSample(0);
            // 在读和读写模式下，更新mNumSamplesRemaining.
            if ((mFileMode == WAVFileMode.READ) || (mFileMode == WAVFileMode.READ_WRITE))
                mNumSamplesRemaining = NumSamples;
        }

        /// <summary>
        /// 移动文件指针到一个给定的音频样本
        /// </summary>
        /// <param name="pSampleNum">移动文件指针的样本数</param>
        public void SeekToAudioSample(long pSampleNum)
        {
            if (mFileStream != null)
            {
                long bytesPerSample = mBitsPerSample / 8;
                try
                {
                    mFileStream.Seek(mDataStartPos + (bytesPerSample * pSampleNum), 0);
                }
                catch (System.IO.IOException exc)
                {
                    throw new WAVFileIOException(
                        "Unable to to seek to sample " + pSampleNum.ToString() + ": " + exc.Message,
                        "WAVFile.SeekToAudioSample()");
                }
                catch (System.NotSupportedException exc)
                {
                    throw new WAVFileIOException(
                        "Unable to to seek to sample " + pSampleNum.ToString() + ": " + exc.Message,
                        "WAVFile.SeekToAudioSample()");
                }
                catch (Exception exc)
                {
                    throw new WAVFileException(exc.Message, "WAVFile.SeekToAudioSample()");
                }
            }
        }

        public static String WAVFileModeStr(WAVFileMode pMode)
        {
            String retval = "";

            switch (pMode)
            {
                case WAVFileMode.READ:
                    retval = "READ";
                    break;
                case WAVFileMode.WRITE:
                    retval = "WRITE";
                    break;
                case WAVFileMode.READ_WRITE:
                    retval = "READ_WRITE";
                    break;
                default:
                    retval = "Unknown";
                    break;
            }

            return retval;
        }

        public String Filename
        {
            get { return mFilename; }
        }

        public char[] WAVHeader
        {
            get { return mWAVHeader; }
        }

        public String WAVHeaderString
        {
            get { return new String(mWAVHeader); }
        }

        public char[] RIFFType
        {
            get { return mRIFFType; }
        }

        public String RIFFTypeString
        {
            get { return new String(mRIFFType); }
        }

        public byte NumChannels
        {
            get { return mNumChannels; }
        }

        public bool IsStereo
        {
            get { return (mNumChannels == 2); }
        }

        public int SampleRateHz
        {
            get { return mSampleRateHz; }
        }

        public int BytesPerSec
        {
            get { return mBytesPerSec; }
        }

        public short BytesPerSample
        {
            get { return mBytesPerSample; }
        }

        public short BitsPerSample
        {
            get { return mBitsPerSample; }
        }

        public int DataSizeBytes
        {
            get { return mDataSizeBytes; }
        }

        public long FileSizeBytes
        {
            get { return mFileStream.Length; }
        }

        public int NumSamples
        {
            get { return (mDataSizeBytes / (int) (mBitsPerSample / 8)); }
        }

        public int NumSamplesRemaining
        {
            get { return mNumSamplesRemaining; }
        }

        public WAVFileMode FileOpenMode
        {
            get { return mFileMode; }
        }

        public WAVFormat AudioFormat
        {
            get { return (new WAVFormat(mNumChannels, mSampleRateHz, mBitsPerSample)); }
        }

        public long FilePosition
        {
            get { return (mFileStream != null ? mFileStream.Position : 0); }
        }

        private void InitMembers()
        {
            mFilename = null;
            mFileStream = null;
            mWAVHeader = new char[4];
            mDataSizeBytes = 0;
            mBytesPerSample = 0;

            mRIFFType = new char[4];

            mNumChannels = 2;
            mSampleRateHz = 44100;
            mBytesPerSec = 176400;
            mBitsPerSample = 16;

            mFileMode = WAVFileMode.READ;

            mNumSamplesRemaining = 0;
        }

        private static bool SampleRatesEqual(String[] pFileList)
        {
            bool sampleRatesMatch = true;

            if (pFileList != null)
            {
                int numFiles = pFileList.GetLength(0);
                if (numFiles > 1)
                {
                    int firstSampleRate = GetAudioFormat(pFileList[0]).SampleRateHz;
                    for (int i = 1; i < numFiles; ++i)
                    {
                        if (GetAudioFormat(pFileList[i]).SampleRateHz != firstSampleRate)
                        {
                            sampleRatesMatch = false;
                            break;
                        }
                    }
                }
            }
            else
                sampleRatesMatch = false;

            return sampleRatesMatch;
        }

        static private bool SamplesRemain(WAVFile[] WAVFileArray)
        {
            bool samplesRemain = false;

            if (WAVFileArray != null)
            {
                for (int i = 0; i < WAVFileArray.GetLength(0); ++i)
                {
                    if (WAVFileArray[i].NumSamplesRemaining > 0)
                    {
                        samplesRemain = true;
                        break;
                    }
                }
            }

            return (samplesRemain);
        }

        /// <summary>
        /// 移除目录，MergeAudioFiles()中进行调用.
        /// </summary>
        private static String DeleteDir(String pPath)
        {
            String retval = "";

            if (Directory.Exists(pPath))
            {
                try
                {
                    // Delete all the files
                    String[] filenames = Directory.GetFiles(pPath);
                    foreach (String filename in filenames)
                        File.Delete(filename);
                    // Delete the directory
                    Directory.Delete(pPath, true);
                }
                catch (System.Exception exc)
                {
                    retval = exc.Message;
                }
            }

            return retval;
        }

        private static byte[] StrToByteArray(String pStr)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(pStr);
        }

        private static short ScaleByteToShort(byte pByteVal)
        {
            short val_16bit = 0;
            double scaleMultiplier = 0.0;
            if (pByteVal > 0)
            {
                scaleMultiplier = (double) pByteVal / (double) byte.MaxValue;
                val_16bit = (short) ((double) short.MaxValue * scaleMultiplier);
            }
            else if (pByteVal < 0)
            {
                scaleMultiplier = (double) pByteVal / (double) byte.MinValue;
                val_16bit = (short) ((double) short.MinValue * scaleMultiplier);
            }

            return val_16bit;
        }

        private static byte ScaleShortToByte(short pShortVal)
        {
            byte val_8bit = 0;
            double scaleMultiplier = 0.0;
            if (pShortVal > 0)
            {
                scaleMultiplier = (double) pShortVal / (double) short.MaxValue;
                val_8bit = (byte) ((double) byte.MaxValue * scaleMultiplier);
            }
            else if (pShortVal < 0)
            {
                scaleMultiplier = (double) pShortVal / (double) short.MinValue;
                val_8bit = (byte) ((double) byte.MinValue * scaleMultiplier);
            }

            return val_8bit;
        }

        private String mFilename;

        private FileStream mFileStream;

        // File header information
        private char[] mWAVHeader;

        //private int mFileSizeBytes;   
        private char[] mRIFFType;

        // Audio format information
        private byte mNumChannels;
        private int mSampleRateHz;
        private int mBytesPerSec;
        private short mBytesPerSample;
        private short mBitsPerSample;
        private int mDataSizeBytes;
        private WAVFileMode mFileMode;

        private int mNumSamplesRemaining;
        private static short mDataStartPos;
    }
}