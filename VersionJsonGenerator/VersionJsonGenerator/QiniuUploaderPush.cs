using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VersionJsonGenerator.Common;

namespace VersionJsonGenerator
{
    public class QiniuUploaderPush
    {
        // ========= 七牛配置 =========
        /* 参数获取方式
         * # 四个参数获取位置（七牛控制台 [portal.qiniu.com](https://portal.qiniu.com)）

## 1. AK = AccessKey；SK = SecretKey

👉 路径：右上角头像 → **个人中心 → 密钥管理**
链接直达：[https://portal.qiniu.com/user/key](https://portal.qiniu.com/user/key)七牛云

- 点【新建密钥】，生成一对 AccessKey / SecretKey，复制保存

> 
> ⚠️ **重要安全提醒**：AK/SK 只放在你的**版本生成器（本地打包工具）**里；**绝对不能放进 Updater 客户端**，一旦泄露别人可以随意删改你七牛存储空间里所有文件。

## 2. bucketName（Bucket 名称 / 空间名）

👉 路径：对象存储 Kodo → **空间管理**
链接直达：[https://portal.qiniu.com/kodo/bucket](https://portal.qiniu.com/kodo/bucket)developer-...

- 你新建存储空间时填的名字（纯英文小写，不带域名）
- 点进这个空间，在**空间概览**页面，可以同时看到：存储区域（华东 z0 / 华北 z1）

> 
> 你代码里的`Zone bucketZone = Zone.ZONE_CN_East` 要和这里存储区域保持一致！

## 3. cdnDomain（CDN 域名，**不带 https://**）

👉 进入你的 Bucket → 域名管理
两种域名：

1. **测试域名**：形如 `xxxxxx.bkt.clouddn.com`，七牛自带，短期测试用，**有效期 30 天**，到期会失效，只能临时调试。
2. **自定义域名**：自己备案域名绑定，长期正式发布用。

> 
> 填入代码时：只填域名，**不要加 https://**，示例：`xxxxxx.bkt.clouddn.com`
> 代码内部自动拼接 `https://{cdnDomain}/xxx`
         */
        readonly string accessKey = "";
        readonly string secretKey = "";  
        readonly string bucketName = "update-test01";
        readonly string cdnDomain = "tmvox66iw.hb-bkt.clouddn.com";

        string gameRootDir = @"D:\XKZ\CSharp\VersionJsonGenerator\RubiksCubeUIRoot";
        string newVersion = "0.0.1";

        Qiniu.Storage.Zone bucketRegion = Qiniu.Storage.Zone.ZONE_CN_North;

        /// <summary>进度通知（可能在后台线程触发，界面需自行 Invoke）</summary>
        public event Action<UploadProgressInfo> OnProgress;

        // ===== 进度/速度计算（线程安全） =====
        private readonly object _progressLock = new object();
        private long _totalBytes;         // 待上传总字节
        private long _uploadedBytes;      // 已完成文件的累计字节
        private long _currentFileBase;    // 当前文件开始前的累计
        private long _currentFileUploaded;// 当前文件已上传字节
        private readonly List<SpeedSample> _speedSamples = new List<SpeedSample>();
        private DateTime _lastUiReport = DateTime.MinValue;
        private static readonly TimeSpan SpeedWindow = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan UiReportInterval = TimeSpan.FromMilliseconds(200);

        // 游戏文件上传阶段占用的进度百分比（其余留给两个清单文件）
        private const int GamePhaseMaxPercent = 98;

        public QiniuUploaderPush(string version, string path)
        {
            newVersion = version;
            gameRootDir = path;
        }

        public async Task PushAsync()
        {
            try
            {
                var qiniu = new QiniuUploader(accessKey, secretKey, bucketName, cdnDomain, bucketRegion);

                RaiseProgress(0, "准备中...", null);

                //1. 下载云端旧清单
                var remoteManifest = await qiniu.DownloadRemoteManifest();
                var cloudDict = new Dictionary<string, string>();
                if (remoteManifest != null)
                {
                    foreach (var item in remoteManifest.items)
                    {
                        cloudDict[item.key] = item.md5;
                        Log($"云旧清单：{item.key} - {item.md5}");
                    }
                }
                else
                {
                    Log("云端旧清单不存在，首次上传");
                }

                var newManifestItems = new List<ManifestItem>();
                var fileList = new List<FileItem>();
                var pendingUploads = new List<PendingUpload>();
                long totalBytes = 0;
                int scannedCount = 0;

                //2. 扫描本地文件，计算MD5，区分「需上传」与「云端一致」
                foreach (var filePath in Directory.GetFiles(gameRootDir, "*.*", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileName(filePath);
                    // 跳过这两个清单文件，不加入version.json的files列表
                    if (fileName == "version.json" || fileName == "remote_manifest.json")
                    {
                        continue;
                    }
                    string relativePath = filePath.Substring(gameRootDir.Length).TrimStart('\\');
                    string qiniuKey = relativePath.Replace("\\", "/");
                    string localMd5 = GetFileMD5(filePath);

                    scannedCount++;
                    if (scannedCount % 50 == 0)
                    {
                        Log($"正在扫描并计算MD5... 已处理 {scannedCount} 个文件");
                    }

                    newManifestItems.Add(new ManifestItem { key = qiniuKey, md5 = localMd5 });

                    var fileItem = new FileItem { path = relativePath, md5 = localMd5 };

                    if (cloudDict.TryGetValue(qiniuKey, out string cloudMd5) && cloudMd5 == localMd5)
                    {
                        Log($"✅云端一致，跳过：{qiniuKey}");
                        fileItem.url = qiniu.GetCdnUrl(qiniuKey);
                    }
                    else
                    {
                        long size = new FileInfo(filePath).Length;
                        totalBytes += size;
                        pendingUploads.Add(new PendingUpload
                        {
                            FilePath = filePath,
                            Key = qiniuKey,
                            Size = size,
                            Item = fileItem
                        });
                    }

                    // 保持原有顺序写入 version.json
                    fileList.Add(fileItem);
                }

                lock (_progressLock)
                {
                    _totalBytes = totalBytes;
                    _uploadedBytes = 0;
                    _currentFileBase = 0;
                    _currentFileUploaded = 0;
                    _speedSamples.Clear();
                    _lastUiReport = DateTime.MinValue;
                }

                Log($"待上传文件 {pendingUploads.Count} 个，共 {FormatBytes(totalBytes)}");

                //3. 逐个上传文件（字节级实时进度）
                foreach (var p in pendingUploads)
                {
                    Log($"⬆️上传：{p.FilePath}-{p.Key}");

                    lock (_progressLock)
                    {
                        _currentFileBase = _uploadedBytes;
                        _currentFileUploaded = 0;
                    }

                    // 传引用以识别当前文件（供界面显示）
                    string currentKey = p.Key;
                    string remoteUrl = await qiniu.UploadFile(p.FilePath, p.Key,
                        (uploadedInFile, totalInFile) => ReportFileProgress(uploadedInFile, currentKey));

                    lock (_progressLock)
                    {
                        _uploadedBytes = _currentFileBase + p.Size;
                        _currentFileUploaded = p.Size;
                    }
                    p.Item.url = remoteUrl;
                    RaiseFileFinishedProgress();
                }

                //4. 生成version.json（Updater用）
                RaiseProgress(GamePhaseMaxPercent, "生成 version.json ...", null);
                var remoteInfo = new RemoteVersionInfo()
                {
                    version = newVersion,
                    files = fileList
                };
                string versionJson = JsonConvert.SerializeObject(remoteInfo, Formatting.Indented);
                string localVersionJsonPath = Path.Combine(gameRootDir, "version.json");
                File.WriteAllText(localVersionJsonPath, versionJson, Encoding.UTF8);
                Log("⬆️上传：version.json");
                await qiniu.UploadFile(localVersionJsonPath, "version.json");
                RaiseProgress(GamePhaseMaxPercent + 1, "version.json 已上传", null);

                //5. 生成remote_manifest.json（版本生成器增量比对用）
                RaiseProgress(GamePhaseMaxPercent + 1, "生成 remote_manifest.json ...", null);
                var manifestJson = JsonConvert.SerializeObject(new RemoteManifest { items = newManifestItems }, Formatting.Indented);
                string localManifestPath = Path.Combine(gameRootDir, "remote_manifest.json");
                File.WriteAllText(localManifestPath, manifestJson, Encoding.UTF8);
                Log("⬆️上传：remote_manifest.json");
                await qiniu.UploadFile(localManifestPath, "remote_manifest.json");

                Log("✅全部处理完成！version.json + remote_manifest.json 已上传");
                RaiseProgress(100, "完成", null);
            }
            catch (Exception ex)
            {
                Log($"❌上传失败：{ex.Message}");
                RaiseProgress(-1, "上传失败", null);
                throw;
            }
        }

        /// <summary>当前文件上传进度回调（来自 SDK，可能在后台线程）</summary>
        private void ReportFileProgress(long uploadedInFile, string currentKey)
        {
            UploadProgressInfo info = null;
            lock (_progressLock)
            {
                if (uploadedInFile > _currentFileUploaded)
                {
                    _currentFileUploaded = uploadedInFile;
                }

                long overall = _currentFileBase + _currentFileUploaded;
                AddSpeedSample(overall);

                // 限流：最多每 200ms 通知一次界面，避免刷屏
                DateTime now = DateTime.Now;
                bool isFinished = _totalBytes > 0 && overall >= _totalBytes;
                if (!isFinished && now - _lastUiReport < UiReportInterval)
                {
                    return;
                }
                _lastUiReport = now;

                int percent;
                if (_totalBytes > 0)
                {
                    percent = (int)Math.Min(GamePhaseMaxPercent, overall * GamePhaseMaxPercent / _totalBytes);
                }
                else
                {
                    percent = GamePhaseMaxPercent;
                }

                info = new UploadProgressInfo
                {
                    Percent = percent,
                    BytesPerSecond = GetSpeed(),
                    UploadedBytes = overall,
                    TotalBytes = _totalBytes,
                    CurrentFile = currentKey,
                    Stage = "上传文件"
                };
            }

            // 在锁外通知界面，避免长时间持锁
            if (info != null)
            {
                OnProgress?.Invoke(info);
            }
        }

        /// <summary>单个文件上传完成后强制刷新一次进度（保证进度条不卡在中间值）</summary>
        private void RaiseFileFinishedProgress()
        {
            UploadProgressInfo info = null;
            lock (_progressLock)
            {
                if (_totalBytes > 0)
                {
                    _lastUiReport = DateTime.Now;
                    info = new UploadProgressInfo
                    {
                        Percent = (int)Math.Min(GamePhaseMaxPercent, _uploadedBytes * GamePhaseMaxPercent / _totalBytes),
                        BytesPerSecond = GetSpeed(),
                        UploadedBytes = _uploadedBytes,
                        TotalBytes = _totalBytes,
                        Stage = "上传文件"
                    };
                }
            }
            if (info != null)
            {
                OnProgress?.Invoke(info);
            }
        }

        /// <summary>主动上报阶段进度（percent 为 -1 表示失败）</summary>
        private void RaiseProgress(int percent, string stage, string currentFile)
        {
            long uploaded;
            long total;
            lock (_progressLock)
            {
                uploaded = _uploadedBytes;
                total = _totalBytes;
            }
            OnProgress?.Invoke(new UploadProgressInfo
            {
                Percent = percent < 0 ? -1 : Math.Min(100, percent),
                BytesPerSecond = 0,
                UploadedBytes = uploaded,
                TotalBytes = total,
                CurrentFile = currentFile,
                Stage = stage
            });
        }

        private void AddSpeedSample(long bytes)
        {
            var now = DateTime.Now;
            _speedSamples.Add(new SpeedSample { Time = now, Bytes = bytes });

            DateTime cutoff = now - SpeedWindow;
            int removeCount = 0;
            while (removeCount < _speedSamples.Count - 2 && _speedSamples[removeCount].Time < cutoff)
            {
                removeCount++;
            }
            if (removeCount > 0)
            {
                _speedSamples.RemoveRange(0, removeCount);
            }
        }

        private double GetSpeed()
        {
            if (_speedSamples.Count < 2) return 0;
            var first = _speedSamples[0];
            var last = _speedSamples[_speedSamples.Count - 1];
            double seconds = (last.Time - first.Time).TotalSeconds;
            if (seconds <= 0) return 0;
            double speed = (last.Bytes - first.Bytes) / seconds;
            return speed > 0 ? speed : 0;
        }

        /// <summary>格式化字节数为可读字符串</summary>
        public static string FormatBytes(double bytes)
        {
            if (bytes < 1024) return $"{bytes:0} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024:0.0} KB";
            if (bytes < 1024L * 1024 * 1024) return $"{bytes / 1024 / 1024:0.00} MB";
            return $"{bytes / 1024 / 1024 / 1024:0.00} GB";
        }

        /// <summary>格式化速度</summary>
        public static string FormatSpeed(double bytesPerSecond)
        {
            return FormatBytes(bytesPerSecond) + "/s";
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
        /// 日志输出（转发到全局 Logger，同时由 Logger 通知界面显示）
        /// 其他窗口/类可直接使用 Logger.Instance.Log(...)
        /// </summary>
        private void Log(string msg, bool writeToFile = true)
        {
            Logger.Instance.Log(msg, writeToFile);
        }

        private struct SpeedSample
        {
            public DateTime Time;
            public long Bytes;
        }

        private class PendingUpload
        {
            public string FilePath;
            public string Key;
            public long Size;
            public FileItem Item;
        }
    }

    /// <summary>上传进度信息</summary>
    public class UploadProgressInfo
    {
        /// <summary>总进度 0-100（-1 表示失败）</summary>
        public int Percent { get; set; }
        /// <summary>实时速度（字节/秒）</summary>
        public double BytesPerSecond { get; set; }
        /// <summary>已上传字节</summary>
        public long UploadedBytes { get; set; }
        /// <summary>待上传总字节</summary>
        public long TotalBytes { get; set; }
        /// <summary>当前文件（七牛key）</summary>
        public string CurrentFile { get; set; }
        /// <summary>当前阶段描述</summary>
        public string Stage { get; set; }
    }

    public class RemoteVersionInfo
    {
        public string version;
        public List<FileItem> files;
    }
}
