using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriUI
{
    class Tuple
    {
        private CaptureWindow captureWindow;
        private int[] parameters;

        public Tuple(CaptureWindow cp, int[] param)
        {
            captureWindow = cp;
            parameters = param;
        }

        public CaptureWindow getCaptureWindow()
        {
            return this.captureWindow;
        }
        public int[] getParameters()
        {
            return this.parameters;
        }
        public void setCaptureWindow(CaptureWindow cp)
        {
            this.captureWindow = cp;
        }
        public void setParameters(int[] param)
        {
            this.parameters = param;
        }

    }
}
