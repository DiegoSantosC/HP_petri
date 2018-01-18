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
using System.Windows.Controls;

namespace PetriUI
{
    /// <summary>
    /// Defines a user's indexed Border
    /// </summary>
    class SelectionBorder: Border
    {
        private int index;
        private Border selectBorder;

        public SelectionBorder(int indx, Border SB)
        {
            index = indx;
            selectBorder = SB;

        }

        // Getters and setters
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
