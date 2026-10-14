using Newtonsoft.Json;
using Qiniu;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// 上传单个文件，8.7.0只有3参数UploadFile
    /// </summary>
    /// <param name="localFilePath">本地完整路径</param>
    /// <param name="key">七牛远端路径</param>
    /// <returns>CDN下载链接</returns>
    public async Task<string> UploadFile(string localFilePath, string key)
    {
        PutPolicy putPolicy = new PutPolicy();
        putPolicy.Scope = $"{_bucket}:{key}";
        putPolicy.InsertOnly = 0; //0=允许覆盖
        putPolicy.SetExpires(3600);

        string policyJson = putPolicy.ToJsonString();
        string upToken = Auth.CreateUploadToken(_mac, policyJson);

        FormUploader formUploader = new FormUploader(_cfg);
        //8.7.0 UploadFile 只有3个参数
        HttpResult result = await Task.Run(() =>
        {
            return formUploader.UploadFile(localFilePath, key, upToken,new PutExtra());
        });

        if (result.Code != 200)
        {
            throw new Exception($"七牛上传失败 Code:{result.Code} Msg:{result.RefText}");
        }
        string cdnUrl = $"https://{_cdnDomain}/{Uri.EscapeDataString(key)}";
        return cdnUrl;
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
