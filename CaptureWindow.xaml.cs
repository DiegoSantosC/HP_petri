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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

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
    /// Interaction logic for CaptureWindow.xaml
    /// 
    /// Functionality : Showing the details for a particular capture,
    /// as well as viewing the capturing process over time
    /// 
    /// Launched by: Show details in CapturePreviews
    /// 
    /// </summary>
    
    public partial class CaptureWindow : Window
    {
        private Task t;
        private captureFramework cf;
        private List<captureFramework> cfs;
        private bool running;
        private Thread newCaptureThread;
        private CapturePreviews cp;

        // This window is responsble for handling the captures of the object that represents, and thus 
        // manages the MainCapture thread and hosts capture taking functions 
        public CaptureWindow(CapturePreviews capt, Image img, int[] param, string folder)
        {
            InitializeComponent();

            this.Width = 1200;
            this.Height = 900;

            cp = capt;

            cfs = new List<captureFramework>();

            // A task holds details for a capture process (See <Task> constructior definition)
            List<Uri> u = new List<Uri>();

            t = new Task(this, param[0], param[1], param[2], param[3], u, folder);

            Directory.CreateDirectory(folder);
            LogFile file = new LogFile(t.getFolder(), t.getIndex(), t.getNumberOfCaptures());

            file.BuildAndSave();

            // Thread initialization 
            MainCapture newCapture = new MainCapture();
            newCaptureThread = new Thread(newCapture.StartCapture);
            newCaptureThread.SetApartmentState(ApartmentState.STA);
            newCaptureThread.Start(t);
            running = true;

            FirstCapture(img);

            Info_Init();

            Logo_Init();

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

        // This Window will be initialized with an image taken from CapturePreviews 
        // featuring the state of the object to capture prior to the capture process
        private void FirstCapture(Image img)
        {
            Image clone1 = new Image();
            Image clone2 = new Image();

            clone1.Source = img.Source;
            clone2.Source = img.Source;

            double ratio = clone1.Source.Width / clone1.Source.Height;

            // Adding image to group
            cfs.Add(new captureFramework(clone1, 250, ratio));

            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count - 1).getBorder());
            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count - 1).getCapturePanel());

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseEnter += new MouseEventHandler(CaptureFocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseLeave += new MouseEventHandler(CaptureUnfocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseDown += new MouseButtonEventHandler(captureClicked);


            cfs.ElementAt(cfs.Count - 1).getCapturePanel().Uid = (cfs.Count - 1).ToString();
            cfs.ElementAt(cfs.Count - 1).getBorder().Background = System.Windows.Media.Brushes.LightBlue;

            cfs.ElementAt(cfs.Count - 1).setPosition(new Thickness(0, 0, 0, 0));
            
            // Setting to last image
            cf = new captureFramework(clone2, 400, ratio);

            LastImageCanvas.Children.Clear();
            LastImageCanvas.Children.Add(cf.getBorder());
            LastImageCanvas.Children.Add(cf.getCapturePanel());

            ProjectButton.Visibility = Visibility.Visible;
            infoLabel.Visibility = Visibility.Visible;

            timeLabel.Content = "Capture taken at: " + cf.getTime();
            timeLabel.Visibility = Visibility.Visible;
        }

        // Information

        private void Info_Init()
        {
            // Icon initialization

            System.Windows.Controls.Image infoImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\BlueQuestionMarkIcon.jpg", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            infoImg.Source = src;
            infoImg.Stretch = Stretch.Uniform;

            dataSP.Children.Add(infoImg);

            dataSP.MouseEnter += new MouseEventHandler(infoShow);
            dataSP.MouseLeave += new MouseEventHandler(infoHide);

            // Label initialization

            dataLabel.Content = "Capture of object " + t.getIndex() + Environment.NewLine +
                "Captures taken each " + t.getInterval() + " minutes" + Environment.NewLine +
                t.getNumberOfCaptures() + " captures to be taken" + Environment.NewLine +
                t.getNumberOfCaptures() + " captures to go";

            if (t.getDelay() != 0)
            {
                dataLabel.Content = dataLabel.Content + Environment.NewLine +
                "Delay of " + t.getDelay() + " minutes until start"; 
            }
        }

        private void infoHide(object sender, MouseEventArgs e)
        {
            dataLabel.Visibility = Visibility.Hidden;
            dataBorder.Visibility = Visibility.Hidden;
        }

        private void infoShow(object sender, MouseEventArgs e)
        {
            dataLabel.Visibility = Visibility.Visible;
            dataBorder.Visibility = Visibility.Visible;
        }

        // Buttons functionalities

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            newCaptureThread.Abort();
            running = false;

        }

        public void KillCaptures()
        {
            MainCapture.stopRequested = true;
        }

        private void Project_IntoMat(object sender, RoutedEventArgs e)
        {
            Image img = null;

            foreach (Image childImg in cf.getCapturePanel().Children)
            {
                img = childImg;
            }

            ProjectedFormHandler handler = new ProjectedFormHandler();

            handler.HandleProjection(img);
        }


        // Function called from MainCapture each time counter is trigged
        public void Trigger_Capture()
        {
               MomentCapture.Capture(t);
        }


        // Management of a new taken image

        public void DrawImage()
        {
            Image img1 = new Image();
            Image img2 = new Image();

            BitmapImage src1 = new BitmapImage();
            BitmapImage src2 = new BitmapImage();

            // Img acquisition 

            src1.BeginInit();
            src1.UriSource = t.getCaptures().ElementAt(t.getCaptures().Count -1);
            src1.CacheOption = BitmapCacheOption.OnLoad;
            src1.EndInit();
            img1.Source = src1;
            img1.Stretch = Stretch.Uniform;

            src2.BeginInit();
            src2.UriSource = t.getCaptures().ElementAt(t.getCaptures().Count - 1);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            img2.Source = src1;
            img2.Stretch = Stretch.Uniform;

            double ratio = src1.Width / src1.Height;

            // Adding image to group

            cfs.Add(new captureFramework(img1, 250, ratio));

            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count -1).getBorder());
            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count - 1).getCapturePanel());

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseEnter += new MouseEventHandler(CaptureFocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseLeave += new MouseEventHandler(CaptureUnfocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseDown += new MouseButtonEventHandler(captureClicked);


            cfs.ElementAt(cfs.Count - 1).getCapturePanel().Uid = (cfs.Count - 1).ToString();
            cfs.ElementAt(cfs.Count - 1).getBorder().Background = System.Windows.Media.Brushes.LightBlue;

            // Positioning management

            if (cfs.Count < 8)
            {

                Thickness lastPosition = cfs.ElementAt(cfs.Count - 2).getBorder().Margin;
                Thickness newPosition = new Thickness(lastPosition.Left + 40, lastPosition.Top + 40, lastPosition.Right, lastPosition.Bottom);

                cfs.ElementAt(cfs.Count - 1).setPosition(newPosition);

            }else
            {
                RelocateImages();
            }

            // Adding image to last

            cf = new captureFramework(img2, 400, ratio);

            LastImageCanvas.Children.Clear();
            LastImageCanvas.Children.Add(cf.getBorder());
            LastImageCanvas.Children.Add(cf.getCapturePanel());

            ProjectButton.Visibility = Visibility.Visible;
            infoLabel.Visibility = Visibility.Visible;

            timeLabel.Content = "Capture taken at: " + cf.getTime();
            timeLabel.Visibility = Visibility.Visible;

            // Update info

            dataLabel.Content = "Capture of object " + t.getIndex() + Environment.NewLine +
               "Captures taken each " + t.getInterval() + " minutes" + Environment.NewLine +
               t.getNumberOfCaptures() + " captures to be taken" + Environment.NewLine +
               (t.getNumberOfCaptures() + 1 - cfs.Count) + " captures to go";

        }

        private void RelocateImages()
        {
            double newDeplacement;

            newDeplacement = 7 * 40 / cfs.Count;

            for (int i=0; i<cfs.Count; i++)
            {
                Thickness newPosition = new Thickness(i*newDeplacement, i * newDeplacement, 0, 0);

                cfs.ElementAt(i).setPosition(newPosition);
            }
        }

        internal void CaptureFinished()
        {
            CapturePreviews.DecrementCaptures();
            finishedCapture.Background = Brushes.Green;
            RunningLabel.Content = "Capture Process Finished";
            running = false;
        }

        public void startTriggered()
        {
            finishedCapture.Background = Brushes.Red;
            RunningLabel.Content = "Capture running";
            RunningLabel.Foreground = Brushes.White;
        }

        // Capture focusing functionalities

        private void CaptureFocused(object sender, MouseEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;

            sp.Width = sp.Width * 1.1;
            sp.Height = sp.Height * 1.1;

            int index = Int32.Parse(sp.Uid);

            captureFramework focused = cfs.ElementAt(index);

            focused.getBorder().Height = cfs.ElementAt(index).getBorder().Height * 1.1;
            focused.getBorder().Width = cfs.ElementAt(index).getBorder().Width * 1.1;

            StackPanel.SetZIndex(sp, 100);
            
        }

        private void CaptureUnfocused(object sender, MouseEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;

            sp.Width = sp.Width / 1.1;
            sp.Height = sp.Height / 1.1;

            int index = Int32.Parse(sp.Uid);

            cfs.ElementAt(index).getBorder().Height = cfs.ElementAt(index).getBorder().Height / 1.1;
            cfs.ElementAt(index).getBorder().Width = cfs.ElementAt(index).getBorder().Width / 1.1;

            StackPanel.SetZIndex(sp, 0);
        }

        private void captureClicked(object sender, MouseEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;

            Image img, clone = new Image();
            
            foreach (object child in sp.Children)
            {
                img = (Image)child;
                clone.Source = img.Source;
            }

            int index = Int32.Parse(sp.Uid);

            timeLabel.Content = "Capture taken at: " + cfs.ElementAt(index).getTime();

            cf.getCapturePanel().Children.Clear();

            cf.getCapturePanel().Children.Add(clone);
        }

        // Closing handling

        private void CaptureWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MainWindow.killRequest)
            {
                newCaptureThread.Abort();
                e.Cancel = false;
            }
            else
            {
                if (running)
                {
                    string msg = "Kill capture process?";

                    MessageBoxResult res =
                      MessageBox.Show(
                          msg,
                          "Closing Dialog",
                          MessageBoxButton.YesNo,
                          MessageBoxImage.Warning);

                    if (res == MessageBoxResult.No)
                    {
                        e.Cancel = true;
                        this.Hide();

                    }
                    else
                    {
                        e.Cancel = false;
                        CapturePreviews.DecrementCaptures();
                        newCaptureThread.Abort();
                        cp.EraseFinishedCapture(Int32.Parse(this.Uid));
                    }

                }
                else
                {
                    cp.EraseFinishedCapture(Int32.Parse(this.Uid));
                    e.Cancel = false;
      
                }
            }
           
        }
    }
}
