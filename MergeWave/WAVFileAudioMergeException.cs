using System;
using System.Runtime.Serialization;

namespace MergeWave
{
    [Serializable]
    internal class WAVFileAudioMergeException : Exception
    {
        private string v1;
        private string v2;

        public WAVFileAudioMergeException()
        {
        }

        public WAVFileAudioMergeException(string message) : base(message)
        {
        }

        public WAVFileAudioMergeException(string v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public WAVFileAudioMergeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WAVFileAudioMergeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}