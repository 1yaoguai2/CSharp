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
        //黑名单
        private readonly HashSet<string> _blackDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".vs", "bin", "obj", "Logs", "Temp", "__pycache__"
        };
        private readonly HashSet<string> _blackFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "version.json"
        };

        private readonly string _configPath = Path.Combine(Application.StartupPath, "config.json");
        private List<string> _allScanFiles = new List<string>();

        public Form1()
        {
            InitializeComponent();
            LoadConfig();
            txtPort.Text = "8080"; //默认端口
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        #region 配置读写
        private void SaveConfig()
        {
            var cfg = new AppConfig
            {
                RootPath = txtRootPath.Text.Trim(),
                ServerBaseUrl = txtServerBaseUrl.Text.Trim(),
                Port = txtPort.Text.Trim()
            };
            string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            File.WriteAllText(_configPath, json);
            Log("✅ 配置已保存");
        }

        private void LoadConfig()
        {
            if (!File.Exists(_configPath)) return;
            try
            {
                string json = File.ReadAllText(_configPath);
                var cfg = JsonConvert.DeserializeObject<AppConfig>(json);
                txtRootPath.Text = cfg.RootPath;
                txtServerBaseUrl.Text = cfg.ServerBaseUrl;
                txtPort.Text = cfg.Port;
                btnLoadOld_Click(this,null);
                Log("✅ 已加载上次保存的配置");
            }
            catch (Exception ex)
            {
                Log($"读取配置失败：{ex.Message}");
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
                MessageBox.Show(this,"根目录不存在！");
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
        #endregion

        #region 生成version.json
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string rootPath = txtRootPath.Text.Trim();
            string baseUrl = txtServerBaseUrl.Text.Trim();
            string newVer = txtNewVersion.Text.Trim();

            if (!Directory.Exists(rootPath))
            {
                MessageBox.Show(this, "发布根目录不存在！");
                return;
            }
            if (string.IsNullOrEmpty(baseUrl))
            {
                MessageBox.Show(this,"ServerBaseUrl不能为空，示例：http://192.168.1.105:8080/");
                return;
            }
            if (string.IsNullOrEmpty(newVer))
            {
                MessageBox.Show(this,"新版本号不能为空");
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
            MessageBox.Show(this,"version.json 生成成功！");
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
            var md5 = System.Security.Cryptography.MD5.Create();
            var stream = File.OpenRead(filePath);
            byte[] hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
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
                MessageBox.Show(this, "请先选择发布根目录！");
                return;
            }
            if (!int.TryParse(portStr, out int port))
            {
                MessageBox.Show(this, "端口号必须是数字！");
                return;
            }

            if (_httpListener != null && _httpListener.IsListening)
            {
                MessageBox.Show(this, "服务已经在运行！");
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
                //string rawPath = req.Url.AbsolutePath.TrimStart('/').Replace("/", "\\");
                string rawPath = Uri.UnescapeDataString(req.Url.AbsolutePath);
                rawPath = rawPath.TrimStart('/').Replace("/", "\\");
                string filePath = Path.Combine(rootDir, rawPath);


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
        void Log(string msg)
        {
            txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {msg}\r\n");
            txtLog.ScrollToCaret();
        }

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

    }

    public class AppConfig
    {
        public string RootPath { get; set; }
        public string ServerBaseUrl { get; set; }
        public string Port { get; set; }
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
