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
        private IStrategy strategy;
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
                zipName.Text = Path.GetFileName(folderPath) + ".7z";
            }
        }
        private void GOButtonClick(object sender, RoutedEventArgs e)
        {
            if (chkBox.IsChecked == true)
            {
                this.strategy = new ConvertAndZip();

                
            }
            else
            {
                this.strategy = new Zip();   
            }

            this.strategy.Execute(savePath, zipName.Text);


            
           
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
