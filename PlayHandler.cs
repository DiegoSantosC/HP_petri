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
        public static int speed;

        public void StartHandler(object param)
        {
            speed = 2;

            CaptureWindow cw = (CaptureWindow)param;

            Thread.Sleep(speed * 1000);

            while (true) {

                if (!cw.playing)
                {
                    return;
                }else
                {
                    App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                     new Action(() => cw.showNextCapture()));

                    Thread.Sleep(speed * 1000);
                }
            }
        }
    }
}
