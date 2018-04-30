# HP_petri


© Copyright 2018 HP Inc.

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
   THE SOFTWARE.


APPLICATION DESCRIPTION:

VisualStudio based application designed to, using HP Sprout's functionalities, be able to capture overtime growing cultures
and perform basic counting and classification analysis over them.


Static parameters used during the application (that can be modified through the Advanced Settings option): 

	
	  Use :               ConnectedComponents, fnAreSimilar
          Definition :        Static grey color value threshold below which two pixels are considered to belong to the same object 
          Recommended value : 60 +- 20;
        

        _nSimilarityTolerance = 60;

          Use :               ConnectedComponents, fnMustInclude
          Definition :        Static grey color value threshold below which a pixel is not considered to be meaningful
          Recommended value : 80 +- 30;
        

        _nRelevanceThreshold = 100;

          Use :               CountAnalysis, FindObjects
          Definition :        Region Of Interest definition in which blobs will be tracked
          Recommended value : According to the receip's size so as to avoid it's borders to be considered
        

        _nROIMargin = 120;

          Use :               Matching-Robinson, findRobinsonReposition
          Definition :        Maximun and Minimum expected pixel deplacements according to the Sprout's images usual missmatches 
          Recommended value : Min: 3 +-2   Max: 5+-2    We strongly recommend not to set large values due to execution efficiente purposes
        

        _nMinReposition = 3, _nMaxReposition = 5;

          Use :               DifferenceComputation, GetGlobalCorrelation
          Definition :        Minimum value for variances to be set if they happen to be smaller or equal to zero
          Recommended value : 1.0e-9, or some other close to zero number
 

        _dEpsilonValue = 1.0e-9;

          Use :               DifferenceComputation, getGreyThreshold getThreshold
          Definition :        Minimum value below which a pixel is not considered meaningful
          Recommended value : 200 +-50
         

        _nThresholdValue = 200;

          Use :               DifferenceComputation, getHystereris
          Definition :        Top and bottom values of the user-defined hysteresis cycle
          Recommended value : Top: 220 +-20  Bottom 180 +- 20
        

        _nTopHysteresis = 220, _nBottomHysteresis = 180;

          Use :               Tacking, blobCompare
          Definition :        Thresholds for bounds and center deplacement as well as their difference
          Recommended value : Bounds diminish : -0.2 +-0.1   Great deplacement : 0.3 +- 0.1
         

        _dBoundsDiminish = -0.2, _dGreatDeplacement = 0.3, _dAbnormalGrowth = 0.5;

          Use :               Tacking, Merged
          Definition :        Tolerance to merging predicted size and center difference
          Recommended value : Tolerance : 0.3 +- 0.1 Distance : 30 +- 10  
         

        _dMergingTolerance = 0.3, _nMergingDistance = 40;

          Use :               ConnectedComponents
          Definition :        Minimum size for a cluster to be considered
          Recommended value : 40 +- 10
         

        _nMinimumSize = Kohonen.AdvancedOptions._nBitmapSize;

          Use :               Tracking
          Definition :        ColorDifference of two colonies to be considered of not the same type
          Recommended value : 0.3 +- 0.1
         

        _dMaxColorDiff = 0.3;

          Use :               Tracking, to be shown in Capture Window
          Definition :        Event messages generated during colony tracking
          Valuee :            Messages can be user-modified, as well as omitted if the event is not considered relevant
                              (in which case, entries should be nullified) 
         

        _sEventMessages = new string[]
            { " Abnormal growth identified ",
             " New colony has appeared ",
             " Merging of two colonies of a different class ",
             " Merging of two colonies of the same class",
             " Colony found to be dangerously close to container's bounds "};
        
          Use :               CountAnalytics
          Definition :        Minimum safe distance between a cluster and the container's borders
          Recommended value : 40 +- 10
         

        _nMinimumDistance = 40;

          Use :              Picture Handling, CompareOutlines
          Definition :        Outline size threshold for two objects to match
          Recommended value : 50 +- 10
        

        _nSizeThreshold = 50;

          Use :              Picture Handling, CompareOutlines
          Definition :        Outline location maximum difference 
          Recommended value : 120 +- 20
       

        _nLocationThreshold = 120;

          Use :               KohonenAlgorithm
          Definition :        Number of iterations the Kohonen network will execute
          Recommended value : Depending on the deepness wanted to obtain from the network
         

        _nNumberOfEpochs = 10;

         Use :               KohonenAlgorithm
         Definition :        Size of the Kohonen map (X, Y)
         Recommended value : Depending on the number of different labeled inputs expected to be obtained
         

        _nKohonenMapSizeX = 10, _nKohonenMapSizeY = 10;

         Use :               KohonenAlgorithm
         Definition :        Size of the bitmap the network will work with
         Recommended value : Depending on the real size of the inputs 
         

        _nBitmapSize = 64;

         Use :               KohonenAlgorithm
         Definition :        Speed to which the network will adapt to changes
         Recommended value : Will depend on the number of epochs, generally chosen so that a full change can be achieved 
         after all iterations are executed
        

        _dInitialLearningFactor = 0.1, _dFinalLearningFactor = 0.001;

         Use :               KohonenAlgorithm
         Definition :         Initial and final radius (in cells) within which the cells will modified after an epoch to match the winner
         Recommended value :  Depending on the deepness the network is designed to have, as well as the expected number of labels, 
         in this case will be initialized as half of the map's size
	 Given that the map is in a toroidal state, /2 is applied to the distance
        
        _nInitialRadius = (int)Math.Sqrt((Math.Pow(_nKohonenMapSizeX/2, 2) + Math.Pow(_nKohonenMapSizeY/2, 2))),  _nFinalRadius = 1;

         Use :               KohonenAlgorithm
         Definition :        Number of epochs until the influence radius, as well as the learning factor, converge
         Recommended value : Will depend on the number of epochs, generally 70% / 80%
        

        _nEpochsUntilConvergence = 80;

         Use :              KohonenAlgorithm
         Definition :        Factor that defines the rate in which the intensity of the modification after the best match in an epoch is found will diminish
                             when getting far from that best cell
         Recommended value : 0.05, will depend on the global impact to the map we want to achieve in an iteration
        
        _dMaxRadiusFactor = 0.05;