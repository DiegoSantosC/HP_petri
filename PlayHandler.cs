using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PetriUI
{
    /// <summary>
    /// 
    /// Thread that holds the user made player that toggles between the taken captures of a process
    /// 
    /// Launched and handled by CaptureWindow whenever the play button is pressed
    /// 
    /// </summary>

    class PlayHandler
    {
        // As long as the parameter "playing" is active in Capture Window, this thread will trigger the function CaptureWindow.showNextCapture at a 
        // speed rate given by the mentioned window

        public void StartHandler(object param)
        {
            CaptureWindow cw = (CaptureWindow)param;

            Thread.Sleep(1000);

            while (true) {

                if (!cw.playing)
                {
                    return;
                }else
                {
                    App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                     new Action(() => cw.showNextCapture()));

                    Thread.Sleep((int)(cw.speed * 1000));
                }
            }
        }
    }
}
