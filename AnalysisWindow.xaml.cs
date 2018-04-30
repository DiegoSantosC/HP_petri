﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// 
    /// Funcionalities : Window structure that holds all analysis pages (Picker, CountAnalytics, ClassAnalytics, ChartPage)
    /// 
    /// Launched at start of a capture process if an analysis is selected
    /// 
    /// Redirected to Picker
    /// 
    /// </summary>

    public partial class AnalysisWindow : NavigationWindow
    {
        private Picker pick;
        private ClassifyAnalytics clssPage;
        private CountAnalytics countPage;
        private ChartPage chartPage;
        private bool closeRequest;

        // This window will work as the link between the capture processes and the analysis processes

        public AnalysisWindow(CaptureWindow cw, bool countAnalysis, bool classAnalysis, string map)
        {
            InitializeComponent();

            this.Width = 1200;
            this.Height = 900;

            if (classAnalysis) { clssPage = new ClassifyAnalytics(); clssPage.Init(map); }
            if (countAnalysis) { countPage = new CountAnalytics(this); chartPage = new ChartPage(); }

            pick = new Picker(cw, countAnalysis, classAnalysis, clssPage, countPage, chartPage);

            Navigate(pick);

            closeRequest = false;
        }
        public void requestClosing()
        {
            closeRequest = true;
        }
        public CountAnalytics getCount()
        {
            return countPage;
        }

        public ClassifyAnalytics getClass()
        {
            return clssPage;
        }

        public Picker getPicker()
        {
            return pick;
        }

        private void CaptureWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!closeRequest)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        public ChartPage getChart()
        {
            return chartPage;
        }
    }
}
