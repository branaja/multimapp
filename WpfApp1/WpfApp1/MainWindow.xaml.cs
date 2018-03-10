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
            textBox1.Text = "";
            listaPathova = new List<string>();
            savePath = "";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //dlg.DefaultExt = ".png";
            dlg.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
        "All files (*.*)|*.*";
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();
            int count = 0;
            
            //dodavanje slika u grid bi možda trebalo u zasebnoj dretvi ili nešta napraviti koja bi
            //dodavala slike u realnom vremenu?
            if (result == true)
            {
                textBox1.Text = "";
                imgGrid.Children.Clear();
                listaPathova.Clear();
                // Open document 
                if (dlg.FileNames != null && dlg.FileNames.Count() > 0)
                {
                    savePath = System.IO.Path.GetDirectoryName(dlg.FileNames[0]);
                    outputPath.Text = savePath;
                }
                foreach (string filename in dlg.FileNames)
                {
                    textBox1.Text += filename + "\n";
                    listaPathova.Add(filename);
                    try
                    {
                        if (count / 5 + 1 > imgGrid.RowDefinitions.Count())
                            imgGrid.RowDefinitions.Add(new RowDefinition());
                        if(imgGrid.ColumnDefinitions.Count() <5)
                            imgGrid.ColumnDefinitions.Add(new ColumnDefinition());

                        Image img = new Image();
                        Grid.SetColumn(img, count%5);
                        Grid.SetRow(img, count/5);
                        img.Source = new BitmapImage(new Uri(filename));
                        img.MouseEnter += removePreview;
                        img.MouseLeave += removeNoMore;
                        imgGrid.Children.Add(img);
                        count++;
                        
                    }
                    catch (Exception ex)
                    {

                    }

                    }
            }
        }

        private void removePreview(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            img.Opacity = 0.2;
            img.Cursor = Cursors.No;
            Label lbl = new Label();
            lbl.Content = "X";
            lbl.Foreground = new SolidColorBrush(Colors.Red);
            
        }

        private void removeNoMore(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            img.Opacity = 1;
        }

        private void convertBttn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                outputPath.Text = dlg.SelectedPath;
                savePath = dlg.SelectedPath;
            }
        }

        private void convertBttn1_Click(object sender, RoutedEventArgs e)
        {
            foreach(string img in listaPathova)
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

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,50L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                string temp = savePath +"\\"+ System.IO.Path.GetFileName(img);
                bmp1.Save(savePath + "\\" + System.IO.Path.GetFileName(img), jgpEncoder,
                    myEncoderParameters);


                //update progressbara ne radi u realnom vremenu :( 
                int i = (listaPathova.IndexOf(img) + 1) / listaPathova.Count() * 100;
                Dispatcher.Invoke(() => { this.pBar.Value = i; });
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
    }

   
}
