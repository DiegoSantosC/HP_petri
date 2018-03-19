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
using hp.pc;
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
        private static MainPage mainPage;
        private static List<CaptureWindow> capturesList;
        private static List<Image> samplesList;
        private static List<string> capturesNames;
        private static int numberCapturesRunning;

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      UI related functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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

        // HP logo initialization

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


        // Navigation to MainPage handler. This feature will always be active to allow new processes to be launched

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

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Building fake carrousel to toggle between processes previews
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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
            capturesNames = new List<string>();
        }


        // Scrolling functionality definitions

        private void scrollRight(object sender, MouseButtonEventArgs e)
        {
            String s = infoLabel.Content.ToString();
            String[] data1 = s.Split(' ');
            String[] data = data1[1].Split('/');

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

                infoLabel.Content = "Process 1/" + capturesList.Count.ToString();
                nameLabel.Content = capturesNames.ElementAt(0);
            }else
            {
                sampleSP.Children.Clear();
                try
                {
                    sampleSP.Children.Add(samplesList.ElementAt(current));
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = "Process " + (current+1) + "/" + capturesList.Count.ToString();
                nameLabel.Content = capturesNames.ElementAt(current);
            }

        }
       
        private void scrollLeft(object sender, MouseButtonEventArgs e)
        {
            String s = infoLabel.Content.ToString();
            String[] data1 = s.Split(' ');
            String[] data = data1[1].Split('/');

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

                infoLabel.Content = "Process " + total + "/" + capturesList.Count.ToString();
                nameLabel.Content = capturesNames.ElementAt(capturesNames.Count - 1);

            }
            else
            {
                sampleSP.Children.Clear();
               
                try
                {
                    sampleSP.Children.Add(samplesList.ElementAt(current - 2));
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = "Process " + (current - 1) + "/" + capturesList.Count.ToString();
                nameLabel.Content = capturesNames.ElementAt(current -2);

            }

        }

        // Artificial Scroll will always be set to the first process that exists

        private void ArtificialScroll()
        {
            String s = infoLabel.Content.ToString();
            String[] data1 = s.Split(' ');
            String[] data = data1[1].Split('/');

            int current = Int32.Parse(data[0]);
            int total = Int32.Parse(data[1]);

            
            sampleSP.Children.Clear();
            sampleSP.Children.Add(samplesList.ElementAt(0));

            infoLabel.Content = "Process 1/" + capturesList.Count.ToString();
            nameLabel.Content = capturesNames.ElementAt(0);
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Capturing processes management
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Capture proccesses decreasing

        public static void DecrementCaptures()
        {
            numberCapturesRunning--;

            if (numberCapturesRunning == 0)
            {
                MainPage.capturesRunning = false;
                mainPage.EnableNewScan();
            }
        }

        // Parameters are received from MainPage listing the features of the new captures to be built
        // In this method they are built and added to the UI

        public void AddCaptures(List<int[]> parameters, List<int> ind, List<string> folders, List<bool[]> analysis, List<string> names, List<PcPhysicalPoint> locations, List<System.Drawing.Point> sizes)
        {
            MainCapture mc = new MainCapture();
            List<Image> samples = new List<Image>();
            List<IPcOutline> outlines;

            object[] returnable = new object[3];

            returnable = mc.Samples(folders, ind, locations, sizes);

            samples = (List<Image>) returnable[0];
            
            outlines = (List<IPcOutline>)returnable[1];

            List<int> moved = (List<int>)returnable[2];

            // Capture Window holds the control of a process. 

            int previousCaptures = capturesList.Count;

            for (int i = 0; i < parameters.Count; i++)
            {
                if(moved[i]!= 2)
                {
                    PcPhysicalPoint location = new PcPhysicalPoint(outlines[i].PhysicalBoundaries.Location.X * (outlines[i].PixelDensity.X), outlines[i].PhysicalBoundaries.Location.Y * (outlines[i].PixelDensity.Y));
                    System.Drawing.Point size = new System.Drawing.Point(Convert.ToInt32(PictureHandling.GetOutlineWidth(outlines[i])), Convert.ToInt32(PictureHandling.GetOutlineHeight(outlines[i])));

                    if (moved[i] == 0)
                    {
                        capturesList.Add(new CaptureWindow(this, samples.ElementAt(i), parameters.ElementAt(parameters.Count - 1 - i), 
                            folders.ElementAt(folders.Count - 1 - i), analysis.ElementAt(analysis.Count - 1 - i), names.ElementAt(names.Count - 1 - i), 
                            location, size, false));

                        Console.WriteLine(names[names.Count - 1 - i] + " " + parameters.ElementAt(parameters.Count - 1 - i)[2]);

                    }
                    else
                    {
                        capturesList.Add(new CaptureWindow(this, samples.ElementAt(i), parameters.ElementAt(parameters.Count - 1 - i), 
                            folders.ElementAt(folders.Count - 1 - i), analysis.ElementAt(analysis.Count - 1 - i), names.ElementAt(names.Count - 1 - i),
                            location, size, true));
                    }

                    capturesList.ElementAt(i + previousCaptures).Uid = (i + previousCaptures).ToString();
                    samplesList.Add(samples.ElementAt(i));
                    samplesList.ElementAt(i + previousCaptures).Uid = (i + previousCaptures).ToString();
                    capturesNames.Add(names.ElementAt(names.Count - 1 - i));

                    numberCapturesRunning++;
                }
                else
                {
                    MessageBox.Show("Capture of process " + names.ElementAt(names.Count - 1 - i) + " could not be accomplished. Object not found.");

                    return;
                }
            }

            // UI modification according to new situation

            sampleSP.Children.Clear();
            sampleSP.Children.Add(samplesList.ElementAt(0));

            infoLabel.Content = "Process 1/" + capturesList.Count.ToString();

            if(capturesList.Count >= 2)
            {
                rightSp.Visibility = Visibility.Visible;
                leftSp.Visibility = Visibility.Visible;

            }

            MainPage.capturesRunning = true;

            Border1.Visibility = Visibility.Visible;
            Border2.Visibility = Visibility.Visible;
            sampleSP.Visibility = Visibility.Visible;
            infoLabel.Visibility = Visibility.Visible;
            nameLabel.Visibility = Visibility.Visible;

            ShowButton.IsEnabled = true;

            // If this is the first time 

            if (parameters.Count == capturesList.Count) ArtificialScroll();
        }
        
        // This function finishes all running processes due to a closing signal from the window, so as not to leave any execution thread alive

        public static void killAllCaptures()
        {
            for (int i = 0; i < capturesList.Count; i++)
            {
                capturesList.ElementAt(i).KillCaptures();
                capturesList.ElementAt(i).Close();
            }
        }

        // Once a capture process is finished and it's handler is closed, the process is whiped from the UI

        public void EraseFinishedCapture(int index)
        {
            int target = index;

            for(int j=0; j<capturesList.Count; j++)
            {
                if (Int32.Parse(capturesList.ElementAt(j).Uid) == index) { capturesList.RemoveAt(j); target = j; }

            }
            for (int j = 0; j < samplesList.Count; j++)
            {
                if (Int32.Parse(samplesList.ElementAt(j).Uid) == index) samplesList.RemoveAt(j);

            }

            capturesNames.RemoveAt(target);

            int i = 0;

            // If the process is being displayed in the preview, a scroll is needed

            foreach (object child in sampleSP.Children)
            {
                Image childImg = (Image)child;
                i = Int32.Parse(childImg.Uid);
            }

            if (i == index)
            {
                if (capturesList.Count > 0)
                {
                    ArtificialScroll();
                }
                else
                {
                    emptyLabel.Visibility = Visibility.Visible;
                    infoLabel.Visibility = Visibility.Hidden;
                    sampleSP.Visibility = Visibility.Hidden;
                    nameLabel.Visibility = Visibility.Hidden;

                    Border1.Visibility = Visibility.Hidden;
                    Border2.Visibility = Visibility.Hidden;

                    ShowButton.IsEnabled = false;
                }
            }

            if(capturesList.Count < 2)
            {
                rightSp.Visibility = Visibility.Hidden;
                leftSp.Visibility = Visibility.Hidden;
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
                    int uid = Int32.Parse(element.Uid);

                    if (uid == index) element.Show();
                }                     
            }   
        }        
    }
}
