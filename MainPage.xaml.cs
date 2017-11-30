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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    /// 


    public partial class MainPage : Page
    {

        public static List<int[]> parameters;
        private static int counter;
       
        public MainPage()
        {
            InitializeComponent();


            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            objectShowStackPanel.Visibility = Visibility.Collapsed;

            parameters = new List<int[]>();

            counter = 1;
        }
        
        private void objectShow_Button_Click(object sender, RoutedEventArgs e)
        {

            parameters = new List<int[]>();
            CaptureDetailsLabel.Content = "";

           MainCapture newCapture = new MainCapture();
            OutlineParameters op = newCapture.ConfirmCapture();

            int numberObjects = OutlineParameters.getCapacity(op);

            if ((op.getSize(op).ElementAt(0).X > 4000) && (op.getSize(op).ElementAt(0).Y > 3000))
            {
                MessageBox.Show("No objects could be found");
                return;
            }

            List<System.Windows.Shapes.Rectangle> outlineDefinitionList = new List<System.Windows.Shapes.Rectangle>();
            List<SelectionBorder> transparentCanvasList = new List<SelectionBorder>();

            System.Drawing.Point globalPicSize = op.getGlobalSize(op);

            ImageBorder.Visibility = Visibility.Visible;

            ImageCanvas.Children.Clear();
            ImageCanvas.Children.Add(objectShowStackPanel);

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
            
            System.Windows.Controls.Image confirmImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(PictureHandling.confirmPath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            confirmImg.Source = src;
            confirmImg.Stretch = Stretch.Uniform;

            objectShowStackPanel.Children.Clear();
            objectShowStackPanel.Children.Add(confirmImg);

            ShowButton.Content = "Scan new Layout";
            ParametersBorder.Visibility = Visibility.Hidden;
        }

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
            SelectionBorder sb = (SelectionBorder) sender;
            MonitoringParametersShow(sb.getIndex());

        }

        private void MonitoringParametersShow(int objectToParameterize)
        {
            
            ParametersBorder.Visibility = Visibility.Visible;

            ParametersTitleLabel.Content = "Object " + objectToParameterize + " capture parameters";

        }

        private void CaptureConfirm_Button_Click(object sender, RoutedEventArgs e)
        {

            for (int i=0; i<parameters.Count; i++)
            {
                CaptureWindow cw = new CaptureWindow(parameters.ElementAt(i));
                cw.Show();

            }

            parameters = new List<int[]>();
            CaptureDetailsLabel.Content = "";

        }

        private void ParameterConfirm_Button_Click(object sender, RoutedEventArgs e)
        {
            
           int minutesInterval, hoursInterval, numberOfCaptures;

           if ((Int32.TryParse(minutesTextBox.Text, out minutesInterval)) && (Int32.TryParse(hoursTextBox.Text, out hoursInterval)) && (Int32.TryParse(numberOfCapturesTextBox.Text, out numberOfCaptures)))
           {
               if (((minutesInterval == 0) && (hoursInterval == 0)) || (numberOfCaptures == 0))
               {
                   MessageBox.Show("Insert valid parameter values");
               }
               else
               {
               
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

                    String str = hours + " hours " + minutes + " minutes";

                    int objIndex = Int32.Parse(ParametersTitleLabel.Content.ToString().Split(' ')[1]);

                    CaptureDetailsLabel.Content = CaptureDetailsLabel.Content +
                         Environment.NewLine + "Capture " + counter + ": " + str + " , " + numberOfCaptures + " captures will be made." ;

                    counter++;

                    ParametersBorder.Visibility = Visibility.Hidden;
                    CaptureConfirmButton.Visibility = Visibility.Visible;
                    minutesTextBox.Text = "0";
                    hoursTextBox.Text = "0";
                    numberOfCapturesTextBox.Text = "0";

                    int[] param = new int[3];
                    param[0] = numberOfCaptures;
                    param[1] = minutesInterval;
                    param[2] = objIndex;
                    parameters.Add(param);

                }
           }
           else
           {
               MessageBox.Show("Parameter parsing error");
           }
        }
    }
}
