/* © Copyright 2018 HP Inc.
*
*  Permission is hereby granted, free of charge, to any person obtaining a copy
*  of this software and associated documentation files (the "Software"), to deal
*  in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
*  all copies or substantial portions of the Software.
*
*  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*  THE SOFTWARE.
*/

// .NET framework namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sprout SDK namespaces
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
using System.IO;

namespace PetriUI
{
    /// <summary>
    /// Interaction logic for SupportWindow.xaml
    /// 
    /// Funcionalities : Window structure that holds MainPage and CapturePreviews pages
    /// 
    /// Launched in start
    /// 
    /// Redirected to MainPage
    /// 
    /// </summary>
    
    public partial class MainWindow : NavigationWindow
    {
        public static bool killRequest;

        public MainWindow()
        {
            InitializeComponent();
            killRequest = false;
        }

        // Closing management (triggered when closing MainPage or CapturePreview)
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

                    Directory.Delete(ToolBox.defaultFilePath, true);
                }
                else
                {
                    e.Cancel = true;
                }
            }else
            {
                try
                {
                    Directory.Delete(ToolBox.defaultFilePath, true);
                }catch(DirectoryNotFoundException notFound)
                {

                }
             
            }
        }
    }
}
