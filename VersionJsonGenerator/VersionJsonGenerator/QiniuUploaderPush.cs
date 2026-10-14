using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VersionJsonGenerator
{
    public class QiniuUploaderPush
    {
        // ========= 七牛配置 =========
        readonly string accessKey = "EiPlmCor8Mc1r-PFJAncjMKXejHPkpxCITtzBrpQ";
        readonly string secretKey = "Y5kxR4HCapyv3XTSbSuk0PUhTi8V-wj05i-_5jjm";
        readonly string bucketName = "update-test01";
        readonly string cdnDomain = "tmvox66iw.hb-bkt.clouddn.com";

        string gameRootDir = @"D:\XKZ\CSharp\VersionJsonGenerator\RubiksCubeUIRoot";
        string newVersion = "0.0.1";

        Qiniu.Storage.Zone bucketRegion = Qiniu.Storage.Zone.ZONE_CN_North;
        public QiniuUploaderPush(string version,string path)
        {
            newVersion = version;
            gameRootDir = path;
        }


        public async void Push()
        {
            var qiniu = new QiniuUploader(accessKey, secretKey, bucketName, cdnDomain, bucketRegion);

            //1. 下载云端旧清单
            var remoteManifest = await qiniu.DownloadRemoteManifest();
            var cloudDict = new Dictionary<string, string>();
            if (remoteManifest != null)
            {
                foreach (var item in remoteManifest.items)
                {
                    cloudDict[item.key] = item.md5;
                }
            }

            var newManifestItems = new List<ManifestItem>();
            var fileList = new List<FileItem>();

            foreach (var filePath in Directory.GetFiles(gameRootDir, "*.*", SearchOption.AllDirectories))
            {
                string relativePath = filePath.Substring(gameRootDir.Length).TrimStart('\\');
                string qiniuKey = relativePath.Replace("\\", "/");
                string localMd5 = GetFileMD5(filePath);

                newManifestItems.Add(new ManifestItem { key = qiniuKey, md5 = localMd5 });

                string remoteUrl;
                if (cloudDict.TryGetValue(qiniuKey, out string cloudMd5) && cloudMd5 == localMd5)
                {
                    Console.WriteLine($"✅云端一致，跳过：{qiniuKey}");
                    //重点：用cdnDomain变量拼接七牛CDN链接
                    remoteUrl = $"https://{cdnDomain}/{Uri.EscapeDataString(qiniuKey)}";
                }
                else
                {
                    Console.WriteLine($"⬆️上传：{qiniuKey}");
                    remoteUrl = await qiniu.UploadFile(filePath, qiniuKey);
                }

                fileList.Add(new FileItem
                {
                    path = relativePath,
                    md5 = localMd5,
                    url = remoteUrl
                });
            }


            //生成version.json（Updater用）
            var remoteInfo = new RemoteVersionInfo()
            {
                version = newVersion,
                files = fileList
            };
            string versionJson = JsonConvert.SerializeObject(remoteInfo, Formatting.Indented);
            string localVersionJsonPath = Path.Combine(gameRootDir, "version.json");
            File.WriteAllText(localVersionJsonPath, versionJson, System.Text.Encoding.UTF8);
            await qiniu.UploadFile(localVersionJsonPath, "version.json");

            //生成remote_manifest.json（版本生成器增量比对用）
            var manifestJson = JsonConvert.SerializeObject(new RemoteManifest { items = newManifestItems }, Formatting.Indented);
            string localManifestPath = Path.Combine(gameRootDir, "remote_manifest.json");
            File.WriteAllText(localManifestPath, manifestJson, Encoding.UTF8);
            await qiniu.UploadFile(localManifestPath, "remote_manifest.json");

            Console.WriteLine("✅全部处理完成！version.json + remote_manifest.json 已上传");

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

    }

    public class RemoteVersionInfo
    {
        public string version;
        public List<FileItem> files;
    }

}
