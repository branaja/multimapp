using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Gallery.xaml
    /// </summary>
    public partial class Gallery : Window
    {
        private List<string> paths;
        private int count = 0;
        private MainWindow parent;
        public Gallery()
        {
            InitializeComponent();
        }

        public Gallery(List<string> paths, MainWindow parent)
        {
            InitializeComponent();
            this.paths = paths;
            this.parent = parent;
            count = 0;
            foreach(string img in paths)
            {
                addImg2Grid(img);
            }
        }

        private void addImg2Grid(string filename)
        {
            try
            {
                if (count / 10 + 1 > gridImg.RowDefinitions.Count())
                    gridImg.RowDefinitions.Add(new RowDefinition());
                if (gridImg.ColumnDefinitions.Count() < 10)
                    gridImg.ColumnDefinitions.Add(new ColumnDefinition());

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(filename);
                image.DecodePixelWidth = 1000;
                image.EndInit();

               
                Image img = new Image();

                Grid.SetColumn(img, count % 10);
                Grid.SetRow(img, count / 10);

                img.Source = image;
                img.MouseEnter += removePreview;
                img.MouseLeave += removeNoMore;
                img.MouseDown += deleteSelected;
                gridImg.Children.Add(img);
                count++;

            }
            catch (Exception ex)
            {

            }
            
        }
        private void removePreview(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            if (img.Opacity != 0)
            {
                img.Opacity = 0.2;
                img.Cursor = Cursors.No;
            }
        }

        private void removeNoMore(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            if(img.Opacity!=0) img.Opacity = 1;
        }

        private void deleteSelected(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            int index = gridImg.Children.IndexOf(img);
            img.Opacity = 0;
            //parent.DeleteImage(index);
            gridImg.Children.RemoveAt(index);
        }
    }
}
