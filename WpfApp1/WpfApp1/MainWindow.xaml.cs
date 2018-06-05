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
using MultimAPP;

/// <summary>
/// 1. Pascal Case kod metoda
/// 2. Nazivi buttona i varijabli
/// 4. Malo hrv malo eng
/// 5. Gallery -> GalleryWindow
/// 7. Zapravo treba omogućiti način rada da se odabere root folder i onda se rekurzivno sve konvertira i 7zipa.
/// 8. Omogućiti izbor accounta 
/// 9. Elegantno riješiti problem izbora jedne od mogućnosti rada programa.
/// </summary>

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> listOfPaths;
        private string folderPath;
        private string savePath;
        public MainWindow()
        {
            InitializeComponent();
            listOfPaths = new List<string>();
            savePath = "";
            doneImg.Visibility = Visibility.Hidden;
        }

        private void ChooseFolderButtonClick(object sender, RoutedEventArgs e)
        {
            doneImg.Visibility = Visibility.Hidden;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                folderPath=dialog.FileName;
                savePath = folderPath;
            }

            /*
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
                listOfPaths.Clear();
                // Open document 
                if (dlg.FileNames != null && dlg.FileNames.Count() > 0)
                {
                    savePath = System.IO.Path.GetDirectoryName(dlg.FileNames[0]);
                }
                foreach (string filename in dlg.FileNames)
                {
                    listOfPaths.Add(filename);
                }
                noImg.Text = "Odabrano je " + listOfPaths.Count() + " slika.";
            }
            #endregion
            */
        }


        /// <summary>
        /// Hahahaha ovo je jeben naming
        /// </summary>
        private void WhatDoesThisButtonDo(object sender, RoutedEventArgs e)
        {
            /*
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                savePath = dlg.SelectedPath;
            }
            */
        }

        /// <summary>
        /// Ne znam jesi li primjetio obsolete atribut, ako pokušamo koristiti ovu metodu negdje u programu dobit ćemo warning.
        /// Postoji još i deprecated, funkcionira analogno ovome.
        /// </summary>
        [Obsolete]
        private void convertBttn1_Click(object sender, RoutedEventArgs e)
        {
            foreach (string img in listOfPaths)
            {
                //convert2jpg(img);


                //update progressbara ne radi u realnom vremenu :( 
                int i = (listOfPaths.IndexOf(img) + 1) / listOfPaths.Count() * 100;
                //Dispatcher.Invoke(() => { this.pBar.Value = i; });
            }
        }

        private void ConvertToJPGAndSave(string folderPath, string savePath)
        {
            foreach(string file in Directory.GetFileSystemEntries(folderPath))
            {
                if( file.IsImage() )
                {
                    //it's an image
                    System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(file);
                    ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

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
            CompressFolder(savePath);
        }

        private void CompressFolder(string savePath)
        {
            var temp = new SevenZipCompressor();
            temp.ScanOnlyWritable = true;
            string savename = "";
            if (zipName.Text.Contains(".7z"))
                savename = zipName.Text;
            else
                savename = zipName.Text + ".7z";
            temp.CompressDirectory(savePath + "\\tempfolder", savePath + "\\" + savename); 
            //temp.CompressDirectory(savePath, savePath + "\\" + savename);

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void GOButtonClick(object sender, RoutedEventArgs e)
        {
            System.IO.Directory.CreateDirectory(savePath + "\\tempfolder");
            if (chkBox.IsChecked == true)
            {
                ConvertToJPGAndSave(folderPath, savePath + "\\tempfolder");
                
            }
            else
            {
                
            }

            CompressFolder(savePath);

            System.IO.Directory.Delete(savePath + "\\tempfolder", true);

            
           
            /*
            Drive.uploadFile(Drive.AuthenticateServiceAccount("serviceaccountmultim@multimapp-200019.iam.gserviceaccount.com", Environment.CurrentDirectory + "\\MultimApp-354bf7b31203.json"), savePath + "\\" + zipName.Text, null);
            var uploadError = Drive.uploadFile(Drive.AuthenticateOauth(), savePath + "\\" + zipName.Text, null);
            if (uploadError != null)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(uploadError);
            }
            else
            {
                doneImg.Visibility = Visibility.Visible;
            }
            */
            
        }

        private void GalleryButtonClick(object sender, RoutedEventArgs e)
        {
            Gallery gallery = new Gallery(listOfPaths, this);
            gallery.Show();
        }

        public void DeleteImage(int index)
        {
            listOfPaths.RemoveAt(index);
            noImg.Text = "Odabrano je " + listOfPaths.Count() + " slika.";
        }

    }
}
