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

namespace PetriUI
{
    class PictureHandling
    {
        private static int marker = 0;

        // This string makes a timestamped session folder for files saved that use this class.
        private static string _saveDirectory;


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
    }
}