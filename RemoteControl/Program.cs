using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RemoteControl
{
    static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 创建互斥体，防止多开
            bool createNew;
            // System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            using (Mutex mutex = new Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    MessageBox.Show("该程序已经在运行，若无法找到，请在任务管理器中结束后重新运行");
                }
            }
        }
    }
}
