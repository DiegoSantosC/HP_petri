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
using System.Windows.Navigation;
using System.ComponentModel;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for SupportWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public static bool killRequest;
        public MainWindow()
        {
            InitializeComponent();
            killRequest = false;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MainPage.capturesRunning)
            {
                string msg = "There are still captures running. Close anyways?";

                MessageBoxResult res =
                  MessageBox.Show(
                      msg,
                      "Closing Dialog",
                      MessageBoxButton.YesNo,
                      MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                    killRequest = true;
                    CapturePreviews.killAllCaptures();
                    
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
