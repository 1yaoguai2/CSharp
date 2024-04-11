using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultithreadingStudy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 单线程做菜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            MessageBox.Show("素菜做好了，花费了两秒", "厨房被占用");
            Thread.Sleep(3000);
            MessageBox.Show("荤菜做好了，花费了三秒", "厨房被占用");
        }

        /// <summary>
        /// 使用线程做菜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(2000);
                MessageBox.Show("素菜做好了，花费了两秒", "厨房有空闲");
                Thread.Sleep(3000);
                MessageBox.Show("荤菜做好了，花费了三秒", "厨房有空闲");
            });
            thread.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                MessageBox.Show("素菜做好了，花费了两秒", "厨房有空闲");
                Thread.Sleep(3000);
                MessageBox.Show("荤菜做好了，花费了三秒", "厨房有空闲");
            });
        }

        /// <summary>
        /// 线程做多道菜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(() =>
            {
                Thread.Sleep(2000);
                MessageBox.Show("素菜做好了，花费了两秒", "厨房有空闲");
            });
            thread1.Start();
            Thread thread2 = new Thread(() =>
            {
                Thread.Sleep(3000);
                MessageBox.Show("荤菜做好了，花费了三秒", "厨房有空闲");
            });
            thread2.Start();
        }

        /// <summary>
        /// Task同时做两道菜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                MessageBox.Show("素菜做好了，花费了两秒", "厨房有空闲");
            });
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                MessageBox.Show("荤菜做好了，花费了三秒", "厨房有空闲");
            });
        }

        /// <summary>
        /// 等待菜全部做好吃饭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button6_Click(object sender, EventArgs e)
        {
            // await Task.Run(() =>
            // {
            //     Thread.Sleep(2000);
            //     MessageBox.Show("素菜做好了，花费了两秒", "按顺序做菜");
            //     Thread.Sleep(3000);
            //     MessageBox.Show("荤菜做好了，花费了三秒", "按顺序做菜");
            // });
            // MessageBox.Show("菜全部做好了，可以吃饭了", "提示");

            //优化同时做菜,优化失败，第二道菜需要等待第一道菜完成才会开始
            // await Task.Run(() =>
            // {
            //     Thread.Sleep(2000);
            //     MessageBox.Show("素菜做好了，花费了两秒", "按顺序做菜");
            // });
            // await Task.Run(() =>
            // {
            //     Thread.Sleep(3000);
            //     MessageBox.Show("荤菜做好了，花费了三秒", "按顺序做菜");
            // });
            // MessageBox.Show("菜全部做好了，可以吃饭了", "提示");

            //再次优化，同时做菜，做好了提示吃饭
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() =>
            {
                Thread.Sleep(2000);
                MessageBox.Show("素菜做好了，花费了两秒", "按顺序做菜");
            }));
            tasks.Add(Task.Run(() =>
            {
                Thread.Sleep(3000);
                MessageBox.Show("荤菜做好了，花费了三秒", "按顺序做菜");
            }));
            Task.WhenAll(tasks).ContinueWith(t =>
            {
                MessageBox.Show("菜全部做好了，可以吃饭了", "提示");
            });
        }
    }
}