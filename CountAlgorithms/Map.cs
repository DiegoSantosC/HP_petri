using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriUI
{
    /// <summary>
    ///
    /// User made class representing a bitmap's pixel information as a grey values array and RGB values array
    /// 
    /// </summary>

    class Map
    {
        private int[,] greyMap;
        private Color[,] RGBMap;

        public Map(int[,] grey, Color[,] RGB)
        {
            greyMap = grey;
            RGBMap = RGB;

        }

        public int[,] getGreyMap()
        {
            return greyMap;
        }
        public Color[,] getRGB()
        {
            return RGBMap;
        }
    }
}
