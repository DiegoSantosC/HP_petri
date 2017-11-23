using hp.pc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriUI
{
    class OutlineParameters
    {
        private List<PcPhysicalPoint> location;
        private List<Point> size;
        private Point globalPicSize;

        public OutlineParameters(List<PcPhysicalPoint> outlineLocation, List<Point> outlineSize, Point gPS)
        {
            location = outlineLocation;
            size = outlineSize;
            globalPicSize = gPS;
        }

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
