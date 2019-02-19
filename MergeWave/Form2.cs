using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace MergeWave
{
    public partial class Form2 : Form
    {
        private int currentCount = 0;

        private System.Timers.Timer timer;

        //定义委托
        public delegate void SetControlValue(string value);

        public Form2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化Timer控件
        /// </summary>
        private void InitTimer()
        {
            //设置定时间隔(毫秒为单位)
            int interval = 1000;
            timer = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer.Enabled = true;
            //绑定Elapsed事件
            timer.Elapsed += TimerUp;
        }

        private void TimerUp(object sender, ElapsedEventArgs e)
        {
            try
            {
                currentCount += 1;
                this.Invoke(new SetControlValue(SetTextBoxText), currentCount.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行定时到点事件失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 设置文本框的值
        /// </summary>
        /// <param name="strValue"></param>
        private void SetTextBoxText(string strValue)
        {
            label1.Text = this.currentCount.ToString().Trim();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitTimer();

            timer.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //InitTimer();
        }
    }
}