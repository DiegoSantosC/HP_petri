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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for Picker.xaml
    /// 
    /// Functionality : Toggle between the different analysis pages
    /// 
    /// Launched by: Analysis Window
    /// 
    /// Launches : ClassPage, CountAnalytics and ClassAnalytics
    /// 
    /// </summary>
    /// 
    public partial class Picker : Page
    {
        private CaptureWindow captureWindow;
        private bool countAnalysis, classAnalysis;
        private ClassifyAnalytics clssPage;
        private CountAnalytics countPage;
        private ChartPage chartPage;

        // UI initialization with a link to every analysis related interface

        public Picker(CaptureWindow cw, bool count, bool classAn, ClassifyAnalytics clss, CountAnalytics cnt, ChartPage chp)
        {
            InitializeComponent();

            captureWindow = cw;
            countAnalysis = count;
            classAnalysis = classAn;

            Logo_Init();

            Panel_Logic_Init();

            clssPage = clss;
            countPage = cnt;

            chartPage = chp;
        }

        // Each stackPanel, when clicked, holds the navigation to the analysis feature it represents
        private void Panel_Logic_Init()
        {
            classSP.MouseEnter += new MouseEventHandler(spEnter);
            classSP.MouseLeave += new MouseEventHandler(spLeave);

            countSP.MouseEnter += new MouseEventHandler(spEnter);
            countSP.MouseLeave += new MouseEventHandler(spLeave);

            chartSP.MouseEnter += new MouseEventHandler(spEnter);
            chartSP.MouseLeave += new MouseEventHandler(spLeave);

            classSP.MouseDown += new MouseButtonEventHandler(classClick);
            countSP.MouseDown += new MouseButtonEventHandler(countClick);
            chartSP.MouseDown += new MouseButtonEventHandler(chartClick);

            Image count = new Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Counting.jpg", UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            count.Source = src2;
            count.Stretch = Stretch.Uniform;
            countSP.Children.Add(count);

            Image chart = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\ClassIcon.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            chart.Source = src;
            chart.Stretch = Stretch.Uniform;
            chartSP.Children.Add(chart);

            Image classif = new Image();
            BitmapImage src3 = new BitmapImage();
            src3.BeginInit();
            src3.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Grid.png", UriKind.Absolute);
            src3.CacheOption = BitmapCacheOption.OnLoad;
            src3.EndInit();
            classif.Source = src3;
            classif.Stretch = Stretch.Uniform;
            classSP.Children.Add(classif);
        }

        // Navigation clicking handlers

        private void chartClick(object sender, MouseButtonEventArgs e)
        {
            if (countAnalysis)
            {
                this.NavigationService.Navigate(chartPage);
            }
        }

        private void countClick(object sender, MouseButtonEventArgs e)
        {
            if (countAnalysis)
            {
                countPage.Show(0);
                this.NavigationService.Navigate(countPage);
            }
        }

        private void classClick(object sender, MouseButtonEventArgs e)
        {
            if (classAnalysis)
            {
                clssPage.ArtificialScroll();
                this.NavigationService.Navigate(clssPage);
            }
        }

        private void spLeave(object sender, MouseEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;
            sp.Opacity = 1;
        }

        private void spEnter(object sender, MouseEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;
            sp.Opacity = 0.8;
        }

        private void Logo_Init()
        {
            System.Windows.Controls.Image logo = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\HP_logo.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            logo.Source = src;
            logo.Stretch = Stretch.Uniform;

            LogoSp.Children.Add(logo);

        }
        
    }
}
