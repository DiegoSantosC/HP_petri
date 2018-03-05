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

// .NET Framework Namespaces
using System;
using System.Drawing;
using System.IO;

// Sprout SDK Namespaces
using hp.pc;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using AnalysisTestApp;

namespace PetriUI
{
    class PictureHandling
    {
        /// <summary>
        /// Management of the resoults of the picture of outline extraction, either saving them
        /// or handling them to the App
        /// </summary>

        private static int marker = 0, marker2 = 0;

        // This string makes a timestamped session folder for files saved that use this class.

        private static string _saveDirectory;
        public static string confirmPath;

        // Save all objects in a picture separately

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

        // Handle pictures given by and index to CapturePreviews

        internal static List<System.Windows.Controls.Image> SaveSamples(IPcPicture picture, List<string> folders, List<int> indexes)
        {
            List<System.Windows.Controls.Image> imgs = new List<System.Windows.Controls.Image>();

            int i = 0;

            for (int j = 0; j < folders.Count; j++)
            {
                i = 0;

                foreach (IPcPicture image in picture.Children)
                {
                    if (i == indexes[j])
                    {
                        string dir = Path.Combine(folders[j], @"Captures\");
                        ToolBox.EnsureDirectoryExists(dir);

                        string fileAndPath = Path.Combine(dir, DateTime.Now.ToString("MM-dd-yyyy_hh.mm.ss" + "_" + marker) + ".bmp");
                        ToolBox.SaveProcessedImage(image.Image, fileAndPath);

                        Uri u = new Uri(fileAndPath, UriKind.Relative);
                        System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                        BitmapImage src = new BitmapImage();

                        src.BeginInit();
                        src.UriSource = u;
                        src.CacheOption = BitmapCacheOption.OnLoad;
                        src.EndInit();
                        im.Source = src;
                        im.Stretch = Stretch.Uniform;
                        im.Stretch = Stretch.Uniform;

                        imgs.Insert(0, im);

                    }

                    i++;
                }
            }

            return imgs;
        }

        // Gets the outlines that match a given index list 

        internal static OutlineParameters getOutlines(IPcOutline parentOutline, List<int> indexes)
        {
            Point globalPicSize = new Point(Convert.ToInt32(GetOutlineWidth(parentOutline)), Convert.ToInt32(GetOutlineHeight(parentOutline)));

            List<Point> outlineBoundaries = new List<Point>();
            List<PcPhysicalPoint> outlineLocations = new List<PcPhysicalPoint>();

            for (int j = 0; j < indexes.Count; j++)
            {
                int i = 0;

                foreach (IPcOutline childOutline in parentOutline.Children)
                {
                    if (i == indexes[j])
                    {

                        outlineBoundaries.Add(new Point(Convert.ToInt32(GetOutlineWidth(childOutline)), Convert.ToInt32(GetOutlineHeight(childOutline))));
                        PcPhysicalPoint loc = new PcPhysicalPoint(childOutline.PhysicalBoundaries.Location.X * (childOutline.PixelDensity.X), childOutline.PhysicalBoundaries.Location.Y * (childOutline.PixelDensity.Y));
                        outlineLocations.Add(loc);
                    }

                    i++;
                }
            }

            OutlineParameters op = new OutlineParameters(outlineLocations, outlineBoundaries, globalPicSize);

            return op;
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

        // Saves the outlines

        public static OutlineParameters SavePicture(IPcPicture picture, IPcOutline parentOutline)
        {
            _saveDirectory = Path.Combine(ToolBox.defaultFilePath, @"Pictures\" + "ConfirmDirectory");

            ToolBox.EnsureDirectoryExists(_saveDirectory);
            ToolBox.EnsureDirectoryExists(ToolBox.defaultFilePath);

            PcImage image = picture.Image;

            confirmPath = Path.Combine(_saveDirectory, "confirmPicture" + marker2 + ".bmp");
            ToolBox.SaveProcessedImage(image, confirmPath);
            marker2++;

            List<Point> outlineBoundaries = new List<Point>();

            Point globalPicSize = new Point(Convert.ToInt32(GetOutlineWidth(parentOutline)), Convert.ToInt32(GetOutlineHeight(parentOutline)));

            foreach (IPcOutline outline in parentOutline.Children)
            {
                if (GetOutlineHeight(outline) > 50 && GetOutlineWidth(outline) > 50)
                {
                    outlineBoundaries.Add(new Point(Convert.ToInt32(GetOutlineWidth(outline)), Convert.ToInt32(GetOutlineHeight(outline))));
                }
            }

            List<PcPhysicalPoint> pictureLocation = new List<PcPhysicalPoint>();

            foreach (IPcPicture pic in picture.Children)
            {
                PcPhysicalPoint loc = new PcPhysicalPoint(pic.PhysicalBoundaries.Location.X * (pic.PixelDensity.X), pic.PhysicalBoundaries.Location.Y * (pic.PixelDensity.Y));
                pictureLocation.Add(loc);

            }

            OutlineParameters op = new OutlineParameters(pictureLocation, outlineBoundaries, globalPicSize);

            return op;
        }

        // Save an image given by a task

        internal static void SaveIndexedImage(IPcPicture picture, IPcOutline outline, Task t)
        {
            string dir = Path.Combine(t.getFolder(), @"Captures\");
            ToolBox.EnsureDirectoryExists(dir);

            int i = 0;

            foreach (IPcPicture image in picture.Children)
            {
                if (i == t.getIndex())
                {
                    if (ConfirmMatch(outline, i, t))
                    {
                        string fileAndPath = Path.Combine(dir, DateTime.Now.ToString("MM-dd-yyyy_hh.mm.ss" + "_" + marker) + ".bmp");
                        ToolBox.SaveProcessedImage(image.Image, fileAndPath);

                        List<Uri> l = new List<Uri>();

                        t.setCaptures(l);

                        Uri u = new Uri(fileAndPath, UriKind.Relative);
                        l.Add(u);
                        t.getCaptureWindow().DrawImage();

                        ++i;

                    }
                    else { MomentCapture.Capture(t, true); }
                }
            }
        }

        public static void SaveIndexedImageRep(IPcPicture picture, IPcOutline outline, Task t)
        {
            // if we are repeating the capture, an error has occured. We will thus try to match the
            // outline with other indexed objects

            string dir = Path.Combine(t.getFolder(), @"Captures\");
            ToolBox.EnsureDirectoryExists(dir);

            List<int> locationDifferences = new List<int>();

            foreach (IPcOutline outlineChild in outline.Children)
            {
                if (ConfirmSize(outlineChild, t))
                {
                    locationDifferences.Add(FindCloser(outline, t));
                }
                else { locationDifferences.Add(Int32.MaxValue); }
            }

            int[] minIndex = new int[] { Int32.MaxValue, Int32.MaxValue };

            for (int i = 0; i < locationDifferences.Count; i++)
            {
                if (locationDifferences[i] < minIndex[0]) { minIndex[0] = locationDifferences[i]; minIndex[1] = i; }
            }

            if(minIndex[0] < AdvancedOptions._nLocationThreshold)
            {
                string fileAndPath = Path.Combine(dir, DateTime.Now.ToString("MM-dd-yyyy_hh.mm.ss" + "_" + marker) + ".bmp");
                ToolBox.SaveProcessedImage(getImage(minIndex[1], picture), fileAndPath);

                List<Uri> l = new List<Uri>();

                t.setCaptures(l);

                Uri u = new Uri(fileAndPath, UriKind.Relative);
                l.Add(u);
                t.getCaptureWindow().DrawImage(false);
            }
            else if(minIndex[0] < Int32.MaxValue)
            {
                string fileAndPath = Path.Combine(dir, DateTime.Now.ToString("MM-dd-yyyy_hh.mm.ss" + "_" + marker) + ".bmp");
                ToolBox.SaveProcessedImage(getImage(minIndex[1], picture), fileAndPath);

                List<Uri> l = new List<Uri>();

                t.setCaptures(l);

                Uri u = new Uri(fileAndPath, UriKind.Relative);
                l.Add(u);
                t.getCaptureWindow().DrawImage(true);
            
            }
            else
            {
                t.getCaptureWindow().CaptureError();
            }
        }

        private static PcImage getImage(int index, IPcPicture pic)
        {
            int counter = 0;

            foreach (IPcPicture img in pic.Children)
            {
                if (counter == index) return img.Image;

                counter++;
            }

            return null;
        }

        // Outlines match

        private static bool ConfirmMatch(IPcOutline outline, int i, Task t)
        {
            int counter = 0;

            foreach (IPcOutline childOutline in outline.Children)
            {
                if (counter == i)
                {
                    if (CompareOutlines(childOutline, t))
                    {
                        return true;
                    }
                    else return false;
                }
                counter++;
            }

            return false;
        }

        private static bool CompareOutlines(IPcOutline childOutline, Task t)
        {
            // Size is considered to be a knockout feature

            Point size = new Point(Convert.ToInt32(GetOutlineWidth(childOutline)), Convert.ToInt32(GetOutlineHeight(childOutline)));

            if (Math.Abs(size.X - t.getSize().X) > AdvancedOptions._nSizeThreshold || Math.Abs(size.Y - t.getSize().Y) > AdvancedOptions._nSizeThreshold) return false;

            PcPhysicalPoint location = new PcPhysicalPoint(childOutline.PhysicalBoundaries.Location.X * (childOutline.PixelDensity.X), childOutline.PhysicalBoundaries.Location.Y * (childOutline.PixelDensity.Y));

            if (Math.Abs(location.X - t.getLocation().X) > AdvancedOptions._nLocationThreshold || Math.Abs(location.Y - t.getLocation().Y) > AdvancedOptions._nLocationThreshold) return false;

            return true;
        }

        private static bool ConfirmSize(IPcOutline childOutline, Task t)
        {
            Point size = new Point(Convert.ToInt32(GetOutlineWidth(childOutline)), Convert.ToInt32(GetOutlineHeight(childOutline)));

            if (Math.Abs(size.X - t.getSize().X) > AdvancedOptions._nSizeThreshold || Math.Abs(size.Y - t.getSize().Y) > AdvancedOptions._nSizeThreshold) return false;

            return true;
        }

        private static int FindCloser(IPcOutline outline, Task t)
        {

            PcPhysicalPoint location = new PcPhysicalPoint(outline.PhysicalBoundaries.Location.X * (outline.PixelDensity.X), outline.PhysicalBoundaries.Location.Y * (outline.PixelDensity.Y));

            // Euclidean distance between the object and the target

            int dist = (int)Math.Sqrt(Math.Pow(location.X - t.getLocation().X, 2) + Math.Pow(location.Y - t.getLocation().Y, 2));

            return dist;
        }
    }    
}