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
    /// Interaction logic for CapturePreviews.xaml
    /// </summary>
    public partial class CapturePreviews : Page
    {
        private MainPage mainPage;
        private List<CaptureWindow> capturesList;
        private List<Image> samplesList;

        public CapturePreviews(MainPage mp)
        {
            mainPage = mp;
            
            InitializeComponent();

            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            System.Windows.Controls.Image navImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "flechaIz.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            navImg.Source = src;
            navImg.Stretch = Stretch.Uniform;

            navigationSp.Children.Add(navImg);

            navigationSp.MouseEnter += new MouseEventHandler(navigationArrowEnter);
            navigationSp.MouseLeave += new MouseEventHandler(navigationArrowLeave);

            navigationSp.MouseDown += new MouseButtonEventHandler(navigationArrowClick);

            capturesList = new List<CaptureWindow>();
            samplesList = new List<Image>();

        }

        public void AddCaptures(List<CaptureWindow> cwList, List<int> ind)
        {
            MainCapture mc = new MainCapture();
            List<Image> samples = new List<Image>();
            samples = mc.Samples(ind);

            for (int i = 0; i < cwList.Count; i++)
            {
                capturesList.Add(cwList.ElementAt(i));
                capturesList.ElementAt(i).Uid = (capturesList.Count -1).ToString();
                samplesList.Add(samples.ElementAt(i));
                samplesList.ElementAt(i).Uid = (samplesList.Count - 1).ToString();
            }

            
            
            sampleSP.Children.Add(samplesList.ElementAt(0));
           
        }

        private void navigationArrowEnter(object sender, MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;
            senderBut.Width = senderBut.Width * 1.1;
            senderBut.Height = senderBut.Height * 1.1;

        }

        private void showDetails(object sender, RoutedEventArgs e)
        {
            for(int i=0; i<capturesList.Count; i++)
            {
                capturesList.ElementAt(i).Show();
            }
        }

        private void navigationArrowLeave(object sender, MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;
            senderBut.Width = senderBut.Width / 1.1;
            senderBut.Height = senderBut.Height / 1.1;

        }

        private void navigationArrowClick(object sender, MouseEventArgs e)
        {
            this.NavigationService.Navigate(mainPage);

        }
    }
}
