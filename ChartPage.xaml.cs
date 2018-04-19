using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for ChartPage.xaml
    /// 
    /// Funcionalities : UI that shows the general results of the counting analysis
    /// regarding number and size of the colonies
    /// 
    /// Launched by being selected through the Picker
    /// 
    /// Receives data from countAnalytics
    /// 
    /// </summary>
    /// 

    public partial class ChartPage : Page
    {
        private List<System.Windows.Controls.Image> chartsList;
        private List<System.Windows.Controls.Image> imagesList;
        private List<System.Windows.Shapes.Rectangle> bbList;


        // Empty initialization
        public ChartPage()
        {
            InitializeComponent();

            chartsList = new List<System.Windows.Controls.Image>();
            imagesList = new List<System.Windows.Controls.Image>();
            bbList = new List<System.Windows.Shapes.Rectangle>();
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Charts management
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        // Page's content is originated when the page is shown, with a link to the source of the data that needs to be presented

        public void initCharts(CountAnalytics analysisDataSource, string _saveDir)
        {
            // TO BE FIXES!!!! SEVERAL ENTRIES????

            double[] countYData = analysisDataSource.getColonyCountData();

            barCharConstruct(countYData, _saveDir, new string[] { "Colony Count Chart", "Number of colonies", "Step" });

            object[] returnable = analysisDataSource.getColonySizeData();

            List<double[]> sizeYData = (List<double[]>)returnable[0];
            List<int[]> bbs = (List<int[]>)returnable[1];
            List<Bitmap> images = (List<Bitmap>)returnable[2];

            bbList = convertToFormat(bbs, images[0].Width, images[0].Height);
            imagesList = convertToFormat(images);

            for (int i = 0; i < sizeYData.Count; i++)
            {
                ListBoxItem lbItem = new ListBoxItem();
                System.Windows.Controls.Label l = new System.Windows.Controls.Label();
                l.Content = "Colony " + i.ToString();

                lbItem.Content = l;

                colonyListBox.Items.Add(lbItem);

                chartsList.Add(barCharConstruct(sizeYData[i], _saveDir, i, new string[] { "Colony " + i + " Size Chart", "Size (mm)", "Step" }));

            }

            Init_List();

        }

        public void barCharConstruct(double[] yData, string _saveDirectory, string[] title)
        {
            Chart ch = new Chart();
            ChartArea area1 = new ChartArea("BarChart");

            ch.ChartAreas.Add(area1);
            Series s = new Series();
            s.Points.DataBindY(yData);

            s.ChartType = SeriesChartType.Column;
            s.ChartArea = "BarChart";

            ch.Series.Add(s);
            ch.ChartAreas[0].AxisX.Title = title[2];
            ch.ChartAreas[0].AxisY.Title = title[1];


            ch.Titles.Add(new Title(title[0]));

            string dir = System.IO.Path.Combine(_saveDirectory, "colonyChart.png");

            ch.SaveImage(dir, ChartImageFormat.Png);

            System.Windows.Controls.Image chart = new System.Windows.Controls.Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(dir, UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            chart.Source = src2;
            chart.Stretch = Stretch.Uniform;
            CountSP.Children.Add(chart);

        }

        public System.Windows.Controls.Image barCharConstruct(double[] yData, string _saveDirectory, int index, string[] title)
        {
            Chart ch = new Chart();
            ChartArea area1 = new ChartArea("BarChart");

            ch.ChartAreas.Add(area1);
            Series s = new Series();
            s.Points.DataBindY(yData);

            s.ChartType = SeriesChartType.Column;
            s.ChartArea = "BarChart";

            ch.Series.Add(s);
            ch.ChartAreas[0].AxisX.Title = title[2];
            ch.ChartAreas[0].AxisY.Title = title[1];


            ch.Titles.Add(new Title(title[0]));

            string dir = System.IO.Path.Combine(_saveDirectory, "Colony" + index + ".png");

            ch.SaveImage(dir, ChartImageFormat.Png);

            System.Windows.Controls.Image chart = new System.Windows.Controls.Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(dir, UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            chart.Source = src2;
            chart.Stretch = Stretch.Uniform;

            return chart;
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      UI related functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void Init_List()
        {
            colonyListBox.SelectedIndex = 0;

            ColonySizesCanvas.Children.Add(chartsList[0]);

            colonyListBox.SelectionChanged += new SelectionChangedEventHandler(listBoxClicked);

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();

            r.Width = 350;
            r.Height = 300;
            r.Stroke = System.Windows.Media.Brushes.Gray;
            r.StrokeThickness = 3;

            ColonySizesCanvas.Children.Add(r);

            ImageCanvas.Children.Add(imagesList[0]);
            BbCanvas.Children.Add(bbList[0]);

            System.Windows.Shapes.Rectangle r2 = new System.Windows.Shapes.Rectangle();

            r2.Width = 350;
            r2.Height = 350;
            r2.Stroke = System.Windows.Media.Brushes.Gray;
            r2.StrokeThickness = 3;

            BbCanvas.Children.Add(r2);
        }

        private void listBoxClicked(object sender, SelectionChangedEventArgs e)
        {

            System.Windows.Controls.ListBox lb = (System.Windows.Controls.ListBox)sender;
            ListBoxItem item = (ListBoxItem)lb.SelectedItem;

            System.Windows.Controls.Label l = (System.Windows.Controls.Label)item.Content;

            int index = Int32.Parse(l.Content.ToString().Split(' ')[1]);

            ColonySizesCanvas.Children.Clear();
            ColonySizesCanvas.Children.Add(chartsList[index]);

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();

            r.Width = 350;
            r.Height = 300;
            r.Stroke = System.Windows.Media.Brushes.Gray;
            r.StrokeThickness = 3;

            ColonySizesCanvas.Children.Add(r);

            ImageCanvas.Children.Clear();
            BbCanvas.Children.Clear();

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = imagesList[index].Source;

            ImageCanvas.Children.Add(img);
            BbCanvas.Children.Add(bbList[index]);

            System.Windows.Shapes.Rectangle r2 = new System.Windows.Shapes.Rectangle();

            r2.Width = 350;
            r2.Height = 350;
            r2.Stroke = System.Windows.Media.Brushes.Gray;
            r2.StrokeThickness = 3;

            BbCanvas.Children.Add(r2);

        }

        private List<System.Windows.Shapes.Rectangle> convertToFormat(List<int[]> bbs, int w, int h)
        {
            List<System.Windows.Shapes.Rectangle> rectList = new List<System.Windows.Shapes.Rectangle>();

            for (int i = 0; i < bbs.Count; i++)
            {
                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();

                // Rectangles' size and position is scaled 

                r.Width = ((bbs[i][2] - bbs[i][0]) * 350 / w) + 5;
                r.Height = ((bbs[i][3] - bbs[i][1]) * 350 / h) + 5;

                r.Margin = new Thickness((bbs[i][0] * 350 / w) + -2, (bbs[i][1] * 350 / h) - 2, 0, 0);

                r.Stroke = System.Windows.Media.Brushes.Green;
                r.StrokeThickness = 3;

                rectList.Add(r);
            }

            return rectList;
        }

        private List<System.Windows.Controls.Image> convertToFormat(List<Bitmap> images)
        {
            List<System.Windows.Controls.Image> imgs = new List<System.Windows.Controls.Image>();

            for (int i = 0; i < images.Count; i++)
            {
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();

                IntPtr ip = images[i].GetHbitmap();

                BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                img.Source = src;

                imgs.Add(img);

            }

            return imgs;
        }

        public Bitmap Resize(Bitmap originalImage)
        {
            int _nSize = 350;

            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, _nSize, _nSize);
            Bitmap destImage = new Bitmap(_nSize, _nSize);

            destImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(destImage, destRect, 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
