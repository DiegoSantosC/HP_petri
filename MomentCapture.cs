using System;
using System.Threading.Tasks;
using System.Timers;

using hp.pc;

namespace PetriUI
{
    class MomentCapture
    {
        public static void Capture(Task t)
        {
           using (IPcLink link = HPPC.CreateLink())
           {
              using (IPcMoment moment = link.CaptureMoment())
              {
                  IPcPicture picture = link.ExtractPicture(moment);
                  PictureHandling.SaveIndexedImage(picture, t);
              }
           }


        }

      
        public static OutlineParameters ConfirmCapture()
        {
            try
            {
                using (IPcLink link = HPPC.CreateLink())
                {
                    using (IPcMoment moment = link.CaptureMoment())
                    {
                        // IPcPictures are extracted pictures from moment capture that contain information about
                        // picture boundaries, pixel density per millimeter, image representations of the picture(s),
                        // as well as child-level IPcPictures that contain the same type of data as the mat (parent)
                        // and all detected objects on the mat (children). IPcPictures and IPcOutlines have Skew Angle
                        // information in radians, which aids in picture rotation correction. Refer to the Sprout
                        // Developer Guide for detailed information on handling skew angles.

                        IPcPicture picture = link.ExtractPicture(moment);
                        IPcOutline outline = link.ExtractOutline(moment);

                        OutlineParameters op = PictureHandling.SavePicture(picture, outline);
                        return op;
                    }
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
