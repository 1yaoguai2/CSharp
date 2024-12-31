using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace SerializeStudy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text)) throw new Exception("信息不能为空！");
                Person person = new Person()
                {
                    Name = textBox1.Text,
                    Sex = textBox2.Text,
                    Age = int.Parse(textBox3.Text),
                    Pic = pictureBox1.Image
                };

                //序列化对象
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, person.Name + ".db");
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter(); //创建二进制序列化器
                    binaryFormatter.Serialize(fs, person);  //将对象序列化到流中
                }
                MessageBox.Show("序列化成功！", "友情提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show("序列化二进制文件报错出错！" + ex.Message, "友情提示");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory; //设置初始目录
            openFileDialog.Filter = "PNG图像(*.png)|*.png | JPG图像(*.jpg)|*.jpg"; //文件筛选器

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(openFileDialog.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;  //目录
            openDialog.Multiselect = false;
            openDialog.Filter = "Person对象(*.db)|*.db";  //文件筛选器

            if (openDialog.ShowDialog() == DialogResult.OK && openDialog.FileName != "")
            {
                string filePath = openDialog.FileName;
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        Person p = (Person)binaryFormatter.Deserialize(fs);
                        textBox1.Text = p.Name;
                        textBox2.Text = p.Sex;
                        textBox3.Text = p.Age.ToString();
                        pictureBox1.Image = p.Pic;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("反序列化失败！" + ex.Message, "友情提示");
                    }
                }
            }
        }
    }


    [Serializable]
    internal class Person
    {
        public string Name { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public Image Pic { get; set; }
    }


}
