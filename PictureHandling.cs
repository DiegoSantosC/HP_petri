/* The MIT License(MIT)
*
*  © Copyright 2015 HP Inc.
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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*  THE SOFTWARE.
*/

// .NET Framework Namespaces
using System;
using System.Drawing;
using System.IO;

// Sprout SDK Namespaces
using hp.pc;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace PetriUI
{
    class PictureHandling
    {
        private static int marker = 0, marker2 = 0;

        // This string makes a timestamped session folder for files saved that use this class.
        private static string _saveDirectory;
        public static string confirmPath;
        
        // This loops through all the child-level images in IPcPicture and saves them as Bitmaps.
        public static void SaveAllImages(IPcPicture picture)
        {
            ToolBox.EnsureDirectoryExists(_saveDirectory);

            int i = 1;
            foreach (IPcPicture image in picture.Children)
            {
                string fileAndPath = Path.Combine(_saveDirectory, "Object_" + i + ".bmp");
                ToolBox.SaveProcessedImage(image.Image, fileAndPath);
                ++i;
            }
        }

        private static int GetOutlineWidth(IPcOutline outline)
        {
            return Convert.ToInt32((outline.PixelDensity.X) * (outline.PhysicalBoundaries.Size.Width));
        }


        private static int GetOutlineHeight(IPcOutline outline)
        {
            return Convert.ToInt32((outline.PixelDensity.Y) * (outline.PhysicalBoundaries.Size.Height));
        }


        // This saves the parent-level image of IPcPicture.
        public static void SaveMatImage(IPcPicture picture)
        {
            _saveDirectory = Path.Combine(ToolBox.defaultFilePath, @"Pictures\" + DateTime.Now.ToString("MM-dd-yyyy_hh.mm.ss" + "_" + marker));

            ToolBox.EnsureDirectoryExists(_saveDirectory);

            PcImage image = picture.Image;

            string fileAndPath = Path.Combine(_saveDirectory, "MatImage.bmp");
            ToolBox.SaveProcessedImage(image, fileAndPath);

            marker++;
        }

        public static OutlineParameters SavePicture(IPcPicture picture, IPcOutline parentOutline)
        {
            _saveDirectory = Path.Combine(ToolBox.defaultFilePath, @"Pictures\" + "ConfirmDirectory");

            ToolBox.EnsureDirectoryExists(_saveDirectory);

            PcImage image = picture.Image;

            confirmPath = Path.Combine(_saveDirectory, "confirmPicture" + marker2 + ".bmp");
            ToolBox.SaveProcessedImage(image, confirmPath);
            marker2++;

            List<Point> outlineBoundaries = new List<Point>();

            foreach (IPcOutline outline in parentOutline.Children)
            {
               
                outlineBoundaries.Add(new Point(Convert.ToInt32(GetOutlineWidth(outline)), Convert.ToInt32(GetOutlineHeight(outline))));
            }

            List<PcPhysicalPoint> pictureLocation = new List<PcPhysicalPoint>();

            foreach (IPcPicture pic in picture.Children)
            {
                pictureLocation.Add(pic.PhysicalBoundaries.Location);

            }

            OutlineParameters op = new OutlineParameters(pictureLocation, outlineBoundaries);

            return op;
        }

        public static void SweepPicture()
        {
            string _sweepDirectory = Path.Combine(ToolBox.defaultFilePath, @"Pictures\" + "ConfirmDirectory");
            string _sweepPath = Path.Combine(_sweepDirectory, "confirmPicture" + (marker2-1) + ".bmp");
          
            ToolBox.deleteImage(_sweepPath);

        }


    }
}