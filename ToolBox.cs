/* The MIT License (MIT)
*
*  © Copyright 2015 HP Inc.
*
*  Permission is hereby granted, free of charge, to any person obtaining a copy
*  of this software and associated documentation files (the "Software"), to deal
*  in the Software without restriction, including without limitation the rights
*  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*  copies of the Software, and to permit persons to whom the Software is
*  furnished to do so, subject to the following conditions:
*
*  The above copyright notice and this permission notice shall be included in
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// Sprout SDK Namespaces
using hp.pc;

namespace PetriUI
{
    // The ToolBox class is a toolbox of helper methods to speed up and simplify the process of executing basic tasks.
    class ToolBox
    {
        // This is a simple static member to allow you to easily save a files to a directory on your Desktop by getting
        // its path. You can easily change the string to your desired path, for example you could set defaultFilePath to
        // something like, @"C:\Temp\SproutStuff"
        public static string defaultFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"TestOutput_Petri");


        // This little method just sends you back a timestamp to use for file/directory names, logging, or console messages.
        // The default format is HourHour.MinuteMinute.SecondSecond.MillisecondMillisecond
        public static string TimeStamp()
        {
            return DateTime.Now.ToString("HH.mm.ss.ff") + " - ";
        }


        // This method checks for the existence of directories that this program will save files to. If they do not
        // exist, it will attempt to make them. If it cannot create the directory, it will attempt to log an error
        // message to the defaultFilePath. If it cannot log said file, it will send a console error message about
        // how to remedy the issue, then kill the process.
        public static void EnsureDirectoryExists(string directory)
        {
            bool cannotSave = false;

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch
            {
                cannotSave = true;
            }
            finally
            {
                if (cannotSave)
                {
                    Console.WriteLine("Cannot write to " + directory + "\n\n" +
                    "Please exit the program and set your defaultFilePath in ToolBox.cs to a\n" +
                    "directory location you have permission to write to.");
                    AppExceptionExit();
                }
            }
        }


        // This method allows the program to exit without throwing exceptions. It is called when files or directories
        // cannot be created, and exits the program after a console error message appears. By default, this method
        // will automatically kill the process after 15 seconds.
        public static void AppExceptionExit()
        {
            Process thisProcess = Process.GetCurrentProcess();
            thisProcess.WaitForExit(15000);
            thisProcess.Kill();
        }


        // This method determines what the PcImage's pixel format was, and creates a corresponding bitmap using that
        // pixel format's requirements.
        // 
        // Side note: Currently, PcImage classifies the pixel format as either BGR, BGRA, Grayscale16, or Grayscale.
        public static Bitmap CreateBitmap(PcImage pcImage, bool fromExistingImage)
        {
            Bitmap bmp;

            if (fromExistingImage)
            {
                switch (pcImage.PixelFormat)
                {
                    case PcPixelFormat.BGR:
                        bmp = new Bitmap(pcImage.Size.Width, pcImage.Size.Height, pcImage.RowSize, PixelFormat.Format24bppRgb, pcImage.BufferHandle);
                        break;
                    case PcPixelFormat.BGRA:
                        bmp = new Bitmap(pcImage.Size.Width, pcImage.Size.Height, pcImage.RowSize, PixelFormat.Format32bppArgb, pcImage.BufferHandle);
                        break;
                    case PcPixelFormat.Grayscale:
                        bmp = new Bitmap(pcImage.Size.Width, pcImage.Size.Height, pcImage.RowSize, PixelFormat.Format16bppGrayScale, pcImage.BufferHandle);
                        break;
                    case PcPixelFormat.Grayscale16:
                        bmp = new Bitmap(pcImage.Size.Width, pcImage.Size.Height, pcImage.RowSize, PixelFormat.Format16bppGrayScale, pcImage.BufferHandle);
                        break;
                    default:
                        bmp = new Bitmap(pcImage.Size.Width, pcImage.Size.Height, pcImage.RowSize, PixelFormat.DontCare, pcImage.BufferHandle);
                        break;
                }
            }
            else
            {
                bmp = new Bitmap(pcImage.Size.Width, pcImage.Size.Height);
            }

            return bmp;
        }


        // This method takes a PcImage and a desired file path and name, then creates a bitmap of it, and saves. Once
        // more, like all methods in the ToolBox class, these are simply time-saving methods. Use at your own discretion.
        public static void SaveProcessedImage(PcImage image, string filePathAndName)
        {
            using (Bitmap bmp = ToolBox.CreateBitmap(image, true))
            {
                bmp.Save(filePathAndName);
            }
        }
    }
}