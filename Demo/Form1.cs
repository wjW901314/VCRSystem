using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Demo.Logic;
using Demo.Logic.Impl;

namespace Demo
{
    public partial class Form1 : Form
    {
        private IWaveIn _captureDevice;
        private WaveFileWriter _writer;
        private List<string> _devices;
        private readonly string _outputFolder;
        private string _outputFilename;
        private bool _recordingState = true;

        public Form1()
        {
            InitializeComponent();
            Disposed += Form1_Disposed;
            _outputFolder = Path.Combine(Path.GetTempPath(), "NADemo");
            Directory.CreateDirectory(_outputFolder);
        }

        private void Form1_Disposed(object sender, EventArgs e)
        {
            Cleanup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ser = "";
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = @"*.wav|*.*|All files(*.*)|*.*";
            openFile.Title = @"选择文件";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;
            string fileName = string.Empty;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                fileName = openFile.FileName;
            }

            string byte64Str = string.Empty;
            int toTal = 0, count, index = 0;

            if (string.IsNullOrWhiteSpace(fileName))
                MessageBox.Show("文件名称无效");

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024 * 1024 * 4];
                while ((count = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    index++;
                    toTal += count;
                    //byte64Str += Convert.ToBase64String(BitConverter.GetBytes(toTal));
                }

                textBox3.Text = fs.Length.ToString();
                textBox2.Text = index.ToString();
                textBox4.Text = toTal.ToString();
                byte64Str = Convert.ToBase64String(BitConverter.GetBytes(toTal));
            }

            textBox1.Text = byte64Str;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string file = ConfigurationManager.AppSettings.Get("BigFile.FilePath");
            string splitFileFormat = ConfigurationManager.AppSettings.Get("BigFile.FileSilitPathFormate");
            int splitMinFileSize = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BigFile.SplitMinFileSize")) *
                                   1024 * 1024 * 1204;
            int splitFileSize = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BigFile.SplitFileSize")) * 1024 *
                                1024;

            FileInfo fileInfo = new FileInfo(file);
            if (fileInfo.Length > splitMinFileSize)
            {
                Console.WriteLine("判定结果：需要分隔文件！");
            }
            else
            {
                Console.WriteLine("判定结果：不需要分隔文件！");
                Console.ReadKey();
                return;
            }

            int steps = (int) (fileInfo.Length / splitFileSize);
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int couter = 1;
                    bool isReadingComplete = false;
                    while (!isReadingComplete)
                    {
                        string filePath = string.Format(splitFileFormat, couter);
                        Console.WriteLine("开始读取文件【{1}】：{0}", filePath,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        byte[] input = br.ReadBytes(splitFileSize);
                        using (FileStream writeFs = new FileStream(filePath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(writeFs, Encoding.UTF8))
                            {
                                bw.Write(input);
                            }
                        }

                        isReadingComplete = (input.Length != splitFileSize);
                        if (!isReadingComplete)
                        {
                            couter += 1;
                        }

                        Console.WriteLine("完成读取文件【{1}】：{0}", filePath,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                }
            }


            Console.WriteLine("分隔完成，请按下任意键结束操作。。。");
        }

      
        private void button3_Click(object sender, EventArgs e)
        {
            if (_recordingState)
            {
                 _devices = new List<string>();
                if (_captureDevice == null)
                {
                    _captureDevice = CreateDevice();
                }
                _outputFilename = Guid.NewGuid().ToString();
                _writer = new WaveFileWriter(Path.Combine(_outputFolder, _outputFilename), _captureDevice.WaveFormat);
                _captureDevice.StartRecording();
                button1.Text = button1.Text.Equals("开始录制") ? @"停止录制" : @"开始录制";
                _recordingState = false;
            }
            else
            {
                StopRecording();
            }
        }

        private string GetFileName()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 创建录音驱动
        /// </summary>
        /// <returns></returns>
        private IWaveIn CreateDevice()
        {
            _devices.Clear();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                _devices.Add(WaveIn.GetCapabilities(i).ProductName);
            }

            var devNumber = _devices.Count - 1; //录音驱动
            IWaveIn newWaveIn = new WaveIn()
            {
                DeviceNumber = devNumber
            };
            var sampleRate = 48000; //采样率
            var channels = 1; //声道数
            newWaveIn.WaveFormat = new WaveFormat(sampleRate, channels);
            newWaveIn.DataAvailable += OnDataAvailable;
            newWaveIn.RecordingStopped += OnRecordingStopped;
            return newWaveIn;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<WaveInEventArgs>(OnDataAvailable), sender, e);
            }
            else
            {
                _writer.Write(e.Buffer, 0, e.BytesRecorded);
                int secondsRecorded = (int) (_writer.Length / _writer.WaveFormat.AverageBytesPerSecond);
                if (secondsRecorded >= 30)
                {
                    StopRecording();
                }
                else
                {
                    progressBar1.Value = secondsRecorded;
                }
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
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
                    MessageBox.Show(string.Format("录制过程中遇到问题：{0}",
                        e.Exception.Message));
                }

                //int newItemIndex = listBoxRecordings.Items.Add(outputFilename);
                //listBoxRecordings.SelectedIndex = newItemIndex;
                //SetControlStates(false);
            }
        }
        private void Cleanup()
        {
            if (_captureDevice != null)
            {
                _captureDevice.Dispose();
                _captureDevice = null;
            }
            FinalizeWaveFile();
        }
        private void FinalizeWaveFile()
        {
            _writer?.Dispose();
            _writer = null;
        }

        private void StopRecording()
        {
            _captureDevice?.StopRecording();
        }
    }
}