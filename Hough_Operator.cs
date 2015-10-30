using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCV_HW_2_5
{
    class Hough_Operator
    {

        private int Rho { get; set; }
        private int[,] HoP_Matrix { get; set; }
        private int MaxPhi { get; set; }


        public Hough_Operator(int imageDiagonalSize)
        {
            Rho = imageDiagonalSize;
            MaxPhi = Rho*2 + 1;
            HoP_Matrix = new int[MaxPhi, 180];

        }


        public void Operate(bool[,] edgeImage, int xsize, int ysize)
        {
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    i = CheckEdge(edgeImage, i, j);

                }
            }
            PrintHopMatrix();
        }

        /// <summary>
        /// checks for, and increments appropriately if found, a binary edge pixel at the given coordinates of an edge image
        /// </summary>
        /// <returns></returns>
        private int CheckEdge(bool[,] edgeImage, int i, int j)
        {
            if (edgeImage[i, j])
            {
                for (int theta = 0; theta < 180; i++)
                {
                    var phi = Normalize((int) (i*Math.Cos(i) + j*Math.Sin(i)) + Rho);

                    if (phi < MaxPhi)
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
            if (adjphi <= MaxPhi)
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
        private void PrintHopMatrix()
        {
            for (int i = 0; i < MaxPhi; i++)
            {
                for (int j = 0; j < 180; j++)
                {
                    Console.Write(String.Format("|{0}|", HoP_Matrix[i, j]));
                }
                Console.WriteLine("");
            }
        }
    }
}
