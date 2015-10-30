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
    class PhiThetaMaximal
    {
        public int Phi;
        public int Theta;

        public PhiThetaMaximal(int phi, int theta)
        {
            Phi = phi;
            Theta = theta;
        }


    }
    class Hough_Operator
    {
        private List<PhiThetaMaximal> phiThetaMaximals;
        private int Rho { get; set; }
        private int[,] HoP_Matrix { get; set; }
        private bool[,] ThresholdMatrix { get; set; }
        private int Scale { get; set; }


        public Hough_Operator(int imageDiagonalSize)
        {
            phiThetaMaximals = new List<PhiThetaMaximal>();
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
            foreach (var kvp in phiThetaMaximals)
            {
                DrawLineOnBitmap(kvp.Phi,kvp.Theta,bitmap);


            }



            bitmap.Save(outfile);
        }

        private void DoThreshold(int localmax, Bitmap bitmap, string filename)
        {

                for (int i = 0; i < Scale; i++)
                {
                    for (int j = 0; j < 180; j++)
                    {
                        if (HoP_Matrix[i, j] >= localmax)
                        {
                            phiThetaMaximals.Add(new PhiThetaMaximal(i - Rho, j));
                        }
                    }
                }
            
        }

        private void DrawLineOnBitmap(int phi, int theta, Bitmap bmp)
        {
            //line given by 
            //phi = x * cos(theta) + y * sin(theta)
            //y = (phi - x * cos(theta))/sin(theta)
            //get where x = 0;
            var radtheta = theta * 180 / Math.PI;
            var s = Math.Sin(radtheta);
            if (s == 0) return;
            var c = Math.Cos(radtheta);
            for (int i = 0; i < bmp.Width; i++)
            {
                var y = Math.Abs((phi - (i * c)) / s);
                if (y <= bmp.Height)
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
                    
                    var radtheta = theta*180/Math.PI;
                    var phi = (int) (i*Math.Cos(radtheta) + j*Math.Sin(radtheta)) + Rho;

                    if (phi < Scale)
                    {
                        HoP_Matrix[phi, theta] ++;
                    }
                    else
                    {
                        
                    }
                }
            }
            return i;
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
