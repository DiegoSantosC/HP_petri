using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Threading;
using System.Windows.Shapes;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for CountAnalytcs.xaml
    /// 
    /// Funcionalities : UI created to hold both the colony count analysis and the
    /// UI that shows it.
    /// 
    /// Launched by AnalysisWindow if countAnalysis is selected as an option
    /// 
    /// Fed with content by the capture process being running by captureWindow
    /// 
    /// </summary>
    public partial class CountAnalytics : Page
    {
        private Bitmap background;
        private List<Bitmap> Tracking_Images;
        private List<List<ListBoxItem>> cluster_Specs;
        private List<List<System.Windows.Shapes.Rectangle>> cluster_locations;
        private Tracking track;
        private AnalysisWindow aw;
        private int step;
        private bool changing, existsClass, analysisInProgress;
        private int[] events;

        public CountAnalytics(AnalysisWindow a)
        {
            InitializeComponent();

            aw = a;

            track = new Tracking();

            step = 0;

            changing = false;

            cluster_Specs = new List<List<ListBoxItem>>();
            cluster_locations = new List<List<System.Windows.Shapes.Rectangle>>();

            Tracking_Images = new List<Bitmap>();

            ScrollLeft_Init();

            ScrollRight_Init();

            ListBox_Init();

            Logo_Init();

            analysisInProgress = false;
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // UI functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


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

        private void ListBox_Init()
        {
            StackPanel aux = new StackPanel();
            System.Windows.Shapes.Rectangle frame = new System.Windows.Shapes.Rectangle();
            frame.Width = 500;
            frame.Height = 400;

            frame.Stroke = System.Windows.Media.Brushes.Blue;
            frame.StrokeThickness = 3;

            aux.Children.Add(frame);
            StackPanel.SetZIndex(aux, 10);

            ClusterCanvas.Children.Add(aux);

            ClusterListBox.SelectionChanged += new SelectionChangedEventHandler(showLocation);
        }

        // An image is shown in the UI

        public void Show(int index)
        {
            // Changing blocks the execution of the handler "ListBox Selection Changed"

            changing = true;

            sampleSP.Children.Clear();

            // A System.Windows.Controls.Image is created from a System.Drawing.Image by acquiring its Bitmap

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            IntPtr ip = Tracking_Images[index].GetHbitmap();

            BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            img.Source = src;
            sampleSP.Children.Add(img);

            infoLabel.Content = (index + 1).ToString() + "/" + Tracking_Images.Count.ToString();

            rectanglesCanvas.Children.Clear();

            ModifyListBox(index);

            changing = false;

            timeLabel.Content = "Capture taken at " + Tracking_Images[index].Tag;

        }

        // Building the new colony specification items according to the image that is shown in the general panel 

        private void ModifyListBox(int index)
        {
            ClusterListBox.Items.Clear();

            if(cluster_Specs.Count == 0)
            {
                return;
            }

            for (int i = 0; i < cluster_Specs[index].Count(); i++)
            {
                ClusterListBox.Items.Add(cluster_Specs[index][i]);
            }
        }

        // UI elements are initialized whenever the UI is called to be shown with the current state of the analysis
        // Elements initialized: Rectangles matching clusters' positions, listBoxItems holding clusters' specifications 
        public void initStatics()
        {
            for (int i = 0; i < track.getTimelineTracking().Count; i++)
            {
                List<ListBoxItem> blobs = new List<ListBoxItem>();
                List<System.Windows.Shapes.Rectangle> locations = new List<System.Windows.Shapes.Rectangle>();

                for (int j = 0; j < track.getTimelineTracking()[i].Count; j++)
                {
                    Cluster c = track.getTimelineTracking()[i][j];

                    ListBoxItem lbItem = createItemFromCluster(c);
                    System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();

                    r.Stroke = System.Windows.Media.Brushes.LightGreen;
                    r.StrokeThickness = 2;

                    // Rectangles' size and position is scaled 

                    r.Width = ((c.getBoundingBox()[2] - c.getBoundingBox()[0]) * 290 / Tracking_Images[0].Width) + 5;
                    r.Height = ((c.getBoundingBox()[3] - c.getBoundingBox()[1]) * 290 / Tracking_Images[0].Height) + 5;

                    r.Margin = new Thickness((c.getBoundingBox()[0] * 290 / Tracking_Images[0].Width) - 2, (c.getBoundingBox()[1] * 290 / Tracking_Images[0].Height) - 2, 0, 0);

                    blobs.Add(lbItem);
                    locations.Add(r);
                }

                cluster_Specs.Add(blobs);
                cluster_locations.Add(locations);
            }
        }

        // A cluster's location is shown in the general panel whenever this cluster is selected in the listbox

        private void showLocation(object sender, SelectionChangedEventArgs e)
        {
            if (!changing)
            {
                String s = infoLabel.Content.ToString();
                String[] data = s.Split('/');

                int currentCapture = Int32.Parse(data[0]);

                int currentCluster = ClusterListBox.SelectedIndex;

                rectanglesCanvas.Children.Clear();

                rectanglesCanvas.Children.Add(cluster_locations[currentCapture - 1][currentCluster]);

            }

        }

        private void showLocation(object sender, MouseEventArgs e)
        {
            String s = infoLabel.Content.ToString();
            String[] data = s.Split('/');

            int currentCapture = Int32.Parse(data[0]);

            int currentCluster = ClusterListBox.SelectedIndex;

            rectanglesCanvas.Children.Clear();

            rectanglesCanvas.Children.Add(cluster_locations[currentCapture - 1][currentCluster]);

        }

        // ListBoxItems creation from cluster info
        // Information show: cluster's ID, size(cm), Average RGB values and events relating to it

        private ListBoxItem createItemFromCluster(Cluster c)
        {
            ListBoxItem item = new ListBoxItem();
            Grid g = new Grid();

            item.Height = 80;
            item.Width = 500;

            // Item structure building

            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(350);
            ColumnDefinition c2 = new ColumnDefinition();
            c1.Width = new GridLength(120);
            ColumnDefinition c3 = new ColumnDefinition();
            c1.Width = new GridLength(120);
            ColumnDefinition c4 = new ColumnDefinition();
            c1.Width = new GridLength(40);


            g.ColumnDefinitions.Add(c1);
            g.ColumnDefinitions.Add(c2);
            g.ColumnDefinitions.Add(c3);
            g.ColumnDefinitions.Add(c4);

            // Item content filling

            Label l1 = new Label();
            l1.Width = 40;
            l1.HorizontalAlignment = HorizontalAlignment.Left;
            l1.VerticalAlignment = VerticalAlignment.Center;
            l1.Content = " Id : " + c.getId().ToString();

            Label l2 = new Label();
            l2.Width = 120;
            l2.HorizontalAlignment = HorizontalAlignment.Left;
            l2.VerticalAlignment = VerticalAlignment.Center;
            l2.Content = " Size : " + c.getcmSize().ToString() + " mm";

            Label l3 = new Label();
            l3.Width = 120;
            l3.HorizontalAlignment = HorizontalAlignment.Left;
            l3.VerticalAlignment = VerticalAlignment.Center;
            l3.Content = " Average RGB color : " + Environment.NewLine + " (" + c.getAvgR() + ", " + c.getAvgG() + ", " + c.getAvgB() + ")";

            Label l4 = new Label();
            l4.Width = 350;
            l4.HorizontalAlignment = HorizontalAlignment.Left;
            l4.VerticalAlignment = VerticalAlignment.Center;
            l4.Content = "";

            if (c.isFirst())
            {
                l4.Content = " New colony appearance";
            }
            if (c.hasMerged())
            {
                if (l4.Content.ToString().Length > 0)
                {
                    l4.Content += Environment.NewLine + "Will merge with Colony " + c.getFather().ToString() + Environment.NewLine;

                    if (c.hasMergedWithDifferent()) l4.Content += " of a different class";
                    else l4.Content += " of the same class";
                }
                else
                {
                    l4.Content = " This colony will merge " + Environment.NewLine + "with Colony " + c.getFather().ToString() + Environment.NewLine;

                    if (c.hasMergedWithDifferent()) l4.Content += " of a different class";
                    else l4.Content += " of the same class";
                }
            }
            if (c.getBranches().Count > 0)
            {
                if (l4.Content.ToString().Length > 0)
                {
                    l4.Content += Environment.NewLine + "Has had merged ";

                    if (c.getBranches().Count == 1) l4.Content += "Colony " + c.getBranches()[0][0].getId();
                    else
                    {
                        l4.Content += "Colonies: ";

                        for (int i = 0; i < c.getBranches().Count; i++)
                        {
                            l4.Content += c.getBranches()[0][0].getId() + " ";
                        }
                    }
                }
                else
                {
                    l4.Content += "This colony has had" + Environment.NewLine + " merged ";

                    if (c.getBranches().Count == 1) l4.Content += "colony " + c.getBranches()[0][0].getId();
                    else
                    {
                        l4.Content += "colonies: ";

                        for (int i = 0; i < c.getBranches().Count; i++)
                        {
                            l4.Content += c.getBranches()[i][0].getId() + " ";
                        }
                    }
                }
            }


            Grid.SetColumn(l1, 0);
            Grid.SetColumn(l2, 1);
            Grid.SetColumn(l3, 2);
            Grid.SetColumn(l4, 3);

            g.Children.Add(l4);
            g.Children.Add(l3);
            g.Children.Add(l2);
            g.Children.Add(l1);

            item.Content = g;

            return item;

        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //  Fake carrousel elements building and handlers
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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

        private void ScrollRight_Init()
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

        }

        // Navigation functionality definitions

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
                    Show(0);
                }
                catch (ArgumentOutOfRangeException) { }

                infoLabel.Content = "1/" + Tracking_Images.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();
                try
                {
                    Show(current);
                }
                catch (ArgumentOutOfRangeException) { }

                infoLabel.Content = (current + 1) + "/" + Tracking_Images.Count.ToString();
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
                    Show(total - 1);
                }
                catch (ArgumentOutOfRangeException) { }

                infoLabel.Content = total + "/" + Tracking_Images.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();

                try
                {
                    Show(current - 2);
                }
                catch (ArgumentOutOfRangeException) { }

                infoLabel.Content = (current - 1) + "/" + Tracking_Images.Count.ToString();
            }

        }

        

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Analysis functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // The first taken image is set as the background image

        public void setBackgound(System.Drawing.Image backgroundImg)
        {
            background = new Bitmap(new Bitmap(backgroundImg));

        }

        // In each step, colony tracking is performed by comparing the current taken image with the
        // first one (serving as background) and the last one in the look for changes

        public void newStep(object param)
        {
            while (analysisInProgress)
            {
                Thread.Sleep(1000);
            }

            analysisInProgress = true;

            object[] objects = (object[])param;

            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)objects[0];
            CaptureWindow cw = (CaptureWindow)objects[1];

            Tracking_Images.Add(bmp);
            Tracking_Images[Tracking_Images.Count - 1].Tag = DateTime.Now.ToString("hh:mm:ss");

            // Firstly we will find the best matching repositioning to the images by selecting the one that minimizes the edges difference 
            // This is needed because of the Sprout's image acquisition impresition, which leads to minor mismatches between the same object's captures

            int[] positions = Matching_Robinson.RobinsonRepositioning(background, bmp);

            // Once the reposition is calculated, a difference between the taken image and the background is performed
            // For this difference, Manhattan difference computation has been used. However, Eucliedean distance computation and
            // Pearson's correlation index difference computation functions are included in the code and ready to be used.

            Map resultDifference = DifferenceComputation.getManhattan(background, bmp, positions);

            // A cluster search is computed

            List<Cluster> frameBlobs = FindObjects(resultDifference, DateTime.Now.ToString("hh:mm:ss"));

            // If this is the first tracking step made, the found clusters are set as base ones,
            // otherwise a tracking algorithm trying to match current clusters with already monitored ones is performed

            if (track.isEmpty()) { track.firstScan(frameBlobs, step); events = new int[0]; }
            else
            {
                events = track.assignBlobs(frameBlobs, step);
            }

            events = checkBounds(events, track.getLast(), bmp);

            if (aw.getPicker().classAnalysis && !aw.getClass().hasError()) {
                
               aw.getClass().newStep(track.getLast(), bmp);

                App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                             new Action(() => cw.processClassification()));
            }

            step++;

            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                            new Action(() => cw.processEvents()));

            analysisInProgress = false;

        }

        public void newStep(System.Drawing.Image img)
        {

            Bitmap bmp = new Bitmap(img);

            Tracking_Images.Add(bmp);
            Tracking_Images[Tracking_Images.Count - 1].Tag = DateTime.Now.ToString("hh:mm:ss");

            // Firstly we will find the best matching repositioning to the images by selecting the one that minimizes the edges difference 
            // This is needed because of the Sprout's image acquisition impresition, which leads to minor mismatches between the same object's captures

            int[] positions = Matching_Robinson.RobinsonRepositioning(background, bmp);

            // Once the reposition is calculated, a difference between the taken image and the background is performed
            // For this difference, Manhattan difference computation has been used. However, Eucliedean distance computation and
            // Pearson's correlation index difference computation functions are included in the code and ready to be used.

            Map resultDifference = DifferenceComputation.getManhattan(background, bmp, positions);

            // A cluster search is computed

            List<Cluster> frameBlobs = FindObjects(resultDifference, DateTime.Now.ToString("hh:mm:ss"));

            // If this is the first tracking step made, the found clusters are set as base ones,
            // otherwise a tracking algorithm trying to match current clusters with already monitored ones is performed

            if (track.isEmpty()) { track.firstScan(frameBlobs, step); events = new int[0]; }
            else
            {
                events = track.assignBlobs(frameBlobs, step);
            }

            step++;

            events = checkBounds(events, track.getLast(), bmp);
        }

        // Same analysis process than the previous, only that in this case the images are statically acquired and thus 
        // every step can be performed at once

        public void staticAnalysis(object param)
        {
            // + last parameter (bool existsClassAnalysis)

            List<object> Params = (List<object>)param;

            List<System.Drawing.Image> images = (List < System.Drawing.Image > )Params.ElementAt(0);
            MainPage mp = (MainPage)Params.ElementAt(1);
            string folder = (string)Params.ElementAt(2);
            existsClass = (bool)Params.ElementAt(3);

            setBackgound(images[0]);

            for(int i = 1; i < images.Count; i++)
            {
                newStep(images[i]);
                if (existsClass && !aw.getClass().hasError())
                    aw.getClass().newStep(track.getLast(), images[i]);

            }

            Directory.CreateDirectory(folder);

            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(() => mp.finished_Analysis(aw, folder)));
        }

        // Builds and returns an event if a cluster's bounds are close to the edge

        private int[] checkBounds(int[] events, List<Cluster> list, Bitmap bmp)
        {
            bool outOfBoundsDanger = false;

            for (int i = 0; i < list.Count; i++)
            {
                int[] bb = list.ElementAt(i).getBoundingBox();

                if (bb[0] - AdvancedOptions._nMinimumDistance < AdvancedOptions._nROIMargin) outOfBoundsDanger = true;
                if (bb[1] - AdvancedOptions._nMinimumDistance < AdvancedOptions._nROIMargin) outOfBoundsDanger = true;
                if (bb[2] + AdvancedOptions._nMinimumDistance > bmp.Width - AdvancedOptions._nROIMargin) outOfBoundsDanger = true;
                if (bb[3] + AdvancedOptions._nMinimumDistance > bmp.Height - AdvancedOptions._nROIMargin) outOfBoundsDanger = true;

            }

            int[] newEvents;

            if (outOfBoundsDanger)
            {
                newEvents = new int[events.Length + 1];
                newEvents[events.Length] = 4;

            }
            else
            {
                newEvents = new int[events.Length];
            }

            for (int i = 0; i < events.Length; i++)
            {
                newEvents[i] = events[i];
            }

            return newEvents;
        }

        // Finding every cluster in a difference

        private List<Cluster> FindObjects(Map resultDifference, string time)
        {
            ConnectedComponents cc = new ConnectedComponents(resultDifference, " ", new ROI(AdvancedOptions._nROIMargin, resultDifference.getGreyMap()), time);

            // Drawing the image will result in acquiring visual feedback of the blobs detected via their bounding boxes

            // Draw(cc.marked);

            return cc.getIdentifiedBlobs();
        }

        // Returnables directed to other classes

        public double[] getColonyCountData()
        {
            List<double> returnable = new List<double>();

            List<List<Cluster>> blobs = track.getTimelineTracking();

            for (int i = 0; i < blobs.Count; i++)
            {
                returnable.Add(0);

                for (int j = 0; j < blobs[i].Count; j++)
                {
                    returnable[i]++;
                }
            }

            return returnable.ToArray();
        }

        public bool hasBlobs(int n)
        {
            List<List<Cluster>> clusters = track.getTracking();

            return clusters[n].Count > 0;
        }

        public bool hasBlobs()
        {
            List<List<Cluster>> clusters = track.getTracking();

            for(int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Count > 0) return true;
            }

            return false;
        }

        public object[] getColonySizeData()
        {
            List<int[]> bbs = new List<int[]>();
            List<double[]> sizes = new List<double[]>();
            List<Bitmap> images = new List<Bitmap>();

            List<List<Cluster>> clusters = track.getTracking();

            for (int i = 0; i<clusters.Count; i++)
            {
                sizes.Add(new double[clusters[i].Count]);

                for(int j = 0; j<clusters[i].Count; j++)
                {
                    sizes[i][j] = clusters[i][j].getcmSize();
                }

               bbs.Add(clusters[i][clusters[i].Count - 1].getBoundingBox());
               images.Add(Tracking_Images[clusters[i][clusters[i].Count - 1].getStep()]); 
            }

            object[] returnable = new object[3];
            returnable[0] = sizes;
            returnable[1] = bbs;
            returnable[2] = images;

            return returnable;
        }

        public int[] getEvents()
        {
            return events;
        }

        public int getStep()
        {
            return step;
        }

        public void checkLast()
        {
            if (aw.getCaptureWindow() != null) aw.getCaptureWindow().checkLast();
        }
    }
}