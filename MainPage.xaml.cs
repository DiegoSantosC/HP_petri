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
using System.Windows.Forms;
using System.IO;

namespace PetriUI
{
    /// <summary>
    /// 
    /// Interaction logic for MainPage.xaml
    /// 
    /// Functionality : First Mat Screen scan to choose captures to be started
    /// as well as the settings related to them, importing captures for analysis
    /// 
    /// Launched by : MainWindow (starting interface)
    /// 
    /// Launches : CapturePreview when captures start running, AnalysisWindow when import mode
    /// 
    /// </summary>

    public partial class MainPage : Page
    {
        public static List<int[]> parameters;
        private static int counter;
        private static List<string> saveFolders;
        private static List<bool[]> analysisToPerform;
        private static List<string> processNames;

        private CapturePreviews cp;

        public static bool capturesRunning;


        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      UI related functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // The interface is initialized blank until a scan is made

        public MainPage()
        {
            InitializeComponent();

            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            objectShowStackPanel.Visibility = Visibility.Collapsed;

            parameters = new List<int[]>();

            analysisToPerform = new List<bool[]>();

            counter = 1;

            cp = new CapturePreviews(this);
            saveFolders = new List<string>();
            processNames = new List<string>();
            capturesRunning = false;

            Logo_Init();

            CloseStatics_Init();
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

        // Definition of outlines functionalities

        private void BorderMouseEnterHandlerEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border senderBut = (Border)sender;
            senderBut.Opacity = 0.3;
        }

        private void BorderMouseEnterHandlerLeave(object sender, System.Windows.Input.MouseEventArgs e)
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

        private void navigationArrowEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;

            foreach (object child in senderBut.Children)
            {
                System.Windows.Controls.Image childImg = (System.Windows.Controls.Image)child;

                childImg.Opacity = 0.7;
            }

        }

        private void navigationArrowLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StackPanel senderBut = (StackPanel)sender;

            foreach (object child in senderBut.Children)
            {
                System.Windows.Controls.Image childImg = (System.Windows.Controls.Image)child;

                childImg.Opacity = 1;
            }
        }

        private void navigationArrowClick(object sender, System.Windows.Input.MouseEventArgs e)
        {

            this.NavigationService.Navigate(cp);

        }

        // Erase pending captures data

        private void CaptureCancel_Button_Click(object sender, RoutedEventArgs e)
        {
            parameters = new List<int[]>();
            CaptureDetailsLabel.Content = "";
            saveFolders = new List<string>();
            CaptureCancelButton.Visibility = Visibility.Hidden;

        }

        // Settings interface close init and handlers

        private void CloseStatics_Init()
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Close_Image.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            img.Source = src;
            img.Stretch = Stretch.Uniform;

            CloseSP.Children.Add(img);

            System.Windows.Controls.Image img2 = new System.Windows.Controls.Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Close_Image.png", UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            img2.Source = src;
            img2.Stretch = Stretch.Uniform;

            CloseSP2.Children.Add(img2);

            CloseSP.MouseEnter += new System.Windows.Input.MouseEventHandler(navigationArrowEnter);
            CloseSP.MouseLeave += new System.Windows.Input.MouseEventHandler(navigationArrowLeave);

            CloseSP.MouseDown += new MouseButtonEventHandler(closeCapturesClick);

            CloseSP2.MouseEnter += new System.Windows.Input.MouseEventHandler(navigationArrowEnter);
            CloseSP2.MouseLeave += new System.Windows.Input.MouseEventHandler(navigationArrowLeave);

            CloseSP2.MouseDown += new MouseButtonEventHandler(closeImportClick);
        }

        // Close and erase content

        private void closeCapturesClick(object sender, MouseButtonEventArgs e)
        {
            ParametersBorder.Visibility = Visibility.Hidden;

            minutesTextBox.Text = "0";
            hoursTextBox.Text = "0";
            numberOfCapturesTextBox.Text = "0";
            delayHTextBox.Text = "0";
            delayMinTextBox.Text = "0";
            FolderLabel.Content = "Not defined";

            Chk1.IsChecked = false;
            Chk2.IsChecked = false;

            ImportButton.IsEnabled = true;
        }

        private void closeImportClick(object sender, MouseButtonEventArgs e)
        {
            ImportBorder.Visibility = Visibility.Hidden;
            ImportChck1.IsChecked = false;
            ImportChck2.IsChecked = false;

            FolderImportLabel.Content = "Not defined";
            FolderSaveLabel.Content = "Not defined";

            ShowButton.IsEnabled = true;
        }

        // Folder election for saving captures and analysis in a made capture process

        private void Folder_Election_Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
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

        // Folder election from which analysis input will be extracted in import mode

        private void Folder_Import_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog sfd = new FolderBrowserDialog();

            DialogResult res = sfd.ShowDialog();

            if (res == DialogResult.OK && !string.IsNullOrWhiteSpace(sfd.SelectedPath))
            {
                FolderImportLabel.Content = sfd.SelectedPath;
            }

        }

        // Folder electon in which analysis results will be saved in import mode

        private void Folder_Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Choose Import Location";

            Nullable<bool> result = sfd.ShowDialog();

            string fileLocation = "";

            if (result == true)
            {
                if (sfd.FileName != "")
                {
                    fileLocation = sfd.FileName;
                }
            }

            FolderSaveLabel.Content = fileLocation;
        }

        // Function that inits and shows the navigation handlers

        private void enableNavigation()
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

            navigationSp.MouseEnter += new System.Windows.Input.MouseEventHandler(navigationArrowEnter);
            navigationSp.MouseLeave += new System.Windows.Input.MouseEventHandler(navigationArrowLeave);

            navigationSp.MouseDown += new MouseButtonEventHandler(navigationArrowClick);

            navigationSp.Visibility = Visibility.Visible;
            navLabel.Visibility = Visibility.Visible;

        }


        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Importing for analysis
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void import_Button_Click(object sender, RoutedEventArgs e)
        {
            ImportBorder.Visibility = Visibility.Visible;
            ImageBorder.Visibility = Visibility.Hidden;
            objShowLabel.Visibility = Visibility.Hidden;

            ShowButton.IsEnabled = false;
        }

        // Import parameters being extracted from the UI

        private void ImportConfirm_Button_Click(object sender, RoutedEventArgs e)
        {
            string folderImport = (string)FolderImportLabel.Content;
            string folderSave = (string)FolderSaveLabel.Content;

            if (folderImport == "Not defined" || folderSave == "Not defined")
            {
                System.Windows.MessageBox.Show(" Invalid directory path");
                return;
            }

            string[] files = Directory.GetFiles(folderImport);

            if (files.Length == 0) { System.Windows.MessageBox.Show(folderImport + " is empty"); return; }

            List<System.Drawing.Image> images = new List<System.Drawing.Image>();

            // Import mode does only have the analysis functionality and thus, an analysis must be selected to be done

            if (!ImportChck1.IsChecked.GetValueOrDefault() && !ImportChck2.IsChecked.GetValueOrDefault())
            {
                System.Windows.MessageBox.Show(" Select an analysis to be performed ");
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]))
                {
                    string[] extension = files[i].Split('.');
                    
                    // We will only accept as viable files for the analysis images (of a wide range of extensions)

                    if (extension[extension.Length - 1] != "png" && extension[extension.Length - 1] != "jpg" && extension[extension.Length - 1] != "jpeg" &&
                        extension[extension.Length - 1] != "bmp")
                    {
                        System.Windows.MessageBox.Show(files[i] + " has not the right file extension");
                        return;

                    }
                    else
                    {
                        images.Add(getImageFromFile(files[i]));
                    }
                }
                else
                {

                    System.Windows.MessageBox.Show(files[i] + " is not a valid file");
                    return;
                }
            }

            if (images.Count == 1) { System.Windows.MessageBox.Show("Not enough images for an analysis to be done"); return; }

            bool countA = ImportChck1.IsChecked.GetValueOrDefault(), classA = ImportChck2.IsChecked.GetValueOrDefault();

            // UI Reset

            AnalysisWindow aw = new AnalysisWindow(null, countA, classA);

            ImportBorder.Visibility = Visibility.Hidden;
            ImportChck1.IsChecked = false;
            ImportChck2.IsChecked = false;

            FolderImportLabel.Content = "Not defined";
            FolderSaveLabel.Content = "Not defined";

            // Count analysis is launched in a separated thread

            if (countA)
            {
                Thread analysisThread = new Thread(aw.getCount().staticAnalysis);

                List<object> list = new List<object>();
                list.Add(images);
                list.Add(this);
                list.Add(folderSave);

                analysisThread.Start(list);

            }

            AnalysisBorder.Visibility = Visibility.Visible;
            AnalysisBorder.Background = System.Windows.Media.Brushes.LightGray;

            ShowButton.IsEnabled = true;
        }

        private System.Drawing.Image getImageFromFile(string path)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(path);

            return img;
        }

        // Event function to unlock analysis functionalities when it is done

        public void finished_Analysis(AnalysisWindow aw, string folder)
        {
            aw.getCount().initStatics();

            aw.getChart().initCharts(aw.getCount(), folder);

            aw.Show();

            AnalysisBorder.Visibility = Visibility.Hidden;
        }




        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Capturing process functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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
                System.Windows.MessageBox.Show("No objects could be found");
                return;
            }

            List<System.Windows.Shapes.Rectangle> outlineDefinitionList = new List<System.Windows.Shapes.Rectangle>();

            // Selection border holds a definition for an object and it's showing structure. (See SelectionBorder constructor definition)

            List<SelectionBorder> transparentCanvasList = new List<SelectionBorder>();

            System.Drawing.Point globalPicSize = op.getGlobalSize(op);

            ImageBorder.Visibility = Visibility.Visible;
            objShowLabel.Visibility = Visibility.Visible;

            ImageCanvas.Children.Clear();
            ImageCanvas.Children.Add(objectShowStackPanel);

            // Definiton of objects by their outlines

            for (int i = 0; i < numberObjects; i++)
            {
                // Rectangle definition

                outlineDefinitionList.Add(new System.Windows.Shapes.Rectangle());
                outlineDefinitionList.ElementAt(i).Width = (op.getSize(op).ElementAt(i).X * 790) / globalPicSize.X; // Window/Canvas ratio
                outlineDefinitionList.ElementAt(i).Height = (op.getSize(op).ElementAt(i).Y * 590) / globalPicSize.Y;

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
                transparentCanvasList.ElementAt(i).MouseEnter += new System.Windows.Input.MouseEventHandler(BorderMouseEnterHandlerEnter);
                transparentCanvasList.ElementAt(i).MouseLeave += new System.Windows.Input.MouseEventHandler(BorderMouseEnterHandlerLeave);

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

        // Settings showing

        private void MonitoringParametersShow(int objectToParameterize)
        {
            ImportButton.IsEnabled = false;

            NameTextBox.Text = "Process " + parameters.Count;

            ParametersBorder.Visibility = Visibility.Visible;

            ParametersTitleLabel.Content = "Object " + objectToParameterize + " capture parameters";

        }

        // Settings confirm

        private void ParameterConfirm_Button_Click(object sender, RoutedEventArgs e)
        {
            int minutesInterval, hoursInterval, numberOfCaptures, delayH, delayMin;
            string folder, name;

            // Input format check and acquisition

            if ((Int32.TryParse(minutesTextBox.Text, out minutesInterval)) && (Int32.TryParse(hoursTextBox.Text, out hoursInterval)) && (Int32.TryParse(numberOfCapturesTextBox.Text, out numberOfCaptures)) && Int32.TryParse(delayHTextBox.Text, out delayH) && Int32.TryParse(delayMinTextBox.Text, out delayMin))
            {
                if (((minutesInterval == 0) && (hoursInterval == 0)) || (numberOfCaptures == 0))
                {
                    System.Windows.MessageBox.Show("Insert valid parameter values");
                }
                else
                {
                    if(NameTextBox.Text.Length == 0)
                    {
                        name = "Process " + parameters.Count;
                    }else
                    {
                        name = NameTextBox.Text;
                    }

                    folder = (string)FolderLabel.Content;

                    if (folder == "Not defined")
                    {
                        System.Windows.MessageBox.Show("Choose a valid directory to save the data");
                        return;
                    }

                    // Conversion to capture minutes

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
                        hoursDel = delayMin / 60;
                        minutesDel = delayMin % 60;
                    }
                    else
                    {
                        delayMin = delayMin + delayH * 60;
                        hoursDel = delayMin / 60;
                        minutesDel = delayMin % 60;

                    }

                    String str = hours + " hours " + minutes + " minutes";

                    String str2;
                    if (hoursDel == 0)
                    {
                        str2 = minutesDel + " minutes";
                    }
                    else
                    {
                        str2 = hoursDel + " hours " + minutesDel + " minutes";
                    }

                    int objIndex = Int32.Parse(ParametersTitleLabel.Content.ToString().Split(' ')[1]);

                    // Pending capture building

                    CaptureDetailsLabel.Content = CaptureDetailsLabel.Content +
                         Environment.NewLine + name + ": " + str + " , " + str2 + " until start, " + numberOfCaptures + " captures will be made.";

                    counter++;

                    folder = (string)FolderLabel.Content;

                    int[] param = new int[4];
                    param[0] = numberOfCaptures;
                    param[1] = minutesInterval;
                    param[2] = objIndex;
                    param[3] = delayMin;
                    parameters.Add(param);

                    saveFolders.Add(folder);
                    processNames.Add(name);

                    // UI reset

                    ParametersBorder.Visibility = Visibility.Hidden;
                    CaptureConfirmButton.Visibility = Visibility.Visible;
                    minutesTextBox.Text = "0";
                    hoursTextBox.Text = "0";
                    numberOfCapturesTextBox.Text = "0";
                    delayHTextBox.Text = "0";
                    delayMinTextBox.Text = "0";
                    FolderLabel.Content = "Not defined";
                                       
                    // Analysis info acquisition

                    bool[] analysis = new bool[2];
                    if (Chk1.IsChecked.GetValueOrDefault()) { analysis[0] = true; }
                    else { analysis[0] = false; }

                    if (Chk2.IsChecked.GetValueOrDefault()) { analysis[1] = true; }
                    else { analysis[1] = false; }

                    Chk1.IsChecked = false;
                    Chk2.IsChecked = false;

                    analysisToPerform.Add(analysis);

                    CaptureCancelButton.Visibility = Visibility.Visible;

                    ImportButton.IsEnabled = true;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Parameter parsing error");
            }
        }

        // Launch captures

        private void CaptureConfirm_Button_Click(object sender, RoutedEventArgs e)
        {
            // Enable navigation

            enableNavigation();

            // Launch pending captures
            
            List<int> indexes = new List<int>();

            for (int i = 0; i < parameters.Count; i++)
            {
                indexes.Add(parameters.ElementAt(i)[2]);
            }

            cp.AddCaptures(parameters, indexes, saveFolders, analysisToPerform, processNames);
            this.NavigationService.Navigate(cp);

            parameters = new List<int[]>();
            CaptureDetailsLabel.Content = "";
            saveFolders = new List<string>();
            analysisToPerform = new List<bool[]>();
            processNames = new List<string>();

            ShowButton.IsEnabled = false;

            CaptureCancelButton.Visibility = Visibility.Hidden;

        }
    }
}
