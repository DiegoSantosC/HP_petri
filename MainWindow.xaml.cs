﻿using System;
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
using System.Threading;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

   
    public partial class MainWindow : Window
    {

        public static Thread captureThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Capture_Button_Click(object sender, RoutedEventArgs e)
        {

            MainCapture newCapture = new MainCapture();
            captureThread = new Thread(newCapture.StartCapture);
            captureThread.Start();
        }
    }

}
