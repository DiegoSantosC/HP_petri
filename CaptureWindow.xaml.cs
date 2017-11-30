using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private static Thread newCaptureThread;
        private Task t;
        public CaptureWindow(int[] parameters)
        {
            InitializeComponent();

            this.Width = 1200;
            this.Height = 900;

            List<Uri> u = new List<Uri>();
            t = new Task(this, parameters[0], parameters[1], parameters[2], u);

            MainCapture newCapture = new MainCapture();
            newCaptureThread = new Thread(newCapture.StartCapture);
            newCaptureThread.SetApartmentState(ApartmentState.STA);
            newCaptureThread.Start(t);
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        public void DrawImage()
        {
            Image img = new Image();
        
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = t.getCaptures().ElementAt(t.getCaptures().Count -1);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            img.Source = src;
            img.Stretch = Stretch.Uniform;
            img.Stretch = Stretch.Uniform;

            StackPanel imgSP = new StackPanel();
            imgSP.Children.Add(img);
            ImagesCanvas.Children.Add(imgSP);
        }
    }
}
