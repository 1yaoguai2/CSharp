using Newtonsoft.Json;
using Qiniu;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class QiniuUploader
{
    private readonly Mac _mac;
    private readonly string _bucket;
    private readonly string _cdnDomain;
    private readonly Config _cfg;

    public QiniuUploader(string ak, string sk, string bucket, string cdnDomain, Zone region)
    {
        _mac = new Mac(ak, sk);
        _bucket = bucket;
        _cdnDomain = cdnDomain.TrimEnd('/');
        _cfg = new Config();
        _cfg.Zone = region;
    }

    /// <summary>
    /// 生成 CDN 下载链接。
    /// 注意：按路径分段转义，保留 "/" 分隔符（不能整体 EscapeDataString，否则 "/" 会变成 "%2F"）。
    /// </summary>
    public string GetCdnUrl(string key)
    {
        var segments = key.Split('/');
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i] = Uri.EscapeDataString(segments[i]);
        }
        return $"https://{_cdnDomain}/{string.Join("/", segments)}";
    }

    /// <summary>
    /// 上传单个文件（带实时进度回调）。
    /// </summary>
    /// <param name="localFilePath">本地完整路径</param>
    /// <param name="key">七牛远端路径</param>
    /// <param name="onProgress">进度回调：参数为(当前文件已上传字节, 当前文件总字节)，可为 null</param>
    /// <returns>CDN 下载链接</returns>
    public async Task<string> UploadFile(string localFilePath, string key, Action<long, long> onProgress = null)
    {
        PutPolicy putPolicy = new PutPolicy();
        putPolicy.Scope = $"{_bucket}:{key}";
        putPolicy.InsertOnly = 0; //0=允许覆盖
        putPolicy.SetExpires(3600);

        string policyJson = putPolicy.ToJsonString();
        string upToken = Auth.CreateUploadToken(_mac, policyJson);

        var extra = new PutExtra();
        if (onProgress != null)
        {
            // SDK 在上传过程中会持续回调 (uploadedBytes, totalBytes)
            extra.ProgressHandler = (uploadedBytes, totalBytes) =>
            {
                try { onProgress(uploadedBytes, totalBytes); }
                catch { /* 忽略进度回调异常，不能影响上传 */ }
            };
        }

        // 断点续传记录文件（放到临时目录，上传结束后删除，避免本地残留 .progress 文件）
        string resumeFile = Path.Combine(Path.GetTempPath(), "qiniu_up_" + Guid.NewGuid().ToString("N") + ".progress");
        extra.ResumeRecordFile = resumeFile;

        // ResumableUploader 支持分片、重试与进度回调（FormUploader 不支持进度）
        var uploader = new ResumableUploader(_cfg);
        HttpResult result;
        try
        {
            result = await Task.Run(() => uploader.UploadFile(localFilePath, key, upToken, extra));
        }
        finally
        {
            try { if (File.Exists(resumeFile)) File.Delete(resumeFile); }
            catch { /* 清理失败可忽略 */ }
        }

        if (result.Code != 200)
        {
            throw new Exception($"七牛上传失败 Code:{result.Code} Msg:{result.RefText}");
        }
        return GetCdnUrl(key);
    }

    /// <summary>
    /// 从七牛下载远程manifest清单
    /// </summary>
    public async Task<RemoteManifest> DownloadRemoteManifest()
    {
        string url = $"https://{_cdnDomain}/remote_manifest.json";
        using (var http = new System.Net.Http.HttpClient())
        {
            try
            {
                var resp = await http.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return null;
                var json = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RemoteManifest>(json);
            }
            catch
            {
                // 文件不存在（首次打包）返回null
                return null;
            }
        }
    }
}

/// <summary>
/// 远程清单实体，保存在七牛
/// </summary>
public class RemoteManifest
{
    public List<ManifestItem> items { get; set; } = new List<ManifestItem>();
}
public class ManifestItem
{
    public string key { get; set; }
    public string md5 { get; set; }
}
