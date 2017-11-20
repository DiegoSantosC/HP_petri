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
using System.Threading;
using System.Windows.Navigation;



namespace PetriUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainPage : Page
    {

        public static Thread captureThread;
        public static int numberOfCaptures, minutesInterval, hoursInterval, interval;

        public MainPage()
        {
            InitializeComponent();


            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            objectShowStackPanel.Visibility = Visibility.Collapsed;
        }
        
        private void objectShow_Button_Click(object sender, RoutedEventArgs e)
        {
            MainCapture newCapture = new MainCapture();
            OutlineParameters op = newCapture.ConfirmCapture();

            int size = OutlineParameters.getCapacity(op);
            for(int i=0; i<size; i++)
            {

            }

            objectShowStackPanel.Visibility = Visibility.Visible;

            Image confirmImg = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(PictureHandling.confirmPath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            confirmImg.Source = src;
            confirmImg.Stretch = Stretch.Uniform;

            objectShowStackPanel.Children.Add(confirmImg);
        }
        
        /*
        private void CaptureStart_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((Int32.TryParse(minutesTextBox.Text, out minutesInterval)) && (Int32.TryParse(hoursTextBox.Text, out hoursInterval)) && (Int32.TryParse(numberOfCapturesTextBox.Text, out numberOfCaptures)))
            {
                if (((minutesInterval == 0) && (hoursInterval == 0)) || (numberOfCaptures == 0))
                {
                    MessageBox.Show("Insert valid parameter values");
                }
                else
                {
                    MainCapture newCapture = new MainCapture();

                    captureThread = new Thread(newCapture.StartCapture);
                    captureThread.Start();

                    interval = hoursInterval * 60 + minutesInterval;
                    CapturePage cp = new CapturePage(numberOfCaptures, interval);
                    this.NavigationService.Navigate(cp);
                }
            }
            else
            {
                MessageBox.Show("Parameter parsing error");
            }

        }
        */
    }

}
