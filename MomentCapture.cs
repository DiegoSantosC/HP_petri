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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

// Sprout SDK namespace
using System.Windows.Controls;
using hp.pc;

namespace PetriUI
{
    /// <summary>
    /// Interaction between the App and the Sprout hardware via IPcLinks
    /// </summary>
    class MomentCapture
    {
        // Picture by index capture
        public static void Capture(Task t, bool repetition)
        {
            try
            {
                using (IPcLink link = HPPC.CreateLink())
                {
                    using (IPcMoment moment = link.CaptureMoment())
                    {
                        IPcPicture picture = link.ExtractPicture(moment);
                        IPcOutline outline = link.ExtractOutline(moment);

                        if (!repetition) PictureHandling.SaveIndexedImage(picture, outline, t);
                        else { PictureHandling.SaveIndexedImageRep(picture, outline, t); }
                        outline.Dispose();
                        picture.Dispose();
                        moment.Dispose();
                    }
                    link.Dispose();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("\t\t*****An error occurred*****\n\n{0}{1}\n\nExit now, or this console will automatically exit in 15 seconds.", ToolBox.TimeStamp(), exception.Message);
                ToolBox.AppExceptionExit();
            }
        }

        // Outlines capture 
        public static OutlineParameters ConfirmCapture()
        {
            try
            {
                OutlineParameters op;
                using (IPcLink link = HPPC.CreateLink())
                {
                    using (IPcMoment moment = link.CaptureMoment())
                    {
                        IPcPicture picture = link.ExtractPicture(moment);
                        IPcOutline outline = link.ExtractOutline(moment);

                        op = PictureHandling.SavePicture(picture, outline);
                        outline.Dispose();
                        picture.Dispose();
                        moment.Dispose();
                    }
                    link.Dispose();
                }
                return op;
            }
            catch (Exception exception)
            {
                Console.WriteLine("\t\t*****An error occurred*****\n\n{0}{1}\n\nExit now, or this console will automatically exit in 15 seconds.", ToolBox.TimeStamp(), exception.Message);
                ToolBox.AppExceptionExit();
                return null;
            }
        }

        // Several pictures extracted by index
        internal static object[] getSamples(List<string> folders, List<int> indexes, List<PcPhysicalPoint> locations, List<System.Drawing.Point> sizes)
        {
            try
            {
                using (IPcLink link = HPPC.CreateLink())
                {
                    object[] returned;
                    using (IPcMoment moment = link.CaptureMoment())
                    {
                        IPcPicture picture = link.ExtractPicture(moment);
                        IPcOutline outlines = link.ExtractOutline(moment);

                        returned = PictureHandling.SaveSamples(picture, outlines, folders, indexes, locations, sizes);
                        outlines.Dispose();
                        picture.Dispose();
                        moment.Dispose();
                    }
                    link.Dispose();
                    return returned;
                }


            }
            catch (Exception exception)
            {
                Console.WriteLine("\t\t*****An error occurred*****\n\n{0}{1}\n\nExit now, or this console will automatically exit in 15 seconds.", ToolBox.TimeStamp(), exception.Message);
                ToolBox.AppExceptionExit();
                return null;
            }
        }
    }
}
