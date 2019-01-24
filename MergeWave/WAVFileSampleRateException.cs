using System;
using System.Runtime.Serialization;

namespace MergeWave
{
    [Serializable]
    internal class WAVFileSampleRateException : Exception
    {
        private string v1;
        private string v2;
        private int pSampleRate;

        public WAVFileSampleRateException()
        {
        }

        public WAVFileSampleRateException(string message) : base(message)
        {
        }

        public WAVFileSampleRateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public WAVFileSampleRateException(string v1, string v2, int pSampleRate)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.pSampleRate = pSampleRate;
        }

        protected WAVFileSampleRateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}