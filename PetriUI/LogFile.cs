using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriUI
{
    class LogFile
    {
        private List<string> data;
        private int index;
        private string targetLocation;

        public LogFile(string targetLocation, int index, List<string> data)
        {
            this.targetLocation = targetLocation;
            this.index = index;
            this.data = data;
        }

        public void BuildAndSave()
        {
            string[] header =
            {
                "Log File for object " + index,
                " ",
                data.Count + " captures have been taken:",
  
            };

            File.WriteAllLines(targetLocation + @"\README.txt", header); 

            string captureTime = "";

            for (int i = 0; i < data.Count; i++)
            {
                captureTime = "\t Capture " + (i + 1) + " taken at " + data.ElementAt(i);
                File.AppendAllText(targetLocation + @"\README.txt", captureTime + Environment.NewLine);
            }       
        }
    }
}
