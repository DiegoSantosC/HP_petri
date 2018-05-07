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
using System.Windows.Forms;
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
        private CountAnalytics countA;
        private static bool errorDuringImport;

        public ClassifyAnalytics()
        {
            InitializeComponent();
            imagesMap = new List<Bitmap>();
            matches = new List<List<int[]>>();
            locations = new List<List<System.Windows.Shapes.Rectangle>>();
            clusters = new List<List<Cluster>>();
            errorDuringImport = false;
        }

        internal void Init(string sourceFolder, CountAnalytics ca)
        {
            object[] returned = DataHandler.ProcessInputTest(sourceFolder);
            if(returned == null)
            {
                System.Windows.MessageBox.Show(" Invalid map folder");
                errorDuringImport = true;
                return;
            }

            List<string> labels = (List<string>)returned[0];
            Cell[,] map = (Cell[,])returned[1];

            lh = new LabelingHandler(labels);
            kn = new KohonenNetwork(lh, map, this);

            Logo_Init();

            ScrollLeft_Init();
            ScrollRight_Init();

            countA = ca;          
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

            StackPanel1.Visibility = Visibility.Hidden;
            Label1.Visibility = Visibility.Hidden;

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
                c2.MouseEnter += new System.Windows.Input.MouseEventHandler(mouseEnter);
                c2.MouseLeave += new System.Windows.Input.MouseEventHandler(mouseLeave);
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

            modifyLeft(t, matches[current - 1][index], index);

        }

        private void modifyLeft(Thickness t, int[] best, int index)
        {
            StackPanel1.Children.Clear();

            StackPanel1.Visibility = Visibility.Visible;
            Label1.Visibility = Visibility.Visible;

            Cell winner = kn.getCellAtPosition(best[0], best[1]);

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

            if (top < 30) top = 30;

            if (top > 200) top = 200;

            StackPanel1.Margin = new Thickness( 0, top, 0, 0);

            List<string> labels = lh.getAllLabels();
            List<int> indexes = lh.getAllIndexes();

            string label = "";
            if (winner.getIndex() < 0)
            {
                label = "       undefined";
                undefinedCanvas.Visibility = Visibility.Visible;

                string s = best[0] + " " + best[1];

                String s2 = infoLabel.Content.ToString();
                String[] data = s2.Split('/');

                int current = Int32.Parse(data[0]) - 1;

                int[] bbx = clusters[current][index].getBoundingBox();

                labelTextBox.Text = "";
                StackPanel2.Children.Clear();
                saveButton.Uid = s;

                BuildRightSP(bbx, imagesMap[current]);
            }
            else {

                label = lh.getLabel(winner.getIndex());
                undefinedCanvas.Visibility = Visibility.Hidden;
                
            }

            Label1.Content = "Winner map position:     [" + best[0] + " " + best[1]  + "] " + Environment.NewLine + 
                "Winner label: " + label;
            Label1.Margin = new Thickness(0, top + 280, 0, 0);

        }

        private void BuildRightSP(int[] bbx, Bitmap bmp)
        {
            Bitmap clusterBmp = new Bitmap(bbx[2] - bbx[0], bbx[3] - bbx[1]);

            for (int j = bbx[1]; j < bbx[3]; j++)
                for (int i = bbx[0]; i < bbx[2]; i++) clusterBmp.SetPixel(i - bbx[0], j - bbx[1], bmp.GetPixel(i, j));

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            IntPtr ip = clusterBmp.GetHbitmap();

            BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            img.Source = src;
            StackPanel2.Children.Add(img);
        }

        private void mouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Canvas c = (Canvas)sender;
            c.Background = System.Windows.Media.Brushes.White;

            c.Opacity = 0.0;
        }

        private void mouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Canvas c = (Canvas)sender;
            c.Background = System.Windows.Media.Brushes.LightGreen;

            c.Opacity = 0.7;
        }

        public bool hasError()
        {
            return errorDuringImport;
        }

        public List<int[]> getLastMatches()
        {
            return matches[matches.Count - 1];
        }

        public List<string> getLastLabels()
        {
            List<string> labels = new List<string>();

            List<int[]> matches = getLastMatches();

            for (int i = 0; i < matches.Count; i++)
            {
                Cell winner = kn.getCellAtPosition(matches[i][0], matches[i][1]);

                string label = "";
                if (winner.getIndex() < 0) { label = "Undefined"; }
                else{ label = lh.getLabel(winner.getIndex());}

                labels.Add(label);
            }

            return labels;
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

            rightSp.MouseEnter += new System.Windows.Input.MouseEventHandler(navigationArrowEnter);
            rightSp.MouseLeave += new System.Windows.Input.MouseEventHandler(navigationArrowLeave);

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

            leftSp.MouseEnter += new System.Windows.Input.MouseEventHandler(navigationArrowEnter);
            leftSp.MouseLeave += new System.Windows.Input.MouseEventHandler(navigationArrowLeave);

            leftSp.MouseDown += new MouseButtonEventHandler(scrollLeft);

        }

        // Navigation functionality definitions

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

            countA.checkLast();
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

                if(img.Width > img.Height) { sampleSP.Width = 350; sampleSP.Height = 350 * sampleSP.Height / sampleSP.Width; }

                r.Width = ((bbx[2] -bbx[0]) * sampleSP.Width * 1.1 / img.Width);
                r.Height = ((bbx[3] - bbx[1]) * sampleSP.Height * 1.1 / img.Height);

                r.Margin = new Thickness((bbx[0] * sampleSP.Width / img.Width), (bbx[1] * sampleSP.Height / img.Height), 0, 0);

                r.Uid = i.ToString();

                locs.Add(r);
            }

            return locs;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string s = labelTextBox.Text;

            if (s.Length < 1) { System.Windows.MessageBox.Show("Please introduce a valid label"); return; }

            FolderBrowserDialog sfd = new FolderBrowserDialog();

            DialogResult res = sfd.ShowDialog();

            if (res == DialogResult.OK && !string.IsNullOrWhiteSpace(sfd.SelectedPath))
            {
                string loc = sfd.SelectedPath;
                string imageName = System.IO.Path.Combine(loc, "savedColony.bmp");
                string fileName = System.IO.Path.Combine(loc, "label.txt");

                foreach (object child in StackPanel2.Children)
                {
                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)child;

                    // !!!!!!!!!!!!!!!!!!!!!!!! FAILING

                    Bitmap bmpOut = null;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img.Source));

                        using (Bitmap bmp = new Bitmap(ms)) bmpOut = new Bitmap(bmp);
                    }

                    bmpOut.Save(imageName);
                }

                string[] content = new string[] {"Colony saved for future trainings", "Map position found to be the closest " + this.Uid, "Label set: " + s};
            }
        }
    }
}
