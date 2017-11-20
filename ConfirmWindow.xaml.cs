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

namespace PetriUI
{

    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow(int numberOfCaptures, int hoursInterval, int minutesInterval)
        {
            InitializeComponent();

            numberOfCapturesInput.Content = numberOfCaptures;

            int hours, minutes;

            if(hoursInterval == 0)
            {
                hours = (minutesInterval * numberOfCaptures)/60;
                minutes = (minutesInterval * numberOfCaptures) % 60;
            }
            else
            {
                minutesInterval = minutesInterval + hoursInterval * 60;
                hours = (minutesInterval * numberOfCaptures) / 60;
                minutes = (minutesInterval * numberOfCaptures) % 60;

            }

            totalTimeInput.Content = hours + " hours " + minutes + " minutes";

            Image confirmImg = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(PictureHandling.confirmPath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            confirmImg.Source = src;
            confirmImg.Stretch = Stretch.Uniform;
          
            confStackPanel.Children.Add(confirmImg);
            
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {

            PictureHandling.SweepPicture();

            this.Close();

        }


    }
}
