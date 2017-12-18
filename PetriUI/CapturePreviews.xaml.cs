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
        private static List<CaptureWindow> capturesList;
        private static List<Image> samplesList;
        private static int numberCapturesRunning;
        public CapturePreviews(MainPage mp)
        {
            mainPage = mp;
            
            InitializeComponent();

            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            numberCapturesRunning = 0;

            // Navigate to previous page

            System.Windows.Controls.Image navImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\flechaIz.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            navImg.Source = src;
            navImg.Stretch = Stretch.Uniform;

            navigationSp.Children.Add(navImg);

            navigationSp.MouseEnter += new MouseEventHandler(navigationArrowEnter);
            navigationSp.MouseLeave += new MouseEventHandler(navigationArrowLeave);

            navigationSp.MouseDown += new MouseButtonEventHandler(navigationArrowClick);

            // Scroll Left

            System.Windows.Controls.Image rightImg = new System.Windows.Controls.Image();
            BitmapImage srcRight = new BitmapImage();
            srcRight.BeginInit();
            srcRight.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Blue_Right.png", UriKind.Absolute);
            srcRight.CacheOption = BitmapCacheOption.OnLoad;
            srcRight.EndInit();
            rightImg.Source = srcRight;
            rightImg.Stretch = Stretch.Uniform;

            rightSp.Children.Add(rightImg);

            rightSp.MouseEnter += new MouseEventHandler(navigationArrowEnter);
            rightSp.MouseLeave += new MouseEventHandler(navigationArrowLeave);

            rightSp.MouseDown += new MouseButtonEventHandler(scrollRight);

            // Scroll Right

            System.Windows.Controls.Image LeftImg = new System.Windows.Controls.Image();
            BitmapImage srcLeft = new BitmapImage();
            srcLeft.BeginInit();
            srcLeft.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Blue_Left.png", UriKind.Absolute);
            srcLeft.CacheOption = BitmapCacheOption.OnLoad;
            srcLeft.EndInit();
            LeftImg.Source = srcLeft;
            LeftImg.Stretch = Stretch.Uniform;

            leftSp.Children.Add(LeftImg);

            leftSp.MouseEnter += new MouseEventHandler(navigationArrowEnter);
            leftSp.MouseLeave += new MouseEventHandler(navigationArrowLeave);

            leftSp.MouseDown += new MouseButtonEventHandler(scrollLeft);

            capturesList = new List<CaptureWindow>();
            samplesList = new List<Image>();
            
        }

        private void scrollRight(object sender, MouseButtonEventArgs e)
        {
            String s = infoLabel.Content.ToString();
            String[] data = s.Split('/');

            int current = Int32.Parse(data[0]);
            int total = Int32.Parse(data[1]);

            if (current == total)
            {
                sampleSP.Children.Clear();
                sampleSP.Children.Add(samplesList.ElementAt(0));

                infoLabel.Content = "1/" + capturesList.Count.ToString();
            }else
            {
                sampleSP.Children.Clear();
                sampleSP.Children.Add(samplesList.ElementAt(current));

                infoLabel.Content = (current+1) + "/" + capturesList.Count.ToString();
            }

        }
        public static void DecrementCaptures()
        {
            numberCapturesRunning--;
            if (numberCapturesRunning == 0)
            {
                MainPage.capturesRunning = false;
            }
        }

        private void scrollLeft(object sender, MouseButtonEventArgs e)
        {
            String s = infoLabel.Content.ToString();
            String[] data = s.Split('/');

            int current = Int32.Parse(data[0]);
            int total = Int32.Parse(data[1]);

            if (current == 1)
            {
                sampleSP.Children.Clear();
                sampleSP.Children.Add(samplesList.ElementAt(total-1));

                infoLabel.Content = total + "/" + capturesList.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();
                sampleSP.Children.Add(samplesList.ElementAt(current -2));

                infoLabel.Content = (current - 1) + "/" + capturesList.Count.ToString();
            }

        }

        public void AddCaptures(List<int[]> parameters, List<int> ind)
        {
            MainCapture mc = new MainCapture();
            List<Image> samples = new List<Image>();
            samples = mc.Samples(ind);

            for (int i = 0; i < parameters.Count; i++)
            {
                capturesList.Add(new CaptureWindow(samples.ElementAt(samples.Count - 1 - i), parameters.ElementAt(i)));
                capturesList.ElementAt(i).Uid = (capturesList.Count -1).ToString();
                samplesList.Add(samples.ElementAt(samples.Count-1-i));
                samplesList.ElementAt(i).Uid = (samplesList.Count - 1).ToString();

                numberCapturesRunning++;
            }

            sampleSP.Children.Clear();
            sampleSP.Children.Add(samplesList.ElementAt(0));

            infoLabel.Content = "1/" + capturesList.Count.ToString();

            MainPage.capturesRunning = true;
           
        }

        private void navigationArrowEnter(object sender, MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;
   
            foreach (object child in senderBut.Children)
            {
                Image childImg = (Image)child;

                childImg.Opacity = 0.7;
            }

        }

        private void showDetails(object sender, RoutedEventArgs e)
        {
            int index;

            foreach (object child in sampleSP.Children)
            {
                Image childImg = (Image)child;
                index = Int32.Parse(childImg.Uid);

                try
                {
                    capturesList.ElementAt(index).Show();
                }
                catch (InvalidOperationException ex) { }               
            }

            
        }

        private void navigationArrowLeave(object sender, MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;
          
            foreach (object child in senderBut.Children)
            {
                Image childImg = (Image)child;

                childImg.Opacity = 1;
            }

        }

        private void navigationArrowClick(object sender, MouseEventArgs e)
        {
            this.NavigationService.Navigate(mainPage);

        }

        public static void killAllCaptures()
        {
            for(int i=0; i<capturesList.Count; i++)
            {
                capturesList.ElementAt(i).KillCaptures();
                capturesList.ElementAt(i).Close();
            }
        }
    }
}
