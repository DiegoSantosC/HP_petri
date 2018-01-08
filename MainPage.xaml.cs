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
using System.Windows.Shapes;
using System.Threading;
using System.Drawing;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace PetriUI
{
    /// <summary>
    /// 
    /// Interaction logic for MainPage.xaml
    /// 
    /// Functionality : First Mat Screen scan to choose captures to be started
    /// as well as the settings related to them
    /// 
    /// Launched by : MainWindow (starting interface)
    /// 
    /// Launches : CapturePreview when captures start running
    /// 
    /// </summary>

    public partial class MainPage : Page
    {
        public static List<int[]> parameters;
        private static int counter;
        private static List<string> saveFolders;

        private CapturePreviews cp;

        public static bool capturesRunning;

        // The interface is initialized blank until a scan is made
        public MainPage()
        {
            InitializeComponent();

            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            objectShowStackPanel.Visibility = Visibility.Collapsed;

            parameters = new List<int[]>();

            counter = 1;

            cp = new CapturePreviews(this);
            saveFolders = new List<string>();
            capturesRunning = false;

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

        // Object scan. This method can be called repeatedly 
        private void objectShow_Button_Click(object sender, RoutedEventArgs e)
        {

            parameters = new List<int[]>();
            CaptureDetailsLabel.Content = "";

            // Acquisition of outlines to define existing objects

            MainCapture newCapture = new MainCapture();
            OutlineParameters op = newCapture.ConfirmCapture();

            int numberObjects = OutlineParameters.getCapacity(op);

            if ((op.getSize(op).ElementAt(0).X > 4000) && (op.getSize(op).ElementAt(0).Y > 3000))
            {
                MessageBox.Show("No objects could be found");
                return;
            }

            List<System.Windows.Shapes.Rectangle> outlineDefinitionList = new List<System.Windows.Shapes.Rectangle>();

            // Selection border holds a definition for an object and it's showing structure. (See SelectionBorder constructor definition)
            List<SelectionBorder> transparentCanvasList = new List<SelectionBorder>();

            System.Drawing.Point globalPicSize = op.getGlobalSize(op);

            ImageBorder.Visibility = Visibility.Visible;

            ImageCanvas.Children.Clear();
            ImageCanvas.Children.Add(objectShowStackPanel);

            // Definiton of objects by their outlines
            for (int i = 0; i < numberObjects; i++)
            {
                // Rectangle definition

                outlineDefinitionList.Add(new System.Windows.Shapes.Rectangle());
                outlineDefinitionList.ElementAt(i).Width = (op.getSize(op).ElementAt(i).X *790)/globalPicSize.X; // Window/Canvas ratio
                outlineDefinitionList.ElementAt(i).Height = (op.getSize(op).ElementAt(i).Y *590)/ globalPicSize.Y;

                ImageCanvas.Children.Add(outlineDefinitionList.ElementAt(i));
                outlineDefinitionList.ElementAt(i).Stroke = System.Windows.Media.Brushes.LightGreen;
                outlineDefinitionList.ElementAt(i).StrokeThickness = 3;
                outlineDefinitionList.ElementAt(i).Margin = 
                    new Thickness((op.getLocation(op).ElementAt(i).X * 790) / globalPicSize.X, (op.getLocation(op).ElementAt(i).Y * 590) / globalPicSize.Y, 0, 0);

                // Canvas definition

                transparentCanvasList.Add(new SelectionBorder(i, new Border()));
                transparentCanvasList.ElementAt(i).Width = (op.getSize(op).ElementAt(i).X * 790) / globalPicSize.X; // Window/Canvas ratio
                transparentCanvasList.ElementAt(i).Height = (op.getSize(op).ElementAt(i).Y * 590) / globalPicSize.Y;

                transparentCanvasList.ElementAt(i).MouseDown += new MouseButtonEventHandler(RectangleClickedHandler);
                ImageCanvas.Children.Add(transparentCanvasList.ElementAt(i));
                transparentCanvasList.ElementAt(i).Opacity = 0;
                transparentCanvasList.ElementAt(i).Background = System.Windows.Media.Brushes.Green;
                transparentCanvasList.ElementAt(i).MouseEnter += new MouseEventHandler(BorderMouseEnterHandlerEnter);
                transparentCanvasList.ElementAt(i).MouseLeave += new MouseEventHandler(BorderMouseEnterHandlerLeave);

                transparentCanvasList.ElementAt(i).Margin =
                    new Thickness((op.getLocation(op).ElementAt(i).X * 790) / globalPicSize.X, (op.getLocation(op).ElementAt(i).Y * 590) / globalPicSize.Y, 0, 0);
            }

            objectShowStackPanel.Visibility = Visibility.Visible;
            
            // Global scan of the mat screen acquisition

            System.Windows.Controls.Image globalImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(PictureHandling.confirmPath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            globalImg.Source = src;
            globalImg.Stretch = Stretch.Uniform;

            objectShowStackPanel.Children.Clear();
            objectShowStackPanel.Children.Add(globalImg);

            ShowButton.Content = "Scan new Layout";
            ParametersBorder.Visibility = Visibility.Hidden;
        }

        // Definition of outlines functionalities

        private void BorderMouseEnterHandlerEnter(object sender, MouseEventArgs e)
        {
            Border senderBut = (Border)sender;
            senderBut.Opacity = 0.3;
        }

        private void BorderMouseEnterHandlerLeave(object sender, MouseEventArgs e)
        {
            Border senderBut = (Border)sender;
            senderBut.Opacity = 0;
        }

        void RectangleClickedHandler(object sender, EventArgs e)
        {
            SelectionBorder sb = (SelectionBorder)sender;
            MonitoringParametersShow(sb.getIndex());

        }

        // Navigation functionalities definition
        private void navigationArrowEnter(object sender, MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;

            foreach (object child in senderBut.Children)
            {
                System.Windows.Controls.Image childImg = (System.Windows.Controls.Image)child;

                childImg.Opacity = 0.7;
            }

        }

        private void navigationArrowLeave(object sender, MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;

            foreach (object child in senderBut.Children)
            {
                System.Windows.Controls.Image childImg = (System.Windows.Controls.Image)child;

                childImg.Opacity = 1;
            }
        }

        private void navigationArrowClick(object sender, MouseEventArgs e)
        {

            this.NavigationService.Navigate(cp);

        }

        // Settings showing
        private void MonitoringParametersShow(int objectToParameterize)
        {
            
            ParametersBorder.Visibility = Visibility.Visible;

            ParametersTitleLabel.Content = "Object " + objectToParameterize + " capture parameters";

        }

        // Settings confirm
        private void ParameterConfirm_Button_Click(object sender, RoutedEventArgs e)
        {

            int minutesInterval, hoursInterval, numberOfCaptures, delayH, delayMin;
            string folder;

            if ((Int32.TryParse(minutesTextBox.Text, out minutesInterval)) && (Int32.TryParse(hoursTextBox.Text, out hoursInterval)) && (Int32.TryParse(numberOfCapturesTextBox.Text, out numberOfCaptures)) && Int32.TryParse(delayHTextBox.Text, out delayH) && Int32.TryParse(delayMinTextBox.Text, out delayMin))
            {
                if (((minutesInterval == 0) && (hoursInterval == 0)) || (numberOfCaptures == 0))
                {
                    MessageBox.Show("Insert valid parameter values");
                }
                else
                {
                    if(FolderLabel.Content.ToString().Length == 0)
                    {
                        MessageBox.Show("Choose a valid directory to save the data");
                        return;
                    }

                    int hours, minutes;

                    if (hoursInterval == 0)
                    {
                        hours = (minutesInterval * numberOfCaptures) / 60;
                        minutes = (minutesInterval * numberOfCaptures) % 60;
                    }
                    else
                    {
                        minutesInterval = minutesInterval + hoursInterval * 60;
                        hours = (minutesInterval * numberOfCaptures) / 60;
                        minutes = (minutesInterval * numberOfCaptures) % 60;

                    }

                    int hoursDel, minutesDel;

                    if (delayH == 0)
                    {
                        hoursDel = delayMin/ 60;
                        minutesDel = delayMin % 60;
                    }
                    else
                    {
                        delayMin = delayMin + delayH * 60;
                        hoursDel = delayMin/ 60;
                        minutesDel = delayMin % 60;
                        
                    }

                    String str = hours + " hours " + minutes + " minutes";

                    String str2;
                    if (hoursDel == 0)
                    {
                        str2 = minutesDel + " minutes";
                    }else
                    {
                        str2 = hoursDel + " hours " + minutesDel + " minutes";
                    }

                    int objIndex = Int32.Parse(ParametersTitleLabel.Content.ToString().Split(' ')[1]);

                    CaptureDetailsLabel.Content = CaptureDetailsLabel.Content +
                         Environment.NewLine + "Capture " + counter + ": " + str + " , " + str2 + " until start, " + numberOfCaptures + " captures will be made.";

                    counter++;

                    folder = (string)FolderLabel.Content;

                    ParametersBorder.Visibility = Visibility.Hidden;
                    CaptureConfirmButton.Visibility = Visibility.Visible;
                    minutesTextBox.Text = "0";
                    hoursTextBox.Text = "0";
                    numberOfCapturesTextBox.Text = "0";
                    delayHTextBox.Text = "0";
                    delayMinTextBox.Text = "0";
                    FolderLabel.Content = "";

                    int[] param = new int[4];
                    param[0] = numberOfCaptures;
                    param[1] = minutesInterval;
                    param[2] = objIndex;
                    param[3] = delayMin;
                    parameters.Add(param);

                    saveFolders.Add(folder);

                    CaptureCancelButton.Visibility = Visibility.Visible;

                }
            }
            else
            {
                MessageBox.Show("Parameter parsing error");
            }
        }

        // Launch captures
        private void CaptureConfirm_Button_Click(object sender, RoutedEventArgs e)
        {

            // The navigaton is now enabled
            System.Windows.Controls.Image navImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\FlechaDcha.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            navImg.Source = src;
            navImg.Stretch = Stretch.Uniform;

            navigationSp.Children.Add(navImg);

            navigationSp.MouseEnter += new MouseEventHandler(navigationArrowEnter);
            navigationSp.MouseLeave += new MouseEventHandler(navigationArrowLeave);

            navigationSp.MouseDown += new MouseButtonEventHandler(navigationArrowClick);

            navigationSp.Visibility = Visibility.Visible;
            navLabel.Visibility = Visibility.Visible;

            List<int> indexes = new List<int>();

            for (int i = 0; i < parameters.Count; i++)
            {
                indexes.Add(parameters.ElementAt(i)[2]);
            }

            cp.AddCaptures(parameters, indexes, saveFolders);
            this.NavigationService.Navigate(cp);

            parameters = new List<int[]>();
            CaptureDetailsLabel.Content = "";
            saveFolders = new List<string>();

            ShowButton.IsEnabled = false;

            CaptureCancelButton.Visibility = Visibility.Hidden;

        }

        private void CaptureCancel_Button_Click(object sender, RoutedEventArgs e)
        {
            parameters = new List<int[]>();
            CaptureDetailsLabel.Content = "";
            saveFolders = new List<string>();
            CaptureCancelButton.Visibility = Visibility.Hidden;

        }

        private void Folder_Election_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Choose Saving Location";

            Nullable<bool> result = sfd.ShowDialog();

            string fileLocation = "";

            if (result == true)
            {
                if (sfd.FileName != "")
                {
                    fileLocation = sfd.FileName;
                }
            }

            FolderLabel.Content = fileLocation;

        }
    }
}
