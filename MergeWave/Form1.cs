using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MergeWave
{
    public partial class Form1 : Form
    {
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
            WavMergeUtil.MergeWav(_filePaths,"MG_" + CommonUtil.GetTimeStamp() + ".wav");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            WAVFile.MergeAudioFiles(_filePaths,"HY_" + CommonUtil.GetTimeStamp() + ".wav",
                "waveTest");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //_outputFolder = Path.Combine(Path.GetTempPath(), "MargeWaveDemo");
            //Directory.CreateDirectory(_outputFolder);
        }
    }
}