﻿/* © Copyright 2018 HP Inc.
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

// .NET framework namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriUI
{
    /// <summary>
    /// Definition of the File creation class
    /// </summary>
    /// 
    class LogFile
    {
        private int index, nOfC;
        private string name;
        private List<string> captureData;
        private List<List<string>> classData;
        private List<List<string>> eventsData;
        private string targetLocation;

        // File creation
        public LogFile(string targetLocation, int ind, string n, int number)
        {
            this.targetLocation = targetLocation;
            this.index = ind;
            nOfC = number;
            name = n;
            captureData = new List<string>();
            classData = new List<List<string>>();
            eventsData = new List<List<string>>();

        }

        // File content building
        public void BuildAndSave()
        {
            string[] header =
            {
                "Log File for process " + name,
                "   Capture of object " + index,
                " ",
                nOfC + " captures have been taken:",

            };

            File.AppendAllLines(targetLocation + @"\README.txt", header);
            File.AppendAllLines(targetLocation + @"\README.txt", new string[] { "\n" });

            File.AppendAllLines(targetLocation + @"\README.txt", new string[] { captureData[0], "\n" });
            File.AppendAllLines(targetLocation + @"\README.txt", new string[] { captureData[1], "\n" });


            for (int i = 0; i < captureData.Count -2; i++)
            {
                // capture data

                File.AppendAllLines(targetLocation + @"\README.txt", new string[] {captureData[i + 2]});

                // events data

                File.AppendAllLines(targetLocation + @"\README.txt", eventsData[i]);

                if(eventsData[i].Count > 0)File.AppendAllLines(targetLocation + @"\README.txt", new string[] { " " });

                File.AppendAllLines(targetLocation + @"\README.txt", classData[i]);

                if (classData[i].Count > 0)
                {
                    File.AppendAllLines(targetLocation + @"\README.txt", new string[] { " ", "\n"});
                }
            }
        }

        public void AppendData(string data)
        {
            captureData.Add(data);
        }

        public void AppendClassData(List<string> data)
        {
            classData.Add(data);
        }
        public void AppendEvents(List<string> events)
        {
            eventsData.Add(events);
        }

        public void AppendComment(string comment)
        {
            File.AppendAllLines(targetLocation + @"\README.txt", new string[] {"\n", comment, "\n"});
        }
    }
}
