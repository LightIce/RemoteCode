using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;



namespace RemoteControl
{


    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern void LockWorkStation();

        NotifyIcon notifyicon = new NotifyIcon();
        Icon ico = new Icon("RemoteControl.ico");
        ContextMenu notifyContextMenu = new ContextMenu();

        // 全局变量，用来表示服务是否开启，false = 未开启， true = 开启
        bool isRun = false;

        // 全局变量，表示线程是否应该暂停，false = 不暂停， true = 暂停
        static bool isPause = false;
        Thread threadGetCode;

        public Form1()
        {

            InitializeComponent();
            this.labelState.ForeColor = System.Drawing.Color.Red;
//             threadGetCode = new Thread(GetCode);
//             threadGetCode.Start();
//             threadGetCode.Suspend();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 设置鼠标留在托盘上面的文字
            this.notifyIcon1.Text = "Remote Control - Beta";
        }

        // 窗口大小变化时的事件
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            // 如果点击的最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Icon = ico;
                this.ShowInTaskbar = false;
                notifyicon.Visible = true;
            }
        }

        // 双击托盘图标的事件
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            // 判断是否窗体已经最小化
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                this.Activate();
                this.ShowInTaskbar = true;
                notifyicon.Visible = false;
            }
        }

        // 点击关闭按钮时的事件
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("真的要退出该程序么？退出后将不能再通过手机控制哦~",
                "Remote Control",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        public delegate void ProcessDelegate();
        // 获取code的线程函数
        private void GetCode()
        {
            this.timer1.Interval = 5000;
            this.timer1.Start();
            return;
        }

        // 计时器函数
        void fnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            if (isRun)
            {
                // MessageBox.Show("fuck!");
                // 获取网页内容
                HttpWebRequest webRequest =
                    (HttpWebRequest)WebRequest.Create("http://lightless.cn/RemoteControl/GetCode.php?sec-code=lightless");
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                Stream st = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st, System.Text.Encoding.GetEncoding("UTF-8"));

                String buffer = reader.ReadToEnd();
                st.Close();
                reader.Close();
                //MessageBox.Show(buffer);
                String[] str = buffer.Split(new char[] { '|'});
                //MessageBox.Show(str[1]);
                switch (str[0])
                {
                    case "RC_LOCK_SCREEN":
                        // 检查是否为有效指令
                        if (str[1] == "1")
                        {
                            //MessageBox.Show("Do nothing.");
                            break;
                        }
                        // 有效指令锁屏
                        LockWorkStation();
                        break;
                }
                
            }
            timer.Start();
        }

        System.Timers.Timer timer;
        // 开启或者关闭服务
        private void button1_Click(object sender, EventArgs e)
        {
            timer = new System.Timers.Timer();
            // 判断当前服务状态并进行对应UI操作
            if (isRun == false)
            {
                // 当前未运行服务
                // 处理UI问题
                this.labelState.Text = "已开启";
                this.labelState.ForeColor = System.Drawing.Color.Green;
                this.buttonStart.Text = "停止服务";
                isRun = true;

                // 开计时器
                timer.Interval = 5000;
                timer.Enabled = true;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(fnTimer);

                // 开线程

                // threadGetCode.Resume();


            }
            else if (isRun == true)
            {
                // 当前服务已经开启
                // 处理UI问题
                this.labelState.Text = "未开启";
                this.labelState.ForeColor = System.Drawing.Color.Red;
                this.buttonStart.Text = "开启服务";
                isRun = false;
                timer.Enabled = false;
                timer.Stop();
                //threadGetCode.Suspend();
                

            }
            else
            {
                MessageBox.Show("系统内部错误，请联系开发者");
                Environment.Exit(0);
            }

        }

        // 设置
        private void button2_Click(object sender, EventArgs e)
        {

        }



        

        

    }
}
