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

// .NET framework namespace
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sprout SDK namespace
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace PetriUI
{
    /// <summary>
    /// Definition of a task, which includes parameters for a capture, the Window
    /// associated to it and the path in which they are saved
    /// </summary>
    class Task
    {
        private CaptureWindow captureWindow;
        private int numberOfCaptures, interval, index;
        private List<Uri> captures;

        // Constructor
        public Task(CaptureWindow cw, int nOfC, int inter, int ind, List<Uri> cpt)
        {
            captureWindow = cw;
            numberOfCaptures = nOfC;
            interval = inter;
            index = ind;
            captures = cpt;
        }

        // @Override Task Constructor
        public Task()
        {
            captureWindow = null;
            numberOfCaptures = 0;
            interval = 0;
            index = 0;
        }

        // Getters and setters
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
