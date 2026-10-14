using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VersionJsonGenerator
{
    public partial class Form1 : Form
    {
        //黑名单（默认值，可从配置加载）
        private HashSet<string> _blackDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".vs", "bin", "obj", "Logs", "Temp", "__pycache__"
        };
        private HashSet<string> _blackFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "version.json"
        };

        private readonly string _configPath = Path.Combine(Application.StartupPath, "config.json");
        private List<string> _allScanFiles = new List<string>();
        private AppConfig _config;

        // 【新增】日志文件路径
        private readonly string _logFilePath = Path.Combine(Application.StartupPath, "log.txt");
        private const long MaxLogFileSize = 100 * 1024 * 1024; // 100MB
        private readonly object _logLock = new object(); // 日志写入锁

        public Form1()
        {
            InitializeComponent();
            // 【新增】初始化日志文件
            InitLogFile();
            LoadConfig();
            txtPort.Text = "8080"; //默认端口
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        #region 配置读写
        private void SaveConfig()
        {
            var cfg = _config ?? new AppConfig();
            cfg.RootPath = txtRootPath.Text.Trim();
            cfg.ServerBaseUrl = txtServerBaseUrl.Text.Trim();
            cfg.Port = txtPort.Text.Trim();
            
            string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            File.WriteAllText(_configPath, json);
            Log("✅ 配置已保存");
        }

        private void LoadConfig()
        {
            if (!File.Exists(_configPath)) 
            {
                _config = new AppConfig();
                return;
            }
            
            try
            {
                string json = File.ReadAllText(_configPath);
                _config = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
                
                txtRootPath.Text = _config.RootPath ?? "";
                txtServerBaseUrl.Text = _config.ServerBaseUrl ?? "";
                txtPort.Text = _config.Port ?? "8080";
                
                // 【新增】从配置加载黑名单
                if (_config.ExcludeDirs != null && _config.ExcludeDirs.Count > 0)
                {
                    _blackDirs = new HashSet<string>(_config.ExcludeDirs, StringComparer.OrdinalIgnoreCase);
                }
                if (_config.ExcludeFiles != null && _config.ExcludeFiles.Count > 0)
                {
                    _blackFiles = new HashSet<string>(_config.ExcludeFiles, StringComparer.OrdinalIgnoreCase);
                }
                
                btnLoadOld_Click(this, null);
                Log("✅ 已加载上次保存的配置");
            }
            catch (Exception ex)
            {
                Log($"读取配置失败：{ex.Message}");
                _config = new AppConfig();
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        /// <summary>
        /// 点击右上角关闭按钮“X”的触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }
        #endregion

        #region 版本加载
        private void btnLoadOld_Click(object sender, EventArgs e)
        {
            string root = txtRootPath.Text.Trim();
            if (!Directory.Exists(root))
            {
                ShowMessage("根目录不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string verFile = Path.Combine(root, "version.json");
            if (!File.Exists(verFile))
            {
                txtOldVersion.Text = "0.0.0";
                txtNewVersion.Text = AutoIncVersion("0.0.0");
                Log("未找到version.json，旧版本：0.0.0");
                return;
            }
            try
            {
                string json = File.ReadAllText(verFile);
                var info = JsonConvert.DeserializeObject<VersionInfo>(json);
                txtOldVersion.Text = info.version;
                txtNewVersion.Text = AutoIncVersion(info.version);
                Log($"读取成功，旧版本：{info.version}，自动推荐新版本：{txtNewVersion.Text}");
            }
            catch (Exception ex)
            {
                Log($"读取version.json失败：{ex.Message}");
                txtOldVersion.Text = "0.0.0";
                txtNewVersion.Text = AutoIncVersion("0.0.0");
            }
        }

        private string AutoIncVersion(string verStr)
        {
            var parts = verStr.Split('.');
            int major = 0, minor = 0, patch = 0;
            if (parts.Length >= 1) int.TryParse(parts[0], out major);
            if (parts.Length >= 2) int.TryParse(parts[1], out minor);
            if (parts.Length >= 3) int.TryParse(parts[2], out patch);
            patch++;
            return $"{major}.{minor}.{patch}";
        }

        /// <summary>
        /// 验证版本号格式（x.x.x 或 x.x）
        /// </summary>
        private bool IsValidVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version)) return false;
            var parts = version.Split('.');
            if (parts.Length < 2 || parts.Length > 4) return false;
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num) || num < 0)
                    return false;
            }
            return true;
        }
        #endregion

        #region 生成version.json
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string rootPath = txtRootPath.Text.Trim();
            string baseUrl = txtServerBaseUrl.Text.Trim();
            string newVer = txtNewVersion.Text.Trim();

            if (!Directory.Exists(rootPath))
            {
                ShowMessage("发布根目录不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(baseUrl))
            {
                ShowMessage("ServerBaseUrl不能为空，示例：http://192.168.1.105:8080/", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(newVer))
            {
                ShowMessage("新版本号不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // 【新增】验证版本号格式
            if (!IsValidVersion(newVer))
            {
                ShowMessage("版本号格式错误！\n正确格式：x.x.x（如 1.0.0）\n每个部分必须是0或正整数", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            _allScanFiles.Clear();
            CollectAllFiles(rootPath, rootPath);

            VersionInfo verInfo = new VersionInfo();
            verInfo.version = newVer;
            verInfo.files = new List<FileItem>();

            Log($"开始扫描，总文件数量：{_allScanFiles.Count}");
            progressBar1.Maximum = _allScanFiles.Count;
            progressBar1.Value = 0;

            for (int i = 0; i < _allScanFiles.Count; i++)
            {
                var file = _allScanFiles[i];
                string relPath = file.Substring(rootPath.Length + 1).Replace("\\", "/");
                string md5 = GetFileMD5(file);
                verInfo.files.Add(new FileItem
                {
                    path = relPath,
                    md5 = md5,
                    url = baseUrl + relPath
                });
                Log($"扫描：{relPath} MD5:{md5}");
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            string outJsonPath = Path.Combine(rootPath, "version.json");
            string jsonText = JsonConvert.SerializeObject(verInfo, Formatting.Indented);
            File.WriteAllText(outJsonPath, jsonText);

            Log($"✅ 生成完成！共扫描 {verInfo.files.Count} 个文件");
            Log($"输出：{outJsonPath}");
            ShowMessage("version.json 生成成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void CollectAllFiles(string scanDir, string rootDir)
        {
            foreach (var file in Directory.GetFiles(scanDir))
            {
                string fileName = Path.GetFileName(file);
                if (_blackFiles.Contains(fileName)) continue;
                _allScanFiles.Add(file);
            }
            foreach (var dir in Directory.GetDirectories(scanDir))
            {
                string dirName = Path.GetFileName(dir);
                if (_blackDirs.Contains(dirName)) continue;
                CollectAllFiles(dir, rootDir);
            }
        }

        static string GetFileMD5(string filePath)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        #endregion

        #region 一键启动HTTP服务

        private HttpListener _httpListener;
        private Task _serverTask;

        private void btnStartHttpServer_Click(object sender, EventArgs e)
        {
            string root = txtRootPath.Text.Trim();
            string portStr = txtPort.Text.Trim();
            if (!Directory.Exists(root))
            {
                ShowMessage("请先选择发布根目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(portStr, out int port))
            {
                ShowMessage("端口号必须是数字！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_httpListener != null && _httpListener.IsListening)
            {
                ShowMessage("服务已经在运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _httpListener = new HttpListener();
            string prefix = $"http://+:{port}/";
            _httpListener.Prefixes.Add(prefix);
            _httpListener.Start();

            string localIp = GetLocalIPv4();
            string url = $"http://{localIp}:{port}/";
            txtServerBaseUrl.Text = url;

            _serverTask = Task.Run(async () =>
            {
                while (_httpListener.IsListening)
                {
                    try
                    {
                        var ctx = await _httpListener.GetContextAsync();
                        await HandleRequest(ctx, root);
                    }
                    catch
                    {
                    }
                }
            });

            Log($"✅ .NET内置Http服务启动成功！");
            Log($"内网地址：{url}");
            Log($"version.json地址：{url}version.json");
        }

        //处理http请求，返回静态文件
        private async Task HandleRequest(HttpListenerContext ctx, string rootDir)
        {
            var req = ctx.Request;
            var resp = ctx.Response;
            try
            {
                string rawPath = Uri.UnescapeDataString(req.Url.AbsolutePath);
                rawPath = rawPath.TrimStart('/').Replace("/", "\\");
                
                // 【安全】防止路径遍历攻击
                if (rawPath.Contains("..") || Path.IsPathRooted(rawPath))
                {
                    resp.StatusCode = 403; // Forbidden
                    return;
                }
                
                string filePath = Path.GetFullPath(Path.Combine(rootDir, rawPath));
                
                // 【安全】验证文件路径在根目录内
                string rootDirFullPath = Path.GetFullPath(rootDir);
                if (!filePath.StartsWith(rootDirFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    resp.StatusCode = 403; // Forbidden
                    return;
                }

                if (!File.Exists(filePath))
                {
                    resp.StatusCode = 404;
                    return;
                }

                // 替换 File.ReadAllBytesAsync，兼容 .NET Framework
                byte[] data;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                {
                    data = new byte[fs.Length];
                    await fs.ReadAsync(data, 0, data.Length);
                }

                resp.ContentLength64 = data.Length;
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                resp.StatusCode = 500;
            }
            finally
            {
                resp.OutputStream.Close();
                resp.Close();
            }
        }


        //停止服务按钮（你可以再加一个stop按钮）
        private void btnStopService_Click(object sender, EventArgs e)
        {
            if (_httpListener != null && _httpListener.IsListening)
            {
                _httpListener.Stop();
                _httpListener.Close();
                Log("✅ HTTP服务已停止");
            }
        }

        // 获取本机内网IPv4（排除127.0.0.1）
        private string GetLocalIPv4()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    string ipStr = ip.ToString();
                    if (!ipStr.StartsWith("127."))
                    {
                        return ipStr;
                    }
                }
            }
            return "127.0.0.1";
        }
        #endregion

        #region UI辅助
        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            // 如果txtRootPath有有效目录，就以此作为初始打开位置
            string currentPath = txtRootPath.Text.Trim();
            if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
            {
                fbd.SelectedPath = currentPath;
            }
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtRootPath.Text = fbd.SelectedPath;
            }
        }

        #endregion


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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void btnGeneratePush_Click(object sender, EventArgs e)
        {
            QiniuUploaderPush qiniuUploaderPush = new QiniuUploaderPush(txtNewVersion.Text, txtRootPath.Text);
            qiniuUploaderPush.Push();
        }
    }

    public class AppConfig
    {
        public string RootPath { get; set; }
        public string ServerBaseUrl { get; set; }
        public string Port { get; set; }
        
        // 【新增】可配置的黑名单
        public List<string> ExcludeDirs { get; set; } = new List<string> 
        { 
            ".git", ".vs", "bin", "obj", "Logs", "Temp", "__pycache__" 
        };
        public List<string> ExcludeFiles { get; set; } = new List<string> 
        { 
            "version.json" 
        };
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
