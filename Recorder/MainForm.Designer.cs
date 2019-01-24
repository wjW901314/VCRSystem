using Recorder;

namespace AudioandVideo
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.ucAudioRecorder1 = new UcAudioRecorder();
            this.SuspendLayout();
            // 
            // ucAudioRecorder1
            // 
            this.ucAudioRecorder1.Location = new System.Drawing.Point(7, 9);
            this.ucAudioRecorder1.Name = "ucAudioRecorder1";
            this.ucAudioRecorder1.Size = new System.Drawing.Size(806, 450);
            this.ucAudioRecorder1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 463);
            this.Controls.Add(this.ucAudioRecorder1);
            this.Name = "MainForm";
            this.Text = "录音&播放系统";
            this.ResumeLayout(false);

        }

        #endregion

        private UcAudioRecorder ucAudioRecorder1;
    }
}

