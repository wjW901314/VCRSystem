using NAudio.Mixer;
using NAudio.Wave;
using Recorder.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Recorder
{
    public partial class UcAudioRecorder : UserControl
    {
        private UnsignedMixerControl _volumeControl;
        private WaveFileWriter _writer;
        private string _waveFileName;
        private string _outputFilename;
        private readonly string _outputFolder;

        [Description("录音驱动")]
        public WaveIn WaveInDevice { get; set; }

        private double _desiredVolume = 100;
        [Description("麦克风声音大小")]
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

        private float _lastPeak;
        [Description("当前输入声音大小")]
        public float CurrentInputLevel => _lastPeak * 100;

        [Description("集样器")]
        public SampleAggregator SampleAggregator { get; }

        [Description("录音状态")]
        public RecordingState RecordingState { get; set; }

        [Description("录音时间")]
        public TimeSpan RecordedTime => _writer == null ? TimeSpan.Zero : TimeSpan.FromSeconds((double)_writer.Length / _writer.WaveFormat.AverageBytesPerSecond);

        private WaveFormat _recordingFormat;
        [Description("录音数据格式")]
        public WaveFormat RecordingFormat
        {
            get => _recordingFormat;
            set
            {
                _recordingFormat = value;
                SampleAggregator.NotificationCount = value.SampleRate / 10;
            }
        }

        public int LeftPosition
        {
            get => leftPosition;
            set
            {
                if (leftPosition != value)
                {
                    leftPosition = value;
                }
            }
        }

        public int RightPosition
        {
            get => rightPosition;
            set
            {
                if (rightPosition != value)
                {
                    rightPosition = value;
                }
            }
        }

        public int TotalWaveFormSamples
        {
            get => totalWaveFormSamples;
            set
            {
                if (totalWaveFormSamples != value)
                {
                    totalWaveFormSamples = value;
                }
            }
        }
        public SampleAggregator SampleAggregatorPlay
        {
            get => sampleAggregatorPlay;
            set
            {
                if (sampleAggregatorPlay != value)
                {
                    sampleAggregatorPlay = value;
                }
            }
        }

        public UcAudioRecorder()
        {
            SampleAggregator = new SampleAggregator();
            sampleAggregatorPlay = new SampleAggregator();
            InitializeComponent();
            InitData();
            InitWaveFormat((int)cmbSampleRate.SelectedItem, cmbChannels.SelectedIndex + 1);
            BeginMonitoring(cmbWaveInDevice.SelectedIndex - 1);
            _outputFolder = Path.Combine(Path.GetTempPath(), "NAudioDemo");
            Directory.CreateDirectory(_outputFolder);
            SampleAggregator.MaximumCalculated += SampleAggregator_MaximumCalculated;
            trackBar1.Value = (int)MicrophoneLevel;
        }

        private void SampleAggregator_MaximumCalculated(object sender, MaxSampleEventArgs e)
        {
              _lastPeak = Math.Max(e.MaxSample, Math.Abs(e.MinSample));
        }

        public void BindingWaveInDeviceName()
        {
            List<string> recordingDevices = new List<string>();
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                recordingDevices.Add(WaveIn.GetCapabilities(n).ProductName);
            }
            cmbWaveInDevice.DataSource = recordingDevices;
            cmbWaveInDevice.SelectedIndex = 0;
        }
        public void InitData()
        {
            BindingWaveInDeviceName();
            cmbSampleRate.DataSource = new[] { 8000, 16000, 22050, 32000, 44100, 48000 };
            cmbSampleRate.SelectedIndex = 0;
            cmbChannels.DataSource = new[] { "单声道", "立体声" };
            cmbChannels.SelectedIndex = 0;

            cmbWaveInDevice.SelectedIndexChanged += (s, a) => Cleanup();
            cmbSampleRate.SelectedIndexChanged += (s, a) => Cleanup();
            cmbChannels.SelectedIndexChanged += (s, a) => Cleanup();
             //trackBar1.Value = (int)MicrophoneLevel;
            trackBar1.ValueChanged += (s, a) => SetMicrophoneLevel();
        }

        private void SetMicrophoneLevel()
        {
            MicrophoneLevel = trackBar1.Value;
        }
        private void InitWaveFormat(int sampleRate, int channels)
        {
            RecordingFormat = new WaveFormat(sampleRate, channels);
        }

        public void BeginMonitoring(int recordingDevice)
        {
            if (RecordingState != RecordingState.Stopped)
            {
                MessageBox.Show(@"处于此状态时无法开始监视: " + RecordingState);
                return;
            }

            WaveInDevice = new WaveIn();
            WaveInDevice.DeviceNumber = recordingDevice;
            WaveInDevice.DataAvailable += OnDataAvailable;
            WaveInDevice.RecordingStopped += OnRecordingStopped;
            WaveInDevice.WaveFormat = RecordingFormat;
            WaveInDevice.StartRecording();
            TryGetVolumeControl();
            RecordingState = RecordingState.Monitoring;
        }

        public void BeginRecording()
        {
            if (RecordingState == RecordingState.Stopped)
                RecordingState = RecordingState.Monitoring;
            if (RecordingState != RecordingState.Monitoring)
            {
                MessageBox.Show(@"处于此状态时无法开始录制: " + RecordingState);
            }
            _outputFilename = GetFileName();
            _waveFileName = Path.Combine(_outputFolder, _outputFilename);
            _writer = new WaveFileWriter(_waveFileName, RecordingFormat);
            RecordingState = RecordingState.Recording;
        }


        public event EventHandler Stopped = delegate { };

        /// <summary>
        /// 停止录音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            RecordingState = RecordingState.Stopped;
            _writer.Dispose();
            Stopped(this, EventArgs.Empty);

            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<StoppedEventArgs>(OnRecordingStopped), sender, e);
            }
            else
            {
                FinalizeWaveFile();
                progressBar1.Value = 0;
                if (e.Exception != null)
                {
                    MessageBox.Show($"录制过程中遇到问题{e.Exception.Message}");
                }
                int newItemIndex = listBoxRecordings.Items.Add(_outputFilename);
                listBoxRecordings.SelectedIndex = newItemIndex;
                SetControlStates(false);
            }
        }

        private void SetControlStates(bool isRecording)
        {
            groupBox1.Enabled = !isRecording;
            btnStratRecording.Enabled = !isRecording;
            btnStopRecording.Enabled = isRecording;
        }

        /// <summary>
        /// 获取音频文件名称
        /// </summary>
        /// <returns></returns>
        private string GetFileName()
        {
            var deviceName = WaveInDevice.GetType().Name;
            var sampleRate = $"{WaveInDevice.WaveFormat.SampleRate / 1000}kHz";
            var channels = WaveInDevice.WaveFormat.Channels == 1 ? "单声道" : "立体";
            return $"{deviceName} {sampleRate} {channels} {Guid.NewGuid()}.wav";
        }
        /// <summary>
        /// 数据验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            WriteToFile(buffer, bytesRecorded);

            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                float sample32 = sample / 32768f;
                SampleAggregator.Add(sample32);
            }
            progressBar1.Value = (int)CurrentInputLevel;
            labRecordingTime.Text = @"录音时间:" + RecordedTime;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="buffer">字节流</param>
        /// <param name="bytesRecorded"></param>
        private void WriteToFile(byte[] buffer, int bytesRecorded)
        {
            long maxFileLength = RecordingFormat.AverageBytesPerSecond * 60;

            if (RecordingState == RecordingState.Recording
                || RecordingState == RecordingState.RequestedStop)
            {
                var toWrite = (int)Math.Min(maxFileLength - _writer.Length, bytesRecorded);
                if (toWrite > 0)
                {
                    _writer.Write(buffer, 0, bytesRecorded);
                }
                else
                {
                    StopRecording();
                }
            }
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        private void StopRecording()
        {
            if (RecordingState == RecordingState.Recording)
            {
                RecordingState = RecordingState.RequestedStop;
                WaveInDevice.StopRecording();

            }
        }

        private void TryGetVolumeControl()
        {
            int waveInDeviceNumber = WaveInDevice.DeviceNumber;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                var mixerLine = WaveInDevice.GetMixerLine();
                foreach (var control in mixerLine.Controls)
                {
                    if (control.ControlType == MixerControlType.Volume)
                    {
                        _volumeControl = control as UnsignedMixerControl;
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

        private void Cleanup()
        {
            if (WaveInDevice != null)
            {
                WaveInDevice.Dispose();
                WaveInDevice = null;
            }
            FinalizeWaveFile();
        }

        private void FinalizeWaveFile()
        {
            _writer?.Dispose();
            _writer = null;
        }

        #region 播放
        private WaveOut waveOut;
        private TrimWaveStream inStream;

        private VoiceRecorderState voiceRecorderState;
        private SampleAggregator sampleAggregatorPlay;
        private int leftPosition;
        private int rightPosition;
        private int totalWaveFormSamples;
        private int samplesPerSecond;


        public TimeSpan StartPosition
        {
            get => inStream.StartPosition;
            set => inStream.StartPosition = value;
        }

        public TimeSpan EndPosition
        {
            get => inStream.EndPosition;
            set => inStream.EndPosition = value;
        }

        public TimeSpan CurrentPosition { get; set; }

        public PlaybackState PlaybackState { get; private set; }


        public void LoadFile(string path)
        {
            CloseWaveOut();
            CloseInStream();
            inStream = new TrimWaveStream(new WaveFileReader(path));
        }

        public void Play()
        {
            CreateWaveOut();
            if (waveOut.PlaybackState == PlaybackState.Stopped)
            {
                inStream.Position = 0;
                waveOut.Play();
            }
        }

        private void CreateWaveOut()
        {
            if (waveOut == null)
            {
                waveOut = new WaveOut();
                waveOut.Init(inStream);
                waveOut.PlaybackStopped += OnPlaybackStopped;
            }
        }

        void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackState = PlaybackState.Stopped;
        }

        public void Stop()
        {
            waveOut.Stop();
            inStream.Position = 0;
        }

        public void Dispose()
        {
            CloseWaveOut();
            CloseInStream();
        }

        private void CloseInStream()
        {
            if (inStream != null)
            {
                inStream.Dispose();
                inStream = null;
            }
        }

        private void CloseWaveOut()
        {
            if (waveOut != null)
            {
                waveOut.Dispose();
                waveOut = null;
            }
        }


        public string LocateLame()
        {
            string lameExePath = Settings.Default.LameExePath;

            if (String.IsNullOrEmpty(lameExePath) || !File.Exists(lameExePath))
            {
                if (MessageBox.Show(@"是否保存为MP3?", @"提示信息", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.FileName = "lame.exe";
                    if ("true".Equals(ofd.ShowDialog()))
                    {
                        if (File.Exists(ofd.FileName) && ofd.FileName.ToLower().EndsWith("lame.exe"))
                        {
                            Settings.Default.LameExePath = ofd.FileName;
                            Settings.Default.Save();
                            return ofd.FileName;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return lameExePath;
        }

        private void RenderFile()
        {
            SampleAggregatorPlay.RaiseRestart();
            using (WaveFileReader reader = new WaveFileReader(_waveFileName))
            {
                this.samplesPerSecond = reader.WaveFormat.SampleRate;
                SampleAggregatorPlay.NotificationCount = reader.WaveFormat.SampleRate / 10;

                byte[] buffer = new byte[1024];
                WaveBuffer waveBuffer = new WaveBuffer(buffer);
                waveBuffer.ByteBufferCount = buffer.Length;
                int bytesRead;
                do
                {
                    bytesRead = reader.Read(waveBuffer, 0, buffer.Length);
                    int samples = bytesRead / 2;
                    for (int sample = 0; sample < samples; sample++)
                    {
                        if (bytesRead > 0)
                        {
                            SampleAggregatorPlay.Add(waveBuffer.ShortBuffer[sample] / 32768f);
                        }
                    }
                } while (bytesRead > 0);
                int totalSamples = (int)reader.Length / 2;
                TotalWaveFormSamples = totalSamples / SampleAggregatorPlay.NotificationCount;
                SelectAll();
            }
            LoadFile(_waveFileName);
        }

        private void SelectAll()
        {
            LeftPosition = 0;
            RightPosition = TotalWaveFormSamples;
        }
        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = @"WAV file (.wav)|*.wav|MP3 file (.mp3)|.mp3";
            saveFileDialog.DefaultExt = ".wav";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveAs(saveFileDialog.FileName);
            }
        }

        private void SaveAs(string fileName)
        {
            AudioSaver saver = new AudioSaver(_waveFileName);
            saver.TrimFromStart = PositionToTimeSpan(LeftPosition);
            saver.TrimFromEnd = PositionToTimeSpan(TotalWaveFormSamples - RightPosition);

            if (fileName.ToLower().EndsWith(".wav"))
            {
                saver.SaveFileFormat = SaveFileFormat.Wav;
                saver.SaveAudio(fileName);
            }
            else if (fileName.ToLower().EndsWith(".mp3"))
            {
                string lameExePath = LocateLame();
                if (lameExePath != null)
                {
                    saver.SaveFileFormat = SaveFileFormat.Mp3;
                    saver.LameExePath = lameExePath;
                    saver.SaveAudio(fileName);
                }
            }
            else
            {
                MessageBox.Show(@"请选择支持的输出格式");
            }
        }

        private TimeSpan PositionToTimeSpan(int position)
        {
            int samples = SampleAggregator.NotificationCount * position;
            return TimeSpan.FromSeconds((double)samples / samplesPerSecond);
        }

        #endregion
        private void BtnStartRecording_Click(object sender, System.EventArgs e)
        {
            //_waveFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
            BeginRecording();
        }

        private void btnStopRecording_Click(object sender, EventArgs e)
        {
            StopRecording();
        }

        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            Process.Start(_outputFolder);
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            RenderFile();
            Play();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void BtnDeleteFile_Click(object sender, EventArgs e)
        {
            if (listBoxRecordings.SelectedItem != null)
            {
                try
                {
                    File.Delete(Path.Combine(_outputFolder, (string)listBoxRecordings.SelectedItem));
                    listBoxRecordings.Items.Remove(listBoxRecordings.SelectedItem);
                    if (listBoxRecordings.Items.Count > 0)
                    {
                        listBoxRecordings.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"不能删除录音文件！" + ex.Message);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}
