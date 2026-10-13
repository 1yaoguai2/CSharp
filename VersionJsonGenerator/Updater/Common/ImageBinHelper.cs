using System.IO;
using System.Drawing;
namespace Updater.Common
{

    public static class ImageBinHelper
    {
        //图片保存为二进制文件
        public static void SaveImageToBin(Image img, string filePath)
        {
            var ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            File.WriteAllBytes(filePath, ms.ToArray());
        }

        //读取二进制文件为图片
        public static Image LoadImageFromBin(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            byte[] data = File.ReadAllBytes(filePath);
            var ms = new MemoryStream(data);
            return Image.FromStream(ms);
        }
    }

}
