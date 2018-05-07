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
using hp.pc;

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
        private static List<string> saveFolders, mapSourceFolders;
        private static List<bool[]> analysisToPerform;
        private static List<string> processNames;
        private static List<System.Drawing.Point> sizes;
        private static List<PcPhysicalPoint> locations;

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
            mapSourceFolders = new List<string>();
            processNames = new List<string>();
            capturesRunning = false;
            sizes = new List<System.Drawing.Point>();
            locations = new List<PcPhysicalPoint>();

            Logo_Init();

            CloseStatics_Init();

            Chk2.Checked += new RoutedEventHandler(Chk2IsChecked);
            ImportChck2.Checked += new RoutedEventHandler(ImportChk2IsChecked);

        }

        private void Chk2IsChecked(object sender, RoutedEventArgs e)
        {
            if (Chk2.IsChecked.GetValueOrDefault() == true)
            {
                Chk1.IsChecked = true;
            }
        }

        private void ImportChk2IsChecked(object sender, RoutedEventArgs e)
        {
            if (ImportChck2.IsChecked.GetValueOrDefault() == true)
            {
                ImportChck1.IsChecked = true;
            }
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
            FolderBrowserDialog sfd = new FolderBrowserDialog();

            DialogResult res = sfd.ShowDialog();

            if (res == DialogResult.OK && !string.IsNullOrWhiteSpace(sfd.SelectedPath))
            {
                FolderLabel.Content = sfd.SelectedPath;
            }
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
                    FolderSaveLabel.Content = fileLocation;

                }
            }

        }

        // Folder election from which analysis input will be extracted in import mode

        private void Map_Import_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog sfd = new FolderBrowserDialog();

            DialogResult res = sfd.ShowDialog();

            if (res == DialogResult.OK && !string.IsNullOrWhiteSpace(sfd.SelectedPath))
            {
                MapImportLabel.Content = sfd.SelectedPath;
            }
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

        public void EnableNewScan()
        {
            ShowButton.IsEnabled = true;
        }

        // Creation of a Form from which AdvancedOptions' settings can be modified

        private void Modify_Advanced_Settings_Click(object sender, RoutedEventArgs e)
        {
            Form settingsForm = new Form();
            settingsForm.Width = 900;
            settingsForm.Height = 550;

            System.Windows.Forms.Label title = new System.Windows.Forms.Label();
            title.Location = new System.Drawing.Point(300, 10);
            title.Text = " Advanced Settings ";
            title.Size = new System.Drawing.Size(new System.Drawing.Point(200, 20));
            title.Font = new Font("Arial", 12, System.Drawing.FontStyle.Bold);

            System.Drawing.Size standardSize = new System.Drawing.Size(new System.Drawing.Point(195, 15));



            System.Windows.Forms.Label t1 = new System.Windows.Forms.Label();
            t1.Text = " Colony detection settings ";
            t1.Size = new System.Drawing.Size(200, 15);
            t1.Font = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
            t1.Location = new System.Drawing.Point(50, 40);

            System.Windows.Forms.Label l11 = new System.Windows.Forms.Label();
            l11.Text = "  Colony similarity tolerance:";
            l11.Size = standardSize;
            l11.Location = new System.Drawing.Point(0, 70);

            System.Windows.Forms.Label l12 = new System.Windows.Forms.Label();
            l12.Text = " Colony relevance threshold: ";
            l12.Size = standardSize;
            l12.Location = new System.Drawing.Point(300, 70);

            System.Windows.Forms.Label l13 = new System.Windows.Forms.Label();
            l13.Text = " Region of Interest margin: ";
            l13.Size = standardSize;
            l13.Location = new System.Drawing.Point(600, 70);

            System.Windows.Forms.TextBox tb11 = new System.Windows.Forms.TextBox();
            tb11.Size = new System.Drawing.Size(40, 20);
            tb11.Text = AdvancedOptions._nSimilarityTolerance.ToString();
            tb11.Location = new System.Drawing.Point(210, 70);

            System.Windows.Forms.TextBox tb12 = new System.Windows.Forms.TextBox();
            tb12.Size = new System.Drawing.Size(40, 20);
            tb12.Text = AdvancedOptions._nRelevanceThreshold.ToString();
            tb12.Location = new System.Drawing.Point(510, 70);

            System.Windows.Forms.TextBox tb13 = new System.Windows.Forms.TextBox();
            tb13.Size = new System.Drawing.Size(40, 20);
            tb13.Text = AdvancedOptions._nROIMargin.ToString();
            tb13.Location = new System.Drawing.Point(810, 70);


            System.Windows.Forms.Label l2 = new System.Windows.Forms.Label();
            l2.Text = " Variance Epsylon Value : ";
            l2.Size = standardSize;
            l2.Location = new System.Drawing.Point(0, 100);

            System.Windows.Forms.TextBox tb2 = new System.Windows.Forms.TextBox();
            tb2.Size = new System.Drawing.Size(40, 20);
            tb2.Text = AdvancedOptions._dEpsilonValue.ToString();
            tb2.Location = new System.Drawing.Point(210, 100);



            System.Windows.Forms.Label t3 = new System.Windows.Forms.Label();
            t3.Text = " Pixel recognition settings ";
            t3.Size = new System.Drawing.Size(200, 15);
            t3.Font = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
            t3.Location = new System.Drawing.Point(50, 140);


            System.Windows.Forms.Label l31 = new System.Windows.Forms.Label();
            l31.Text = "  Pixel recognition threshold:";
            l31.Size = standardSize;
            l31.Location = new System.Drawing.Point(0, 170);

            System.Windows.Forms.Label l32 = new System.Windows.Forms.Label();
            l32.Text = " Hysteresis difference top value: ";
            l32.Size = standardSize;
            l32.Location = new System.Drawing.Point(300, 170);

            System.Windows.Forms.Label l33 = new System.Windows.Forms.Label();
            l33.Text = " Hysteresis difference bottom value: ";
            l33.Size = standardSize;
            l33.Location = new System.Drawing.Point(600, 170);

            System.Windows.Forms.TextBox tb31 = new System.Windows.Forms.TextBox();
            tb31.Size = new System.Drawing.Size(40, 20);
            tb31.Text = AdvancedOptions._nThresholdValue.ToString();
            tb31.Location = new System.Drawing.Point(210, 170);

            System.Windows.Forms.TextBox tb32 = new System.Windows.Forms.TextBox();
            tb32.Size = new System.Drawing.Size(40, 20);
            tb32.Text = AdvancedOptions._nTopHysteresis.ToString();
            tb32.Location = new System.Drawing.Point(510, 170);

            System.Windows.Forms.TextBox tb33 = new System.Windows.Forms.TextBox();
            tb33.Size = new System.Drawing.Size(40, 20);
            tb33.Text = AdvancedOptions._nBottomHysteresis.ToString();
            tb33.Location = new System.Drawing.Point(810, 170);



            System.Windows.Forms.Label t4 = new System.Windows.Forms.Label();
            t4.Text = " Colony tracking comparison settings ";
            t4.Size = new System.Drawing.Size(250, 15);
            t4.Font = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
            t4.Location = new System.Drawing.Point(50, 210);


            System.Windows.Forms.Label l41 = new System.Windows.Forms.Label();
            l41.Text = "  Colony bounds diminish (%):";
            l41.Size = standardSize;
            l41.Location = new System.Drawing.Point(0, 240);

            System.Windows.Forms.Label l42 = new System.Windows.Forms.Label();
            l42.Text = " Colony position greatly deplaced (%): ";
            l42.Size = standardSize;
            l42.Location = new System.Drawing.Point(300, 240);

            System.Windows.Forms.Label l43 = new System.Windows.Forms.Label();
            l43.Text = " Colony color difference (%): ";
            l43.Size = standardSize;
            l43.Location = new System.Drawing.Point(600, 240);

            System.Windows.Forms.TextBox tb41 = new System.Windows.Forms.TextBox();
            tb41.Size = new System.Drawing.Size(40, 20);
            tb41.Text = AdvancedOptions._dBoundsDiminish.ToString();
            tb41.Location = new System.Drawing.Point(210, 240);

            System.Windows.Forms.TextBox tb42 = new System.Windows.Forms.TextBox();
            tb42.Size = new System.Drawing.Size(40, 20);
            tb42.Text = AdvancedOptions._dGreatDeplacement.ToString();
            tb42.Location = new System.Drawing.Point(510, 240);

            System.Windows.Forms.TextBox tb43 = new System.Windows.Forms.TextBox();
            tb43.Size = new System.Drawing.Size(40, 20);
            tb43.Text = AdvancedOptions._dMaxColorDiff.ToString();
            tb43.Location = new System.Drawing.Point(810, 240);


            System.Windows.Forms.Label l44 = new System.Windows.Forms.Label();
            l44.Text = "  Colony merging tolerance (%):";
            l44.Size = standardSize;
            l44.Location = new System.Drawing.Point(0, 270);

            System.Windows.Forms.Label l45 = new System.Windows.Forms.Label();
            l45.Text = " Colony centers merging distance: ";
            l45.Size = standardSize;
            l45.Location = new System.Drawing.Point(300, 270);

            System.Windows.Forms.Label l46 = new System.Windows.Forms.Label();
            l46.Text = " Colony minimum size: ";
            l46.Size = standardSize;
            l46.Location = new System.Drawing.Point(600, 270);

            System.Windows.Forms.TextBox tb44 = new System.Windows.Forms.TextBox();
            tb44.Size = new System.Drawing.Size(40, 20);
            tb44.Text = AdvancedOptions._dMergingTolerance.ToString();
            tb44.Location = new System.Drawing.Point(210, 270);

            System.Windows.Forms.TextBox tb45 = new System.Windows.Forms.TextBox();
            tb45.Size = new System.Drawing.Size(40, 20);
            tb45.Text = AdvancedOptions._nMergingDistance.ToString();
            tb45.Location = new System.Drawing.Point(510, 270);

            System.Windows.Forms.TextBox tb46 = new System.Windows.Forms.TextBox();
            tb46.Size = new System.Drawing.Size(40, 20);
            tb46.Text = AdvancedOptions._nMinimumSize.ToString();
            tb46.Location = new System.Drawing.Point(810, 270);

            System.Windows.Forms.Label l47 = new System.Windows.Forms.Label();
            l47.Text = " Colony size matching difference: ";
            l47.Size = standardSize;
            l47.Location = new System.Drawing.Point(0, 300);

            System.Windows.Forms.Label l48 = new System.Windows.Forms.Label();
            l48.Text = " Colony location matching difference: ";
            l48.Size = standardSize;
            l48.Location = new System.Drawing.Point(300, 300);

            System.Windows.Forms.TextBox tb47 = new System.Windows.Forms.TextBox();
            tb47.Size = new System.Drawing.Size(40, 20);
            tb47.Text = AdvancedOptions._nSizeThreshold.ToString();
            tb47.Location = new System.Drawing.Point(210, 300);

            System.Windows.Forms.TextBox tb48 = new System.Windows.Forms.TextBox();
            tb48.Size = new System.Drawing.Size(40, 20);
            tb48.Text = AdvancedOptions._nLocationThreshold.ToString();
            tb48.Location = new System.Drawing.Point(510, 300);


           
            System.Windows.Forms.Label t5 = new System.Windows.Forms.Label();
            t5.Text = " Colony related events settings ";
            t5.Size = new System.Drawing.Size(250, 15);
            t5.Font = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
            t5.Location = new System.Drawing.Point(50, 340);


            System.Windows.Forms.Label l51 = new System.Windows.Forms.Label();
            l51.Text = "  Colony abnormal growth (%):";
            l51.Size = standardSize;
            l51.Location = new System.Drawing.Point(0, 370);

            System.Windows.Forms.Label l52 = new System.Windows.Forms.Label();
            l52.Text = " Colony bounds to borders min distance: ";
            l52.Size = standardSize;
            l52.Location = new System.Drawing.Point(300, 370);

            System.Windows.Forms.TextBox tb51 = new System.Windows.Forms.TextBox();
            tb51.Size = new System.Drawing.Size(40, 20);
            tb51.Text = AdvancedOptions._dAbnormalGrowth.ToString();
            tb51.Location = new System.Drawing.Point(210, 370);

            System.Windows.Forms.TextBox tb52 = new System.Windows.Forms.TextBox();
            tb52.Size = new System.Drawing.Size(40, 20);
            tb52.Text = AdvancedOptions._nMinimumDistance.ToString();
            tb52.Location = new System.Drawing.Point(510, 370);


            System.Windows.Forms.Label infoLabel = new System.Windows.Forms.Label();
            infoLabel.Text = "  More info regarding parameter significance and recommended values in README file";
            infoLabel.Size = new System.Drawing.Size(400, 15);
            infoLabel.Location = new System.Drawing.Point(50, 480);


            System.Windows.Forms.Button sendBut = new System.Windows.Forms.Button();
            sendBut.Text = "Accept";
            sendBut.Size = new System.Drawing.Size(130, 25);
            sendBut.Location = new System.Drawing.Point(700, 450);
            sendBut.Click += new EventHandler(settingsFormClicked);


            settingsForm.Controls.Add(title);

            settingsForm.Controls.Add(t1);

            settingsForm.Controls.Add(l11);
            settingsForm.Controls.Add(l12);
            settingsForm.Controls.Add(l13);
            settingsForm.Controls.Add(tb11);
            settingsForm.Controls.Add(tb12);
            settingsForm.Controls.Add(tb13);

            settingsForm.Controls.Add(l2);
            settingsForm.Controls.Add(tb2);

            settingsForm.Controls.Add(t3);

            settingsForm.Controls.Add(l31);
            settingsForm.Controls.Add(l32);
            settingsForm.Controls.Add(l33);
            settingsForm.Controls.Add(tb31);
            settingsForm.Controls.Add(tb32);
            settingsForm.Controls.Add(tb33);

            settingsForm.Controls.Add(t4);

            settingsForm.Controls.Add(l41);
            settingsForm.Controls.Add(l42);
            settingsForm.Controls.Add(l43);
            settingsForm.Controls.Add(tb41);
            settingsForm.Controls.Add(tb42);
            settingsForm.Controls.Add(tb43);
            settingsForm.Controls.Add(l44);
            settingsForm.Controls.Add(l45);
            settingsForm.Controls.Add(l46);
            settingsForm.Controls.Add(tb44);
            settingsForm.Controls.Add(tb45);
            settingsForm.Controls.Add(tb46);
            settingsForm.Controls.Add(l47);
            settingsForm.Controls.Add(l48);
            settingsForm.Controls.Add(tb47);
            settingsForm.Controls.Add(tb48);

            settingsForm.Controls.Add(t5);

            settingsForm.Controls.Add(l51);
            settingsForm.Controls.Add(l52);
            settingsForm.Controls.Add(tb51);
            settingsForm.Controls.Add(tb52);

            settingsForm.Controls.Add(infoLabel);
            settingsForm.Controls.Add(sendBut);


            settingsForm.Show();
        }

        private void settingsFormClicked(object sender, EventArgs e)
        {
            System.Windows.Forms.Button b = (System.Windows.Forms.Button)sender;
            Form form = (Form)b.Parent;

            List<string> parList = new List<string>();

            foreach (object child in form.Controls)
            {
                try
                {
                    System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)child;
                    parList.Add(tb.Text);

                }
                catch (Exception) { }
            }

            ModifyStaticParameters(parList, form);

        }

        private void ModifyStaticParameters(List<string> list, Form f)
        {
            try
            {

                AdvancedOptions._nSimilarityTolerance = Int32.Parse(list[0]);
                AdvancedOptions._nRelevanceThreshold = Int32.Parse(list[1]);
                AdvancedOptions._nROIMargin = Int32.Parse(list[2]);
                AdvancedOptions._dEpsilonValue = Double.Parse(list[3]);
                AdvancedOptions._nThresholdValue = Int32.Parse(list[4]);
                AdvancedOptions._nTopHysteresis = Int32.Parse(list[5]);
                AdvancedOptions._nBottomHysteresis = Int32.Parse(list[6]);
                AdvancedOptions._dBoundsDiminish = Double.Parse(list[7]);
                AdvancedOptions._dGreatDeplacement = Double.Parse(list[8]);
                AdvancedOptions._dMaxColorDiff = Double.Parse(list[9]);
                AdvancedOptions._dMergingTolerance = Double.Parse(list[10]);
                AdvancedOptions._nMergingDistance = Int32.Parse(list[11]);
                AdvancedOptions._nMinimumSize = Int32.Parse(list[12]);
                AdvancedOptions._nSizeThreshold = Int32.Parse(list[13]);
                AdvancedOptions._nLocationThreshold = Int32.Parse(list[13]);
                AdvancedOptions._dAbnormalGrowth = Double.Parse(list[13]);
                AdvancedOptions._nMinimumDistance = Int32.Parse(list[13]);

                f.Close();

            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Parameter parsing error");
            }

        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Importing for analysis
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void import_Button_Click(object sender, RoutedEventArgs e)
        {
            ImportBorder.Visibility = Visibility.Visible;
            ImageBorder.Visibility = Visibility.Hidden;
            objShowLabel.Visibility = Visibility.Hidden;
            DisclaimerLabel.Visibility = Visibility.Hidden;

            ShowButton.IsEnabled = false;
        }

        // Import parameters being extracted from the UI

        private void ImportConfirm_Button_Click(object sender, RoutedEventArgs e)
        {
            string folderImport = (string)FolderImportLabel.Content;
            string folderSave = (string)FolderSaveLabel.Content;
            string mapFolder = (string)MapImportLabel.Content;

            if (folderImport == "Not defined" || folderSave == "Not defined")
            {
                System.Windows.MessageBox.Show(" Invalid directory path");
                return;
            }

            if (mapFolder == "Not defined (classification analysis only)" && ImportChck2.IsChecked.GetValueOrDefault())
            {
                System.Windows.MessageBox.Show(" A source Kohonen network must be chosen "+ Environment.NewLine + " for a classification analysis to be done");
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

            if (!ImportChck1.IsChecked.GetValueOrDefault() && ImportChck2.IsChecked.GetValueOrDefault())
            {
                System.Windows.MessageBox.Show("Classification analysis cannot be performed without a " + Environment.NewLine + " previous Colony Tracking analysis.");
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

            AnalysisWindow aw = new AnalysisWindow(null, countA, classA, mapFolder);

            ImportBorder.Visibility = Visibility.Hidden;
            ImportChck1.IsChecked = false;
            ImportChck2.IsChecked = false;

            FolderImportLabel.Content = "Not defined";
            FolderSaveLabel.Content = "Not defined";

            // Count analysis is launched in a separated thread

            if(classA && !countA)
            {
                System.Windows.MessageBox.Show("Classification analysis cannot be performed without a " + Environment.NewLine + " previous Colony Tracking analysis");
                return;
            }

            if (countA && !classA)
            {
                Thread analysisThread = new Thread(aw.getCount().staticAnalysis);

                List<object> list = new List<object>();
                list.Add(images);
                list.Add(this);
                list.Add(folderSave);
                list.Add(false);

                analysisThread.Start(list);


                AnalysisBorder.Visibility = Visibility.Visible;
                AnalysisBorder.Background = System.Windows.Media.Brushes.LightGray;
            }

            if (classA)
            {

                aw.getClass().Init(MapImportLabel.Content.ToString(), aw.getCount());

                MapImportLabel.Content = "Not defined (classification analysis only)";

                Thread analysisThread = new Thread(aw.getCount().staticAnalysis);

                List<object> list = new List<object>();
                list.Add(images);
                list.Add(this);
                list.Add(folderSave);
                list.Add(true);

                AnalysisBorder.Visibility = Visibility.Visible;

                analysisThread.Start(list);

                AnalysisBorder.Background = System.Windows.Media.Brushes.LightGray;

                mapSourceFolders = new List<string>();
            }

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

            sizes = op.getSize(op);
            locations = op.getLocation(op);

            List<System.Windows.Shapes.Rectangle> outlineDefinitionList = new List<System.Windows.Shapes.Rectangle>();

            // Selection border holds a definition for an object and it's showing structure. (See SelectionBorder constructor definition)

            List<SelectionBorder> transparentCanvasList = new List<SelectionBorder>();

            System.Drawing.Point globalPicSize = op.getGlobalSize(op);

            ImageBorder.Visibility = Visibility.Visible;
            objShowLabel.Visibility = Visibility.Visible;
            DisclaimerLabel.Visibility = Visibility.Visible;

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

                    // Analysis info acquisition

                    bool[] analysis = new bool[2];
                    if (Chk1.IsChecked.GetValueOrDefault()) { analysis[0] = true; }
                    else { analysis[0] = false; }

                    if (Chk2.IsChecked.GetValueOrDefault()) { analysis[1] = true; }
                    else { analysis[1] = false; }

                    if (analysis[1] && !analysis[0])
                    {
                        System.Windows.MessageBox.Show("Classification analysis cannot be performed without a " + Environment.NewLine + " previous Colony Tracking analysis");
                        return;
                    }

                    if (analysis[1])
                    {
                        ChooseSourceFolder();
                    }
                    else
                    {
                        mapSourceFolders.Add("");
                    }

                    Chk1.IsChecked = false;
                    Chk2.IsChecked = false;

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

                    analysisToPerform.Add(analysis);

                    // UI reset

                    ParametersBorder.Visibility = Visibility.Hidden;
                    CaptureConfirmButton.Visibility = Visibility.Visible;
                    minutesTextBox.Text = "0";
                    hoursTextBox.Text = "0";
                    numberOfCapturesTextBox.Text = "0";
                    delayHTextBox.Text = "0";
                    delayMinTextBox.Text = "0";
                    FolderLabel.Content = "Not defined";

                    CaptureCancelButton.Visibility = Visibility.Visible;

                    ImportButton.IsEnabled = true;

                }
            }
            else
            {
                System.Windows.MessageBox.Show("Parameter parsing error");
            }
        }

        private void ChooseSourceFolder()
        {
            Form f = new Form();
            f.Height = 200;
            f.Width = 500;
            f.Location = new System.Drawing.Point(1000, 500);

            f.Text  = "Choose Classification Map Source Location";
            f.FormClosing += new FormClosingEventHandler(AvoidClosing);

            System.Windows.Forms.Button b = new System.Windows.Forms.Button();
            b.Text = "Choose Location";
            b.Size = new System.Drawing.Size(100, 20);
            b.Click += new EventHandler(FormButtonClicked);
            b.Location = new System.Drawing.Point(30, 30);

            System.Windows.Forms.Label l = new System.Windows.Forms.Label();
            l.Text = "Not defined";
            l.Size = new System.Drawing.Size(200, 40);
            l.Location = new System.Drawing.Point(150, 30);


            System.Windows.Forms.Button b2 = new System.Windows.Forms.Button();
            b2.Text = "Accept";
            b2.Size = new System.Drawing.Size(60, 20);
            b2.Click += new EventHandler(AcceptClicked);
            b2.Location = new System.Drawing.Point(300, 100);

            f.Controls.Add(b);
            f.Controls.Add(b2);
            f.Controls.Add(l);

            f.Show();

            CaptureConfirmButton.IsEnabled = false;
        }

        private void AcceptClicked(object sender, EventArgs e)
        {
            System.Windows.Forms.Button b = (System.Windows.Forms.Button)sender;

            Form f = (Form)b.Parent;

            foreach (object child in f.Controls)
            {
                try
                {
                    System.Windows.Forms.Label l = (System.Windows.Forms.Label)child;
                    mapSourceFolders.Add(l.Text);
                }
                catch (Exception) { }
            }

            f.Close();
        }

        private void AvoidClosing(object sender, FormClosingEventArgs e)
        {
            Form f = (Form)sender;

            foreach (object child in f.Controls)
            {
                try
                {
                    System.Windows.Forms.Label l = (System.Windows.Forms.Label)child;
                    if (l.Text == "Not defined")
                    {
                        e.Cancel = true;
                    }else
                    {
                        CaptureConfirmButton.IsEnabled = true;
                        e.Cancel = false;
                    }
                }
                catch (Exception) { }
            }
        }

        private void FormButtonClicked(object sender, EventArgs e)
        {
            FolderBrowserDialog sfd = new FolderBrowserDialog();

            string fileLocation = "";

            DialogResult res = sfd.ShowDialog();

            if (res == DialogResult.OK && !string.IsNullOrWhiteSpace(sfd.SelectedPath))
            {
                fileLocation = sfd.SelectedPath;
            }

            System.Windows.Forms.Button b = (System.Windows.Forms.Button)sender;

            Form f = (Form)b.Parent;

            foreach (object child in f.Controls)
            {
                try
                {
                    System.Windows.Forms.Label l = (System.Windows.Forms.Label)child;
                    l.Text = fileLocation;
                }
                catch (Exception) { }
            }
        }

        // Launch captures

        private void CaptureConfirm_Button_Click(object sender, RoutedEventArgs e)
        {
            // Enable navigation

            enableNavigation();

            // Launch pending captures
            
            List<int> indexes = new List<int>();
            List<PcPhysicalPoint> sendingLocs = new List<PcPhysicalPoint>();
            List<System.Drawing.Point> sendingSizes = new List<System.Drawing.Point>(); 

            for (int i = 0; i < parameters.Count; i++)
            {
                indexes.Add(parameters.ElementAt(i)[2]);
                sendingLocs.Add(locations[parameters.ElementAt(i)[2]]);
                sendingSizes.Add(sizes[parameters.ElementAt(i)[2]]);

            }
            
            cp.AddCaptures(parameters, indexes, saveFolders, analysisToPerform, processNames, sendingLocs, sendingSizes, mapSourceFolders);
            this.NavigationService.Navigate(cp);

            parameters = new List<int[]>();

            CaptureDetailsLabel.Content = "";
            saveFolders = new List<string>();
            mapSourceFolders = new List<string>();
            analysisToPerform = new List<bool[]>();
            processNames = new List<string>();
            mapSourceFolders = new List<string>();

            ShowButton.IsEnabled = false;

            CaptureCancelButton.Visibility = Visibility.Hidden;

        }
    }
}
