using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
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

        internal static List<Image> getSamples(List<int> indexes)
        {
            try
            {
                using (IPcLink link = HPPC.CreateLink())
                {
                    using (IPcMoment moment = link.CaptureMoment())
                    {
                        IPcPicture picture = link.ExtractPicture(moment);

                        List<Image> img = PictureHandling.SaveSamples(picture, indexes);
                        return img;
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
