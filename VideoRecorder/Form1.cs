using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;

namespace VideoRecorder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private FilterInfoCollection videoDevices; //摄像头设备
        private VideoCaptureDevice videoSource; //视频的来源选择
        private VideoSourcePlayer videoSourcePlayer;
        private VideoFileWriter writer; //写入到视频
        private bool is_record_video = false; //是否开始录像
        System.Timers.Timer timer_count;
        int tick_num = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.label5.Visible = false;

            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.videoSource = new VideoCaptureDevice();
            this.writer = new VideoFileWriter();

            //设置视频编码格式
            this.comboBox_videoecode.Items.Add("Raw");
            this.comboBox_videoecode.Items.Add("MPEG2");
            this.comboBox_videoecode.Items.Add("FLV1");
            this.comboBox_videoecode.Items.Add("H263p");
            this.comboBox_videoecode.Items.Add("MSMPEG4v3");
            this.comboBox_videoecode.Items.Add("MSMPEG4v2");
            this.comboBox_videoecode.Items.Add("WMV2");
            this.comboBox_videoecode.Items.Add("WMV1");
            this.comboBox_videoecode.Items.Add("MPEG4");
            this.comboBox_videoecode.SelectedIndex = 1;

            //设置视频来源
            try
            {
                // 枚举所有视频输入设备
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException(); //没有找到摄像头设备

                foreach (FilterInfo device in videoDevices)
                {
                    this.comboBox_camera.Items.Add(device.Name);
                }

                //this.comboBox_camera.SelectedIndex = 0;  //注释掉，选择摄像头来源的时候才会才会触发显示摄像头信息
            }
            catch (ApplicationException)
            {
                videoDevices = null;
                MessageBox.Show("没有找到摄像头设备", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //秒表
            this.timer_count = new System.Timers.Timer(); //实例化Timer类，设置间隔时间为10000毫秒；
            this.timer_count.Elapsed += new System.Timers.ElapsedEventHandler(tick_count); //到达时间的时候执行事件；
            this.timer_count.AutoReset = true; //设置是执行一次（false）还是一直执行(true)；
            this.timer_count.Interval = 1000;
        }

        private void btnExist_Click(object sender, EventArgs e)
        {
            if (this.writer.IsOpen)
            {
                MessageBox.Show("视频流还没有写完，请点击结束录制。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.videoSource.SignalToStop();
            this.videoSource.WaitForStop();
            this.videoSourcePlayer.SignalToStop();
            this.videoSourcePlayer.WaitForStop();
            this.Hide();
            this.Close();
            this.Dispose();
        }

        //视频源选择下拉框选择之后的响应函数
        private void comboBox_camera_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected_index = this.comboBox_camera.SelectedIndex;
            this.videoSource = new VideoCaptureDevice(videoDevices[selected_index].MonikerString);
            // set NewFrame event handler
            videoSource.NewFrame += new NewFrameEventHandler(show_video);
            videoSource.Start();
            videoSourcePlayer.VideoSource = videoSource;
            videoSourcePlayer.Start();
            this.label5.Text = "连接中...";
            this.label5.Visible = true;
            isshowed = true;
        }

        bool isshowed = true;

        //新帧的触发函数
        private void show_video(object sender, NewFrameEventArgs eventArgs)
        {
            if (isshowed)
            {
                isshowed = false;
                this.label5.Visible = false;
            }

            Bitmap bitmap = eventArgs.Frame; //获取到一帧图像
            pictureBox1.Image = Image.FromHbitmap(bitmap.GetHbitmap());
            if (is_record_video)
            {
                writer.WriteVideoFrame(bitmap);
            }
        }

        //拍摄图像按钮响应函数
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.videoSource.IsRunning && this.videoSourcePlayer.IsRunning)
            {
                Bitmap bitmap = this.videoSourcePlayer.GetCurrentVideoFrame();
                bitmap.Save("img.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
                MessageBox.Show("摄像头没有运行", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //开始录像按钮响应函数
        private void button_start_Click(object sender, EventArgs e)
        {
            int width = 640; //录制视频的宽度
            int height = 480; //录制视频的高度
            int fps = 9;

            //创建一个视频文件
            String video_format = this.comboBox_videoecode.Text.Trim(); //获取选中的视频编码
            if (this.videoSource.IsRunning && this.videoSourcePlayer.IsRunning)
            {
                if (-1 != video_format.IndexOf("MPEG"))
                {
                    writer.Open("test.avi", width, height, fps, VideoCodec.MPEG4);
                }
                else if (-1 != video_format.IndexOf("WMV"))
                {
                    writer.Open("test.wmv", width, height, fps, VideoCodec.WMV1);
                }
                else
                {
                    writer.Open("test.mkv", width, height, fps, VideoCodec.Default);
                }
            }
            else
                MessageBox.Show("没有视频源输入，无法录制视频。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            timer_count.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；
            this.label5.Visible = true;
            this.label5.Text = "REC";
            this.is_record_video = true;
        }


        //停止录制视频响应函数
        private void button_stop_Click(object sender, EventArgs e)
        {
            this.label5.Visible = false;
            this.is_record_video = false;
            this.writer.Close();
            this.timer_count.Enabled = false;
            tick_num = 0;
        }

        //暂停按钮响应函数
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.btnTrimOut.Text.Trim() == "暂停录像")
            {
                this.is_record_video = false;
                this.label5.Visible = false;
                this.btnTrimOut.Text = "恢复录像";
                timer_count.Enabled = false; //暂停计时
                return;
            }

            if (this.btnTrimOut.Text.Trim() == "恢复录像")
            {
                this.is_record_video = true;
                timer_count.Enabled = true; //恢复计时
                this.label5.Visible = true;
                this.btnTrimOut.Text = "暂停录像";
            }
        }

        //计时器响应函数
        public void tick_count(object source, System.Timers.ElapsedEventArgs e)
        {
            tick_num++;
            int temp = tick_num;

            int sec = temp % 60;

            int min = temp / 60;
            if (60 == min)
            {
                min = 0;
                min++;
            }

            int hour = min / 60;

            String tick = hour.ToString() + "：" + min.ToString() + "：" + sec.ToString();
            this.labTime.Text = "计时:" + tick;
        }
    }
}