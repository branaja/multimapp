using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimAPP
{
    class ConvertAndZip : IStrategy
    {

        private void ConvertToJPGAndSave(string folderPath, string savePath)
        {
            foreach (string file in Directory.GetFileSystemEntries(folderPath))
            {
                if (file.IsImage())
                {
                    //it's an image
                    System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(file);
                    ImageCodecInfo jgpEncoder = Util.GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID
                    // for the Quality parameter category.
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;

                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 70L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    string temp = savePath + "\\" + System.IO.Path.GetFileNameWithoutExtension(file) + ".jpg";
                    bmp1.Save(temp, jgpEncoder,
                        myEncoderParameters);
                }
                else if (Directory.Exists(file))
                {
                    //it's a folder or something else

                    //check folder name, don't loop back into tempfolder
                    if (System.IO.Path.GetFileName(file) != "tempfolder")
                    {
                        System.IO.Directory.CreateDirectory(savePath + "\\" + System.IO.Path.GetFileName(file));
                        ConvertToJPGAndSave(file, savePath + "\\" + System.IO.Path.GetFileName(file));
                    }
                }
            }

        }
        
        public void Execute(string path, string zipName)
        {
            System.IO.Directory.CreateDirectory(path + "\\tempfolder");

            ConvertToJPGAndSave(path, path + "\\tempfolder");
            Util.CompressFolder(path, zipName);
            System.IO.Directory.Delete(path + "\\tempfolder", true);

            throw new NotImplementedException();
        }
    }
}
