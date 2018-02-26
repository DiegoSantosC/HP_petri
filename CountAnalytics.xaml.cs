﻿using AnalysisTestApp;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for CountAnalytics.xaml
    /// </summary>
    public partial class CountAnalytics : Page
    {
        private Bitmap background;
        private List<Bitmap> Tracking_Images;
        private List<List<ListBoxItem>> cluster_Specs;
        private List<List<System.Windows.Shapes.Rectangle>> cluster_locations;
        private Tracking track;
        private int step;
        private bool changing;

        // Test inputs
        private Bitmap[] testBmps;

        public CountAnalytics()
        {
            InitializeComponent();

            track = new Tracking();

            step = 0;

            changing = false;

            cluster_Specs = new List<List<ListBoxItem>>();
            cluster_locations = new List<List<System.Windows.Shapes.Rectangle>>();

            // Test inputs initialization

            testBmps = new Bitmap[11];

            Tracking_Images = new List<Bitmap>();

            testBmps[0] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C1.png"));
            testBmps[1] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C2.png"));
            testBmps[2] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\MergingTest.png"));
            testBmps[3] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C4.png"));
            testBmps[4] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C5.png"));
            testBmps[5] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C6.png"));
            testBmps[6] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C7.png"));
            testBmps[7] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C8.png"));
            testBmps[8] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C9.png"));
            testBmps[9] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C10.png"));
            testBmps[10] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C11.png"));

            ScrollLeft_Init();

            ScrollRight_Init();

            ListBox_Init();
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // UI functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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

        public void Show(int index)
        {
            changing = true;

            sampleSP.Children.Clear();

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            IntPtr ip = Tracking_Images[index].GetHbitmap();

            BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            img.Source = src;
            sampleSP.Children.Add(img);

            infoLabel.Content = 1 + "/" + Tracking_Images.Count.ToString();

            rectanglesCanvas.Children.Clear();

            ModifyListBox(index);

            changing = false;
        }

        private void ModifyListBox(int index)
        {
            ClusterListBox.Items.Clear();

            for (int i=0; i<cluster_Specs[index].Count(); i++)
            {
                ClusterListBox.Items.Add(cluster_Specs[index][i]);
            }
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
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = "1/" + Tracking_Images.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();
                try
                {
                    Show(current);
                }
                catch (ArgumentOutOfRangeException ex) { }

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
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = total + "/" + Tracking_Images.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();

                try
                {
                    Show(current - 2);
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = (current - 1) + "/" + Tracking_Images.Count.ToString();
            }

        }
   
        public void initStatics()
        {
            for(int i = 0; i < track.getTimelineTracking().Count; i++)
            {
                List<ListBoxItem> blobs = new List<ListBoxItem>();
                List<System.Windows.Shapes.Rectangle> locations = new List<System.Windows.Shapes.Rectangle>();

                for(int j = 0; j < track.getTimelineTracking()[i].Count; j++)
                {
                    Cluster c = track.getTimelineTracking()[i][j];

                    ListBoxItem lbItem = createItemFromCluster(c);
                    System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();

                    r.Stroke = System.Windows.Media.Brushes.LightGreen;
                    r.StrokeThickness = 2;
                    
                    // Rectangles' size and position is scaled 

                    r.Width = ((c.getBoundingBox()[2] - c.getBoundingBox()[0]) * 290 / Tracking_Images[0].Width) + 5;
                    r.Height = ((c.getBoundingBox()[3] - c.getBoundingBox()[1]) * 290/ Tracking_Images[0].Height) + 5;

                    r.Margin = new Thickness((c.getBoundingBox()[0] * 290 / Tracking_Images[0].Width)-2, (c.getBoundingBox()[1] * 290/ Tracking_Images[0].Height)-2, 0, 0);

                    blobs.Add(lbItem);
                    locations.Add(r);
                }

                cluster_Specs.Add(blobs);
                cluster_locations.Add(locations);
            }
        }

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

        private ListBoxItem createItemFromCluster(Cluster c)
        {
            ListBoxItem item = new ListBoxItem();
            Grid g = new Grid();
           
            item.Height = 80;
            item.Width = 500;

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

            Label l1 = new Label();
            l1.Width = 40;
            l1.HorizontalAlignment = HorizontalAlignment.Left;
            l1.VerticalAlignment = VerticalAlignment.Center;
            l1.Content = " Id : " + c.getId().ToString();
      
            Label l2 = new Label();
            l2.Width = 120;
            l2.HorizontalAlignment = HorizontalAlignment.Left;
            l2.VerticalAlignment = VerticalAlignment.Center;
            l2.Content = " Size : " + c.getSize().ToString() + " pixels";

            Label l3 = new Label();
            l3.Width = 120;
            l3.HorizontalAlignment = HorizontalAlignment.Left;
            l3.VerticalAlignment = VerticalAlignment.Center;
            l3.Content = " Taken at : " + c.getTime();

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
                if(l4.Content.ToString().Length > 0)
                {
                    l4.Content += Environment.NewLine + "Will merge with Colony " + c.getFather().ToString();

                    if (c.hasMergedWithDifferent()) l4.Content += " of a different class";
                    else l4.Content += " of the same class";
                }
                else
                {
                    l4.Content = " This colony will merge " + Environment.NewLine +  "with Colony " + c.getFather().ToString();

                    if (c.hasMergedWithDifferent()) l4.Content += " of a different class";
                    else l4.Content += " of the same class";
                }
            }
            if (c.getBranches().Count > 0)
            {
                if (l4.Content.ToString().Length > 0)
                {
                    l4.Content += Environment.NewLine + "Will have merged ";

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
                    l4.Content += "This colony will have" + Environment.NewLine + " merged ";

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
        // Analysis functions
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void setBackgound(System.Drawing.Image backgroundImg)
        {
            Console.WriteLine("Background set");

            //background = new Bitmap(backgroundImg);

            background = testBmps[0];
        }

        public int[] newStep(System.Drawing.Image img, string time)
        {
            //Bitmap bmp = new Bitmap(img);

            // For testing purposes, taken images will be replaced for static ones

            Bitmap bmp = testBmps[step + 1];

            Tracking_Images.Add(bmp);

            int[] positions = Matching_Robinson.RobinsonRepositioning(background, bmp);

            Map resultDifference = DifferenceComputation.getManhattan(background, bmp, positions);

            List<Cluster> frameBlobs = FindObjects(resultDifference, time);

            Console.WriteLine();
            Console.WriteLine("Tracking " + step + ": " + frameBlobs.Count + " blobs encountered");

            int[] events;

            if (track.isEmpty()) { track.firstScan(frameBlobs, step); events = new int[0]; }
            else
            {
                events = track.assignBlobs(frameBlobs, step);
            }

            step++;

            events = checkBounds(events, track.getLast(), bmp);

            return events;
        }

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

            for(int i=0; i<events.Length; i++)
            {
                newEvents[i] = events[i];
            }

            return newEvents;
        }

        private List<Cluster> FindObjects(Map resultDifference, string time)
        {
            ConnectedComponents cc = new ConnectedComponents(resultDifference, " ", new ROI(AdvancedOptions._nROIMargin, resultDifference.getGreyMap()), time);

            // Drawing the image will result in acquiring visual feedback of the blobs detected via their bounding boxes

            // Draw(cc.marked);

            return cc.getIdentifiedBlobs();
        }
    }
}