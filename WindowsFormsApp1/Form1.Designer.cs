namespace WindowsFormsApp1
{
    partial class Form1
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
         /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
         private void InitializeComponent()
         {
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonPz = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.picCapture = new System.Windows.Forms.PictureBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCapture)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(45, 3);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(103, 39);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "开始录像";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(211, 3);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(93, 39);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "停止录像";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPz
            // 
            this.buttonPz.Location = new System.Drawing.Point(365, 3);
            this.buttonPz.Name = "buttonPz";
            this.buttonPz.Size = new System.Drawing.Size(90, 39);
            this.buttonPz.TabIndex = 3;
            this.buttonPz.Text = "拍  照";
            this.buttonPz.UseVisualStyleBackColor = true;
            this.buttonPz.Click += new System.EventHandler(this.buttonPz_Click);
            // 
            // panelTop
            // 
            this.panelTop.AutoSize = true;
            this.panelTop.Controls.Add(this.picCapture);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(546, 403);
            this.panelTop.TabIndex = 4;
            // 
            // picCapture
            // 
            this.picCapture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picCapture.InitialImage = null;
            this.picCapture.Location = new System.Drawing.Point(0, 0);
            this.picCapture.Name = "picCapture";
            this.picCapture.Size = new System.Drawing.Size(546, 403);
            this.picCapture.TabIndex = 0;
            this.picCapture.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonStart);
            this.panelBottom.Controls.Add(this.buttonStop);
            this.panelBottom.Controls.Add(this.buttonPz);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 354);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(546, 49);
            this.panelBottom.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 403);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCapture)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picCapture;
         private System.Windows.Forms.Button buttonStart;
         private System.Windows.Forms.Button buttonStop;
         private System.Windows.Forms.Button buttonPz;
         private System.Windows.Forms.Panel panelTop;
         private System.Windows.Forms.Panel panelBottom;
    }
}

