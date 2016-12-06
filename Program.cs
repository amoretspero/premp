﻿using System;
using System.Collections.Generic;
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
            settings.Density = new Density(600, 600);

            /*using (MagickImageCollection images = new MagickImageCollection())
            {
                Console.WriteLine("Started reading image.");
                images.Read("test.pdf", settings);
                Console.WriteLine("Read finish.");

                int page = 1;
                foreach (MagickImage image in images)
                {
                    image.Format = MagickFormat.Jpg;
                    Console.WriteLine("Started writing page {0}", page);
                    image.Write("test-Page" + page + ".jpg");
                    Console.WriteLine("Finished writing page {0}", page);
                    page++;
                }
            }*/
            Mat img = new Mat("test-Page1.jpg", Emgu.CV.CvEnum.LoadImageType.AnyColor);
            int imgWidth = img.Width;
            int imgHeight = img.Height;
            int horizontalSize = 256;
            int verticalSize = 256;
            int horizontalStride = 128;
            int verticalStride = 64;
            int minRegionWidth = img.Width / 3;
            int maxRegionWidth = img.Width / 2;
            int minRegionHeight = img.Height / 24;
            int maxRegionHeight = img.Height / 2;

            double stdMin = 16.0;
            double stdMax = 72.0;

            //List<KeyValuePair<CellLocation, Cell>> cells = new List<KeyValuePair<CellLocation, Cell>>();

            var cells = new Cell[(imgHeight - verticalSize) / verticalStride + 1, (imgWidth - horizontalSize) / horizontalStride + 1];

            foreach (var file in Directory.GetFiles("../../SubImageStrideTemp"))
            {
                File.Delete(file);
            }
            File.Delete("../../SubImageStrideTemp/AvgSdvInfo.txt");
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
                    cells[vs / verticalStride, hs / horizontalStride] = new Cell() { verticalLocation = vs, horizontalLocation = hs, width = verticalSize, height = horizontalSize, averageSdv = std, cellImage = subimgTemp, averageColor = averageColor };
                }
            }
            Console.WriteLine("NumberOfChannels: {0}", img.NumberOfChannels);

            Console.WriteLine("Finished collecting cell info.");

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
                    var subImg = cell.cellImage;
                    var vs = cell.verticalLocation;
                    var hs = cell.horizontalLocation;
                    var averageColor = cell.averageColor;
                    var isSelected = false;
                    if (((std.V0 < stdMax && std.V0 > stdMin) && (std.V1 < stdMax && std.V1 > stdMin) && (std.V2 < stdMax && std.V2 > stdMin))
                        /*&& ((cell_u1.averageSdv.V0 < stdMax && cell_u1.averageSdv.V0 > stdMin) && (cell_u1.averageSdv.V1 < stdMax && cell_u1.averageSdv.V1 > stdMin) && (cell_u1.averageSdv.V2 < stdMax && cell_u1.averageSdv.V2 > stdMin))
                        && ((cell_d1.averageSdv.V0 < stdMax && cell_d1.averageSdv.V0 > stdMin) && (cell_d1.averageSdv.V1 < stdMax && cell_d1.averageSdv.V1 > stdMin) && (cell_d1.averageSdv.V2 < stdMax && cell_d1.averageSdv.V2 > stdMin))*/
                        && ((cell_l1.averageSdv.V0 < stdMax && cell_l1.averageSdv.V0 > stdMin) && (cell_l1.averageSdv.V1 < stdMax && cell_l1.averageSdv.V1 > stdMin) && (cell_l1.averageSdv.V2 < stdMax && cell_l1.averageSdv.V2 > stdMin))
                        && ((cell_r1.averageSdv.V0 < stdMax && cell_r1.averageSdv.V0 > stdMin) && (cell_r1.averageSdv.V1 < stdMax && cell_r1.averageSdv.V1 > stdMin) && (cell_r1.averageSdv.V2 < stdMax && cell_r1.averageSdv.V2 > stdMin))
                        && ((Math.Abs(std.V0 - cell_l1.averageSdv.V0) > 0.1 && Math.Abs(std.V0 - cell_r1.averageSdv.V0) > 0.1 && Math.Abs(cell_l1.averageSdv.V0 - cell_r1.averageSdv.V0) > 0.1) && (Math.Abs(std.V1 - cell_l1.averageSdv.V1) > 0.1 && Math.Abs(std.V1 - cell_r1.averageSdv.V1) > 0.1 && Math.Abs(cell_l1.averageSdv.V1 - cell_r1.averageSdv.V1) > 0.1) && (Math.Abs(std.V2 - cell_l1.averageSdv.V2) > 0.1 && Math.Abs(std.V2 - cell_r1.averageSdv.V2) > 0.1 && Math.Abs(cell_l1.averageSdv.V2 - cell_r1.averageSdv.V2) > 0.1)))
                    {
                        subImg.Save("../../SubImageStrideTemp/subimgTemp-" + vs.ToString() + "-" + hs.ToString() + ".jpg");
                        Console.WriteLine("Sub image with vertical pixel from {0} to {1}, horizontal pixel from {2} to {3} saved.", vs, vs + verticalSize, hs, hs + horizontalSize);
                        isSelected = true;
                        
                    }
                    var isSelectedString = "[X]";
                    if (isSelected) { isSelectedString = "[O]"; }
                    File.AppendAllText("../../SubImageStrideTemp/AvgSdvInfo.txt", "[" + vs.ToString() + " - " + (vs + verticalSize).ToString() + ", " + hs.ToString() + "-" + (hs + horizontalSize).ToString() + "] = Average Color: " + averageColor.ToString() + ", Standard Deviation[V0, V1, V2, V3]: [" + std.V0.ToString() + ", " + std.V1.ToString() + ", " + std.V2.ToString() + ", " + std.V3.ToString() + "]" + ", " + isSelectedString + System.Environment.NewLine);
                }
            }

        }
    }

    public class Cell
    {
        public int verticalLocation { get; set; }

        public int horizontalLocation { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public MCvScalar averageSdv { get; set; }

        public Image<Bgr, Byte> cellImage { get; set; }

        public Bgr averageColor { get; set; }
    }

    public class CellLocation
    {
        public int x { get; set; }

        public int y { get; set; }
    }
}
