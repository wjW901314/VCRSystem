using System;
using System.Runtime.Serialization;

namespace MergeWave
{
    [Serializable]
    internal class WAVFileBitsPerSampleException : Exception
    {
        private string v1;
        private string v2;
        private short pBitsPerSample;

        public WAVFileBitsPerSampleException()
        {
        }

        public WAVFileBitsPerSampleException(string message) : base(message)
        {
        }

        public WAVFileBitsPerSampleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public WAVFileBitsPerSampleException(string v1, string v2, short pBitsPerSample)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.pBitsPerSample = pBitsPerSample;
        }

        protected WAVFileBitsPerSampleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}