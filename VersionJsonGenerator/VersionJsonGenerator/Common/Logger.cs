using System;
using System.IO;

namespace VersionJsonGenerator.Common
{
    /// <summary>
    /// 全局日志管理器（单例）
    ///
    /// 功能：
    ///   1. 将日志写入 log.txt
    ///   2. 日志文件超过指定大小后自动清理（删除前 99%，保留最后 1%）
    ///   3. 通过 OnLog 事件通知界面实时显示
    ///   4. 线程安全（内部加锁）
    ///
    /// 用法：
    ///   Logger.Instance.Initialize();               // 程序启动时初始化一次
    ///   Logger.Instance.OnLog += msg =&gt; { ... };    // 界面订阅以显示日志
    ///   Logger.Instance.Log("消息");                 // 任意窗口/类记录日志
    /// </summary>
    public sealed class Logger
    {
        private static readonly Lazy<Logger> _instance =
            new Lazy<Logger>(() => new Logger(), true);

        /// <summary>单例访问点</summary>
        public static Logger Instance => _instance.Value;

        private readonly object _lock = new object();
        private string _logFilePath;
        private long _maxFileSize = DefaultMaxFileSize;

        private const long DefaultMaxFileSize = 100 * 1024 * 1024; // 100MB

        /// <summary>
        /// 日志写入通知。参数为不含时间戳的原始消息，界面可订阅以实时显示。
        /// 注意：可能在非UI线程触发，订阅方需自行处理 Invoke。
        /// </summary>
        public event Action<string> OnLog;

        private Logger() { }

        /// <summary>日志文件完整路径（默认：程序目录\log.txt）</summary>
        public string LogFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
                }
                return _logFilePath;
            }
        }

        /// <summary>当前日志文件大小上限（字节）</summary>
        public long MaxFileSize => _maxFileSize;

        /// <summary>
        /// 初始化日志（建议在程序入口或首个窗口构造时调用一次）
        /// </summary>
        /// <param name="logFilePath">日志文件路径，默认 程序目录\log.txt</param>
        /// <param name="maxFileSize">日志文件大小上限（字节），默认 100MB</param>
        public void Initialize(string logFilePath = null, long maxFileSize = DefaultMaxFileSize)
        {
            lock (_lock)
            {
                _logFilePath = string.IsNullOrWhiteSpace(logFilePath)
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt")
                    : logFilePath;
                _maxFileSize = maxFileSize > 0 ? maxFileSize : DefaultMaxFileSize;
            }
            EnsureLogFile();
        }

        /// <summary>
        /// 记录一条日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="writeToFile">是否写入文件（默认 true）</param>
        public void Log(string message, bool writeToFile = true)
        {
            // 1. 通知界面（异常不影响主流程）
            try
            {
                OnLog?.Invoke(message);
            }
            catch
            {
                // 忽略界面回调异常
            }

            // 2. 写入文件
            if (writeToFile)
            {
                WriteToFile($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
            }
        }

        /// <summary>
        /// 手动清理日志文件（删除前 99%，保留最后 1%，至少 100 行）
        /// </summary>
        public void CleanLogFile()
        {
            try
            {
                lock (_lock)
                {
                    string path = LogFilePath;
                    if (!File.Exists(path)) return;

                    var allLines = File.ReadAllLines(path);
                    int totalLines = allLines.Length;

                    // 保留最后 1%，至少保留 100 行
                    int keepLines = Math.Max(totalLines / 100, 100);
                    int startIndex = Math.Max(totalLines - keepLines, 0);

                    var linesToKeep = new string[totalLines - startIndex];
                    Array.Copy(allLines, startIndex, linesToKeep, 0, linesToKeep.Length);

                    File.WriteAllLines(path, linesToKeep);
                    File.AppendAllText(path,
                        $"\r\n========== 日志清理 {DateTime.Now:yyyy-MM-dd HH:mm:ss} 已删除前99%的旧日志 ==========\r\n");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清理日志文件失败：{ex.Message}");
            }
        }

        /// <summary>确保日志文件存在，必要时清理</summary>
        private void EnsureLogFile()
        {
            try
            {
                string path = LogFilePath;

                if (File.Exists(path))
                {
                    var fi = new FileInfo(path);
                    if (fi.Length > _maxFileSize)
                    {
                        CleanLogFile();
                    }
                }
                else
                {
                    string dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllText(path,
                        $"========== 应用启动 {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==========\r\n");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化日志文件失败：{ex.Message}");
            }
        }

        /// <summary>写入单行日志，并按需清理</summary>
        private void WriteToFile(string logLine)
        {
            try
            {
                lock (_lock)
                {
                    File.AppendAllText(LogFilePath, logLine + "\r\n");

                    var fi = new FileInfo(LogFilePath);
                    if (fi.Length > _maxFileSize)
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
    }
}
