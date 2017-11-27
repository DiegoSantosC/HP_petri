using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PetriUI
{
    class SelectionBorder: Border
    {
        private int index;
        private Border selectBorder;

        public SelectionBorder(int indx, Border SB)
        {
            index = indx;
            selectBorder = SB;

        }

        public int getIndex()
        {
            return this.index;
        }

        public Border getBorder()
        {
            return this.selectBorder;
        }

        public void setIndex(int i)
        {
            this.index = i;
        }
        public void setBorder(Border b)
        {
            this.selectBorder = b;
        }
    }
}
