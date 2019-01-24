using System;
using NAudio.Wave;

namespace Recorder.Logic
{
    public interface IAudioRecorder
    {
        void BeginMonitoring(int recordingDevice);
        void BeginRecording(string path);
        void Stop();
        double MicrophoneLevel { get; set; }
        RecordingState RecordingState { get; }
        SampleAggregator SampleAggregator { get; }
        event EventHandler Stopped;
        WaveFormat RecordingFormat { get; set; }
        TimeSpan RecordedTime { get; }
    }
}