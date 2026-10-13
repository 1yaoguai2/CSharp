using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Updater.Common;

namespace Updater
{
    public partial class Form2 : Form
    {
        public AppSettings NewSettings { get; private set; }
        private readonly string _iconBinPath = Path.Combine(Application.StartupPath, "icon.bin");
        private readonly string _bgBinPath = Path.Combine(Application.StartupPath, "bg.bin");

        public Form2(AppSettings oldSet)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            NewSettings = new AppSettings
            {
                TitleText = oldSet.TitleText,
                RemoteVersionUrl = oldSet.RemoteVersionUrl,
                ShowIcon = oldSet.ShowIcon,
                IconBinFileName = oldSet.IconBinFileName,
                BackgroundBinFileName = oldSet.BackgroundBinFileName,
                GameName = oldSet.GameName
            };
            //初始化设置框UI
            txtTitle.Text = NewSettings.TitleText;
            txtRemoteUrl.Text = NewSettings.RemoteVersionUrl;
            txtGameName.Text = NewSettings.GameName;
            chkShowIcon.Checked = NewSettings.ShowIcon;
        }


        //选择图标图片
        private void btnSelectIcon_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图片文件|*.png;*.jpg;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(ofd.FileName);
                ImageBinHelper.SaveImageToBin(img, _iconBinPath);
                picIconPreview.Image = img;
            }
        }
        //选择背景图片
        private void btnSelectBg_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图片文件|*.png;*.jpg;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(ofd.FileName);
                ImageBinHelper.SaveImageToBin(img, _bgBinPath);
                picBgPreview.Image = img;
            }
        }

        //保存+退出设置
        private void btnExitSetting_Click(object sender, EventArgs e)
        {
            NewSettings.TitleText = txtTitle.Text.Trim();
            NewSettings.RemoteVersionUrl = txtRemoteUrl.Text.Trim();
            NewSettings.ShowIcon = chkShowIcon.Checked;
            NewSettings.GameName = txtGameName.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
