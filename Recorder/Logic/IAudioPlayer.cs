using System;

namespace Recorder.Logic
{
    public interface IAudioPlayer
    {
        void LoadFile(string path);
        void Play();
        void Stop();
        TimeSpan CurrentPosition { get; set; }
        TimeSpan StartPosition { get; set; }
        TimeSpan EndPosition { get; set; }
    }
}