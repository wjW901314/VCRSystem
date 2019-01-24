using System;
using System.Runtime.Serialization;

namespace MergeWave
{
    [Serializable]
    internal class WAVFileIOException : Exception
    {
        private string v1;
        private string v2;

        public WAVFileIOException()
        {
        }

        public WAVFileIOException(string message) : base(message)
        {
        }

        public WAVFileIOException(string v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public WAVFileIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WAVFileIOException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}