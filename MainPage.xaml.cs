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
using System.Windows.Navigation;



namespace PetriUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

   
    public partial class MainPage : Page
    {

        //public static Thread captureThread;
        public static int numberOfCaptures, interval;

        public MainPage()
        {
            InitializeComponent();

            
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        private void Capture_Button_Click(object sender, RoutedEventArgs e)
        {

            
            if((Int32.TryParse(intervalTextBox.Text, out interval))&& (Int32.TryParse(numberOfCapturesTextBox.Text, out numberOfCaptures)))
                {

                
                MainCapture newCapture = new MainCapture();
                newCapture.ConfirmCapture();
                //captureThread = new Thread(newCapture.ConfirmCapture);
                //captureThread.Start();  

                ConfirmWindow confirmWindow = new ConfirmWindow(numberOfCaptures, interval);
                confirmWindow.Show();
                this.IsEnabled = false;

            }
            else
            {
                MessageBox.Show("Parameter parsing error");
                
            }
            
        }
    }

}
