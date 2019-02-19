namespace MyWebcam
{
    partial class WebCam
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.videobox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnstop = new System.Windows.Forms.Button();
            this.btncapture = new System.Windows.Forms.Button();
            this.btnstart = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.videobox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.videobox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(306, 273);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // videobox
            // 
            this.videobox.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanel1.SetColumnSpan(this.videobox, 3);
            this.videobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videobox.Location = new System.Drawing.Point(3, 3);
            this.videobox.Name = "videobox";
            this.videobox.Size = new System.Drawing.Size(300, 233);
            this.videobox.TabIndex = 4;
            this.videobox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnstop);
            this.panel1.Controls.Add(this.btncapture);
            this.panel1.Controls.Add(this.btnstart);
            this.panel1.Location = new System.Drawing.Point(41, 242);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(224, 28);
            this.panel1.TabIndex = 5;
            // 
            // btnstop
            // 
            this.btnstop.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnstop.Location = new System.Drawing.Point(150, 0);
            this.btnstop.Name = "btnstop";
            this.btnstop.Size = new System.Drawing.Size(75, 28);
            this.btnstop.TabIndex = 4;
            this.btnstop.Text = "停止";
            this.btnstop.UseVisualStyleBackColor = true;
            this.btnstop.Click += new System.EventHandler(this.btnstop_Click);
            // 
            // btncapture
            // 
            this.btncapture.Dock = System.Windows.Forms.DockStyle.Left;
            this.btncapture.Location = new System.Drawing.Point(75, 0);
            this.btncapture.Name = "btncapture";
            this.btncapture.Size = new System.Drawing.Size(75, 28);
            this.btncapture.TabIndex = 3;
            this.btncapture.Text = "拍照";
            this.btncapture.UseVisualStyleBackColor = true;
            this.btncapture.Click += new System.EventHandler(this.btncapture_Click);
            // 
            // btnstart
            // 
            this.btnstart.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnstart.Location = new System.Drawing.Point(0, 0);
            this.btnstart.Name = "btnstart";
            this.btnstart.Size = new System.Drawing.Size(75, 28);
            this.btnstart.TabIndex = 2;
            this.btnstart.Text = "启动";
            this.btnstart.UseVisualStyleBackColor = true;
            this.btnstart.Click += new System.EventHandler(this.btnstart_Click);
            // 
            // WebCam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WebCam";
            this.Size = new System.Drawing.Size(306, 273);
            this.Load += new System.EventHandler(this.WebCam_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.videobox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox videobox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnstop;
        private System.Windows.Forms.Button btncapture;
        private System.Windows.Forms.Button btnstart;

    }
}
