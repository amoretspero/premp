using System;
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

namespace PdfConverter
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
            Mat subimg = new Mat(img, new System.Drawing.Rectangle(360, 1536, 2048, 2048));
            Image<Bgr, Byte> subimgTemp = subimg.ToImage<Bgr, Byte>();
            subimgTemp.Save("subimgTemp.jpg");
            Console.WriteLine("NumberOfChannels: {0}", img.NumberOfChannels);
        }
    }
}
