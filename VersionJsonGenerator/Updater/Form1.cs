using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        
        // 【优化】复用 HttpClient 实例
        private static HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };
        
        // 【新增】下载取消令牌
        private CancellationTokenSource _downloadCts;
        
        // 【新增】日志文件路径
        private readonly string _logFilePath = Path.Combine(Application.StartupPath, "log.txt");
        private const long MaxLogFileSize = 10 * 1024 * 1024; // 10MB
        private readonly object _logLock = new object(); // 日志写入锁

        public Form1()
        {
            InitializeComponent();
            SetUpdateUIVisible(false);
            
            // 【新增】初始化日志文件
            InitLogFile();
            
            LoadAllSettings();
            
            // 【新增】根据配置设置超时
            if (_settings?.DownloadTimeoutSeconds > 0)
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(_settings.DownloadTimeoutSeconds);
            }
            
            CheckGameIsRunning();
            _ = CheckRemoteVersionAsync(); //初始化后台检测更新
            
            // 窗体关闭时释放资源
            this.FormClosing += (s, e) => {
                _downloadCts?.Cancel();
                _downloadCts?.Dispose();
            };
        }

        /// <summary>
        /// 【新增】初始化日志文件
        /// </summary>
        private void InitLogFile()
        {
            try
            {
                // 检查日志文件大小
                if (File.Exists(_logFilePath))
                {
                    var fileInfo = new FileInfo(_logFilePath);
                    if (fileInfo.Length > MaxLogFileSize)
                    {
                        CleanLogFile();
                    }
                }
                else
                {
                    // 创建日志文件
                    File.WriteAllText(_logFilePath, $"========== 应用启动 {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==========\r\n");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化日志文件失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 【新增】清理日志文件（删除前99%的内容，保留最后1%）
        /// </summary>
        private void CleanLogFile()
        {
            try
            {
                lock (_logLock)
                {
                    // 读取所有日志
                    var allLines = File.ReadAllLines(_logFilePath);
                    int totalLines = allLines.Length;
                    
                    // 保留最后 1% 的内容
                    int keepLines = Math.Max(totalLines / 100, 100); // 至少保留100行
                    var linesToKeep = new string[Math.Min(keepLines, totalLines)];
                    
                    for (int i = 0; i < linesToKeep.Length; i++)
                    {
                        linesToKeep[i] = allLines[totalLines - linesToKeep.Length + i];
                    }
                    
                    // 写回文件
                    File.WriteAllLines(_logFilePath, linesToKeep);
                    
                    // 添加清理标记
                    File.AppendAllText(_logFilePath, $"\r\n========== 日志清理 {DateTime.Now:yyyy-MM-dd HH:mm:ss} 已删除前99%的旧日志 ==========\r\n");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清理日志文件失败：{ex.Message}");
            }
        }

        private void SetUpdateUIVisible(bool isShow)
        {
           UpdatePanel.Visible = isShow;
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
            btnClose.Enabled = running;
            _gameRunning = running;
        }
        #endregion

        #region 远程版本检测，无更新隐藏控件
        private async System.Threading.Tasks.Task CheckRemoteVersionAsync()
        {
            try
            {
                // 【优化】使用复用的 HttpClient
                string json = await _httpClient.GetStringAsync(_settings.RemoteVersionUrl);
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
                ShowMessage("连接更新服务器失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            form2.StartPosition = FormStartPosition.CenterParent;
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

        //【更新按钮】优化后的更新逻辑（含下载速度和剩余时间显示）
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_gameRunning)
            {
                StopGameProcess(_gameProcessName);
            }
            
            // 【新增】初始化取消令牌
            _downloadCts = new CancellationTokenSource();
            var token = _downloadCts.Token;
            
            btnUpdate.Enabled = false;
            btnStart.Enabled = false;
            btnClose.Enabled = false;
            lblProgress.Text = "0%";
            progressBar1.Value = 0;
            labelSpeed.Text = "计算中...";

            string localGameDir = Path.Combine(Application.StartupPath, _settings.GameName);

            try
            {
                var needDownloadList = new List<FileItem>();

                // 第一步：筛选需要下载的文件
                foreach (var fileItem in _remoteVersionInfo.files)
                {
                    if (token.IsCancellationRequested)
                    {
                        Log("更新已取消");
                        return;
                    }
                    
                    string localFilePath = Path.Combine(localGameDir, fileItem.path);
                    bool needDl = true;
                    if (File.Exists(localFilePath))
                    {
                        string localMd5 = GetFileMD5(localFilePath);
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
                    ShowMessage("已是最新版本！");
                    SetUpdateUIVisible(false);
                    btnUpdate.Enabled = true;
                    btnStart.Enabled = true;
                    CheckGameIsRunning();
                    return;
                }

                // 【新增】获取下载总大小
                Log("正在获取文件大小信息...");
                long totalBytes = await GetTotalDownloadSizeAsync(needDownloadList, token);
                Log($"总下载大小：{FormatSize(totalBytes)}");
                
                long downloadedBytes = 0;
                int totalFileCount = needDownloadList.Count;
                int finishedFileCount = 0;
                
                // 用于计算下载速度
                var speedWatcher = new SpeedWatcher();

                //第二步：逐个下载文件
                foreach (var fileItem in needDownloadList)
                {
                    if (token.IsCancellationRequested)
                    {
                        Log("更新已取消");
                        ShowMessage("更新已取消");
                        return;
                    }
                    
                    string localFilePath = Path.Combine(localGameDir, fileItem.path);
                    string dir = Path.GetDirectoryName(localFilePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    Log($"正在下载：{fileItem.path}");
                    
                    // 【修改】带进度回调的下载
                    int maxRetries = _settings?.MaxRetryCount > 0 ? _settings.MaxRetryCount : 3;
                    bool success = await DownloadWithRetryAsync(fileItem, localFilePath, token, maxRetries, 
                        (bytesRead, fileSize) => {
                            // 进度回调：更新下载速度和剩余时间
                            var info = speedWatcher.Update(downloadedBytes + bytesRead, totalBytes);
                            UpdateProgressUI(finishedFileCount, totalFileCount, downloadedBytes + bytesRead, totalBytes, info);
                        });
                    
                    if (!success)
                    {
                        throw new Exception($"下载失败：{fileItem.path}");
                    }
                    
                    //MD5校验
                    string newFileMd5 = GetFileMD5(localFilePath);
                    if (newFileMd5 != fileItem.md5)
                    {
                        File.Delete(localFilePath);
                        throw new Exception($"{fileItem.path} MD5校验失败，文件损坏，已删除");
                    }
                    
                    // 更新已下载字节数
                    downloadedBytes += new FileInfo(localFilePath).Length;
                    finishedFileCount++;
                    
                    // 最终更新进度
                    var finalInfo = speedWatcher.Update(downloadedBytes, totalBytes);
                    UpdateProgressUI(finishedFileCount, totalFileCount, downloadedBytes, totalBytes, finalInfo);
                    
                    Log($"完成：{fileItem.path}");
                }

                Log("✅全部更新完成！");
                ShowMessage("更新完成，可以启动程序！");
                SetUpdateUIVisible(false);
            }
            catch (OperationCanceledException)
            {
                Log("更新已取消");
                ShowMessage("更新已取消");
            }
            catch (Exception ex)
            {
                Log($"更新失败：{ex.Message}");
                ShowMessage("更新异常：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnUpdate.Enabled = true;
                btnStart.Enabled = true;
                _downloadCts?.Dispose();
                _downloadCts = null;
            }
        }

        /// <summary>
        /// 【新增】获取所有需要下载文件的总大小
        /// </summary>
        private async Task<long> GetTotalDownloadSizeAsync(List<FileItem> files, CancellationToken token)
        {
            long totalSize = 0;
            foreach (var file in files)
            {
                try
                {
                    // 使用 HEAD 请求获取文件大小
                    var request = new HttpRequestMessage(HttpMethod.Head, file.url);
                    var response = await _httpClient.SendAsync(request, token);
                    
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength.HasValue)
                    {
                        totalSize += response.Content.Headers.ContentLength.Value;
                    }
                }
                catch
                {
                    // 获取失败时忽略，后续下载时会知道实际大小
                }
            }
            return totalSize > 0 ? totalSize : -1; // -1 表示无法获取总大小
        }

        /// <summary>
        /// 【新增】更新进度UI（百分比、速度）
        /// </summary>
        private void UpdateProgressUI(int finishedFiles, int totalFiles, long downloadedBytes, long totalBytes, SpeedInfo info)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateProgressUI(finishedFiles, totalFiles, downloadedBytes, totalBytes, info)));
                return;
            }
            
            // 更新进度条和百分比
            if (totalBytes > 0)
            {
                int percent = (int)(downloadedBytes * 100.0 / totalBytes);
                progressBar1.Value = Math.Min(percent, 100);
                lblProgress.Text = $"{percent}% ({FormatSize(downloadedBytes)}/{FormatSize(totalBytes)})";
            }
            else
            {
                // 无法获取总大小时，按文件数量显示
                int percent = (int)(finishedFiles * 100.0 / totalFiles);
                progressBar1.Value = Math.Min(percent, 100);
                lblProgress.Text = $"{percent}%";
            }
            
            // 更新下载速度
            labelSpeed.Text = FormatSpeed(info.Speed);
        }

        /// <summary>
        /// 【新增】格式化文件大小
        /// </summary>
        private static string FormatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unitIndex = 0;
            
            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }
            
            return $"{size:0.##} {units[unitIndex]}";
        }

        /// <summary>
        /// 【新增】格式化下载速度
        /// </summary>
        private static string FormatSpeed(double bytesPerSecond)
        {
            if (bytesPerSecond <= 0) return "0 B/s";
            
            string[] units = { "B/s", "KB/s", "MB/s", "GB/s" };
            double speed = bytesPerSecond;
            int unitIndex = 0;
            
            while (speed >= 1024 && unitIndex < units.Length - 1)
            {
                speed /= 1024;
                unitIndex++;
            }
            
            return $"{speed:0.##} {units[unitIndex]}";
        }

        /// <summary>
        /// 【辅助类】速度计算器
        /// </summary>
        private class SpeedWatcher
        {
            private readonly Stopwatch _stopwatch = new Stopwatch();
            private double _currentSpeed = 0;
            private readonly Queue<(long ticks, long bytes)> _history = new Queue<(long, long)>();
            private const int HistorySize = 10; // 保留最近10个采样点

            public SpeedWatcher()
            {
                _stopwatch.Start();
            }

            public SpeedInfo Update(long currentBytes, long totalBytes)
            {
                var now = _stopwatch.ElapsedTicks;
                
                // 记录历史
                _history.Enqueue((now, currentBytes));
                
                // 只保留最近的采样点
                while (_history.Count > HistorySize)
                {
                    _history.Dequeue();
                }
                
                // 计算速度
                if (_history.Count >= 2)
                {
                    var oldest = _history.Peek();
                    var elapsedSeconds = (now - oldest.ticks) / (double)Stopwatch.Frequency;
                    var bytesDiff = currentBytes - oldest.bytes;
                    
                    if (elapsedSeconds > 0)
                    {
                        _currentSpeed = bytesDiff / elapsedSeconds;
                    }
                }
                
                return new SpeedInfo
                {
                    Speed = _currentSpeed
                };
            }
        }

        /// <summary>
        /// 【辅助结构】速度信息
        /// </summary>
        private struct SpeedInfo
        {
            public double Speed; // 字节/秒
        }

        /// <summary>
        /// 【修改】带重试机制的下载方法（支持进度回调）
        /// </summary>
        /// <param name="progressCallback">进度回调 (已读取字节, 文件总大小)</param>
        private async Task<bool> DownloadWithRetryAsync(FileItem fileItem, string localFilePath, 
            CancellationToken token, int maxRetries = 3, Action<long, long> progressCallback = null)
        {
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    if (retry > 0)
                    {
                        Log($"重试下载 ({retry}/{maxRetries})：{fileItem.path}");
                        await Task.Delay(1000, token); // 重试前等待1秒
                    }
                    
                    var resp = await _httpClient.GetAsync(fileItem.url, HttpCompletionOption.ResponseHeadersRead, token);
                    resp.EnsureSuccessStatusCode();

                    // 获取文件总大小
                    long totalBytes = resp.Content.Headers.ContentLength ?? -1;
                    long bytesRead = 0;

                    // 【优化】使用 using 确保流释放
                    using (var stream = await resp.Content.ReadAsStreamAsync())
                    using (var fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        byte[] buffer = new byte[8192];
                        int readLen;
                        
                        // 【新增】计时器，用于限制进度更新频率
                        var lastUpdateTime = DateTime.Now;
                        var updateInterval = TimeSpan.FromMilliseconds(100); // 每100ms更新一次
                        
                        while ((readLen = await stream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                        {
                            await fs.WriteAsync(buffer, 0, readLen, token);
                            bytesRead += readLen;
                            
                            // 【新增】进度回调（限制频率避免UI卡顿）
                            if (progressCallback != null)
                            {
                                var now = DateTime.Now;
                                if (now - lastUpdateTime >= updateInterval || bytesRead == totalBytes)
                                {
                                    progressCallback(bytesRead, totalBytes);
                                    lastUpdateTime = now;
                                }
                            }
                        }
                        
                        // 最终回调
                        progressCallback?.Invoke(bytesRead, totalBytes);
                    }

                    return true; // 下载成功
                }
                catch (OperationCanceledException)
                {
                    throw; // 取消操作，向上抛出
                }
                catch (Exception ex)
                {
                    Log($"下载失败：{ex.Message}");
                    // 删除可能的部分下载文件
                    if (File.Exists(localFilePath))
                    {
                        try { File.Delete(localFilePath); } catch { }
                    }
                    
                    if (retry == maxRetries - 1)
                    {
                        return false; // 重试次数用尽
                    }
                }
            }
            return false;
        }


        //日志输出函数（同时写入文件）
        private void Log(string msg, bool writeToFile = true)
        {
            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg}";
            
            // 【优化】启用日志显示
            if (txtLog != null)
            {
                if (txtLog.InvokeRequired)
                {
                    txtLog.Invoke(new Action(() => Log(msg, writeToFile)));
                    return;
                }
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {msg}\r\n");
                txtLog.ScrollToCaret();
            }
            
            // 【新增】写入日志文件
            if (writeToFile)
            {
                WriteLogToFile(logLine);
            }
        }

        /// <summary>
        /// 【新增】写入日志到文件
        /// </summary>
        private void WriteLogToFile(string logLine)
        {
            try
            {
                lock (_logLock)
                {
                    File.AppendAllText(_logFilePath, logLine + "\r\n");
                    
                    // 每次写入后检查文件大小
                    var fileInfo = new FileInfo(_logFilePath);
                    if (fileInfo.Length > MaxLogFileSize)
                    {
                        CleanLogFile();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"写入日志文件失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 【新增】显示消息框并记录到日志
        /// </summary>
        private void ShowMessage(string message, string title = "提示", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            // 先写入日志
            Log($"[消息框-{title}] {message}");
            
            // 再显示MessageBox
            MessageBox.Show(this, message, title, buttons, icon);
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
            using (var md5 = System.Security.Cryptography.MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }


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
