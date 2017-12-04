/* The MIT License (MIT)
*
*  © Copyright 2015 HP Inc.
*
*  Permission is hereby granted, free of charge, to any person obtaining a copy
*  of this software and associated documentation files (the "Software"), to deal
*  in the Software without restriction, including without limitation the rights
*  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*  copies of the Software, and to permit persons to whom the Software is
*  furnished to do so, subject to the following conditions:
*
*  The above copyright notice and this permission notice shall be included in
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

namespace PetriUI
{
    //                    Picture Extraction Sample App Overview
    // This sample app is an educational aid in showcasing how to extract pictures from
    // Sprout's captured moments, and various ways a developer may want to use or save
    // that information, via helper methods and built-in SDK binding support. This sample
    // app assumes you understand the basics of IPcLinks and IPcMoments. If you need
    // refreshing on those, refer to the Capture Moment sample app, the Sprout Developer
    // Guide, or the C# Binding Documentation.
    //
    // This sample app has been developed in .NET 2.0 specifically to increase support
    // of other frameworks and software development kits. You may change the project's
    // properties to a later version of .NET to gain access to more advanced library
    // support and functionality.

    class MainCapture
    {
        public static bool stopRequested;

        public void StartCapture(object param)
        {
            Task t = new Task();

            Task auxCp = (Task)param;

            stopRequested = false;

            t.setCaptureWindow(auxCp.getCaptureWindow());
            t.setNumberOfCaptures(auxCp.getNumberOfCaptures());
            t.setInteval(auxCp.getInterval());
            t.setIndex(auxCp.getIndex());

            for (int i=0; i<=t.getNumberOfCaptures(); i++)
            {
                if (stopRequested) break;

                App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() => t.getCaptureWindow().Trigger_Capture()));

                // interval*60 missing for testing purposes

                Thread.Sleep(t.getInterval() * 1000);
            }

            Console.WriteLine("Capture for object finished");
        }

        public OutlineParameters ConfirmCapture()
        {
           
            OutlineParameters op = MomentCapture.ConfirmCapture();

            return op;         
        }    
    }
}