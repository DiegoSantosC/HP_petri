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
        
        private Thread newCaptureThread, playThread;
        private CapturePreviews cp;
        private LogFile file;

        // Params to be shared with the PlayHandler
        public double speed;
        public bool playing;

        public AnalysisWindow aw;


        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      UI related functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // This window is responsble for handling the captures of the object that represents, and thus 
        // manages the MainCapture thread and hosts capture taking functions 
        public CaptureWindow(CapturePreviews capt, Image img, int[] param, string folder, bool[] analysis, string name, PcPhysicalPoint location, System.Drawing.Point size, bool moved)
        {
            InitializeComponent();

            this.Width = 1200;


            this.Height = 900;

            this.Left = 300;
            this.Top = 100;

            
            cp = capt;

            cfs = new List<captureFramework>();

            // A task holds details for a capture process (See <Task> constructior definition)
            List<Uri> u = new List<Uri>();

            t = new Task(this, param[0], param[1], param[2], param[3], u, folder, analysis[0], analysis[1], name, location, size);

            aw = new AnalysisWindow(this, t.getCountAnalysis(), t.getClassAnalysis());

            Directory.CreateDirectory(folder);
            file = new LogFile(t.getFolder(), t.getIndex(), t.getName(), t.getNumberOfCaptures());

            file.BuildAndSave();

            titleLabel.Content = name;

            if (!analysis[0] && !analysis[1])
            {
                generalAnalysisBut.Visibility = Visibility.Hidden;
            }

            // Thread initialization for triggering the captures

            MainCapture newCapture = new MainCapture();
            newCaptureThread = new Thread(newCapture.StartCapture);
            newCaptureThread.SetApartmentState(ApartmentState.STA);
            newCaptureThread.Start(t);
            running = true;
            playing = false;

            // UI elements initialization

            StackPanel aux = new StackPanel();
            Rectangle frame = new Rectangle();
            frame.Visibility = Visibility.Hidden;
            frame.Width = 575;
            frame.Height = 400;

            frame.Stroke = Brushes.Black;
            frame.StrokeThickness = 4;
            
            aux.Children.Add(frame);
            StackPanel.SetZIndex(aux, 10);

            ImagesCanvas.Children.Add(aux);

            StackPanel aux2 = new StackPanel();
            Rectangle frame2 = new Rectangle();
            frame2.Width = 500;
            frame2.Height = 120;

            frame2.Stroke = Brushes.Black;
            frame2.StrokeThickness = 4;

            aux2.Children.Add(frame2);
            StackPanel.SetZIndex(aux2, 10);

            EventsCanvas.Children.Add(aux2);

            FirstCapture(img, moved);

            Info_Init();

            Logo_Init();

            PlayNStop_Init();

            CapturesListBox.SelectionChanged += new SelectionChangedEventHandler(listBoxClicked);

            EventsListBox.SelectionChanged += new SelectionChangedEventHandler(eventClicked);

            if (analysis[0]) { EventsListBox.Visibility = Visibility.Visible; CountAnalysisBut.Visibility = Visibility.Visible; frame.Visibility = Visibility.Visible;}

            speed = 1;

        }

        // When a listBoxItem is clicked, it's capture is focused

        private void listBoxClicked(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            ListBoxItem item = (ListBoxItem)lb.SelectedItem;

            Grid sp = (Grid)item.Content;

            Image img, clone = new Image();

            StackPanel childSP = new StackPanel();

            int counter = 0;
            foreach (object child in sp.Children)
            {
                if (counter == 2)
                {
                    childSP = (StackPanel)child;

                    foreach (object childImg in childSP.Children)
                    {
                        img = (Image)childImg;
                        clone.Source = img.Source;
                    }
                }

                counter++;
            }

            for (int i = 0; i < cfs.Count; i++)
            {
                cfs.ElementAt(i).getBorder().Background = Brushes.White;
            }

            int index = Int32.Parse(childSP.Uid);

            timeLabel.Content = "Capture taken at: " + cfs.ElementAt(index).getTime();

            cfs.ElementAt(index).getBorder().Background = Brushes.LightBlue;

            cf.getCapturePanel().Children.Clear();

            clone.Uid = cfs.ElementAt(index).getCapturePanel().Uid;
            cf.getCapturePanel().Children.Add(clone);

        }

        // UI elements initialization

        private void PlayNStop_Init()
        {
            Image play = new Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Play.png", UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            play.Source = src2;
            play.Stretch = Stretch.Uniform;
            play_stopSP.Children.Add(play);

            Image speedUp = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\SpeedUp.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            speedUp.Source = src;
            speedUp.Stretch = Stretch.Uniform;
            speedUp_SP.Children.Add(speedUp);

            Image speedDown = new Image();
            BitmapImage src3 = new BitmapImage();
            src3.BeginInit();
            src3.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\SpeedDown.png", UriKind.Absolute);
            src3.CacheOption = BitmapCacheOption.OnLoad;
            src3.EndInit();
            speedDown.Source = src3;
            speedDown.Stretch = Stretch.Uniform;
            speedDown_SP.Children.Add(speedDown);
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

        // Comment button is initialized

        private Button initializeButton(int index)
        {
            Button commentBut = new Button();

            commentBut.Content = "Add Comment";
            commentBut.HorizontalAlignment = HorizontalAlignment.Center;
            commentBut.VerticalAlignment = VerticalAlignment.Center;

            commentBut.Width = 100;
            commentBut.Height = 24;
            commentBut.FontSize = 14;

            commentBut.Click += new RoutedEventHandler(addComment);
            commentBut.Uid = index.ToString();

            return commentBut;
        }

        // A user made comment is added for a specific capture

        private void addComment(object sender, RoutedEventArgs e)
        {
            Button senderBut = (Button)sender;
            string indexString = senderBut.Uid;

            System.Windows.Forms.Form commentDialog = new System.Windows.Forms.Form();
            commentDialog.Name = "Instert comment";

            commentDialog.Size = new System.Drawing.Size(400, 300);

            System.Windows.Forms.Button insertButton = new System.Windows.Forms.Button();

            insertButton.Width = 60;
            insertButton.Height = 23;
            insertButton.Text = "Insert";
            insertButton.Location = new System.Drawing.Point(200, 160);
            insertButton.Name = indexString;
            insertButton.Click += new EventHandler(insertComment);

            // Comments content acquisition;

            Grid parentGrid = (Grid)senderBut.Parent;

            int counter = 0; string content = "";

            foreach (object child in parentGrid.Children)
            {
               if (counter == 1)
                {
                    Label commentLabel = (Label)child;

                    content = commentLabel.Content.ToString();

                }
                counter++;
            }


            System.Windows.Forms.TextBox tb = new System.Windows.Forms.TextBox();
            tb.Width = 220;
            tb.Height = 100;
            tb.Multiline = true;
            tb.Location = new System.Drawing.Point(40, 20);
            tb.Text = content;

            commentDialog.Controls.Add(tb);
            commentDialog.Controls.Add(insertButton);

            commentDialog.Show();

        }

        // Comment inserting handler

        private void insertComment(object sender, EventArgs e)
        {
            string content = "";

            // A form is created to receive user's input

            System.Windows.Forms.Button senderBut = (System.Windows.Forms.Button)sender;
            string indexString = senderBut.Name;

            System.Windows.Forms.Form parentForm = (System.Windows.Forms.Form)senderBut.Parent;

            bool first = true;
            foreach (object child in parentForm.Controls)
            {
                if (first)
                {
                    System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)child;
                    content = tb.Text;
                }
                first = false;
            }

            ListBoxItem itemSelected = (ListBoxItem)CapturesListBox.Items.GetItemAt(Int32.Parse(indexString));

            Grid itemGrid = (Grid)itemSelected.Content;

            int counter = 0;

            // UI is modified after the comment being inserted

            foreach (object child in itemGrid.Children)
            {
                if (counter == 0)
                {
                    Button commentBut = (Button)child;
                    commentBut.Content = "Modify";
                }
                if (counter == 1)
                {
                    Label commentLabel = (Label)child;
                    if (content.Length > 35)
                    {
                        string[] words = content.Split(' ');

                        for (int i = 0; i < words.Length; i++)
                        {
                            commentLabel.Content += " " + words[i];
                            if (i % 5 == 0 && i != 0) { commentLabel.Content += Environment.NewLine; }
                        }

                    }
                    else { commentLabel.Content = content; }

                }
                counter++;
            }

            parentForm.Close();

            string textToAppend = "Comment added on capture " + indexString + Environment.NewLine + content + Environment.NewLine;
            file.AppendData(textToAppend);
        }

        // Buttons functionalities

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            newCaptureThread.Abort();
            CaptureFinished();

            Button b = (Button)sender;
            b.Visibility = Visibility.Hidden;
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

        // UI Closing handling

        private void CaptureWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MainWindow.killRequest)
            {
                newCaptureThread.Abort();
                e.Cancel = false;
                aw.Close();

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
                        aw.requestClosing();
                        aw.Close();

                    }

                }
                else
                {
                    cp.EraseFinishedCapture(Int32.Parse(this.Uid));
                    e.Cancel = false;
                    aw.requestClosing();
                    aw.Close();

                }
                if (playing)
                {
                    playThread.Abort();
                }
            }
        }

        // User made player that switches from capture to capture initialization and button handlers

        private void Play_StopClick(object sender, RoutedEventArgs e)
        {
            if (!playing)
            {
                Image stop = new Image();
                BitmapImage src2 = new BitmapImage();
                src2.BeginInit();
                src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Stop.png", UriKind.Absolute);
                src2.CacheOption = BitmapCacheOption.OnLoad;
                src2.EndInit();
                stop.Source = src2;
                stop.Stretch = Stretch.Uniform;

                play_stopSP.Children.Clear();
                play_stopSP.Children.Add(stop);

                playing = true;

                PlayHandler ph = new PlayHandler();
                playThread = new Thread(ph.StartHandler);
                playThread.SetApartmentState(ApartmentState.STA);
                playThread.Start(this);
            }
            else
            {
                Image play = new Image();
                BitmapImage src2 = new BitmapImage();
                src2.BeginInit();
                src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\play.png", UriKind.Absolute);
                src2.CacheOption = BitmapCacheOption.OnLoad;
                src2.EndInit();
                play.Source = src2;
                play.Stretch = Stretch.Uniform;

                play_stopSP.Children.Clear();
                play_stopSP.Children.Add(play);

                playing = false;

            }

        }

        private void SpeedUp_Click(object sender, RoutedEventArgs e)
        {
            if (speed < 4)
            {
                speed = speed * 2;
                speedLabel.Content = "Speed x" + (1 / speed);
            }
        }

        private void SpeedDown_Click(object sender, RoutedEventArgs e)
        {
            if (speed > 0.25)
            {
                speed = speed / 2;
                speedLabel.Content = "Speed x" + (1 / speed);
            }

        }

        internal void showNextCapture()
        {
            Image img, clone = new Image();
            int index = 0;

            foreach (object child in cf.getCapturePanel().Children)
            {
                img = (Image)child;
                index = Int32.Parse(img.Uid);

            }

            for (int i = 0; i < cfs.Count; i++)
            {
                cfs.ElementAt(i).getBorder().Background = Brushes.White;
            }

            timeLabel.Content = "Capture taken at: " + cfs.ElementAt(index).getTime();

            if (index == cfs.Count - 1)
            {
                foreach (object child in cfs.ElementAt(0).getCapturePanel().Children)
                {
                    img = (Image)child;
                    clone.Source = img.Source;
                    clone.Uid = img.Uid;
                }
                cfs.ElementAt(0).getBorder().Background = Brushes.LightBlue;
                CapturesListBox.SelectedIndex = 0;
                CapturesListBox.Focus();
            }
            else
            {
                foreach (object child in cfs.ElementAt(index + 1).getCapturePanel().Children)
                {
                    img = (Image)child;
                    clone.Source = img.Source;
                    clone.Uid = img.Uid;
                }

                cfs.ElementAt(index + 1).getBorder().Background = Brushes.LightBlue;
                CapturesListBox.SelectedIndex = index + 1;
                CapturesListBox.Focus();
            }

            cf.getCapturePanel().Children.Clear();
            cf.getCapturePanel().Children.Add(clone);

        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Analysis related functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Show analysis of the instant in which an event appeared

        private void eventClicked(object sender, SelectionChangedEventArgs e)
        {
            int selectedChild = EventsListBox.SelectedIndex;
            int counter = 0;

            foreach (ListBoxItem item in EventsListBox.Items)
            {
                if(counter == selectedChild)
                {
                    aw.Show();
                    aw.Navigate(aw.getCount());
                    aw.getCount().initStatics();
                    aw.getCount().Show(Int32.Parse(item.Uid)-1);

                }
            }
        }

        // An even has raised from the analysis 

        public void addEvent(int step, string eventText)
        {
            ListBoxItem item = new ListBoxItem();
            Grid g = new Grid();

            item.Height = 30;
            item.Width = 480;

            ColumnDefinition c1 = new ColumnDefinition();
            //c1.Width = new GridLength(80);
            ColumnDefinition c2 = new ColumnDefinition();
            //c1.Width = new GridLength(400);

            g.ColumnDefinitions.Add(c1);
            g.ColumnDefinitions.Add(c2);

            Label l1 = new Label();
            l1.Width = 80;
            l1.HorizontalAlignment = HorizontalAlignment.Center;
            l1.VerticalAlignment = VerticalAlignment.Center;
            l1.Content = "   Capture " + step;

            Label l2 = new Label();
            l2.HorizontalAlignment = HorizontalAlignment.Center;
            l2.VerticalAlignment = VerticalAlignment.Center;
            l2.Content = eventText;

            Grid.SetColumn(l1, 0);
            Grid.SetColumn(l2, 1);

            g.Children.Add(l1);
            g.Children.Add(l2);

            item.Content = g;
            item.Uid = step.ToString();

            EventsListBox.Items.Add(item);

        }

        // Show the specific anlysis for a capture

        public void Show_Analysis(object sender, RoutedEventArgs e)
        {
            Button senderBut = (Button)sender;

            if (t.getCountAnalysis())
            {
                if (cfs.Count > 1)
                {

                    aw.Show();
                    aw.getCount().initStatics();

                    aw.Navigate(aw.getCount());

                    if (CapturesListBox.SelectedIndex == 0)
                    {
                        aw.getCount().Show(0);
                    }
                    else { aw.getCount().Show(CapturesListBox.SelectedIndex - 1); }
                }
                else { MessageBox.Show("Not enough captures taken for an analysis to be performed"); }
            }
            else
            {

                MessageBox.Show("No analysis selected to be performed");
            }
        }

        // Show general analysis

        public void General_Analysis(object sender, RoutedEventArgs e)
        {
            Button senderBut = (Button)sender;

            if (cfs.Count > 1)
            {
                if (t.getClassAnalysis() || t.getCountAnalysis())
                {
                    aw.Show();
                    aw.getCount().initStatics();
                    aw.Navigate(aw.getPicker());
                    aw.getChart().initCharts(aw.getCount(), t.getFolder());

                }
                else
                {
                    MessageBox.Show("No analysis selected to be performed");
                }
            }
            else { MessageBox.Show("Not enough captures taken for an analysis to be performed"); }

        }




        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //      Capture related functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        // This Window will be initialized with an image taken from CapturePreviews 
        // featuring the state of the object to capture prior to the capture process

        private void FirstCapture(Image img, bool moved)
        {
            Image clone1 = new Image();
            Image clone2 = new Image();

            clone1.Uid = "0";
            clone2.Uid = "0";

            clone1.Source = img.Source;
            clone2.Source = img.Source;

            double ratio = clone1.Source.Width / clone1.Source.Height;

            // Adding image to group

            cfs.Add(new captureFramework(clone1, 120, ratio));

            ListBoxItem item = new ListBoxItem();
            Grid arrangingGrid = new Grid();

            arrangingGrid.Width = 560;
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(150);
            ColumnDefinition c2 = new ColumnDefinition();
            c1.Width = new GridLength(350);
            ColumnDefinition c3 = new ColumnDefinition();
            c1.Width = new GridLength();

            arrangingGrid.ColumnDefinitions.Add(c1);
            arrangingGrid.ColumnDefinitions.Add(c2);
            arrangingGrid.ColumnDefinitions.Add(c3);

            Button commentBut = initializeButton(cfs.Count-1);
            commentBut.HorizontalAlignment = HorizontalAlignment.Left;
            commentBut.Margin = new Thickness(30,0,0,0);

            Label emptyLabel = new Label();
            emptyLabel.HorizontalAlignment = HorizontalAlignment.Center;
            emptyLabel.VerticalAlignment = VerticalAlignment.Center;

            Grid.SetColumn(commentBut, 0);
            Grid.SetColumn(emptyLabel, 1);
            Grid.SetColumn(cfs.ElementAt(cfs.Count - 1).getCapturePanel(), 2);

            arrangingGrid.Children.Add(commentBut);
            arrangingGrid.Children.Add(emptyLabel);
            arrangingGrid.Children.Add(cfs.ElementAt(cfs.Count - 1).getCapturePanel());

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().HorizontalAlignment = HorizontalAlignment.Right;
            cfs.ElementAt(cfs.Count - 1).setPosition(new Thickness(0, 0, 30, 0));


            item.Content = arrangingGrid;

            CapturesListBox.Items.Add(item);

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().Uid = (cfs.Count - 1).ToString();
            cfs.ElementAt(cfs.Count - 1).getBorder().Background = Brushes.LightBlue;

            
            // Setting to last image

            cf = new captureFramework(clone2, 400, ratio);

            LastImageCanvas.Children.Clear();
            LastImageCanvas.Children.Add(cf.getBorder());
            LastImageCanvas.Children.Add(cf.getCapturePanel());

            ProjectButton.Visibility = Visibility.Visible;
            infoLabel.Visibility = Visibility.Visible;

            timeLabel.Content = "Capture taken at: " + cf.getTime();
            timeLabel.Visibility = Visibility.Visible;

            file.AppendData("\t First capture taken at :" + cf.getTime());

            file.AppendData("\t " + t.getDelay() + " minutes of delay waited");

            CapturesListBox.SelectedIndex = 0;
            CapturesListBox.Focus();

            if (moved)
            {
                emptyLabel.Content = "Capture #" + (cfs.Count - 1) + " " + cf.getTime() + ": " + Environment.NewLine + "Object deplacement detected";
            }
            else
            {
                emptyLabel.Content = "Capture #" + (cfs.Count - 1) + " " + cf.getTime() + ": " + "Success capture";
            }

            // Set image into bitmap format to be analyzed

            if (t.getCountAnalysis())
            {
                MemoryStream ms = new MemoryStream();
                BmpBitmapEncoder bbe = new BmpBitmapEncoder();
                bbe.Frames.Add(BitmapFrame.Create(new Uri(img.Source.ToString(), UriKind.RelativeOrAbsolute)));
                bbe.Save(ms);
                System.Drawing.Image img2 = System.Drawing.Image.FromStream(ms);

                aw.getCount().setBackgound(img2);
            }
        }     

        // Information about the capture is initialized and updated

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

        
        // Function called from MainCapture each time counter is trigged

        public void Trigger_Capture()
        {
            if(cfs.Count -1 < t.getNumberOfCaptures())
            {
                Console.WriteLine("Executing capture");
                MomentCapture.Capture(t, false);

            }
        }

        // An error has occurred during capture process

        public void CaptureError()
        {
            MessageBox.Show("Error during the capture process, no object matching target could be found");
            CaptureFinished();
            newCaptureThread.Abort();

        }

        // Management of a new taken image

        public void DrawImage(bool moved)
        {
            Image img1 = new Image();
            Image img2 = new Image();

            BitmapImage src1 = new BitmapImage();
            BitmapImage src2 = new BitmapImage();

            img1.Uid = cfs.Count.ToString();
            img2.Uid = cfs.Count.ToString();

            Console.WriteLine(t.getName() + " showing capture " + (cfs.Count - 1));


            // Img acquisition 

            src1.BeginInit();
            src1.UriSource = t.getCaptures().ElementAt(t.getCaptures().Count - 1);
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

            cfs.Add(new captureFramework(img1, 120, ratio));

            ListBoxItem item = new ListBoxItem();
            Grid arrangingGrid = new Grid();

            arrangingGrid.Width = 560;
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(150);
            ColumnDefinition c2 = new ColumnDefinition();
            c1.Width = new GridLength(350);
            ColumnDefinition c3 = new ColumnDefinition();
            c1.Width = new GridLength();

            arrangingGrid.ColumnDefinitions.Add(c1);
            arrangingGrid.ColumnDefinitions.Add(c2);
            arrangingGrid.ColumnDefinitions.Add(c3);

            Button commentBut = initializeButton(cfs.Count - 1);
            commentBut.HorizontalAlignment = HorizontalAlignment.Left;
            commentBut.Margin = new Thickness(30, 0, 0, 0);

            Label emptyLabel = new Label();
            emptyLabel.HorizontalAlignment = HorizontalAlignment.Center;
            emptyLabel.VerticalAlignment = VerticalAlignment.Center;

            Grid.SetColumn(commentBut, 0);
            Grid.SetColumn(emptyLabel, 1);
            Grid.SetColumn(cfs.ElementAt(cfs.Count - 1).getCapturePanel(), 2);

            arrangingGrid.Children.Add(commentBut);
            arrangingGrid.Children.Add(emptyLabel);
            arrangingGrid.Children.Add(cfs.ElementAt(cfs.Count - 1).getCapturePanel());

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().HorizontalAlignment = HorizontalAlignment.Right;
            cfs.ElementAt(cfs.Count - 1).setPosition(new Thickness(0, 0, 30, 0));


            item.Content = arrangingGrid;

            CapturesListBox.Items.Add(item);

            cfs.ElementAt(cfs.Count - 1).getCapturePanel().Uid = (cfs.Count - 1).ToString();
            cfs.ElementAt(cfs.Count - 1).getBorder().Background = Brushes.LightBlue;

            for (int i = 0; i < cfs.Count - 1; i++)
            {
                cfs.ElementAt(i).getBorder().Background = Brushes.White;
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

            file.AppendData("\t Capture " + cfs.Count() + " taken at: " + cf.getTime());

            CapturesListBox.SelectedIndex = cfs.Count - 1;
            CapturesListBox.Focus();

            if (moved)
            {
                emptyLabel.Content = "Capture #" + (cfs.Count -1) + " " + cf.getTime() + ": " + Environment.NewLine +  "Object deplacement detected";
            }
            else
            {
                emptyLabel.Content = "Capture #" + (cfs.Count - 1) + " " + cf.getTime() + ": " + "Success capture";
            }

            // Set image into bitmap format to be analyzed

            if (t.getCountAnalysis())
            {
                MemoryStream ms = new MemoryStream();
                BmpBitmapEncoder bbe = new BmpBitmapEncoder();
                bbe.Frames.Add(BitmapFrame.Create(new Uri(img1.Source.ToString(), UriKind.RelativeOrAbsolute)));
                bbe.Save(ms);
                System.Drawing.Image bmpImg = System.Drawing.Image.FromStream(ms);

                int[] events = aw.getCount().newStep(bmpImg, DateTime.Now.ToString("hh:mm:ss"));

                if (events.Length > 0) file.AppendData("\t\t Events happened : ");
                     
                for(int i = 0; i < events.Length; i++)
                {
                    addEvent(cfs.Count -1, AdvancedOptions._sEventMessages[events[i]]);
                    file.AppendData("\t\t\t " + AdvancedOptions._sEventMessages[events[i]]);
                    
                }
                if (events.Length > 0) file.AppendData(" ");
            }
        }

        // Capture starting and finished handlers

        internal void CaptureFinished()
        {
            CapturePreviews.DecrementCaptures();
            finishedCapture.Background = Brushes.Green;
            RunningLabel.Content = "Capture Process Finished";
            running = false;

            StopButton.Visibility = Visibility.Hidden;

            if(t.getCountAnalysis())aw.getChart().initCharts(aw.getCount(), t.getFolder());
        }

        public void startTriggered()
        {
            finishedCapture.Background = Brushes.Red;
            RunningLabel.Content = "Capture running";
            RunningLabel.Foreground = Brushes.White;
        }
    }
}
