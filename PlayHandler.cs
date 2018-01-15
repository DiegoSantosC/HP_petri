using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PetriUI
{
    class PlayHandler
    {
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
