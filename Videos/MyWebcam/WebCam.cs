using System;
using System.Windows.Forms;

namespace MyWebcam
{
    public partial class WebCam : UserControl
    {
        VideoClass vc = new VideoClass();
        bool isstart = false;
        public WebCam()
        {
            InitializeComponent();
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            start();
        }

        public void start()
        {
            vc.Initialize(videobox.Handle, videobox.Width, videobox.Height);

            if (vc.StartWebcam())
            {
                isstart = true;
            }
            else
            {
                MessageBox.Show("启动摄像头失败");
            }
            UpdateButton();
        }

        private void UpdateButton()
        {
            if (isstart)
            {
                btnstart.Enabled = false;
                btncapture.Enabled = true;
                btnstop.Enabled = true;
            }
            else
            {
                btnstart.Enabled = true;
                btncapture.Enabled = false;
                btnstop.Enabled = false;
            }
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            stop();
        }

        public void stop()
        {
            if (vc.StopWebcam())
            {
                isstart = false;
            }
            UpdateButton();
        }

        private void btncapture_Click(object sender, EventArgs e)
        {
            string guid = Guid.NewGuid().ToString("N");
            string strname = string.Format("{0}.jpg", guid);
            //vc.SaveImage(strname);
            //if (File.Exists(strname))
            //{
            //    Image img = new Bitmap(strname);
            //    string newfilename = string.Format("{0}.jpg", guid);
            //    img.Save(newfilename, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    img.Dispose();
            //    File.Delete(strname);
            //}
            capture(strname);
        }

        public void capture(string strname)
        {
            vc.CopyToClipBorad();
            System.Drawing.Image img = vc.getCaptureImage();
            img.Save(strname, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void WebCam_Load(object sender, EventArgs e)
        {
            UpdateButton();
        }
    }
}
