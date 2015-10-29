using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCV_HW_2_5
{
    public class Program
    {
        private static bool continueRun = true;

        private static string inputfile;
        private static string outputfile;
        private static int lowthreshold = 0;
        private static int highthreshold = 255;

        private static Size size;


        /// <summary>
        /// entry point
        /// </summary>
        static void Main(string[] args)
        {
            Write("Welcome to LoG Utility!");

            GetInputFilename();
            GetOutputFilename();

            var continueRun = true;

            while (continueRun)
            {
                ProcessImage();
                continueRun = Reprocess();
            }

            SignalGrandExit();

        }






        #region PROCESS_IMAGE




        /// <summary>
        /// processes the histogram equalization
        /// </summary>
        private static bool ProcessImage()
        {
            try
            {

                // Open an Image file, and get its bitmap
                var currentdir = Directory.GetCurrentDirectory();
                Image myImage = Image.FromFile(inputfile);
                var bitmap = new Bitmap(myImage);
                //Bitmap gray = GetGrayscaleImage(bitmap);
                //create log filter

                var sigma = GetSigma();
                int m = GetM(sigma);

                var op = new LoG_Operator(m, sigma);


                //create image large enough to convolve based on grayscale image
                var paddedArray = PadImageForConvolution(bitmap, m);
                //paddedGray.Save(currentdir + "\\" + "paddedGray.bmp");

                //get convolved image output
                var stuf = op.Convolve(bitmap, paddedArray);


                //do zero crossing filter ==> output bitmap of he same size as myImage
                var output = new ZeroCrossingOperator().OperateOverArrayWithSize(stuf, bitmap.Width, bitmap.Height);

                output.Save(currentdir + "\\" + outputfile);
                Write(String.Format("File: {0} generated ok.", outputfile));
                return true;
            }
            catch (Exception e)
            {
                Write(String.Format("Exception thrown: {0}", e.Message));
                return false;
            }
        }



        /// <summary>
        /// pads image by m pixels all the way around
        /// </summary>
        private static int[,] PadImageForConvolution(Bitmap gray, int m)
        {
            var width = gray.Width + (m * 2);
            var height = gray.Height + (m * 2);
            int[,] arr = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    arr[x, y] = 0;
                }
            }
            int i = 0, j = 0;
            for (int x = m + 1; x < gray.Width + m; x++)
            {
                for (int y = m + 1; y < gray.Height + m; y++)
                {
                    arr[x, y] = GetGrayscale(gray.GetPixel(i, j));
                    j++;
                }
                j = 0;
                i++;


            }

            return arr;
        }





        #endregion







        #region Utility




        /// <summary>
        /// gets a minimal value for m
        /// </summary>
        /// <param name="sigma"></param>
        /// <returns></returns>
        private static int GetM(double sigma)
        {
            int res = (int)(6 * sigma);
            if (res <= 3) return 3;
            if (res % 2 == 0) return res + 1;
            return res;

        }

        /// <summary>
        /// requests & validates user input for sigma
        /// </summary>
        /// <returns></returns>
        private static double GetSigma()
        {
            while (true)
            {

                Write("Enter value of Sigma (0.0 - 5.0):");
                var c = Console.ReadLine();
                double v;
                if (double.TryParse(c, out v))
                {
                    if (v > 0 && v <= 5)
                        return v;
                }

            }
        }



        /// <summary>
        /// determines whether or not we continue
        /// </summary>
        private static bool Reprocess()
        {
            Write("");
            Write("Enter 'Q' to quit, or any other key to continue.");
            var key = Console.ReadKey(false);
            if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                return false;
            return true;
        }




        /// <summary>
        /// gets name of output file
        /// </summary>
        private static void GetOutputFilename()
        {
            Write("Please enter the output file name only:");
            outputfile = Console.ReadLine();
        }






        /// <summary>
        /// gets input filename
        /// </summary>
        private static void GetInputFilename()
        {
            while (true)
            {
                Write("Please enter relative file name:");
                var line = Console.ReadLine();
                if (!File.Exists(line))
                {
                    Write("No such file.");
                    continue;
                }
                inputfile = line;
                break;
            }
        }


        /// <summary>
        /// prints to console
        /// </summary>
        private static void Write(String s)
        {
            Console.WriteLine(s);
        }



        /// <summary>
        /// prints outro
        /// </summary>
        private static void SignalGrandExit()
        {
            int b = 1;
            for (int i = b; b < 10000; i++)
            {
                string s = "";
                for (int a = 0; a < b; a++)
                {
                    s += "*";
                }

                b *= 2;
                Write(s);
            }

        }





        #endregion







        #region GRAYSCALE




        /// <summary>
        /// counts occurrences of each possible intensity
        /// </summary>
        private static Bitmap GetGrayscaleImage(Bitmap colorImage)
        {

            var bmp = new Bitmap(colorImage.Width, colorImage.Height);
            //size = colorImage.Size;

            //index across each position in the 2-d image
            for (int x = 0; x < colorImage.Width; x++)
            {
                for (int y = 0; y < colorImage.Height; y++)
                {
                    var pi = colorImage.GetPixel(x, y);
                    SetGrayscaleForPixel(pi, bmp, x, y);
                }
            }
            return bmp;
        }

        /// <summary>
        /// sets 1 pixel to grayscale
        /// </summary>
        private static void SetGrayscaleForPixel(Color pi, Bitmap output, int x, int y)
        {
            int grayscale = GetGrayscale(pi);
            output.SetPixel(x, y, Color.FromArgb(pi.A, grayscale, grayscale, grayscale));
        }



        /// <summary>
        /// here we get the grayscale intensity
        /// </summary>
        private static int GetGrayscale(Color pi)
        {
            // 0.2989, 0.5870, 0.1140.
            return (int)(0.2989 * pi.R + 0.5870 * pi.G + 0.1140 * pi.B);
        }



        #endregion
        







        #region BINARY IMAGE

        /// <summary>
        /// requires the argb color of the pixel, the output image to set into, and the position of each pixel to operate upon
        /// </summary>
        private static void SetBinaryPixel(Color pi, Bitmap output, int x, int y)
        {
            int grayscale = GetGrayscale(pi);
            //if the grayscale value meets some criteria
            if (conditionIsTrue(grayscale))
            {
                //set the corresponding output pixel
                output.SetPixel(x, y, Color.White);
            }
            else
            {
                output.SetPixel(x, y, Color.Black);
            }
        }

        /// <summary>
        /// probably some threshold or zero crossing detection
        /// </summary>
        private static bool conditionIsTrue(int grayscale)
        {
            return grayscale > lowthreshold && grayscale < highthreshold;
        }


        #endregion





    }
}
