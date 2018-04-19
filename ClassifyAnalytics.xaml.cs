using Kohonen;
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
    /// Interaction logic for ClassifyAnalytics.xaml
    /// </summary>
    public partial class ClassifyAnalytics : Page
    {
        private KohonenNetwork kn;
        private LabelingHandler lh;
        private bool failedImport;

        public ClassifyAnalytics()
        {
            InitializeComponent();
        }

        internal void Init(string sourceFolder)
        {
            object[] returned = DataHandler.ProcessInputTest(sourceFolder);
            if(returned == null)
            {
                failedImport = true;
                return;
            }

            List<string> labels = (List<string>)returned[0];
            Cell[,] map = (Cell[,])returned[1];

            lh = new LabelingHandler(labels);
            kn = new KohonenNetwork(lh, map, this);
        }
    }
}
