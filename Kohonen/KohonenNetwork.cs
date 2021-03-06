﻿using PetriUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Kohonen
{
    class KohonenNetwork
    {
        private List<List<Bitmap>> sourceData;
        private LabelingHandler lh;
        private Cell[,] map;
        private ClassifyAnalytics cw;

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Construction
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public KohonenNetwork(LabelingHandler labels, Cell[,] srcMap, ClassifyAnalytics classWindow)
        {
            lh = labels;

            map = srcMap;

            cw = classWindow;

            sourceData = new List<List<Bitmap>>();
        }

        private void Init_Map()
        {
            int sizeX = AdvancedOptions._nKohonenMapSizeX;
            int sizeY = AdvancedOptions._nKohonenMapSizeY;

            map = new Cell[sizeY, sizeX];

            for (int i = 0; i < sizeY; i++)
                for (int j = 0; j < sizeX; j++) map[i, j] = generateCell(i, j);
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Kohonen Algorithm
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private double ComputeDistanceFactor(int currEpoch)
        {
            // The distance factor will be computed so that the change applied to far away cells from the winner one is negligible,
            // and is exctracted from the formula "e^(c*dist(AB))" , being c the distance factor, and dist(AB) the Euclidean distance
            // between cell A (the winner) and B (the current one)

            if (currEpoch > AdvancedOptions._nEpochsUntilConvergence)
                currEpoch = AdvancedOptions._nEpochsUntilConvergence;

            double posEpoch = ((double)(AdvancedOptions._nEpochsUntilConvergence - currEpoch)) / ((double)(AdvancedOptions._nEpochsUntilConvergence - 0));
            double currRadius = AdvancedOptions._nFinalRadius + posEpoch * (AdvancedOptions._nInitialRadius - AdvancedOptions._nFinalRadius); // distance???????????????

            double F = Math.Log(AdvancedOptions._dMaxRadiusFactor) / (Math.Pow(currRadius, 2) + 1E-11);

            return F;
        }

        public List<int[]> Test_Network(List<Bitmap> clustList)
        {
            sourceData.Add(clustList);
            List<int[]> bestMatches = new List<int[]>();

            for(int i=0; i < clustList.Count; i++)
            {
                int[] bestMatch = FindBestMatch(clustList[i], -1);

                bestMatches.Add(bestMatch);
            }

            return bestMatches;
        }

        private int[] FindBestMatch(Bitmap bitmap, int label)
        {
            int[] bestMatch = new int[2] { -1, -1};
            double bestDifference = double.MaxValue;

            for(int i = 0; i < map.GetLength(0); i++)
            {
                for(int j = 0; j < map.GetLength(1); j++)
                {
                    // Take best match considering label

                    if ( label == -1 || map[i, j].getIndex() == -1 || map[i, j].getIndex() == label)
                    {
                        // If the cell's label is adequate, compute difference

                        double difference = ComputeDifference(bitmap, map[i, j].getSource());

                        // If the difference is the least, take as winner

                        if (difference < bestDifference)
                        {
                            bestMatch = new int[] { i, j };
                            bestDifference = difference;
                        }
                    }
                }
            }

            //Console.WriteLine(bestDifference);

            return bestMatch;
        }

        private void ModifyMap(int[] pos,  Bitmap sourceBmp, int currEpoch, double LF, int label)
        {
            for(int i = 0; i < AdvancedOptions._nKohonenMapSizeY; i++)
            {
                for (int j = 0; j < AdvancedOptions._nKohonenMapSizeX; j++)
                {
                    double disY = Math.Abs(pos[0] - i);
                    double disX = Math.Abs(pos[1] - j);

                    disY = Math.Min(disY, AdvancedOptions._nKohonenMapSizeY - disY);
                    disX = Math.Min(disX, AdvancedOptions._nKohonenMapSizeX - disX);
                    
                    double distance = Math.Pow(disY, 2) + Math.Pow(disX, 2);

                    if(label == -1 || map[i, j].getIndex() == -1 || map[i, j].getIndex() == label)
                    {
                        ModifyCell(map[i, j], distance, sourceBmp, currEpoch, LF);
                    }
                }
            }
        }

        private void ModifyCell(Cell cell, double distance, Bitmap sourceBmp, int currEpoch, double LF)
        {
            double distanceFactor = ComputeDistanceFactor(currEpoch);

            float[][][] bmp = cell.getSource();

            for(int i = 0; i < sourceBmp.Height; i++)
            {
                for(int j = 0; j < sourceBmp.Width; j++)
                {
                    bmp[i][j] = ModifiedPixel(bmp[i][j], sourceBmp.GetPixel(i, j), distance, LF, distanceFactor);
                }
            }
        }

        private float[] ModifiedPixel(float[] color1, Color color2, double distance, double k, double distanceFactor)
        {
            float diffR = color2.R - color1[0];
            float diffG = color2.G - color1[1];
            float diffB = color2.B - color1[2];

            if(distanceFactor == -1)
            {
                return color1;
            }
            else
            {
                float newR = (float)(color1[0] + k * diffR * Math.Exp(distanceFactor * distance));
                float newG = (float)(color1[1] + k * diffG * Math.Exp(distanceFactor * distance));
                float newB = (float)(color1[2] + k * diffB * Math.Exp(distanceFactor * distance));

                return new float[]{ newR, newG, newB };
            }
        }

        private double ComputeLearningFactor(int epoch)
        {
            if (epoch > AdvancedOptions._nEpochsUntilConvergence) epoch = AdvancedOptions._nEpochsUntilConvergence;

            double posEpoch = ((double)epoch) / ((double)AdvancedOptions._nEpochsUntilConvergence);

            return (AdvancedOptions._dFinalLearningFactor - AdvancedOptions._dInitialLearningFactor) * posEpoch + AdvancedOptions._dInitialLearningFactor;           

        }

        private double ComputeDifference(Bitmap bitmap1, float[][][] bitmap2)
        {
            double acc = 0;

            for (int i = 0; i < bitmap1.Height; i++)
                for(int j = 0; j < bitmap1.Width; j++)
                    acc += PixelDifference(bitmap1.GetPixel(i,j), bitmap2[i][j]);
                
            return acc;
        }

        private double PixelDifference(Color c1, float[] c2)
        {
            return (Math.Sqrt(Math.Pow((c1.R - c2[0]), 2) + Math.Pow((c1.G - c2[1]), 2) + Math.Pow((c1.B - c2[2]), 2))); 
        }

        private Cell generateCell(int x, int y)
        {
            int size = AdvancedOptions._nBitmapSize;

            float[][][] bmp = new float[size][][];

            Random r = new Random();

            for (int i = 0; i < size; i++)
            {
                bmp[i] = new float[size][];
                for (int j = 0; j < size; j++) bmp[i][j] = generateRandomPixel(r);

            }

            return new Cell(bmp, x, y);
        }

        private Cell generateCell(Bitmap bmp, int x, int y)
        {
            int h = bmp.Height;
            int w = bmp.Width;

            float[][][] source = new float[h][][];

            for(int i = 0; i < h; i++)
            {
                source[i] = new float[w][];
                for(int j = 0; j < w; j++)
                {
                    source[i][j] = new float[] { bmp.GetPixel(i, j).R, bmp.GetPixel(i, j).G, bmp.GetPixel(i, j).B };
                }
            }

            return new Cell(source, x, y);
        }

        private float[] generateRandomPixel(Random r)
        {
            float R = r.Next(0, 255);
            float G = r.Next(0, 255);
            float B = r.Next(0, 255);

            return new float[] { R, G, B };
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Communication
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public Cell getCellAtPosition(int x, int y)
        {
            return map[y, x];
        }
    }
}
