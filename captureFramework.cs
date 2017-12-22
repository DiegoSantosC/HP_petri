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

// .NET Framework namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sprout SDK namespaces
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;


/// <summary>
/// 
/// Initializes a new instance of the capture Framework class
/// Includes definitions of data relating the capture taken and sets the structure in
/// which the captured image will be shown
/// 
/// </summary>

namespace PetriUI
{
    class captureFramework
    {
        private Border frameworkBorder;
        private StackPanel capturePanel;
        private String timeOfCapture;

        public captureFramework(System.Windows.Controls.Image img, int size, double ratio)
        {
            // img : taken capture
            // size : target size of the capture's Border and Panel
            // ratio : capture height / width ratio

            frameworkBorder = new Border();
            capturePanel = new StackPanel();

            if (ratio >= 1)
            {
                capturePanel.Width = size;
                capturePanel.Height = size / ratio;

                frameworkBorder.Width = size + 5;
                frameworkBorder.Height = (size + 5) / ratio;

            }
            else
            {
                capturePanel.Width = size * ratio;
                capturePanel.Height = size;

                frameworkBorder.Width = (size + 5)*ratio;
                frameworkBorder.Height = (size + 5);

            }

            capturePanel.Opacity = 1;
            capturePanel.Children.Add(img);

            timeOfCapture = DateTime.Now.ToString("hh:mm:ss"); ;

        }

        // Getters and setters

        public Border getBorder()
        {
            return frameworkBorder;
        }

        public String getTime()
        {
            return timeOfCapture;
        }

        public StackPanel getCapturePanel()
        {
            return capturePanel;
        }

        internal void setPosition(Thickness newPosition)
        {
            frameworkBorder.Margin = newPosition;
            capturePanel.Margin = newPosition;
        }
    }
}
