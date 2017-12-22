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
using hp.pc;
using System.Drawing;
using System.Collections.Generic;

namespace PetriUI
{
    class OutlineParameters
    {
        /// <summary>
        /// OutlineParameters defines objects outlines given a Location, Size and the Size of the screen they are located in
        /// </summary>
        private List<PcPhysicalPoint> location;
        private List<Point> size;
        private Point globalPicSize;

        public OutlineParameters(List<PcPhysicalPoint> outlineLocation, List<Point> outlineSize, Point gPS)
        {
            location = outlineLocation;
            size = outlineSize;
            globalPicSize = gPS;
        }

        // Getters and setters
        public List<PcPhysicalPoint> getLocation(OutlineParameters op)
        {
            return op.location;
        }
        public List<Point> getSize(OutlineParameters op)
        {
            return op.size;
        }

        public Point getGlobalSize(OutlineParameters op)
        {
            return op.globalPicSize;
        }

        public void setLocation(List<PcPhysicalPoint> outlineLocation)
        {
            this.location = outlineLocation;
        }
        public void setSize(List<Point> outlineSize)
        {
            this.size = outlineSize;
        }
        public static int getCapacity(OutlineParameters op)
        {
            return op.size.Count();
        }
    }
}
