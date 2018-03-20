using SevenZip;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Apis.Drive.v2;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Drive.v2.Data;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Reflection;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> listaPathova;
        private string savePath;
        public MainWindow()
        {
            InitializeComponent();
            listaPathova = new List<string>();
            savePath = "";
            doneImg.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            doneImg.Visibility = Visibility.Hidden;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //dlg.DefaultExt = ".png";
            dlg.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
        "All files (*.*)|*.*";
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();
           
            
            //dodavanje slika u grid bi možda trebalo u zasebnoj dretvi ili nešta napraviti koja bi
            //dodavala slike u realnom vremenu?
            if (result == true)
            {
                listaPathova.Clear();
                // Open document 
                if (dlg.FileNames != null && dlg.FileNames.Count() > 0)
                {
                    savePath = System.IO.Path.GetDirectoryName(dlg.FileNames[0]);
                }
                foreach (string filename in dlg.FileNames)
                {
                    listaPathova.Add(filename);
                }
                noImg.Text = "Odabrano je " + listaPathova.Count() + " slika.";
            }
        }

        

        private void convertBttn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                savePath = dlg.SelectedPath;
            }
        }

        private void convertBttn1_Click(object sender, RoutedEventArgs e)
        {
            foreach(string img in listaPathova)
            {
                //convert2jpg(img);


                //update progressbara ne radi u realnom vremenu :( 
                int i = (listaPathova.IndexOf(img) + 1) / listaPathova.Count() * 100;
                //Dispatcher.Invoke(() => { this.pBar.Value = i; });
            }
        }
        private void convert2jpg(string img, string savePath)
        {
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(img);
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            string temp = savePath + "\\" + System.IO.Path.GetFileNameWithoutExtension(img) + ".jpg";
            bmp1.Save(temp, jgpEncoder,
                myEncoderParameters);
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            compress(savePath);
        }

        private void compress(string savePath)
        {
            var temp = new SevenZipCompressor();
            temp.ScanOnlyWritable = true;
            string savename = "";
            if (zipName.Text.Contains(".7z"))
                savename = zipName.Text;
            else
                savename = zipName.Text + ".7z";
            temp.CompressDirectory(savePath + "\\tempfolder", savePath + "\\" + savename);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.IO.Directory.CreateDirectory(savePath+"\\tempfolder");
            if(chkBox.IsChecked == true)
            {
                foreach(string img in listaPathova)
                {
                    convert2jpg(img, savePath + "\\tempfolder");
                }
            } else
            {
                foreach(string img in listaPathova)
                {
                    System.IO.File.Copy(img, savePath + "\\tempfolder\\" + System.IO.Path.GetFileName(img));
                }
            }

            compress(savePath);

            System.IO.Directory.Delete(savePath + "\\tempfolder", true);

            doneImg.Visibility = Visibility.Visible;
        }

        private void galleryBttn_Click(object sender, RoutedEventArgs e)
        {
            Gallery gallery = new Gallery(listaPathova, this);
            gallery.Show();
        }

        public void deleteImage(int index)
        {
            listaPathova.RemoveAt(index);
            noImg.Text = "Odabrano je " + listaPathova.Count() + " slika.";
        }

        /*
        private void auth()
        {
            string[] scopes = new string[] { DriveService.Scope.Drive }; // Full access

            var keyFilePath = @"c:\file.p12";    // Downloaded from https://console.developers.google.com
            var serviceAccountEmail = "branimir.jungic@gmail.com";  // found https://console.developers.google.com

            //loading the Key file
            var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes
            }.FromCertificate(certificate));

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });


        }

        public static File createDirectory(DriveService _service, string _title, string _description, string _parent)
        {

            File NewDirectory = null;

            // Create metaData for a new Directory
            File body = new File();
            body.Title = _title;
            body.Description = _description;
            body.MimeType = "application/vnd.google-apps.folder";
            body.Parents = new List() { new ParentReference() { Id = _parent } };
            try
            {
                FilesResource.InsertRequest request = _service.Files.Insert(body);
                NewDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return NewDirectory;
        }

        // tries to figure out the mime type of the file.
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public static File uploadFile(DriveService _service, string _uploadFile, string _parent) {
            
            if (System.IO.File.Exists(_uploadFile))
            {
                File body = new File();
                body.Title = System.IO.Path.GetFileName(_uploadFile);
                body.Description = "File uploaded by Diamto Drive Sample";
                body.MimeType = GetMimeType(_uploadFile);
                body.Parents = new List() { new ParentReference() { Id = _parent } };

                // File's content.
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.InsertMediaUpload request = _service.Files.Insert(body, stream, GetMimeType(_uploadFile));
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else {
                Console.WriteLine("File does not exist: " + _uploadFile);
                return null;
            }           
        
        }
        */

    }

   
}
