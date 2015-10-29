using System;
using System.Collections.Generic;
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
            HoP_Matrix = new int[imageDiagonalSize, 180];

        }
    }
}
