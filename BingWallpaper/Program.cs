using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BingWallpaper
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, string lpvParam, Int32 fuWinIni);
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 1;
        public const int SPIF_SENDCHANGE = 2;

        static void Main(string[] args)
        {
            var xmlSetDoc = new XmlDocument();
            xmlSetDoc.Load("setting.xml");
            var bingarchive = xmlSetDoc.SelectSingleNode("setting/bingarchive").InnerText.Trim();
            var resolution = xmlSetDoc.SelectSingleNode("setting/resolution").InnerText.Trim();
            var savepath = xmlSetDoc.SelectSingleNode("setting/savepath").InnerText.Trim();
            var replace = Convert.ToBoolean(xmlSetDoc.SelectSingleNode("setting/replace").InnerText.Trim());
            var beforedate = Convert.ToInt32(xmlSetDoc.SelectSingleNode("setting/beforedate").InnerText.Trim());
            for (int i = 0; i <= beforedate; i++)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(string.Format(bingarchive, i));
                string baseUrl = xmlDoc.SelectSingleNode("images/image/urlBase").InnerText.Trim();
                string enddate = xmlDoc.SelectSingleNode("images/image/enddate").InnerText.Trim();
                string save = string.Format("{0}\\{1}.jpg", savepath, enddate);

                baseUrl = string.Format("http://www.bing.com{0}_{1}.jpg", baseUrl, resolution);
                WebClient client = new WebClient();
                Console.WriteLine("正在下载{0}天前的Bing壁纸...", i);
                byte[] data = client.DownloadData(baseUrl);
                while (!replace && File.Exists(save))
                {
                    Console.WriteLine("已存在文件：" + save);
                    Console.WriteLine("---请先处理---");
                    Console.WriteLine("处理完成?按任意键继续...");
                    Console.ReadKey();
                }
                using (var ms = new MemoryStream(data))
                {
                    Bitmap img = new Bitmap(ms);
                    img.Save(save, ImageFormat.Jpeg);
                    img.Dispose();
                }
            }

            //Console.WriteLine("设置壁纸...");
            //SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, savepath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
    }
}
