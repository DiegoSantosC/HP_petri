using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {          
        public CaptureWindow(int[] parameters)
        {
            InitializeComponent();

            MainCapture newCapture = new MainCapture();

            Tuple t = new Tuple(this, parameters);

            Thread newCaptureThread = new Thread(newCapture.StartCapture);
            newCaptureThread.SetApartmentState(ApartmentState.STA);
            newCaptureThread.Start(t); 

            this.Width = 1200;
            this.Height = 900;
       

        }
        
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {

            //MainPage.captureThread.Abort();
        }

        public void DrawImage(BitmapImage img)
        {
            Image image = new Image();
            image.Source = img;
            image.Stretch = Stretch.Uniform;

            StackPanel imgSP = new StackPanel();
            imgSP.Children.Add(image);
            ImagesCanvas.Children.Add(imgSP);

            Console.WriteLine("Traza");
        }
    }
}
