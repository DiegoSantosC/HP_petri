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
    /// Interaction logic for AnalysisWindow.xaml
    /// </summary>
    public partial class AnalysisWindow : NavigationWindow
    {
        private Picker pick;
        private ClassifyAnalytics clssPage;
        private CountAnalytics countPage;

        public AnalysisWindow(CaptureWindow cw, bool countAnalysis, bool classAnalysis)
        {
            InitializeComponent();

            this.Width = 1200;
            this.Height = 900;

            if (classAnalysis) clssPage = new ClassifyAnalytics();
            if (countAnalysis) countPage = new CountAnalytics();

            pick = new Picker(cw, countAnalysis, classAnalysis, clssPage, countPage);

            Navigate(pick);

        }

        public CountAnalytics getCount()
        {
            return countPage;
        }

        public ClassifyAnalytics getClass()
        {
            return clssPage;
        }
    }
}
