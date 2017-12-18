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
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private Task t;
        private captureFramework cf;
        private List<captureFramework> cfs;
        private bool running;
        private Thread newCaptureThread;

        public CaptureWindow(Image img, int[] parameters)
        {
            InitializeComponent();

            this.Width = 1200;
            this.Height = 900;

            cfs = new List<captureFramework>(); 

            List<Uri> u = new List<Uri>();
            t = new Task(this, parameters[0], parameters[1], parameters[2], u);

            MainCapture newCapture = new MainCapture();
            newCaptureThread = new Thread(newCapture.StartCapture);
            newCaptureThread.SetApartmentState(ApartmentState.STA);
            newCaptureThread.Start(t);
            running = true;

            FirstCapture(img);
        }

        private void FirstCapture(Image img)
        {
            Image clone1 = new Image();
            Image clone2 = new Image();

            clone1.Source = img.Source;
            clone2.Source = img.Source;

            double ratio = clone1.Source.Width / clone1.Source.Height;

            cfs.Add(new captureFramework(clone1, 250, ratio));

            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count - 1).getBorder());
            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count - 1).getCapturePanel());

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseEnter += new MouseEventHandler(CaptureFocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseLeave += new MouseEventHandler(CaptureUnfocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseDown += new MouseButtonEventHandler(captureClicked);


            cfs.ElementAt(cfs.Count - 1).getCapturePanel().Uid = (cfs.Count - 1).ToString();
            cfs.ElementAt(cfs.Count - 1).getBorder().Background = System.Windows.Media.Brushes.LightBlue;

            cfs.ElementAt(cfs.Count - 1).setPosition(new Thickness(0, 0, 0, 0));
            
            cf = new captureFramework(clone2, 400, ratio);

            LastImageCanvas.Children.Clear();
            LastImageCanvas.Children.Add(cf.getBorder());
            LastImageCanvas.Children.Add(cf.getCapturePanel());

            ProjectButton.Visibility = Visibility.Visible;
            infoLabel.Visibility = Visibility.Visible;

            timeLabel.Content = "Capture taken at: " + cf.getTime();
            timeLabel.Visibility = Visibility.Visible;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            newCaptureThread.Abort();
            running = false;

        }

        private void Save_Results(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Choose Saving Location";

            Nullable<bool> result = sfd.ShowDialog();

            string fileLocation = "";

            List<string> data = new List<string>();

            for (int i = 0; i < cfs.Count; i++){

                data.Add(cfs.ElementAt(i).getTime());
            }

            if(result == true)
            {
                if (sfd.FileName != "")
                {
                    fileLocation = sfd.FileName;
                }

                ToolBox.SaveResults(t.getIndex(), fileLocation, data);
            }
        }

        public void KillCaptures()
        {
            MainCapture.stopRequested = true;
        }

        public void Trigger_Capture()
        {
               MomentCapture.Capture(t);
        }

        public void DrawImage()
        {
            Image img1 = new Image();
            Image img2 = new Image();

            BitmapImage src1 = new BitmapImage();
            BitmapImage src2 = new BitmapImage();

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

            cfs.Add(new captureFramework(img1, 250, ratio));

            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count -1).getBorder());
            ImagesCanvas.Children.Add(cfs.ElementAt(cfs.Count - 1).getCapturePanel());

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseEnter += new MouseEventHandler(CaptureFocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseLeave += new MouseEventHandler(CaptureUnfocused);
            cfs.ElementAt(cfs.Count - 1).getCapturePanel().MouseDown += new MouseButtonEventHandler(captureClicked);


            cfs.ElementAt(cfs.Count - 1).getCapturePanel().Uid = (cfs.Count - 1).ToString();
            cfs.ElementAt(cfs.Count - 1).getBorder().Background = System.Windows.Media.Brushes.LightBlue;

            if (cfs.Count < 8)
            {

                Thickness lastPosition = cfs.ElementAt(cfs.Count - 2).getBorder().Margin;
                Thickness newPosition = new Thickness(lastPosition.Left + 40, lastPosition.Top + 40, lastPosition.Right, lastPosition.Bottom);

                cfs.ElementAt(cfs.Count - 1).setPosition(newPosition);

            }else
            {
                RelocateImages();
            }


            cf = new captureFramework(img2, 400, ratio);

            LastImageCanvas.Children.Clear();
            LastImageCanvas.Children.Add(cf.getBorder());
            LastImageCanvas.Children.Add(cf.getCapturePanel());

            ProjectButton.Visibility = Visibility.Visible;
            infoLabel.Visibility = Visibility.Visible;

            timeLabel.Content = "Capture taken at: " + cf.getTime();
            timeLabel.Visibility = Visibility.Visible;
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
            SaveButton.Visibility = Visibility.Visible;
            running = false;
        }

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

        private void Project_IntoMat(object sender, RoutedEventArgs e)
        {

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

                    }

                }
                else
                {
                    e.Cancel = false;
      
                }
            }
           
        }
    }
}
