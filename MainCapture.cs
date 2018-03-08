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

// .NET Framework Namespaces
using System;
using System.Threading;
using System.Timers;

// Sprout SDK Namespaces
using hp.pc;
using System.Windows.Controls;
using System.Collections.Generic;

namespace PetriUI
{
    /// <summary>
    /// Thread defined to either trigger a capture immediately or to
    /// trigger them over time given a task parameters
    /// </summary>
    class MainCapture
    {
        public static bool stopRequested;

        // Capture over time
        public void StartCapture(object param)
        {
            Task t = new Task();

            Task auxCp = (Task)param;

            stopRequested = false;

            t.setCaptureWindow(auxCp.getCaptureWindow());
            t.setNumberOfCaptures(auxCp.getNumberOfCaptures());
            t.setInteval(auxCp.getInterval());
            t.setIndex(auxCp.getIndex());
            t.setDelay(auxCp.getDelay());


            Thread.Sleep(t.getDelay()*1000);

            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                     new Action(() => t.getCaptureWindow().startTriggered()));

            for (int i=0; i<t.getNumberOfCaptures(); i++)
            {
                // interval*60 missing for testing purposes
                Thread.Sleep(t.getInterval() * 1000);

                if (stopRequested) break;

                App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() => t.getCaptureWindow().Trigger_Capture()));
            }

            if(!stopRequested)App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                   new Action(() => t.getCaptureWindow().CaptureFinished()));
            
        }

        // Outline capture 
        public OutlineParameters ConfirmCapture()
        {
            OutlineParameters op = MomentCapture.ConfirmCapture();

            return op;         
        }    

        // Image captures samples 
        public object[] Samples(List<string> folders, List<int> indexes, List<PcPhysicalPoint> locations, List<System.Drawing.Point> sizes)
        {
            object[] returnable = MomentCapture.getSamples(folders, indexes, locations, sizes);

            return returnable;
        }
    }
}