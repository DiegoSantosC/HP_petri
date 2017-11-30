using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PetriUI
{
    class Task
    {
        private CaptureWindow captureWindow;
        private int numberOfCaptures, interval, index;
        private List<Uri> captures;

        public Task(CaptureWindow cw, int nOfC, int inter, int ind, List<Uri> cpt)
        {
            captureWindow = cw;
            numberOfCaptures = nOfC;
            interval = inter;
            index = ind;
            captures = cpt;
        }

        public Task()
        {
            captureWindow = null;
            numberOfCaptures = 0;
            interval = 0;
            index = 0;
        }

        public CaptureWindow getCaptureWindow()
        {
            return this.captureWindow;
        }
        public List<Uri> getCaptures()
        {
            return this.captures;
        }
        public int getNumberOfCaptures()
        {
            return this.numberOfCaptures;
        }
        public int getInterval()
        {
            return this.interval;
        }
        public int getIndex()
        {
            return this.index;
        }
        public void setCaptureWindow(CaptureWindow cp)
        {
            this.captureWindow = cp;
        }
        public void setNumberOfCaptures(int nOfC)
        {
            this.numberOfCaptures = nOfC;
        }
        public void setInteval(int inter)
        {
            this.interval = inter;
        }

        public void setIndex(int ind)
        {
            this.index = ind;
        }
        public void setCaptures(List<Uri> cpt)
        {
            this.captures = cpt;
        }


    }
}
