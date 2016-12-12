using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImageMagick;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace Premp
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            foreach (var file in Directory.GetFiles(currentDirectory))
            {
                Console.WriteLine("File: {0}", file);
            }

            MagickNET.SetTempDirectory("../../ImageMagickTemp");
            MagickNET.SetGhostscriptDirectory(Directory.GetCurrentDirectory());
            Console.WriteLine("Temp directory: {0}", Path.GetFullPath("../../ImageMagickTemp"));
            Console.WriteLine("Ghostscript directory: {0}", Directory.GetCurrentDirectory());

            MagickReadSettings settings = new MagickReadSettings();
            settings.Density = new Density(300, 300);

            ImageUtils iu = new ImageUtils();

            //iu.ConvertPdfToImg("test.pdf", "test-Page", settings);

            File.Copy("test-Page1.jpg", "../../SubImageStrideTemp/test-Page1.jpg");

            Mat img = new Mat("../../SubImageStrideTemp/test-Page1.jpg", Emgu.CV.CvEnum.LoadImageType.AnyColor);
            Image<Bgr, Byte> bgrImg = img.ToImage<Bgr, Byte>();
            Image<Gray, Byte> grayImg = bgrImg.Convert<Gray, Byte>();
            int imgWidth = img.Width;
            int imgHeight = img.Height;
            int horizontalSize = 32;
            int verticalSize = 32;
            int horizontalStride = 8;
            int verticalStride = 8;
            int minRegionWidth = img.Width / 3;
            int maxRegionWidth = img.Width / 2;
            int minRegionHeight = img.Height / 24;
            int maxRegionHeight = img.Height / 2;

            //double stdMin = 16.0;
            //double stdMax = 96.0;

            //List<KeyValuePair<CellLocation, Cell>> cells = new List<KeyValuePair<CellLocation, Cell>>();

            var cells = new Cell[(imgHeight - verticalSize) / verticalStride + 1, (imgWidth - horizontalSize) / horizontalStride + 1];

            foreach (var file in Directory.GetFiles("../../SubImageStrideTemp"))
            {
                File.Delete(file);
            }
            File.Delete("../../SubImageStrideTemp/AvgSdvInfo.txt");

            grayImg.Save("../../SubImageStrideTemp/grayImg.jpg");

            var maxPooledImg1 = iu.MaxPooling(grayImg, 2, 2, 2);
            maxPooledImg1.Save("../../SubImageStrideTemp/maxPooledImg1.jpg");

            var avgPooledImg1 = iu.AvgPooling(grayImg, 2, 2, 2);
            avgPooledImg1.Save("../../SubImageStrideTemp/avgPooledImg1.jpg");

            var maxPooledImg2 = iu.MaxPooling(grayImg, 3, 3, 2);
            maxPooledImg1.Save("../../SubImageStrideTemp/maxPooledImg2.jpg");

            var avgPooledImg2 = iu.AvgPooling(grayImg, 3, 3, 2);
            avgPooledImg1.Save("../../SubImageStrideTemp/avgPooledImg2.jpg");

            var maxPooledImg3 = iu.MaxPooling(grayImg, 3, 3, 1);
            maxPooledImg1.Save("../../SubImageStrideTemp/maxPooledImg3.jpg");

            var avgPooledImg3 = iu.AvgPooling(grayImg, 3, 3, 1);
            avgPooledImg1.Save("../../SubImageStrideTemp/avgPooledImg3.jpg");

            var maxavgPooledImg1 = iu.MaxPooling(avgPooledImg1, 2, 2, 2);
            maxavgPooledImg1.Save("../../SubImageStrideTemp/maxavgPooledImg1.jpg");

            var avgmaxPooledImg1 = iu.AvgPooling(maxPooledImg1, 2, 2, 2);
            avgmaxPooledImg1.Save("../../SubImageStrideTemp/avgmaxPooledImg1.jpg");

            var maxavgPooledImg2 = iu.MaxPooling(avgPooledImg2, 3, 3, 2);
            maxavgPooledImg1.Save("../../SubImageStrideTemp/maxavgPooledImg2.jpg");

            var avgmaxPooledImg2 = iu.AvgPooling(maxPooledImg2, 3, 3, 2);
            avgmaxPooledImg1.Save("../../SubImageStrideTemp/avgmaxPooledImg2.jpg");

            var maxavgPooledImg3 = iu.MaxPooling(avgPooledImg3, 3, 3, 2);
            maxavgPooledImg1.Save("../../SubImageStrideTemp/maxavgPooledImg3.jpg");

            var avgmaxPooledImg3 = iu.AvgPooling(maxPooledImg3, 3, 3, 2);
            avgmaxPooledImg1.Save("../../SubImageStrideTemp/avgmaxPooledImg3.jpg");

            //var maxmaxPooledImg1 = iu.MaxPooling(iu.MaxPooling(grayImg, 3, 3, 2), 3, 3, 2);
            //maxmaxPooledImg1.Save("../../SubImageStrideTemp/maxmaxPooledImg1.jpg");

            //var maxmaxPooledImg2 = iu.MaxPooling(iu.MaxPooling(grayImg, 3, 3, 1), 3, 3, 1);
            //maxmaxPooledImg2.Save("../../SubImageStrideTemp/maxmaxPooledImg2.jpg");

            var avgavgPooledImg1 = iu.AvgPooling(iu.AvgPooling(grayImg, 3, 3, 2), 3, 3, 2);
            avgavgPooledImg1.Save("../../SubImageStrideTemp/avgavgPooledImg1.jpg");

            var avgavgPooledImg2 = iu.AvgPooling(iu.AvgPooling(grayImg, 3, 3, 1), 3, 3, 1);
            avgavgPooledImg2.Save("../../SubImageStrideTemp/avgavgPooledImg2.jpg");

            /*
            for (int vs = 0; vs + verticalSize < imgHeight; vs += verticalStride)
            {
                for (int hs = 0; hs + horizontalSize < imgWidth; hs += horizontalStride)
                {
                    Mat subimg = new Mat(img, new System.Drawing.Rectangle(hs, vs, horizontalSize, verticalSize));
                    Image<Bgr, Byte> subimgTemp = subimg.ToImage<Bgr, Byte>();
                    Bgr averageColor = new Bgr();
                    MCvScalar std = new MCvScalar();
                    subimgTemp.AvgSdv(out averageColor, out std);
                    
                    //cells.Add(new KeyValuePair<CellLocation, Cell>(new CellLocation() { x = vs, y = hs }, new Cell() { verticalLocation = vs, horizontalLocation = hs, width = verticalSize, height = horizontalSize });
                    cells[vs / verticalStride, hs / horizontalStride] = new Cell() { verticalLocation = vs, horizontalLocation = hs, width = verticalSize, height = horizontalSize, averageSdv = std, averageColor = averageColor };
                }
            }
            Console.WriteLine("NumberOfChannels: {0}", img.NumberOfChannels);

            Console.WriteLine("Finished collecting cell info.");

            var isSdvInRange = new bool[(imgHeight - verticalSize) / verticalStride + 1, (imgWidth - horizontalSize) / horizontalStride + 1];

            StringBuilder AvgSdvInfoText = new StringBuilder();

            for (int i = 1; i < (imgHeight - verticalSize)/verticalStride - 2; i++)
            {
                for (int j = 1; j < (imgWidth - horizontalSize)/horizontalStride - 2; j++)
                {
                    var cell = cells[i, j];
                    var cell_u1 = cells[i - 1, j];
                    var cell_d1 = cells[i + 1, j];
                    var cell_l1 = cells[i, j - 1];
                    var cell_r1 = cells[i, j + 1];
                    var std = cell.averageSdv;
                    //var subImg = cell.cellImage;
                    var vs = cell.verticalLocation;
                    var hs = cell.horizontalLocation;
                    var averageColor = cell.averageColor;
                    var isSelected = false;
                    if (((std.V0 < stdMax && std.V0 > stdMin) && (std.V1 < stdMax && std.V1 > stdMin) && (std.V2 < stdMax && std.V2 > stdMin))
                        //&& ((cell_u1.averageSdv.V0 < stdMax && cell_u1.averageSdv.V0 > stdMin) && (cell_u1.averageSdv.V1 < stdMax && cell_u1.averageSdv.V1 > stdMin) && (cell_u1.averageSdv.V2 < stdMax && cell_u1.averageSdv.V2 > stdMin))
                        //&& ((cell_d1.averageSdv.V0 < stdMax && cell_d1.averageSdv.V0 > stdMin) && (cell_d1.averageSdv.V1 < stdMax && cell_d1.averageSdv.V1 > stdMin) && (cell_d1.averageSdv.V2 < stdMax && cell_d1.averageSdv.V2 > stdMin))
                        && ((cell_l1.averageSdv.V0 < stdMax && cell_l1.averageSdv.V0 > stdMin) && (cell_l1.averageSdv.V1 < stdMax && cell_l1.averageSdv.V1 > stdMin) && (cell_l1.averageSdv.V2 < stdMax && cell_l1.averageSdv.V2 > stdMin))
                        && ((cell_r1.averageSdv.V0 < stdMax && cell_r1.averageSdv.V0 > stdMin) && (cell_r1.averageSdv.V1 < stdMax && cell_r1.averageSdv.V1 > stdMin) && (cell_r1.averageSdv.V2 < stdMax && cell_r1.averageSdv.V2 > stdMin))
                        && ((Math.Abs(std.V0 - cell_l1.averageSdv.V0) > 0.1 && Math.Abs(std.V0 - cell_r1.averageSdv.V0) > 0.1 && Math.Abs(cell_l1.averageSdv.V0 - cell_r1.averageSdv.V0) > 0.1) && (Math.Abs(std.V1 - cell_l1.averageSdv.V1) > 0.1 && Math.Abs(std.V1 - cell_r1.averageSdv.V1) > 0.1 && Math.Abs(cell_l1.averageSdv.V1 - cell_r1.averageSdv.V1) > 0.1) && (Math.Abs(std.V2 - cell_l1.averageSdv.V2) > 0.1 && Math.Abs(std.V2 - cell_r1.averageSdv.V2) > 0.1 && Math.Abs(cell_l1.averageSdv.V2 - cell_r1.averageSdv.V2) > 0.1)))
                    {
                        //subImg.Save("../../SubImageStrideTemp/subimgTemp-" + vs.ToString() + "-" + hs.ToString() + ".jpg");
                        //Console.WriteLine("Sub image with vertical pixel from {0} to {1}, horizontal pixel from {2} to {3} saved.", vs, vs + verticalSize, hs, hs + horizontalSize);
                        //Console.WriteLine("Sub image with vertical pixel from {0} to {1}, horizontal pixel from {2} to {3} selected.", vs, vs + verticalSize, hs, hs + horizontalSize);
                        isSelected = true;
                        //bgrImg.Draw(new System.Drawing.Rectangle(hs, vs, verticalStride, horizontalStride), new Bgr(255, 0, 0));
                    }
                    else
                    {
                        //Console.WriteLine("Sub image with vertical pixel from {0} to {1}, horizontal pixel from {2} to {3} NOT selected.", vs, vs + verticalSize, hs, hs + horizontalSize);
                    }
                    var isSelectedString = "[X]";
                    if (isSelected)
                    {
                        isSelectedString = "[O]";
                        isSdvInRange[i, j] = true;
                    }
                    else
                    {
                        isSdvInRange[i, j] = false;
                    }
                    //File.AppendAllText("../../SubImageStrideTemp/AvgSdvInfo.txt", "[" + vs.ToString() + " - " + (vs + verticalSize).ToString() + ", " + hs.ToString() + "-" + (hs + horizontalSize).ToString() + "] = Average Color: " + averageColor.ToString() + ", Standard Deviation[V0, V1, V2, V3]: [" + std.V0.ToString() + ", " + std.V1.ToString() + ", " + std.V2.ToString() + ", " + std.V3.ToString() + "]" + ", " + isSelectedString + System.Environment.NewLine);
                    AvgSdvInfoText.AppendFormat("[{0} - {1}, {2} - {3}] = Average Color: {4}, Standard Deviation[V0, V1, V2, V3]: [{5}, {6}, {7}, {8}] - [{9}]\n", vs, (vs + verticalSize), hs, (hs + horizontalSize), averageColor, std.V0, std.V1, std.V2, std.V3, isSelectedString);
                }
                Console.WriteLine("Sub images with vertical pixel from {0} to {1} finished.", cells[i, 0].verticalLocation, cells[i, 0].verticalLocation + verticalSize);
            }
            //new MatrixUtils().PrintMatrix<bool>(isSdvInRange, "../../SubImageStrideTemp/SdvInfo.txt");
            new MatrixUtils().PrintMatrix(isSdvInRange, "../../SubImageStrideTemp/SdvInfo.txt");
            File.AppendAllText("../../SubImageStrideTemp/AvgSdvInfo.txt", AvgSdvInfoText.ToString());

            int avgPoolingStride = 3;
            int avgPoolingWidth = 3;
            int avgPoolingHeight = 3;
            double avgPoolingThreshold = 0.3;

            double[,] avgPooledMatrix = new double[(imgHeight - verticalSize) / (verticalStride * avgPoolingStride), (imgWidth - horizontalSize) / (horizontalStride * avgPoolingStride)];

            for (int i = 1; i < (imgHeight - verticalSize)/verticalStride - 2; i = i + avgPoolingStride)
            {
                for (int j = 1; j < (imgWidth - horizontalSize)/horizontalStride - 2; j = j + avgPoolingStride)
                {
                    var centerCell = cells[i, j];
                    var avg = 0.0;
                    for (int poolingRow = -1; poolingRow < 2; poolingRow++)
                    {
                        for (int poolingColumn = -1; poolingColumn < 2; poolingColumn++)
                        {
                            if (isSdvInRange[poolingRow + i, poolingColumn + j])
                            {
                                avg += 1.0;
                            }
                        }
                    }
                    avg = avg / (avgPoolingWidth * avgPoolingHeight);
                    avgPooledMatrix[(i - 1)/avgPoolingStride, (j - 1)/avgPoolingStride] = avg;
                    if (avg > avgPoolingThreshold)
                    {
                        bgrImg.Draw(new System.Drawing.Rectangle(centerCell.horizontalLocation - (1 * horizontalStride), centerCell.verticalLocation - (1 * verticalStride), avgPoolingWidth * horizontalStride, avgPoolingHeight * verticalStride), new Bgr(0, 255, 0), 5);
                    }
                }
            }

            int avgPooling2Stride = 1;
            int avgPooling2Width = 3;
            int avgPooling2Height = 3;
            double avgPooling2Threshold = avgPoolingThreshold*0.5;
            int avgPooledMatrixHeight = avgPooledMatrix.GetLength(0);
            int avgPooledMatrixWidth = avgPooledMatrix.GetLength(1);

            double[,] avgPooled2Matrix = new double[avgPooledMatrix.GetLength(0) / avgPooling2Stride - 1, avgPooledMatrix.GetLength(1) / avgPooling2Stride - 1];

            for (int i = (avgPooling2Height / 2); i < (avgPooledMatrixHeight - avgPooling2Height) - ((avgPooling2Height / 2) + 0); i = i + avgPooling2Stride)
            {
                for (int j = (avgPooling2Height / 2); j < (avgPooledMatrixWidth - avgPooling2Width) - ((avgPooling2Height / 2) + 0); j = j + avgPooling2Stride)
                {
                    var avg = 0.0;
                    var centerCell = cells[i * avgPoolingWidth + 1, j * avgPoolingHeight + 1];
                    for (int poolingRow = -(avgPooling2Height/2); poolingRow < (avgPooling2Height / 2) + 1; poolingRow++)
                    {
                        for (int poolingColumn = -(avgPooling2Height / 2); poolingColumn < (avgPooling2Height / 2) + 1; poolingColumn++)
                        {
                            avg += avgPooledMatrix[poolingRow + i, poolingColumn + j];
                        }
                    }
                    avg = avg / (avgPooling2Width * avgPooling2Height);
                    avgPooled2Matrix[(i - (avgPooling2Height / 2))/avgPooling2Stride, (j - (avgPooling2Height / 2))/avgPooling2Stride] = avg;
                    if (avg > avgPooling2Threshold)
                    {
                        bgrImg.Draw(new System.Drawing.Rectangle(centerCell.horizontalLocation - ((2 * avgPoolingWidth + 1) * horizontalStride), centerCell.verticalLocation - ((2 * avgPoolingHeight + 1) * verticalStride), avgPoolingWidth * avgPooling2Width * horizontalStride, avgPoolingHeight * avgPooling2Height * verticalStride), new Bgr(0, 0, 255), 5);
                        bgrImg.Draw(avg.ToString("0.000"), new System.Drawing.Point(centerCell.horizontalLocation, centerCell.verticalLocation), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Bgr(0, 0, 0));
                    }
                    else
                    {
                        bgrImg.Draw(avg.ToString("0.000"), new System.Drawing.Point(centerCell.horizontalLocation, centerCell.verticalLocation), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, new Bgr(0, 0, 0));
                    }
                }
            }

            bgrImg.Save("../../SubImageStrideTemp/test-Page1-Rectangle.jpg");
            */
        }
        
    }

    public class Cell
    {
        public int verticalLocation { get; set; }

        public int horizontalLocation { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public MCvScalar averageSdv { get; set; }

        //public Image<Bgr, Byte> cellImage { get; set; }

        public Bgr averageColor { get; set; }
    }

    public class CellLocation
    {
        public int x { get; set; }

        public int y { get; set; }
    }

    public class MatrixUtils
    {
        public void PrintMatrix<T>(T[,] mat, string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var rowCnt = mat.GetLength(0);
            var columnCnt = mat.GetLength(1);
            var resultString = "";
            for (int i = 0; i < rowCnt; i++)
            {
                for (int j = 0; j < columnCnt; j++)
                {
                    resultString += mat[i, j].ToString();
                    if (j < columnCnt - 1)
                    {
                        resultString += ", ";
                    }
                }
                resultString += System.Environment.NewLine;
            }
            File.WriteAllText(fileName, resultString);
        }

        public void PrintMatrix(bool[, ] mat, string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var rowCnt = mat.GetLength(0);
            var columnCnt = mat.GetLength(1);
            StringBuilder resultString = new StringBuilder();
            for (int i = 0; i < rowCnt; i++)
            {
                for (int j = 0; j < columnCnt; j++)
                {
                    if (mat[i, j])
                    {
                        resultString.AppendFormat("1");
                    }
                    else
                    {
                        resultString.AppendFormat("0");
                    }
                    if (j < columnCnt - 1)
                    {
                        resultString.AppendFormat(", ");
                    }
                }
                resultString.AppendFormat(System.Environment.NewLine);
            }
            File.WriteAllText(fileName, resultString.ToString());
        }
    }
    
    public class ImageUtils
    {
        public void ConvertPdfToImg(string pdfName, string imgPrefix, MagickReadSettings settings)
        {
            MagickImageCollection images = new MagickImageCollection();
            Console.WriteLine("Started reading image.");
            images.Read(pdfName, settings);
            Console.WriteLine("Read finish.");

            int page = 1;
            foreach (MagickImage image in images)
            {
                image.Format = MagickFormat.Jpg;
                Console.WriteLine("Started writing page {0}", page);
                image.Write(imgPrefix + page + ".jpg");
                Console.WriteLine("Finished writing page {0}", page);
                page++;
            }
        }

        /// <summary>
        /// Operate max pooling on given gray scale image.
        /// </summary>
        /// <param name="origin">Original image to perform max pooling.</param>
        /// <param name="filterWidth">Width of max pool filter.</param>
        /// <param name="filterHeight">Height of max pool filter.</param>
        /// <param name="stride">Stride of max pooling.</param>
        /// <returns>Result of max-pooling(Gray scale image).</returns>
        public Image<Gray, Byte> MaxPooling(Image<Gray, Byte> origin, int filterWidth, int filterHeight, int stride)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            int originImageWidth = origin.Width;
            int originImageHeight = origin.Height;
            int outputImageWidth = (origin.Width - filterWidth) / stride + 1;
            int outputImageHeight = (origin.Height - filterHeight) / stride + 1;

            Image<Gray, Byte> output = new Image<Gray, Byte>(new System.Drawing.Size(outputImageWidth, outputImageHeight));

            byte[,,] originData = origin.Data;
            byte[,,] outputData = output.Data;

            int outputVerticalLocation = 0;
            int outputHorizontalLocation = 0;

            for (int vr = 0; vr < originImageHeight - stride - 1; vr += stride, outputVerticalLocation++)
            {
                for (int hr = 0; hr < originImageWidth - stride - 1; hr += stride, outputHorizontalLocation++)
                {
                    byte maxValue = 0;
                    for (int i = 0; i < filterHeight; i++)
                    {
                        for (int j = 0; j < filterWidth; j++)
                        {
                            byte currentPixel = originData[vr + i, hr + j, 0];
                            if (currentPixel > maxValue)
                            {
                                maxValue = currentPixel;
                            }
                        }
                    }
                    outputData[outputVerticalLocation, outputHorizontalLocation, 0] = maxValue;
                }
                outputHorizontalLocation = 0;
            }
            outputVerticalLocation = 0;

            output.Data = outputData;

            sw.Stop();

            Console.WriteLine("Time elapsed for max-pooling gray scale image of {0}x{1} to {2}x{3} is: {4} ms", originImageWidth, originImageHeight, outputImageWidth, outputImageHeight, sw.Elapsed.TotalMilliseconds);

            return output;
        }

        public Image<Gray, Byte> AvgPooling(Image<Gray, Byte> origin, int filterWidth, int filterHeight, int stride)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            int originImageWidth = origin.Width;
            int originImageHeight = origin.Height;
            int outputImageWidth = (origin.Width - filterWidth) / stride + 1;
            int outputImageHeight = (origin.Height - filterHeight) / stride + 1;

            Image<Gray, Byte> output = new Image<Gray, Byte>(new System.Drawing.Size(outputImageWidth, outputImageHeight));

            byte[,,] originData = origin.Data;
            byte[,,] outputData = output.Data;

            int outputVerticalLocation = 0;
            int outputHorizontalLocation = 0;

            for (int vr = 0; vr < originImageHeight - stride - 1; vr += stride, outputVerticalLocation++)
            {
                for (int hr = 0; hr < originImageWidth - stride - 1; hr += stride, outputHorizontalLocation++)
                {
                    int cumulatedValue = 0;
                    for (int i = 0; i < filterHeight; i++)
                    {
                        for (int j = 0; j < filterWidth; j++)
                        {
                            byte currentPixel = originData[vr + i, hr + j, 0];
                            cumulatedValue += currentPixel;
                        }
                    }
                    cumulatedValue = cumulatedValue / (filterWidth * filterHeight);
                    outputData[outputVerticalLocation, outputHorizontalLocation, 0] = (byte)cumulatedValue;
                }
                outputHorizontalLocation = 0;
            }
            outputVerticalLocation = 0;

            output.Data = outputData;

            sw.Stop();

            Console.WriteLine("Time elapsed for average-pooling gray scale image of {0}x{1} to {2}x{3} is: {4} ms", originImageWidth, originImageHeight, outputImageWidth, outputImageHeight, sw.Elapsed.TotalMilliseconds);

            return output;
        }
    }
}
