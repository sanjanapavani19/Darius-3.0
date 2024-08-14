using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using MVStitchingLibrary;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.DragonFruit;
using System.Windows.Forms;

namespace MVStitchintLibrary
{
    /// <summary>
    /// you need to reference these libraries:
    /// MVStitchingLibrary.dll
    ///
    /// nuget packages:
    /// Emgu.CV 4.5.2.
    /// Emgu.CV.Bitmap 4.5.2.
    /// Emgu.CV.runtime.windows 4.5.2. (make sure to set the project properties to build in x64 (release and debug)
    /// NLog 4.7.10
    /// BitMiracle.LibTiff.NET 2.4.639
    /// LinqStatistics 2.3.0
    /// </summary>
    public class StitcherClass
    {
        /// <summary>
        /// Demo application to convert Histolix scan images into seamless whole slide images.
        /// </summary>
        /// <param name="imageOffsetX">Estimated offset between adjacent overlapping source images in pixels in X direction</param>
        /// <param name="imageOffsetY">Estimated offset between adjacent overlapping source images in pixels in Y direction</param>
        /// <param name="maxOffsetError">Maximum error in the estimated offset.</param>
        /// <param name="inputDirectory">Directory of input files. </param>
        /// <param name="outputSvs">Output file path for creating a SVS file</param>
        public ProgressBar P;
        public void Process(ref ProgressBar Pbar,int imageOffsetX, int imageOffsetY, int maxOffsetError, DirectoryInfo inputDirectory = null, string outputSvs = null)
        {
            this.P = Pbar;
            if (imageOffsetX == 0 || imageOffsetY == 0 || maxOffsetError == 0)
            {
                Console.WriteLine("The commandline parameters imageOffsetX, imageOffsetY, and maxOffsetError cannot be 0.\nPress any key to close the application");
                Console.ReadKey();
                return;
            }
            if (inputDirectory == null || outputSvs == null)
            {
                Console.WriteLine("The commandline parameters for input directory and outputSVS cannot be empty.\nPress any key to close the application");
                Console.ReadKey();
                return;
            }

            #region read all images

            var files = inputDirectory.EnumerateFiles().Where(f => f.Extension.Contains("bmp") || f.Extension.Contains("jpg") || f.Extension.Contains("png"));

            Stopwatch sw = Stopwatch.StartNew();
            var imagedatabase = files.Select(f => new Tuple<string, Point>(f.FullName, GetLogicalIndexFromHistolixFullFilePath(f.FullName, 5))).ToList();
            sw.Stop();
            Console.WriteLine("loading images from disk took: " + sw.ElapsedMilliseconds);
            #endregion

            Size rawImageSize = new Image<Bgr, byte>(imagedatabase.First().Item1).Size;

            Rectangle logicalSubRegion = Rectangle.Empty;

            if (logicalSubRegion != Rectangle.Empty)
            {
                imagedatabase = imagedatabase.Where(data => logicalSubRegion.Contains(data.Item2)).ToList();
            }

            bool zerobased = imagedatabase.Any(t => t.Item2.X == 0 || t.Item2.Y == 0);

            Size logicalGridSize = new Size(
                imagedatabase.Max(t => t.Item2.X) + (zerobased ? 1 : 0),
                imagedatabase.Max(t => t.Item2.Y) + (zerobased ? 1 : 0));

            if (logicalSubRegion != Rectangle.Empty) logicalGridSize = logicalSubRegion.Size; //if region is cropped use that logical size

            //load all images to stitcher
            int xoffset = zerobased ? 0 : 1; //default offset to convert from 1-based to zerobased
            int yoffset = zerobased ? 0 : 1; //default offset to convert from 1-based to zerobased

            if (logicalSubRegion != Rectangle.Empty)
            {
                xoffset = logicalSubRegion.X;
                yoffset = logicalSubRegion.Y;
            }

            GridStitcherParameters parameters = new GridStitcherParameters();
            parameters.EstimatedHorizontalTranslation = imageOffsetX;
            parameters.EstimatedVerticalTranslation = imageOffsetY;
            parameters.MaximumErrorInEstimatedTranslations = maxOffsetError;
            parameters.OutputFilePathSVS = outputSvs;
            parameters.ObjectiveMagnification = 10;
            parameters.Resolution = 1;
            parameters.InputImageSize = rawImageSize;
            parameters.LogicalGridSize = logicalGridSize;
            parameters.JpegQualityValue = 90;
            parameters.OutputTileSize = 256;

            IGridStitcher stitcher = GridStitcherFactory.CreateGridStitcher(parameters);


            foreach (var imageItem in imagedatabase)
            {
                stitcher.AddImage(imageItem.Item1, imageItem.Item2.X - xoffset, imageItem.Item2.Y - yoffset);
            }
            //clean up the raw data
            imagedatabase.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //register for progress notifications
            stitcher.OnStitchingProgressChanged += Stitcher_OnStitchingProgressChanged;

            var result = stitcher.ProcessAllImages();
            
            Console.WriteLine("Stitching result: " + result.State);
            Console.WriteLine(result.ResultInformation);

            Console.WriteLine("Press any key to close the application.");
            //Console.ReadKey();
        }

        public void Stitcher_OnStitchingProgressChanged(double progressInPercent, StitchResult result)
        {

            Console.WriteLine("Stitcher reported progress: " + progressInPercent.ToString("0.0") + "%");
            P.Value = (int)progressInPercent;
            bool finished = result != null;
            if (finished)
            {
                Console.WriteLine("Stitching process is finshed.");
                Console.WriteLine("result is: " + result.State);
                Console.WriteLine("result info is: " + result.ResultInformation);
                P.Value = 0;
            }
        }
        public static Point GetLogicalIndexFromHistolixFullFilePath(string filename, int posInfoDigits)
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            //x is the last digits

            string xpos = filename.Substring(filename.Length - posInfoDigits, posInfoDigits);

            string ypos = filename.Substring(filename.Length - 2 * (posInfoDigits + 1), posInfoDigits);
            return new Point(int.Parse(xpos), int.Parse(ypos));
        }
    }
}
