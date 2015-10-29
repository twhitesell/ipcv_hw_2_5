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


        public Hough_Operator(int imageDiagonalSize)
        {
            Rho = imageDiagonalSize;
            HoP_Matrix = new int[imageDiagonalSize * 2 + 1, 180];

        }

        public void Operate(bool[,] edgeImage, int xsize, int ysize)
        {
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    if (edgeImage[i, j])
                    {

                        for (int theta = 0; theta < 180; i++)
                        {
                            //must offset upwards by a value of rho to get the appropriate location...as the actual values output will be members of a set symmetrical about 0, that is, +- image diagonal
                            var phi = (int)(i * Math.Cos(i) + j * Math.Sin(i)) + Rho;
                            HoP_Matrix[phi,theta] ++;
                        }
                    }

                }

            }
        }

        public int GetValueAt(int phi, int theta)
        {
            var adjphi = phi + Rho;
            if (adjphi <= 2*Rho + 1)
            {
                //ok, it will be in the array somewhere, return it
                return HoP_Matrix[adjphi, theta];
            }
            return 0;
        }

    }
}
