using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;

namespace AnalysisTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            System.Windows.Controls.Image firstImg = new System.Windows.Controls.Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\first.png", UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            firstImg.Source = src2;
            firstImg.Stretch = Stretch.Uniform;
            sp1.Children.Add(firstImg);

            System.Windows.Controls.Image secondImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\second.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            secondImg.Source = src;
            secondImg.Stretch = Stretch.Uniform;
            sp2.Children.Add(secondImg);


            System.Drawing.Image first = System.Drawing.Image.FromFile(@"Resources\first.png");

            System.Drawing.Image second = System.Drawing.Image.FromFile(@"Resources\second.png");

            Bitmap bitmap1 = new Bitmap(first);
            Bitmap bitmap2 = new Bitmap(second);

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[bitmap1.Width, bitmap1.Height];
            Bitmap bmp = new Bitmap(300, 300);

            for (int x = 0; x < bitmap1.Width; x++)
            {
                for (int y = 0; y < bitmap1.Height; y++)
                {
                    System.Drawing.Color pixel1 = bitmap1.GetPixel(x, y);
                    System.Drawing.Color pixel2 = bitmap2.GetPixel(x, y);

                    int colorValue = (Math.Abs(pixel1.R - pixel2.R) + Math.Abs(pixel1.G - pixel2.G) + Math.Abs(pixel1.B - pixel2.B));

                    pixelArray[x, y] = System.Drawing.Color.FromArgb(colorValue, colorValue, colorValue);

                    bmp.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            System.Drawing.Image img3 = bmp;
            MemoryStream ms2 = new MemoryStream();
            img3.Save(ms2, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = ms2;
            bi.EndInit();

            System.Windows.Controls.Image resultImg = new System.Windows.Controls.Image();
            resultImg.Source = bi;
            resultImg.Stretch = Stretch.Uniform;
            sp3.Children.Add(resultImg);

        }
    }
}
