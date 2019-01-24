namespace MergeWave
{
    public class WAVFormat
    {
        private byte mNumChannels;
        private int mSampleRateHz;
        private short mBitsPerSample;

        public WAVFormat()
        {

        }

        public WAVFormat(byte mNumChannels, int mSampleRateHz, short mBitsPerSample)
        {
            this.mNumChannels = mNumChannels;
            this.mSampleRateHz = mSampleRateHz;
            this.mBitsPerSample = mBitsPerSample;
        }

        public short BitsPerSample { get; internal set; }
        public int SampleRateHz { get; internal set; }
        public byte NumChannels { get; internal set; }
    }
}