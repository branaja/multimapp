using SevenZip;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimAPP
{
    class Util
    {
        internal static void CompressFolder(string savePath, string zipName)
        {
            var temp = new SevenZipCompressor();
            temp.ScanOnlyWritable = true;
            string savename = Path.GetFileName(savePath);
            if (zipName.Contains(".7z"))
                savename = zipName;
            else
                savename = zipName + ".7z";
            temp.CompressDirectory(savePath + "\\tempfolder", System.IO.Directory.GetParent(savePath) + "\\" + savename);
        }

        internal static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
