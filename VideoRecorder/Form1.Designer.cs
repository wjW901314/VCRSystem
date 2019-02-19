namespace VideoRecorder
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
         Video video;
        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_camera = new System.Windows.Forms.ComboBox();
            this.labTime = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPhoto = new System.Windows.Forms.Button();
            this.comboBox_videoecode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTrimOut = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExist = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new AForge.Controls.PictureBox();
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 480);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "视频源:";
            // 
            // comboBox_camera
            // 
            this.comboBox_camera.FormattingEnabled = true;
            this.comboBox_camera.Location = new System.Drawing.Point(102, 478);
            this.comboBox_camera.Name = "comboBox_camera";
            this.comboBox_camera.Size = new System.Drawing.Size(121, 21);
            this.comboBox_camera.TabIndex = 4;
            this.comboBox_camera.SelectedIndexChanged += new System.EventHandler(this.comboBox_camera_SelectedIndexChanged);
            // 
            // labTime
            // 
            this.labTime.AutoSize = true;
            this.labTime.Location = new System.Drawing.Point(358, 481);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(42, 14);
            this.labTime.TabIndex = 5;
            this.labTime.Text = "计时:";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(252, 478);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "开始录像";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.button_start_Click);
            // 
            // btnPhoto
            // 
            this.btnPhoto.Location = new System.Drawing.Point(479, 478);
            this.btnPhoto.Name = "btnPhoto";
            this.btnPhoto.Size = new System.Drawing.Size(75, 23);
            this.btnPhoto.TabIndex = 7;
            this.btnPhoto.Text = "拍摄照片";
            this.btnPhoto.UseVisualStyleBackColor = true;
            this.btnPhoto.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox_videoecode
            // 
            this.comboBox_videoecode.FormattingEnabled = true;
            this.comboBox_videoecode.Location = new System.Drawing.Point(102, 519);
            this.comboBox_videoecode.Name = "comboBox_videoecode";
            this.comboBox_videoecode.Size = new System.Drawing.Size(121, 21);
            this.comboBox_videoecode.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 521);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 14);
            this.label3.TabIndex = 8;
            this.label3.Text = "视频编码:";
            // 
            // btnTrimOut
            // 
            this.btnTrimOut.Location = new System.Drawing.Point(252, 519);
            this.btnTrimOut.Name = "btnTrimOut";
            this.btnTrimOut.Size = new System.Drawing.Size(75, 23);
            this.btnTrimOut.TabIndex = 10;
            this.btnTrimOut.Text = "暂停录像";
            this.btnTrimOut.UseVisualStyleBackColor = true;
            this.btnTrimOut.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(360, 521);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "停止录像";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // btnExist
            // 
            this.btnExist.Location = new System.Drawing.Point(490, 521);
            this.btnExist.Name = "btnExist";
            this.btnExist.Size = new System.Drawing.Size(75, 23);
            this.btnExist.TabIndex = 12;
            this.btnExist.Text = "退出";
            this.btnExist.UseVisualStyleBackColor = true;
            this.btnExist.Click += new System.EventHandler(this.btnExist_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(46, 585);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "label5";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = null;
            this.pictureBox1.Location = new System.Drawing.Point(12, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(339, 451);
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.Location = new System.Drawing.Point(485, 65);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(353, 213);
            this.videoSourcePlayer1.TabIndex = 16;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 675);
            this.Controls.Add(this.videoSourcePlayer1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnExist);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnTrimOut);
            this.Controls.Add(this.comboBox_videoecode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnPhoto);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.labTime);
            this.Controls.Add(this.comboBox_camera);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_camera;
        private System.Windows.Forms.Label labTime;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPhoto;
        private System.Windows.Forms.ComboBox comboBox_videoecode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTrimOut;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExist;
        private System.Windows.Forms.Label label5;
        private AForge.Controls.PictureBox pictureBox1;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
    }
}

