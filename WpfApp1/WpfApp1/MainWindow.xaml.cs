using SevenZip;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using Google.Apis.Drive.v2;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

/// <summary>
/// 1. Pascal Case kod metoda, općenito nekonzistentno
/// 2. Nazivi buttona i to
/// 3. OO - convert2jpg skriveno radi i save
/// 4. Malo hrv malo eng
/// 5. Gallery -> GalleryWindow
/// 6. Fix naziv projekt
/// 7. Zapravo treba omogućiti način rada da se odabere root folder i onda se rekurzivno sve konvertira i 7zipa.
/// 8. Omogućiti izbor accounta 
/// </summary>

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
            //CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "";
            //dialog.IsFolderPicker = true;
            //if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            //{
            //    listaPathova.Add(dialog.FileName);
            //}

            #region branimir
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
            #endregion
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

        [Obsolete]
        private void convertBttn1_Click(object sender, RoutedEventArgs e)
        {
            foreach (string img in listaPathova)
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

            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 70L);
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

        [Obsolete]
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
            //temp.CompressDirectory(savePath + "\\tempfolder", savePath + "\\" + savename); FIXME
            temp.CompressDirectory(savePath, savePath + "\\" + savename);

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.IO.Directory.CreateDirectory(savePath + "\\tempfolder");
            if (chkBox.IsChecked == true)
            {
                foreach (string img in listaPathova)
                {
                    convert2jpg(img, savePath + "\\tempfolder");
                }
            }
            else
            {
                foreach (string img in listaPathova)
                {
                    System.IO.File.Copy(img, savePath + "\\tempfolder\\" + System.IO.Path.GetFileName(img));//FIXME
                }
            }

            compress(listaPathova[0]);//FIXME

            System.IO.Directory.Delete(savePath + "\\tempfolder", true);

            //Drive.uploadFile(Drive.AuthenticateServiceAccount("serviceaccountmultim@multimapp-200019.iam.gserviceaccount.com", Environment.CurrentDirectory + "\\MultimApp-354bf7b31203.json"), savePath + "\\" + zipName.Text, null);
            var uploadError = Drive.uploadFile(Drive.AuthenticateOauth(), savePath + "\\" + zipName.Text, null);
            if (uploadError != null)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(uploadError);
            }
            else
            {
                doneImg.Visibility = Visibility.Visible;
            }
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

    }


}
