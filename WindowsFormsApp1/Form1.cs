using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        cVideo video;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            //btnPz.Enabled = true;
            video = new cVideo(picCapture.Handle, picCapture.Width, picCapture.Height);
            video.StartWebCam();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            buttonPz.Enabled = false;
            video.CloseWebcam();
        }

        private void buttonPz_Click(object sender, EventArgs e)
        {
            video.GrabImage(picCapture.Handle, "d:\\a.bmp");
        }
    }
}
