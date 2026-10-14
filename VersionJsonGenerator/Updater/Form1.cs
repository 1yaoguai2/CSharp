using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using Updater.Common;

namespace Updater
{
    public partial class Form1 : Form
    {
        private AppSettings _settings;
        private readonly string _settingJsonPath = Path.Combine(Application.StartupPath, "appSetting.json");
        private readonly string _iconBinPath = Path.Combine(Application.StartupPath, "icon.bin");
        private readonly string _bgBinPath = Path.Combine(Application.StartupPath, "bg.bin");
        private VersionInfo _remoteVersionInfo;
        private string _gameProcessName = "Game"; //你的Unity程序进程名（不带.exe）

        private bool _gameRunning = false;

        public Form1()
        {
            InitializeComponent();
            SetUpdateUIVisible(false);
            LoadAllSettings();
            CheckGameIsRunning();
            _ = CheckRemoteVersionAsync(); //初始化后台检测更新
        }

        private void SetUpdateUIVisible(bool isShow)
        {
            btnUpdate.Visible = isShow;
            progressBar1.Visible = isShow;
            lblProgress.Visible = isShow;
        }

        #region 加载配置+图片
        private void LoadAllSettings()
        {
            //加载json配置
            if (File.Exists(_settingJsonPath))
            {
                string json = File.ReadAllText(_settingJsonPath);
                _settings = JsonConvert.DeserializeObject<AppSettings>(json);
            }
            else
            {
                //默认配置
                _settings = new AppSettings
                {
                    TitleText = "XXXXX数字孪生",
                    RemoteVersionUrl = "http://127.0.0.1:8080/version.json",
                    ShowIcon = true,
                    IconBinFileName = "icon.bin",
                    BackgroundBinFileName = "bg.bin",
                    GameName = "Game"
                };
            }
            //设置窗口标题文字
            this.Text = _settings.TitleText;
            labelTitle.Text = _settings.TitleText;
            _gameProcessName = _settings.GameName;

            //加载背景图
            var bgImg = ImageBinHelper.LoadImageFromBin(_bgBinPath);
            if (bgImg != null)
            {
                this.BackgroundImage = bgImg;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            //加载左上角图标
            if (_settings.ShowIcon)
            {
                var icoImg = ImageBinHelper.LoadImageFromBin(_iconBinPath);
                if (icoImg != null)
                {
                    pictureBoxIcon.Image = icoImg;
                    pictureBoxIcon.Visible = true;
                }
            }
            else
            {
                pictureBoxIcon.Visible = false;
            }
        }
        #endregion

        #region 检测游戏进程，控制【启动】按钮可用状态
        private void CheckGameIsRunning()
        {
            bool running = Process.GetProcessesByName(_gameProcessName).Length > 0;
            btnStart.Enabled = !running;
            _gameRunning = running;
        }
        #endregion

        #region 远程版本检测，无更新隐藏控件
        private async System.Threading.Tasks.Task CheckRemoteVersionAsync()
        {
            try
            {
                var httpClient = new System.Net.Http.HttpClient();
                string json = await httpClient.GetStringAsync(_settings.RemoteVersionUrl);
                _remoteVersionInfo = JsonConvert.DeserializeObject<VersionInfo>(json);

                //【这里写你本地版本读取逻辑，对比版本号】
                bool hasUpdate = CheckLocalNeedUpdate(_remoteVersionInfo);
                if (hasUpdate) 
                {
                    //存在跟新
                    SetUpdateUIVisible(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "连接更新服务器失败：" + ex.Message);
            }
        }

        //简单示例：MD5比对判断是否存在更新（你可以复用之前的MD5函数）
        private bool CheckLocalNeedUpdate(VersionInfo remoteVer)
        {
            string localGameDir = Path.Combine(Application.StartupPath, _settings.GameName);
            foreach (var file in remoteVer.files)
            {
                string localFile = Path.Combine(localGameDir, file.path);
                if (!File.Exists(localFile)) return true;
                string localMd5 = GetFileMD5(localFile);
                if (localMd5 != file.md5) return true;
            }
            return false;
        }
        #endregion

        //【设置按钮】打开Form2设置界面
        private void btnSetting_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(_settings);
            if (form2.ShowDialog() == DialogResult.OK)
            {
                //保存新配置并重新加载界面
                SaveSettings(form2.NewSettings);
                LoadAllSettings();
            }
        }
        private void SaveSettings(AppSettings set)
        {
            string json = JsonConvert.SerializeObject(set, Formatting.Indented);
            File.WriteAllText(_settingJsonPath, json);
            _settings = set;
        }

        //【更新按钮】原有更新逻辑
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_gameRunning)
            {
                StopGameProcess(_gameProcessName);
            }
            btnUpdate.Enabled = false;
            btnStart.Enabled = false;
            btnClose.Enabled = false;
            lblProgress.Text = "0%";
            progressBar1.Value = 0;

            string localGameDir = Path.Combine(Application.StartupPath, _settings.GameName);

            try
            {
                var httpClient = new System.Net.Http.HttpClient();
                var needDownloadList = new List<FileItem>();

                // 第一步：筛选需要下载的文件
                foreach (var fileItem in _remoteVersionInfo.files)
                {
                    string localFilePath = Path.Combine(localGameDir, fileItem.path);
                    bool needDl = true;
                    if (File.Exists(localFilePath))
                    {
                        string localMd5 = GetFileMD5(localFilePath);
                        GC.Collect(); //强制回收，释放文件锁
                        GC.WaitForPendingFinalizers();
                        Thread.Sleep(50); //微小延时，确保文件锁释放
                        if (localMd5 == fileItem.md5)
                        {
                            Log($"跳过：{fileItem.path} MD5一致");
                            needDl = false;
                        }
                    }
                    if (needDl)
                    {
                        needDownloadList.Add(fileItem);
                    }
                }

                if (needDownloadList.Count == 0)
                {
                    MessageBox.Show(this, "已是最新版本！");
                    SetUpdateUIVisible(false);
                    btnUpdate.Enabled = true;
                    btnStart.Enabled = true;
                    CheckGameIsRunning();
                    return;
                }

                int totalFileCount = needDownloadList.Count;
                int finishedFileCount = 0;

                //第二步：逐个下载文件
                foreach (var fileItem in needDownloadList)
                {
                    string localFilePath = Path.Combine(localGameDir, fileItem.path);
                    string dir = Path.GetDirectoryName(localFilePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    Log($"正在下载：{fileItem.path}");
                    var resp = await httpClient.GetAsync(fileItem.url, HttpCompletionOption.ResponseHeadersRead);
                    resp.EnsureSuccessStatusCode();

                    var stream = await resp.Content.ReadAsStreamAsync();
                    var fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);

                    byte[] buffer = new byte[4096];
                    int readLen;
                    while ((readLen = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fs.WriteAsync(buffer, 0, readLen);
                    }
       

                    fs.Close();
                    fs.Dispose();  //强制释放
                    stream.Close();
                    stream.Dispose();
                    System.Threading.Thread.Sleep(50); //微小延时，释放文件锁

                    //MD5校验
                    string newFileMd5 = GetFileMD5(localFilePath);
                    if (newFileMd5 != fileItem.md5)
                    {
                        File.Delete(localFilePath);
                        throw new Exception($"{fileItem.path} MD5校验失败，文件损坏，已删除");
                    }
                    finishedFileCount++;
                    // 按文件数量计算进度
                    int percent = (int)(finishedFileCount * 100.0 / totalFileCount);
                    progressBar1.Invoke(new Action(() =>
                    {
                        progressBar1.Value = percent;
                        lblProgress.Text = $"{percent}%";
                    }));
                    Log($"完成：{fileItem.path}");
                }

                Log("✅全部更新完成！");
                MessageBox.Show(this, "更新完成，可以启动程序！");
                SetUpdateUIVisible(false);
            }
            catch (Exception ex)
            {
                Log($"更新失败：{ex.Message}");
                MessageBox.Show(this, "更新异常：" + ex.Message);
            }
            finally
            {
                btnUpdate.Enabled = true;
                btnStart.Enabled = true;
                //CheckGameIsRunning();
            }
        }


        //日志输出函数（放在Form1里面）
        private void Log(string msg)
        {
            //如果你有日志文本框txtLog
            //txtLog.Invoke(new Action(()=> txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {msg}\r\n")));
        }


        //【启动按钮】启动游戏
        private void btnStart_Click(object sender, EventArgs e)
        {
            string exePath = Path.Combine(Application.StartupPath, _settings.GameName, $"{_settings.GameName}.exe");
            if (File.Exists(exePath))
            {
                Process.Start(exePath);
            }
            this.Close();
        }

        //【关闭按钮】
        private void btnClose_Click(object sender, EventArgs e)
        {
            StopGameProcess(_gameProcessName);
            btnClose.Enabled = false;
            btnStart.Enabled = true;
        }

        //MD5函数，和版本生成器保持一致
        public static string GetFileMD5(string filePath)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var stream = File.OpenRead(filePath);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    Form2 f2 = new Form2();
        //    f2.StartPosition = FormStartPosition.CenterParent;
        //    f2.ShowDialog(this);   // 非模态打开（可以操作其他窗口）
        //}

        /// <summary>
        /// 根据exe文件名（不带.exe）查找并关闭进程
        /// </summary>
        /// <param name="exeName">例如：CPK  不要写 CPK.exe</param>
        /// <returns>true=成功全部关闭</returns>
        private bool StopGameProcess(string exeName)
        {
            bool allKilled = true;
            Process[] processes = Process.GetProcessesByName(exeName);
            foreach (var p in processes)
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(1500); // 等待最多1.5秒让进程退出
                }
                catch
                {
                    allKilled = false;
                }
            }
            _gameRunning = false;
            return allKilled;
        }


        public class VersionInfo
        {
            public string version;
            public List<FileItem> files;
        }
        public class FileItem
        {
            public string path;
            public string md5;
            public string url;
        }

    }
}
