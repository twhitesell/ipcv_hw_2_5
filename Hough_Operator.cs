using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge;

namespace IPCV_HW_2_5
{
    class Hough_Operator
    {

        private int Rho { get; set; }
        private int[,] HoP_Matrix { get; set; }
        private bool[,] ThresholdMatrix { get; set; }
        private int Scale { get; set; }


        public Hough_Operator(int imageDiagonalSize)
        {
            Rho = imageDiagonalSize;
            Scale = Rho*2 + 1;
            HoP_Matrix = new int[Scale, 180];
            ThresholdMatrix = new bool[Scale, 180];

        }

        /// <summary>
        /// returns maximum of A(phi,theta)
        /// </summary>
        public int Operate(bool[,] edgeImage, int xsize, int ysize)
        {
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    i = CheckEdge(edgeImage, i, j);
                }
            }
            var max = GetMatrixMaximum();
            return max;
        }

        public void DrawLines(Bitmap bitmap, int localmax, string outfile)
        {
             DoThreshold(localmax,bitmap,outfile);
        }

        private void DoThreshold(int localmax, Bitmap bitmap, string filename)
        {

                for (int i = 0; i < Scale; i++)
                {
                    for (int j = 0; j < 180; j++)
                    {
                        if (HoP_Matrix[i, j] >= localmax)
                        {
                            DrawLineOnBitmap(i, j, bitmap);
                        }

                    }
                }
            bitmap.Save(filename);
        }

        private void DrawLineOnBitmap(int normalized, int theta, Bitmap bmp)
        {
            var phi = normalized - Rho;
            //line given by 
            //phi = x * cos(theta) + y * sin(theta)
            //y = (phi - x * cos(theta))/sin(theta)
            //get where x = 0;
            var s = Math.Sin(theta);
            if (s == 0) return;
            var c = Math.Cos(theta);
            for (int i = 0; i < bmp.Width; i++)
            {
                var y = (phi - i * c) / s;
                if (y <= bmp.Width && y >= 0)
                {
                    bmp.SetPixel(i, (int) y, Color.Red);
                }
            }

        }

       

        /// <summary>
        /// checks for, and increments appropriately if found, a binary edge pixel at the given coordinates of an edge image
        /// </summary>
        /// <returns></returns>
        private int CheckEdge(bool[,] edgeImage, int i, int j)
        {
            if (edgeImage[i, j])
            {
                for (int theta = 0; theta < 180; theta++)
                {
                    var phi = Normalize((int) (i*Math.Cos(theta) + j*Math.Sin(theta)) + Rho);

                    if (phi < Scale)
                    {
                        HoP_Matrix[phi, theta] ++;
                    }
                }
            }
            return i;
        }

        /// <summary>
        /// gets transform intensity at A(phi,theta)
        /// </summary>
        public int GetValueAt(int phi, int theta)
        {
            var adjphi = Normalize(phi);
            if (adjphi <= Scale)
            {
                //ok, it will be in the array somewhere, return it
                return HoP_Matrix[adjphi, theta];
            }
            return 0;
        }

        //must offset upwards by a value of rho to get the appropriate location...as the actual values output will be members of a set symmetrical about 0, that is, +- image diagonal
        private int Normalize(int phi)
        {
            return phi + Rho;
        }


        /// <summary>
        /// trace
        /// </summary>
        private int GetMatrixMaximum()
        {
            var max = 0;
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < 180; j++)
                {
                    if (HoP_Matrix[i, j] > max)
                    {
                        max = HoP_Matrix[i, j];
                    }//Console.Write(String.Format("|{0}|", HoP_Matrix[i, j]));
                }
                //Console.WriteLine("");
            }
            return max;
        }

      
    }
}
