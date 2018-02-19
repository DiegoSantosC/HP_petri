using AnalysisTestApp;
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
        private Tracking track;
        private int step;

        // Test inputs
        private Bitmap[] testBmps;

        public CountAnalytics()
        {
            InitializeComponent();

            track = new Tracking();

            step = 0;

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
            frame.Width = 575;
            frame.Height = 400;

            frame.Stroke = System.Windows.Media.Brushes.Blue;
            frame.StrokeThickness = 3;

            aux.Children.Add(frame);
            StackPanel.SetZIndex(aux, 10);

            ImagesCanvas.Children.Add(aux);
        }


        public void Show(int index)
        {
            sampleSP.Children.Clear();

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            IntPtr ip = Tracking_Images[index].GetHbitmap();

            BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            img.Source = src;
            sampleSP.Children.Add(img);

            infoLabel.Content = index.ToString() + "/" + Tracking_Images.Count.ToString();
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

 // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Analysis functions
 // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void setBackgound(System.Drawing.Image backgroundImg)
        {
            Console.WriteLine("Background set");

            //background = new Bitmap(backgroundImg);

            background = testBmps[0];
        }

        public int[] newStep(System.Drawing.Image img)
        {
            //Bitmap bmp = new Bitmap(img);

            // For testing purposes, taken images will be replaced for static ones

            Bitmap bmp = testBmps[step + 1];

            Tracking_Images.Add(bmp);

            int[] positions = Matching_Robinson.RobinsonRepositioning(background, bmp);

            Map resultDifference = DifferenceComputation.getManhattan(background, bmp, positions);

            List<Cluster> frameBlobs = FindObjects(resultDifference);

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

        private List<Cluster> FindObjects(Map resultDifference)
        {
            ConnectedComponents cc = new ConnectedComponents(resultDifference, " ", new ROI(AdvancedOptions._nROIMargin, resultDifference.getGreyMap()), "");

            // Drawing the image will result in acquiring visual feedback of the blobs detected via their bounding boxes

            // Draw(cc.marked);

            return cc.getIdentifiedBlobs();
        }

    }
}