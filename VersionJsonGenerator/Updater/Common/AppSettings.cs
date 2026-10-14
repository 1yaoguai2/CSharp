using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater.Common
{
    public class AppSettings
    {
        public string TitleText { get; set; }
        public string RemoteVersionUrl { get; set; }
        public bool ShowIcon { get; set; }
        //图片不存这里，只记录文件名
        public string IconBinFileName { get; set; }
        public string BackgroundBinFileName { get; set; }
        //不带.exe
        public string GameName { get; set; }
        
        // 【新增】下载相关配置
        public int DownloadTimeoutSeconds { get; set; } = 60;
        public int MaxRetryCount { get; set; } = 3;
        public bool EnableLog { get; set; } = true;
    }

}
