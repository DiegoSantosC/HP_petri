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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for CapturePreviews.xaml
    /// </summary>
    public partial class CapturePreviews : Page
    {
        public CapturePreviews()
        {
            InitializeComponent();

            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }
    }
}
