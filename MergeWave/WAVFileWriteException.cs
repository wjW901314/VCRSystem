using System;
using System.Runtime.Serialization;

namespace MergeWave
{
    [Serializable]
    internal class WAVFileWriteException : Exception
    {
        private string v1;
        private string v2;

        public WAVFileWriteException()
        {
        }

        public WAVFileWriteException(string message) : base(message)
        {
        }

        public WAVFileWriteException(string v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public WAVFileWriteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WAVFileWriteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}