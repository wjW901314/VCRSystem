using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MergeWave
{
    public partial class Form1 : Form
    {
         public int currentCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private string[] _filePaths = new string[2];

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "选择第一个Wave文件";
            openFile.Filter = "*.wav|*.*|All files(*.*)|*.*";
            openFile.FilterIndex = 2;
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFile.FileName;
            }

            _filePaths[0] = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "选择第二个Wave文件";
            openFile.Filter = "*.wav|*.*|All files(*.*)|*.*";
            openFile.FilterIndex = 2;
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFile.FileName;
            }

            _filePaths[1] = textBox2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(Environment.CurrentDirectory);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            WavMergeUtil.MergeWav(_filePaths, "MG_" + CommonUtil.GetTimeStamp() + ".wav");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            WAVFile.MergeAudioFiles(_filePaths, "HY_" + CommonUtil.GetTimeStamp() + ".wav",
                "waveTest");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //_outputFolder = Path.Combine(Path.GetTempPath(), "MargeWaveDemo");
            //Directory.CreateDirectory(_outputFolder);

            //设置Timer控件可用
            this.timer.Enabled = true;
            //设置时间间隔（毫秒为单位）
            this.timer.Interval = 1000;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV Files (*.wav)|*.wav|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var inputFileName = openFileDialog.FileName;
                var outputFileName = inputFileName.Substring(0, inputFileName.Length - 3) + "mp3";

                var mediaType = MediaFoundationEncoder.SelectMediaType(
                    AudioSubtypes.MFAudioFormat_MP3,
                    new WaveFormat(44100, 1),
                    0);

                using (var reader = new MediaFoundationReader(inputFileName))
                {
                    using (var encoder = new MediaFoundationEncoder(mediaType))
                    {
                        encoder.Encode(outputFileName, reader);
                    }
                }
            }

            MessageBox.Show("操作成功");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //this.timer.Start();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            currentCount += 1;
            this.label4.Text = currentCount.ToString().Trim();
        }
    }
}