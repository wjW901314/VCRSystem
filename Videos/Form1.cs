using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Videos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void VideoShow()
        {
            //video = new Video(panelPreview.Handle, panelPreview.Width, panelPreview.Height);
            video = new Video(pictureBox1.Handle,pictureBox1.Width,pictureBox1.Height);
            video.StartWebCam();
            video.get();
            video.Capparms.fYield = true;
            video.Capparms.fAbortLeftMouse = false;
            video.Capparms.fAbortRightMouse = false;
            video.set();

        }
        private void Form1_Load(object sender, EventArgs e)
        {
           VideoShow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("开始录象！");
            video.StarKinescope("d://" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".avi"); 
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            video.CloseWebcam(0);
        }
    }
}
