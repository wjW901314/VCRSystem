using NAudio.Mixer;
using NAudio.Wave;
using System;
using System.Linq;

namespace Recorder.Logic.Impl
{
    public class AudioRecorder : IAudioRecorder
    {
        WaveIn _waveIn;
        readonly SampleAggregator _sampleAggregator;
        UnsignedMixerControl _volumeControl;
        double _desiredVolume = 100;
        RecordingState _recordingState;
        WaveFileWriter _writer;
        WaveFormat _recordingFormat;

        public event EventHandler Stopped = delegate { };

        public AudioRecorder()
        {
            _sampleAggregator = new SampleAggregator();
            RecordingFormat = new WaveFormat(44100, 1);
        }

        public WaveFormat RecordingFormat
        {
            get => _recordingFormat;
            set
            {
                _recordingFormat = value;
                _sampleAggregator.NotificationCount = value.SampleRate / 10;
            }
        }

        public void BeginMonitoring(int recordingDevice)
        {
            if (_recordingState != RecordingState.Stopped)
            {
                throw new InvalidOperationException("处于此状态时无法开始监视: " + _recordingState);
            }
            _waveIn = new WaveIn();
            _waveIn.DeviceNumber = recordingDevice;
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;
            _waveIn.WaveFormat = _recordingFormat;
            _waveIn.StartRecording();
            TryGetVolumeControl();
            _recordingState = RecordingState.Monitoring;
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            _recordingState = RecordingState.Stopped;
            _writer.Dispose();
            Stopped(this, EventArgs.Empty);
        }

        public void BeginRecording(string waveFileName)
        {
            if (_recordingState != RecordingState.Monitoring)
            {
                throw new InvalidOperationException("处于此状态时无法开始录制: " + _recordingState);
            }
            _writer = new WaveFileWriter(waveFileName, _recordingFormat);
            _recordingState = RecordingState.Recording;
        }

        public void Stop()
        {
            if (_recordingState == RecordingState.Recording)
            {
                _recordingState = RecordingState.RequestedStop;
                _waveIn.StopRecording();
            }
        }

        private void TryGetVolumeControl()
        {
            int waveInDeviceNumber = _waveIn.DeviceNumber;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                var mixerLine = _waveIn.GetMixerLine();
                foreach (var control in mixerLine.Controls)
                {
                    if (control.ControlType == MixerControlType.Volume)
                    {
                        this._volumeControl = control as UnsignedMixerControl;
                        MicrophoneLevel = _desiredVolume;
                        break;
                    }
                }
            }
            else
            {
                var mixer = new Mixer(waveInDeviceNumber);
                foreach (var destination in mixer.Destinations
                    .Where(d => d.ComponentType == MixerLineComponentType.DestinationWaveIn))
                {
                    foreach (var source in destination.Sources
                        .Where(source => source.ComponentType == MixerLineComponentType.SourceMicrophone))
                    {
                        foreach (var control in source.Controls
                            .Where(control => control.ControlType == MixerControlType.Volume))
                        {
                            _volumeControl = control as UnsignedMixerControl;
                            MicrophoneLevel = _desiredVolume;
                            break;
                        }
                    }
                }
            }

        }

        public double MicrophoneLevel
        {
            get => _desiredVolume;
            set
            {
                _desiredVolume = value;
                if (_volumeControl != null)
                {
                    _volumeControl.Percent = value;
                }
            }
        }

        public SampleAggregator SampleAggregator => _sampleAggregator;

        public RecordingState RecordingState => _recordingState;

        public TimeSpan RecordedTime => _writer == null ? TimeSpan.Zero : TimeSpan.FromSeconds((double)_writer.Length / _writer.WaveFormat.AverageBytesPerSecond);

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            WriteToFile(buffer, bytesRecorded);

            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                float sample32 = sample / 32768f;
                _sampleAggregator.Add(sample32);
            }
        }

        private void WriteToFile(byte[] buffer, int bytesRecorded)
        {
            long maxFileLength = this._recordingFormat.AverageBytesPerSecond * 60;

            if (_recordingState != RecordingState.Recording && _recordingState != RecordingState.RequestedStop) return;
            var toWrite = (int)Math.Min(maxFileLength - _writer.Length, bytesRecorded);
            if (toWrite > 0)
            {
                _writer.Write(buffer, 0, bytesRecorded);
            }
            else
            {
                Stop();
            }
        }
    }
}