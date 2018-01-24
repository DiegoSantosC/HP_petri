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
        private bool drawing;

        public MainWindow()
        {

            InitializeComponent();

            System.Windows.Controls.Image firstImg = new System.Windows.Controls.Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\First.png", UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            firstImg.Source = src2;
            firstImg.Stretch = Stretch.Uniform;
            sp1.Children.Add(firstImg);

            System.Windows.Controls.Image secondImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Second.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            secondImg.Source = src;
            secondImg.Stretch = Stretch.Uniform;
            sp2.Children.Add(secondImg);


            System.Drawing.Image first = System.Drawing.Image.FromFile(@"Resources\First.png");

            System.Drawing.Image second = System.Drawing.Image.FromFile(@"Resources\Second.png");

            Bitmap bitmap1 = new Bitmap(first);
            Bitmap bitmap2 = new Bitmap(second);

            Bitmap bmp = new Bitmap(300, 300);

            bmp = getGlobalCorrelation(bitmap1, bitmap2);
           
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

            bmp.Save(@"Resources\Result.bmp");

        }

        private Bitmap getManhattan(Bitmap bmp1, Bitmap bmp2)
        {
            int width, height;

            int[] deplacement = getDepacement(bmp1, bmp2);
            int hD1, hD2, wD1, wD2;

            if (bmp1.Height < bmp2.Height)
            {
                height = bmp1.Height;
                hD1 = 0;
                hD2 = deplacement[1];

            }
            else
            {
                height = bmp2.Height;
                hD1 = deplacement[1];
                hD2 = 0;

            }

            if (bmp1.Width < bmp2.Width)
            {
                width = bmp1.Width;
                wD1 = 0;
                wD2 = deplacement[0];

            }
            else
            {
                width = bmp2.Width;
                wD1 = deplacement[0];
                wD2 = 0;

            }

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[width, height];

            Bitmap bmp = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x+wD1, y+hD1);
                    System.Drawing.Color pixel2 = bmp2.GetPixel(x+wD2, y+hD2);


                    int colorValue = (Math.Abs(pixel1.R - pixel2.R) + Math.Abs(pixel1.G - pixel2.G) + Math.Abs(pixel1.B - pixel2.B))/3;

                    pixelArray[x, y] = getThreshold(Math.Abs(255 - colorValue));

                    bmp.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return bmp;
        }

        private Bitmap getEuclidean(Bitmap bmp1, Bitmap bmp2)
        {
            int width, height;

            int[] deplacement = getDepacement(bmp1, bmp2);
            int hD1, hD2, wD1, wD2;

            if (bmp1.Height < bmp2.Height)
            {
                height = bmp1.Height;
                hD1 = 0;
                hD2 = deplacement[1];
            }
            else
            {
                height = bmp2.Height;
                hD1 = deplacement[1];
                hD2 = 0;
            }

            if (bmp1.Width < bmp2.Width)
            {
                width = bmp1.Width;
                wD1 = 0;
                wD2 = deplacement[0];
            }
            else
            {
                width = bmp2.Width;
                wD1 = deplacement[0];
                wD2 = 0;
            }

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[width, height];

            Bitmap bmp = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x + wD1, y + hD1);
                    System.Drawing.Color pixel2 = bmp2.GetPixel(x + wD2, y + hD2);


                    int colorValue = (int)Math.Round(Math.Sqrt(Math.Pow((pixel1.R - pixel2.R), 2) + Math.Pow((pixel1.G - pixel2.G),2) + Math.Pow((pixel1.B - pixel2.B),2))/3);

                    pixelArray[x, y] = getHysteresis(255 - colorValue);

                    bmp.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return bmp;
        }

        private Bitmap getGlobalCorrelation(Bitmap bmp1, Bitmap bmp2)
        {
            int width, height;

            int[] deplacement = getDepacement(bmp1, bmp2);
            int hD1, hD2, wD1, wD2;

            if (bmp1.Height < bmp2.Height)
            {
                height = bmp1.Height;
                hD1 = 0;
                hD2 = deplacement[1];

            }
            else
            {
                height = bmp2.Height;
                hD1 = deplacement[1];
                hD2 = 0;

            }

            if (bmp1.Width < bmp2.Width)
            {
                width = bmp1.Width;
                wD1 = 0;
                wD2 = deplacement[0];

            }
            else
            {
                width = bmp2.Width;
                wD1 = deplacement[0];
                wD2 = 0;

            }

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[width, height];

            Bitmap bmp = new Bitmap(width, height);

            long acc1 = 0, acc2 = 0;
            int mean1 = 0, mean2 = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x + wD1, y + hD1);

                    acc1 += (Math.Abs(pixel1.R) + Math.Abs(pixel1.G) + Math.Abs(pixel1.B))/3;
                }
            }

            mean1 = (int)(acc1 / (width * height));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel2 = bmp2.GetPixel(x + wD2, y + hD2);

                    acc2 += (Math.Abs(pixel2.R) + Math.Abs(pixel2.G) + Math.Abs(pixel2.B))/3;
                }
            }

            mean2 = (int)(acc2 / (width * height));

            

            double dev1, dev2;

            acc1 = 0;
            acc2 = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x + wD1, y + hD1);

                    acc1 += (int)Math.Pow(((Math.Abs(pixel1.R - mean1) + Math.Abs(pixel1.G - mean1) + Math.Abs(pixel1.B- mean1)) / 3), 2);
                }
            }

            dev1 = Math.Sqrt(acc1/mean1);

            Console.WriteLine(dev1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel2 = bmp2.GetPixel(x + wD2, y + hD2);

                    acc2 += (int)Math.Pow(((Math.Abs(pixel2.R - mean2) + Math.Abs(pixel2.G - mean2) + Math.Abs(pixel2.B - mean2)) / 3), 2);
                }
            }

            dev2 = Math.Sqrt(acc2 / mean2);

            Console.WriteLine((acc1/mean1) + " " + (acc2/mean2));

            Console.WriteLine(dev2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x + wD1, y + hD1);
                    System.Drawing.Color pixel2 = bmp2.GetPixel(x + wD2, y + hD2);

                    // ????????????????

                    int diff1 = (Math.Abs(pixel1.R - mean1) + Math.Abs(pixel1.G - mean1) + Math.Abs(pixel1.B - mean1))/3;
                    int diff2 = (Math.Abs(pixel2.R - mean2) + Math.Abs(pixel2.G - mean2) + Math.Abs(pixel2.B - mean2))/3;

                    double cov = diff1 * diff2;

                    double index = cov / (dev1 * dev2);

                    // we turn the correlation into a grey scale (0-255)

                    pixelArray[x, y] = System.Drawing.Color.FromArgb((int)Math.Abs(index*255), (int)Math.Abs(index * 255), (int)Math.Abs(index * 255));

                    bmp.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return bmp;
        }

        private System.Drawing.Color getGrey(int colorValue)
        {
            return (System.Drawing.Color.FromArgb(colorValue, colorValue, colorValue));
        }

        private System.Drawing.Color getGreyThreshold(int colorValue)
        {
            int threshold = 200;

            if (colorValue < threshold)
            {
                // return white

                return (System.Drawing.Color.FromArgb(0, 0, 0));
            }
            else
            {
                // reuturn grey value

                return (System.Drawing.Color.FromArgb(colorValue, colorValue, colorValue));
            }
        }

        private System.Drawing.Color getThreshold(int colorValue)
        {
            int threshold = 200;

            if (colorValue < threshold)
            {
                // return white

                return (System.Drawing.Color.FromArgb(0, 0, 0));
            }else
            {
                // return black

                return (System.Drawing.Color.FromArgb(255, 255, 255));
            }
        }

        private System.Drawing.Color getHysteresis(int colorValue)
        {
            int downThreshold = 180, upThreshold = 220;

            if (drawing)
            {
                if (colorValue < downThreshold)
                {
                    drawing = false;
                    return (System.Drawing.Color.FromArgb(0, 0, 0));
                }
                else
                {
                    return (System.Drawing.Color.FromArgb(255, 255, 255));
                }
            }
            else
            {
                if (colorValue > upThreshold)
                {
                    drawing = true;
                    return (System.Drawing.Color.FromArgb(255, 255, 255));
                }
                else
                {
                    return (System.Drawing.Color.FromArgb(0, 0, 0));
                }
            }
        }

        private int[] getDepacement(Bitmap bmp1, Bitmap bmp2)
        {
            int[] d = new int[2];

            d[0] = Math.Abs((bmp1.Width-bmp2.Width)/2);
            d[1] = Math.Abs((bmp1.Height - bmp2.Height) / 2);

            return d;
        }

        private Bitmap[] findReposition(Bitmap bmp1, Bitmap bmp2, int[] d)
        {
            // set range of values to prove

            int x1, y1, x2, y2, height, width;
            Bitmap[] bitmapArray = new Bitmap[2];

            if (d[0] == 0) { x1 = 1; x2 = 1; width = bmp1.Width; }
            else
            {
                if (bmp1.Width > bmp2.Width) { x2 = d[0]; x1 = 1; width = bmp1.Width; }
                else { x1 = d[0]; x2 = 1; width = bmp2.Width; }
            }

            if (d[1] == 0) { y1 = 1; y2 = 1; height = bmp1.Height; }
            else
            {
                if (bmp1.Height > bmp2.Height) { y2 = d[1]; y1 = 1; height = bmp1.Height; }
                else { y1 = d[0]; y2 = 1; height = bmp2.Height; }
            }

            int[] bestPositions = new int[2];
            int bestDifference = 255 * height * width;
             
            // deplace 1 in relation to 2

            for(int i=0; i<y1; i++)
            {
                for(int j=0; j<x1; j++)
                {
                    int diff = 0;

                    // match pixel by pixel 
                    for(int h=0; h<height; h++)
                    {
                        for(int w=0; w<width; w++)
                        {
                            System.Drawing.Color pixel1, pixel2;

                            if (h < i || w < j) { pixel1 = System.Drawing.Color.FromArgb(0, 0, 0); }
                            else if (h > i + bmp1.Height || w > j + bmp1.Width) { pixel1 = System.Drawing.Color.FromArgb(0, 0, 0); }
                            else { pixel1 = bmp1.GetPixel(w-j, h - i); }
                            if (h > bmp2.Height || w > bmp2.Width) {pixel2 = System.Drawing.Color.FromArgb(0, 0, 0); }
                            else{ pixel2 = bmp2.GetPixel(w, h);}

                            diff += (Math.Abs(pixel1.R - pixel2.R) + Math.Abs(pixel1.G - pixel2.G) + Math.Abs(pixel1.B - pixel2.B))/3;
                        }
                    }

                    if (diff < bestDifference) { bestDifference = diff; bestPositions[0] = j; bestPositions[1] = j; }
                }
            }

            bitmapArray[0] = executeReposition(bmp1, bestPositions,  height, width);

            // deplace 2 in relation to 1

            bestPositions = new int[2];
            bestDifference = 255 * height * width;

            for (int i = 0; i < y2; i++)
            {
                for (int j = 0; j < x2; j++)
                {
                    int diff = 0;

                    // match pixel by pixel 
                    for (int h = 0; h < height; h++)
                    {
                        for (int w = 0; w < width; w++)
                        {
                            System.Drawing.Color pixel1, pixel2;

                            if (h < i || w < j) { pixel2 = System.Drawing.Color.FromArgb(0, 0, 0); }
                            else if (h > i + bmp2.Height || w > j + bmp2.Width) { pixel2 = System.Drawing.Color.FromArgb(0, 0, 0); }
                            else { pixel2 = bmp2.GetPixel(w - j, h - i); }
                            if (h > bmp1.Height || w > bmp1.Width) { pixel1 = System.Drawing.Color.FromArgb(0, 0, 0); }
                            else { pixel1 = bmp1.GetPixel(w, h); }

                            diff += (Math.Abs(pixel1.R - pixel2.R) + Math.Abs(pixel1.G - pixel2.G) + Math.Abs(pixel1.B - pixel2.B)) / 3;
                        }
                    }

                    if (diff < bestDifference) { bestDifference = diff; bestPositions[0] = j; bestPositions[1] = j; }
                }
            }

            bitmapArray[1] = executeReposition(bmp2, bestPositions, height, width);

            return bitmapArray;
        }

        private Bitmap executeReposition(Bitmap bmp1, int[] bestPositions, int h, int w)
        {
            Bitmap b = new Bitmap(h, w);

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[h, w];

            for(int y = 0; y<h; y++)
            {
                for(int x = 0; x<w; x++)
                {
                    if (y < bestPositions[1] || w < bestPositions[0]) { pixelArray[x,y] = System.Drawing.Color.FromArgb(0, 0, 0); }
                    else if (h > bestPositions[1] + bmp1.Height || w > bestPositions[0] + bmp1.Width) { pixelArray[x,y] = System.Drawing.Color.FromArgb(0, 0, 0); }
                    else { pixelArray[x, y] = bmp1.GetPixel(x - bestPositions[0], h - bestPositions[1]); }

                    b.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return b;
        }
    }
}
