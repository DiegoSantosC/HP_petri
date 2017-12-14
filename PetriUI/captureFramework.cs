using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PetriUI
{
    class captureFramework
    {
        private Border frameworkBorder;
        private StackPanel capturePanel;
        private String timeOfCapture;

        public captureFramework(System.Windows.Controls.Image img, int size, double ratio)
        {
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
