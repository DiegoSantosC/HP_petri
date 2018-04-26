using Kohonen;
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
using System.Windows.Shapes;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for ClassifyAnalytics.xaml
    /// </summary>
    public partial class ClassifyAnalytics : Page
    {
        private KohonenNetwork kn;
        private LabelingHandler lh;
        private List<Bitmap> imagesMap;
        private List<List<int[]>> matches;
        private List<List<System.Windows.Shapes.Rectangle>> locations;
        private List<List<Cluster>> clusters;

        public ClassifyAnalytics()
        {
            InitializeComponent();
            imagesMap = new List<Bitmap>();
            matches = new List<List<int[]>>();
            locations = new List<List<System.Windows.Shapes.Rectangle>>();
            clusters = new List<List<Cluster>>();
        }

        internal void Init(string sourceFolder)
        {
            object[] returned = DataHandler.ProcessInputTest(sourceFolder);
            if(returned == null)
            {
                return;
            }

            List<string> labels = (List<string>)returned[0];
            Cell[,] map = (Cell[,])returned[1];

            lh = new LabelingHandler(labels);
            kn = new KohonenNetwork(lh, map, this);

            Logo_Init();

            ScrollLeft_Init();
            ScrollRight_Init();
            
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

        // An image is shown in the UI

        public void Show(int index)
        {
            sampleSP.Children.Clear();

            // A System.Windows.Controls.Image is created from a System.Drawing.Image by acquiring its Bitmap

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            IntPtr ip = imagesMap[index].GetHbitmap();

            BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            img.Source = src;
            sampleSP.Children.Add(img);

            infoLabel.Content = (index + 1).ToString() + "/" + imagesMap.Count.ToString();

            RebuildClusterLocations(index);            
        }

        private void RebuildClusterLocations(int index)
        {
            rectanglesCanvas.Children.Clear();

            for(int i = 0; i < locations[index].Count; i++)
            {
                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();

                r.Stroke = System.Windows.Media.Brushes.LightGreen;
                r.StrokeThickness = 2;

                r.Height = locations[index][i].Height;
                r.Width = locations[index][i].Width;

                r.Margin = locations[index][i].Margin;

                Canvas c = new Canvas();
                Canvas c2 = new Canvas();
                c.Children.Add(r);
                c.Children.Add(c2);

                c2.Height = r.Height;
                c2.Width = r.Width;
                c2.Margin = r.Margin;

                c2.Background = System.Windows.Media.Brushes.White;
                c2.Opacity = 0;
                c2.MouseEnter += new MouseEventHandler(mouseEnter);
                c2.MouseLeave += new MouseEventHandler(mouseLeave);
                c2.MouseLeftButtonDown += new MouseButtonEventHandler(clicked);
                c2.Uid = i.ToString();

                rectanglesCanvas.Children.Add(c);
            }
        }

        private void clicked(object sender, MouseButtonEventArgs e)
        {
            int index;

            String s = infoLabel.Content.ToString();
            String[] data = s.Split('/');

            int current = Int32.Parse(data[0]);
            
            Canvas c = (Canvas)sender;
            index = Int32.Parse(c.Uid);

            Thickness t = c.Margin;

            if (t.Left > 150) modifyRight(t, matches[current - 1][index]);
            else { modifyLeft(t, matches[current - 1][index]); }

        }

        private void modifyLeft(Thickness t, int[] best)
        {
            StackPanel1.Children.Clear();

            Cell winner = kn.getCellAtPosition(best[1], best[0]);

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            Bitmap bmp = winner.getAsBmp();
            BitmapImage src = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                src.BeginInit();
                src.StreamSource = memory;
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();

            }

            img.Source = src;
            img.Stretch = Stretch.Uniform;

            StackPanel1.Children.Add(img);

            int top = ((int)(t.Top * 500 / 350) - 50);

            if (top < 0) top = 0;

            StackPanel1.Margin = new Thickness( 0, top, 0, 0);

            List<string> labels = lh.getAllLabels();
            List<int> indexes = lh.getAllIndexes();

            for(int i = 0; i < labels.Count; i++)
            {
                Console.Write(labels[i] + " ");

            }
            Console.WriteLine();

            for (int i = 0; i < indexes.Count; i++)
            {
                Console.Write(indexes[i] + "     ");

            }

            Console.WriteLine(winner.getIndex());

            Console.WriteLine(winner.getIndex() + " " + lh.getLabel(winner.getIndex()));

            string label = "";
            //string label = lh.getLabel(winner.getIndex());
            if (label.Trim().Length < 1) label = "       undefined";

            Label1.Content = "Winner map position:     [" + best[1] + " " + best[0]  + " " + Environment.NewLine + 
                "Winner label: " + label;
            Label1.Margin = new Thickness(0, top + 280, 0, 0);
        }

        private void modifyRight(Thickness t, int[] best)
        {
            StackPanel2.Children.Clear();

            Cell winner = kn.getCellAtPosition(best[1], best[0]);

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            Bitmap bmp = winner.getAsBmp();
            BitmapImage src = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                src.BeginInit();
                src.StreamSource = memory;
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
            }

            img.Source = src;
            img.Stretch = Stretch.Uniform;

            StackPanel2.Children.Add(img);

            int top = ((int)(t.Top * 500 / 350) - 50);

            if (top < 0) top = 0;

            StackPanel2.Margin = new Thickness(0, top, 0, 0);

            List<string> labels = lh.getAllLabels();
            List<int> indexes = lh.getAllIndexes();

            for (int i = 0; i < labels.Count; i++)
            {
                Console.Write(labels[i] + " ");

            }
            Console.WriteLine();

            for (int i = 0; i < indexes.Count; i++)
            {
                Console.Write(indexes[i] + "     ");

            }

            Console.WriteLine(winner.getIndex());

            Console.WriteLine(winner.getIndex() + " " + lh.getLabel(winner.getIndex()));

            string label = lh.getLabel(winner.getIndex());
            if (label.Trim().Length < 1) label = "undefined";

            Label2.Content = "Winner map position: " + best[1] + best[0] + Environment.NewLine +
                "Winner label: " + label;

            Label2.Margin = new Thickness(0, top + 280, 0, 0);

        }

        private void mouseLeave(object sender, MouseEventArgs e)
        {
            Canvas c = (Canvas)sender;
            c.Background = System.Windows.Media.Brushes.White;

            c.Opacity = 0.0;
        }

        private void mouseEnter(object sender, MouseEventArgs e)
        {
            Canvas c = (Canvas)sender;
            c.Background = System.Windows.Media.Brushes.LightGreen;

            c.Opacity = 0.7;
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
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = "1/" + imagesMap.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();
                try
                {
                    Show(current);
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = (current + 1) + "/" + imagesMap.Count.ToString();
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
                catch (Exception) { }

                infoLabel.Content = total + "/" + imagesMap.Count.ToString();
            }
            else
            {
                sampleSP.Children.Clear();

                try
                {
                    Show(current - 2);
                }
                catch (ArgumentOutOfRangeException ex) { }

                infoLabel.Content = (current - 1) + "/" + imagesMap.Count.ToString();
            }

        }

        // Artificial Scroll will always be set to the first process that exists

        public void ArtificialScroll()
        {
            for (int i = 0; i < clusters.Count; i++)
            {
                locations.Add(getRectanglesFromBbxs(clusters[i], imagesMap[i]));
            }

            Show(0);
            infoLabel.Content = "1/" + imagesMap.Count.ToString();
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Building of the data 
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        internal void newStep(List<Cluster> list, System.Drawing.Image image)
        {
            List<Bitmap> stepInputs = extractBitmapsFromClusters(list, image);

            List<int[]> bestMatches = kn.Test_Network(stepInputs);

            clusters.Add(list);

            matches.Add(bestMatches);
            imagesMap.Add(new Bitmap(image));
        }

        private List<Bitmap> extractBitmapsFromClusters(List<Cluster> list, System.Drawing.Image image)
        {
            List<Bitmap> bmps = new List<Bitmap>();

            for(int i = 0; i < list.Count; i++)
            {
                int[] bbx = list[i].getBoundingBox();

                Bitmap sourceBmp = new Bitmap(image, image.Width, image.Height);

                bmps.Add(getBitmapFromBbx(bbx, sourceBmp));
            }

            return bmps;
        }

        private Bitmap getBitmapFromBbx(int[] bbx, Bitmap sourceBmp)
        {
            int size = Kohonen.AdvancedOptions._nBitmapSize;
            Bitmap bmp = new Bitmap(bbx[2] - bbx[0], bbx[3] - bbx[1]);

            for(int j= bbx[1]; j < bbx[3]; j++)
            {
                for(int i = bbx[0]; i < bbx[2]; i++)
                {
                    bmp.SetPixel(i - bbx[0], j - bbx[1], sourceBmp.GetPixel(i,j));
                }
            }

            Bitmap resizedBmp = new Bitmap(bmp, size, size);

            return resizedBmp;
        }

        private List<System.Windows.Shapes.Rectangle> getRectanglesFromBbxs(List<Cluster> list, System.Drawing.Image img)
        {
            List<System.Windows.Shapes.Rectangle> locs = new List<System.Windows.Shapes.Rectangle>();

            for (int i = 0; i < list.Count; i++)
            {
                int[] bbx = list[i].getBoundingBox();

                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();

                // Rectangles' size and position is scaled 

                r.Width = ((bbx[2] -bbx[0]) * 360 / img.Width);
                r.Height = ((bbx[3] - bbx[1]) * 360 / img.Height);

                r.Margin = new Thickness((bbx[0] * 350 / img.Width), (bbx[1] * 350 / img.Height), 0, 0);

                locs.Add(r);
            }

            return locs;
        }
    }
}
