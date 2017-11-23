using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Threading;
using System.Drawing;
using System.Windows.Navigation;




namespace PetriUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainPage : Page
    {

        public static Thread captureThread;
        public static int numberOfCaptures, minutesInterval, hoursInterval, interval;

        public MainPage()
        {
            InitializeComponent();


            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            objectShowStackPanel.Visibility = Visibility.Collapsed;
        }
        
        private void objectShow_Button_Click(object sender, RoutedEventArgs e)
        {
            MainCapture newCapture = new MainCapture();
            OutlineParameters op = newCapture.ConfirmCapture();

            int numberObjects = OutlineParameters.getCapacity(op);

            if ((op.getSize(op).ElementAt(0).X > 4000) && (op.getSize(op).ElementAt(0).Y > 3000))
            {
                MessageBox.Show("No objects could be found");
                return;
            }

            List<System.Windows.Shapes.Rectangle> outlineDefinitionList = new List<System.Windows.Shapes.Rectangle>();
            List<Border> transparentCanvasList = new List<Border>();

            System.Drawing.Point globalPicSize = op.getGlobalSize(op);

            ImageBorder.Visibility = Visibility.Visible;

            for (int i = 0; i < numberObjects; i++)
            {
                // Rectangle definition

                outlineDefinitionList.Add(new System.Windows.Shapes.Rectangle());
                outlineDefinitionList.ElementAt(i).Width = (op.getSize(op).ElementAt(i).X *790)/globalPicSize.X; // Window/Canvas ratio
                outlineDefinitionList.ElementAt(i).Height = (op.getSize(op).ElementAt(i).Y *590) / globalPicSize.Y;

                outlineDefinitionList.ElementAt(i).MouseDown += new MouseButtonEventHandler(RectangleClickedHandler);
                ImageCanvas.Children.Add(outlineDefinitionList.ElementAt(i));
                outlineDefinitionList.ElementAt(i).Stroke = System.Windows.Media.Brushes.LightGreen;
                outlineDefinitionList.ElementAt(i).StrokeThickness = 3;
                outlineDefinitionList.ElementAt(i).Margin = 
                    new Thickness((op.getLocation(op).ElementAt(i).X * 790) / globalPicSize.X, (op.getLocation(op).ElementAt(i).Y * 590) / globalPicSize.Y, 0, 0);

                // Canvas definition

                transparentCanvasList.Add(new Border());
                transparentCanvasList.ElementAt(i).Width = (op.getSize(op).ElementAt(i).X * 790) / globalPicSize.X; // Window/Canvas ratio
                transparentCanvasList.ElementAt(i).Height = (op.getSize(op).ElementAt(i).Y * 590) / globalPicSize.Y;

                transparentCanvasList.ElementAt(i).MouseDown += new MouseButtonEventHandler(RectangleClickedHandler);
                ImageCanvas.Children.Add(transparentCanvasList.ElementAt(i));
                transparentCanvasList.ElementAt(i).Opacity = 0;
                transparentCanvasList.ElementAt(i).Background = System.Windows.Media.Brushes.Green;
                transparentCanvasList.ElementAt(i).MouseEnter += new MouseEventHandler((null, null) => BorderMouseEnterHandler(this, null, transparentCanvasList.ElementAt(i));
                
                transparentCanvasList.ElementAt(i).Margin =
                    new Thickness((op.getLocation(op).ElementAt(i).X * 790) / globalPicSize.X, (op.getLocation(op).ElementAt(i).Y * 590) / globalPicSize.Y, 0, 0);
            }

            objectShowStackPanel.Visibility = Visibility.Visible;
            
            System.Windows.Controls.Image confirmImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(PictureHandling.confirmPath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            confirmImg.Source = src;
            confirmImg.Stretch = Stretch.Uniform;

            objectShowStackPanel.Children.Add(confirmImg);
        }

        private void BorderMouseEnterHandler(object sender, MouseEventArgs e, Border b)
        {
            b.Opacity = 0.2;
        }

        void RectangleClickedHandler(object sender, EventArgs e)
        {
            MonitoringParametersShow(0);

        }

        private void MonitoringParametersShow(int i)
        {
            
            ParametersBorder.Visibility = Visibility.Visible;
       
        }

        private void CaptureConfirm_Button_Click(object sender, RoutedEventArgs e)
        {
            /*

           if ((Int32.TryParse(minutesTextBox.Text, out minutesInterval)) && (Int32.TryParse(hoursTextBox.Text, out hoursInterval)) && (Int32.TryParse(numberOfCapturesTextBox.Text, out numberOfCaptures)))
           {
               if (((minutesInterval == 0) && (hoursInterval == 0)) || (numberOfCaptures == 0))
               {
                   MessageBox.Show("Insert valid parameter values");
               }
               else
               {
                   MainCapture newCapture = new MainCapture();

                   captureThread = new Thread(newCapture.StartCapture);
                   captureThread.Start();

                   interval = hoursInterval * 60 + minutesInterval;
                   CapturePage cp = new CapturePage(numberOfCaptures, interval);
                   this.NavigationService.Navigate(cp);
               }
           }
           else
           {
               MessageBox.Show("Parameter parsing error");
           }
           */

        }

    }

    }
