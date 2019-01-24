using System;
using System.Runtime.Serialization;

namespace MergeWave
{
    [Serializable]
    internal class WAVFileReadException : Exception
    {
        private string v1;
        private string v2;

        public WAVFileReadException()
        {
        }

        public WAVFileReadException(string message) : base(message)
        {
        }

        public WAVFileReadException(string v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public WAVFileReadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WAVFileReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}