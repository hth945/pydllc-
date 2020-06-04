using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsPy.runPyDdllc;

namespace WindowsFormsPy
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();

            runPyDdllc.init();
            Application.Idle += Application_Idle; //程序空闲
        }
        void Application_Idle(object sender, EventArgs e)
        {
            QueueData b;
            while (runPyDdllc.outRecQueue.TryDequeue(out b)) //清空队列
            {
                if (b.cmd == "computeOK")  //串口来的数据
                {
                    float[] f = (float[])b.o;
                    textBox1.Text += b.id + ":" + f[0] + "\r\n";
                }
            }
        }

        int temI = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            Thread mThread = new Thread(() => // Lambda 表达式
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int i = 0; i < 3000; i++)
                {
                    FileStream indataFile = new FileStream("0_7.bin", FileMode.Open);

                    byte[] inData = new byte[indataFile.Length];
                    indataFile.Read(inData, 0, inData.Length);
                    indataFile.Close();
                    Thread.Sleep(200);
                    runPyDdllc.setAsynInData(inData, temI.ToString());
                    temI += 1;
                }

                //耗时巨大的代码
                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;
                Console.WriteLine("Stopwatch总共花费{0}ms.", ts2.TotalMilliseconds);
            });
            mThread.Start();  // 开始
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            runPyDdllc.deinit();
        }
    }
}


