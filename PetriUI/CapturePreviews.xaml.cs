/* © Copyright 2018 HP Inc.
*
*  Permission is hereby granted, free of charge, to any person obtaining a copy
*  of this software and associated documentation files (the "Software"), to deal
*  in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
*  all copies or substantial portions of the Software.
*
*  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*  THE SOFTWARE.
*/

// .NET framework namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sprout SDK namespaces
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
    /// 
    /// Interaction logic for CapturePreviews.xaml
    /// 
    /// Functionality: showing the user running captures, allowing to
    /// view detailed information about each of them
    /// 
    /// Launched by: MainPage
    /// Launches : One CaptureWindow for each capture
    /// 
    /// </summary>
    public partial class CapturePreviews : Page
    {
        private MainPage mainPage;
        private static List<CaptureWindow> capturesList;
        private static List<Image> samplesList;
        private static int numberCapturesRunning;

        // This Page is initialized without content, and thus will be shown and made avaliable 
        // when the first capture starts to run

        public CapturePreviews(MainPage mp)
        {
            mainPage = mp;
            
            InitializeComponent();

            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            numberCapturesRunning = 0;

            PageNavigation_Init();

            Logo_Init();

            ScrollLeft_Init();

            scrollRight_Init();

            emptyLabel.Visibility = Visibility.Hidden;
            emptyLabel.Content = emptyLabel.Content + Environment.NewLine + "Go to Capture Settings for new Captures";
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

            LogoSP.Children.Add(logo);
        }


        // Navigation to MainPage 
        private void PageNavigation_Init()
        {
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

        }

        // Navigation functionality definitions
        private void navigationArrowEnter(object sender, MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;

            foreach (object child in senderBut.Children)
            {
                Image childImg = (Image)child;

                childImg.Opacity = 0.7;
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


        // Scroll left elements and functionality
        private void ScrollLeft_Init()
        {
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
        }

        // Scroll right elements and functionality
        private void scrollRight_Init()
        {
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


        // Scrolling functionality definitions
        private void scrollRight(object sender, MouseButtonEventArgs e)
        {
            String s = infoLabel.Content.ToString();
            String[] data = s.Split('/');

            int current = Int32.Parse(data[0]);
            int total = Int32.Parse(data[1]);

            if (current == total)
            {
                sampleSP.Children.Clear();
                try
                {
                    sampleSP.Children.Add(samplesList.ElementAt(0));
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = "1/" + capturesList.Count.ToString();
            }else
            {
                sampleSP.Children.Clear();
                try
                {
                    sampleSP.Children.Add(samplesList.ElementAt(current));
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = (current+1) + "/" + capturesList.Count.ToString();
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
                
                try
                {
                    sampleSP.Children.Add(samplesList.ElementAt(total - 1));
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = total + "/" + capturesList.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();
               
                try
                {
                    sampleSP.Children.Add(samplesList.ElementAt(current - 2));
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = (current - 1) + "/" + capturesList.Count.ToString();
            }

        }

        private void ArtficialScroll()
        {
            String s = infoLabel.Content.ToString();
            String[] data = s.Split('/');

            int current = Int32.Parse(data[0]);
            int total = Int32.Parse(data[1]);

            
            sampleSP.Children.Clear();
            sampleSP.Children.Add(samplesList.ElementAt(0));

            infoLabel.Content = "1/" + capturesList.Count.ToString();
        }

        // Captures management
        public static void DecrementCaptures()
        {
            numberCapturesRunning--;
            if (numberCapturesRunning == 0)
            {
                MainPage.capturesRunning = false;
            }
        }

        public void AddCaptures(List<int[]> parameters, List<int> ind, List<string> folders)
        {
            MainCapture mc = new MainCapture();
            List<Image> samples = new List<Image>();
            samples = mc.Samples(folders, ind);

            for (int i = 0; i < parameters.Count; i++)
            {
                capturesList.Add(new CaptureWindow(this, samples.ElementAt(i), parameters.ElementAt(parameters.Count - 1 -i), folders.ElementAt(folders.Count -1 -i)));
                capturesList.ElementAt(i).Uid = (i).ToString();
                samplesList.Add(samples.ElementAt(i));
                samplesList.ElementAt(i).Uid = (i).ToString();

                numberCapturesRunning++;
            }

            sampleSP.Children.Clear();
            sampleSP.Children.Add(samplesList.ElementAt(0));

            infoLabel.Content = "1/" + capturesList.Count.ToString();

            MainPage.capturesRunning = true;

            Border1.Visibility = Visibility.Visible;
            Border2.Visibility = Visibility.Visible;
            sampleSP.Visibility = Visibility.Visible;
            infoLabel.Visibility = Visibility.Visible;

            ShowButton.IsEnabled = true;
        }
        
        public static void killAllCaptures()
        {
            for (int i = 0; i < capturesList.Count; i++)
            {
                capturesList.ElementAt(i).KillCaptures();
                capturesList.ElementAt(i).Close();
            }
        }

        public void EraseFinishedCapture(int index)
        {
            for(int j=0; j<capturesList.Count; j++)
            {
                if (Int32.Parse(capturesList.ElementAt(j).Uid) == index) capturesList.RemoveAt(j);

            }
            for (int j = 0; j < samplesList.Count; j++)
            {
                if (Int32.Parse(samplesList.ElementAt(j).Uid) == index) samplesList.RemoveAt(j);

            }

            int i = 0;

            foreach (object child in sampleSP.Children)
            {
                Image childImg = (Image)child;
                i = Int32.Parse(childImg.Uid);
            }

            if (i == index)
            {
                if (capturesList.Count > 0)
                {
                    ArtficialScroll();
                }
                else
                {
                    emptyLabel.Visibility = Visibility.Visible;
                    infoLabel.Visibility = Visibility.Hidden;
                    sampleSP.Visibility = Visibility.Hidden;

                    Border1.Visibility = Visibility.Hidden;
                    Border2.Visibility = Visibility.Hidden;

                    ShowButton.IsEnabled = false;
                }
            }
        }

        // Link with CaptureWindow interface
        private void showDetails(object sender, RoutedEventArgs e)
        {
            int index;

            foreach (object child in sampleSP.Children)
            {
                Image childImg = (Image)child;
                index = Int32.Parse(childImg.Uid);

                foreach (CaptureWindow element in capturesList)
                {
                    if(Int32.Parse(element.Uid) == index) element.Show();
                }                     
            }   
        }

        
    }
}
