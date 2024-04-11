using System;
using System.Diagnostics;
using System.Windows.Forms;
using java.lang;
using StringBuilder = System.Text.StringBuilder;

namespace StingStudy
{
    public partial class Form1 : Form
    {
        private readonly string testStr = "学以致用，学而思之，知识迭代！";
        private double time1;
        private double time2;
        private double time3;
        private double time4;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     string使用”+“进行字符串拼接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            var watch = new Stopwatch();
            watch.Start();

            var str = "";
            for (var i = 0; i < 100000; i++)
            {
                //str = str + "学以致用，学而思之，知识迭代！";
                str = str + testStr;
                progressBar1.Value++;
            }

            watch.Stop();
            time1 = watch.Elapsed.TotalMilliseconds;
            label1.Text = $"{label1.Text}：{time1 / 1000:0.000}秒，字符串长度：{str.Length}";
            label6.Text = $"{label6.Text},使用+号耗时：{time1 / 1000:0.000}秒";
        }

        /// <summary>
        ///     使用 $和 {}进行字符串拼接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            progressBar2.Value = 0;
            var watch = new Stopwatch();
            watch.Start();

            var str = "";
            for (var i = 0; i < 100000; i++)
            {
                //str = str + "学以致用，学而思之，知识迭代！";
                str = $"{str}{testStr}";
                progressBar2.Value++;
            }

            watch.Stop();
            time2 = watch.Elapsed.TotalMilliseconds;
            label2.Text = $"{label2.Text}：{time2 / 1000:0.000}秒，字符串长度：{str.Length}";
            label6.Text = $"{label6.Text},使用$号耗时：{time2 / 1000:0.000}秒";
        }

        /// <summary>
        ///     使用stringBuilder进行字符串拼接，线程不安全，直接修改string的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            progressBar3.Value = 0;
            var watch = new Stopwatch();
            watch.Start();

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < 100000; i++)
            {
                //str = str + "学以致用，学而思之，知识迭代！";
                stringBuilder.Append(i + testStr);
                progressBar3.Value++;
            }

            watch.Stop();
            time3 = watch.Elapsed.TotalMilliseconds;
            label3.Text = $"{label3.Text}：{time3 / 1000:0.000}秒，字符串长度：{stringBuilder.Length}";
            label6.Text = $"{label6.Text},使用StringBulider号耗时：{time3 / 1000:0.000}秒";
        }

        /// <summary>
        ///     使用stringBuffer进行字符串的拼接，线程安全,属于Java
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("StringBuffer这是Java 的语句，C#用不了！", "提示");
            return;
            progressBar4.Value = 0;
            var watch = new Stopwatch();
            watch.Start();

            var stringBuffer = new StringBuffer("16");
            for (var i = 0; i < 100000; i++)
            {
                //str = str + "学以致用，学而思之，知识迭代！";
                stringBuffer.append(testStr);
                progressBar4.Value++;
            }

            watch.Stop();
            time4 = watch.Elapsed.TotalMilliseconds;
            label4.Text = $"{label4.Text}：{time4 / 1000:0.000}秒，字符串长度：{stringBuffer.length()}";
            label6.Text = $"{label6.Text},使用StringBuffer号耗时：{time4 / 1000:0.000}秒";
        }
    }

    /* 字符串 sting， 不可修改值，线程安全，改变值，需要新建一个对象，将新的值赋给新对象，然后删除旧对象，占用大量了内存，对于频繁修改值的字符串消耗性能较多。
     1.每次修改都需要申请新的内存空间
     2.每次修改都需要复制原字符穿到新的空间
     3.每次修改都需要销毁原有空间
     */
    /* StingBuilder， 可修改值，线程不安全，改变值，直接修改本身，存储内存按16，32，64，128，256，512，1024，...，+=8000，+=8000，进行扩展，性能高。
     1.空间默认初始16个字符，32个byte
     2.超过空间大小，则申请一个相同大小的内存空间，16，32，64，128，256，512，1024，2048，4096，8192，16192，最大申请8000字符。
     3.提升效率，又不会浪费内存空间。
     4.每次扩容不复制字符串，将原空间与新空间链接起来。
     5.因为可以修改值属于线程不安全代码，需要添加锁，加锁或者解锁需要消耗性能。
     */
}