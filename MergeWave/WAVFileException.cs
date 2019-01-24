using System;
using System.Runtime.Serialization;

namespace MergeWave
{
    [Serializable]
    internal class WAVFileException : Exception
    {
        private string v1;
        private string v2;

        public WAVFileException()
        {
        }

        public WAVFileException(string message) : base(message)
        {
        }

        public WAVFileException(string v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public WAVFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WAVFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}