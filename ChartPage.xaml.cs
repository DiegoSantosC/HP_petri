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

        // Empty initialization
        public ChartPage()
        {
            InitializeComponent();
        }

        // Page's content is originated when the page is shown, with a link to the source of the data that needs to be presented

        public void initCharts(CountAnalytics analysisDataSource, string _saveDir)
        {
            double[] countYData = analysisDataSource.getColonyCountData();

            barCharConstruct(countYData, _saveDir, new string[] { "Colony Count Chart", "Number of colonies", "Step" });

            List<double[]> sizeYData = analysisDataSource.getColonySizeData();
       
            for (int i=0; i < sizeYData.Count; i++)
            {
                StackPanel sp = new StackPanel();
                sp.Width = 300;
                sp.Height = 230;

                sp.Margin = new Thickness(305 * (i/3), 235 * (i%3), 0, 0);

                Rectangle r = new Rectangle();

                r.Width = 300;
                r.Height = 230;

                r.Margin = new Thickness(305 * (i / 3), 235 * (i % 3), 0, 0);

                r.Stroke = Brushes.Gray;
                r.StrokeThickness = 3;

                ColonySizesCanvas.Children.Add(sp);
                ColonySizesCanvas.Children.Add(r);

                barCharConstruct(sizeYData[i], _saveDir, i, new string[] { "Colony Size Chart", "Size (mm)", "Step" }, sp);
            }
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

            Image chart = new Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(dir, UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            chart.Source = src2;
            chart.Stretch = Stretch.Uniform;
            CountSP.Children.Add(chart);
        }

        public void barCharConstruct(double[] yData, string _saveDirectory, int index, string[] title, StackPanel sp)
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

            Image chart = new Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(dir, UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            chart.Source = src2;
            chart.Stretch = Stretch.Uniform;
            sp.Children.Add(chart);

        }
    }
}
