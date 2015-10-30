using System;
using System.Drawing;

namespace IPCV_HW_2_5
{
    public class LoG_Operator
    {

        public int[,] Mask { get; set; }
        private int K { get; set; }
        private int Scale { get; set; }
        private double Sigma { get; set; }
        private int M { get; set; }





        #region Construct





        public LoG_Operator(int m, double sigma)
        {
            M = m;
            int x = -m;
            int y = -m;

            Sigma = sigma;
            Scale = (m * 2) + 1;
            GetK();
            Operate(x, y);
            var total = CheckMask();
            while (total != 0)
            {
                ScaleMask(total);
                total = CheckMask();
            }
            Console.WriteLine(String.Format("Sum of mask coefficients: {0}", total));
            PrintMask();
        }

        private void ScaleMask(int total)
        {
            if (total > 0)
            {
                ReduceMask(total);
            }
            if (total < 0)
            {
                EnhanceMask(total);
            }

        }

        private void EnhanceMask(int total)
        {
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    if (Mask[i, j] < 0)
                    {
                        Mask[i, j] = (int)(Mask[i, j] * .9);
                    }
                }
            }
        }

        private void ReduceMask(int total)
        {
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    if (Mask[i, j] > 0)
                    {
                        Mask[i, j] = (int)(Mask[i, j] * .9);
                    }
                }
            }
        }

        //required to interactively scale results such that they are reasonable
        private void GetK()
        {
            K = 10;
            while (Math.Abs(GetSlidesValue(0, 0)) < (Scale*2))
                K += 10;

        }

        private int CheckMask()
        {

            var total = 0;
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    total += Mask[i, j];
                }
            }
            if (total > 0 && total < 10)
            {
                var center = (Scale - 1) / 2;
                Mask[center, center] -= total;
                return 0;
            }
            else if (total < 0 && total > -10)
            {
                var center = (Scale - 1) / 2;
                Mask[center, center] += total;
                return 0;
            }
            else
            {
                return total;
            }
        }

        private void PrintMask()
        {
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    Console.Write(String.Format("|{0}|", Mask[i, j]));
                }
                Console.WriteLine("");
            }
        }


        private void Operate(int x, int y)
        {
            var xorig = x;
            //x and y represent top left
            Mask = new int[Scale, Scale];
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    SetCell(i, j, x, y);
                    x++;
                }
                y++;
                x = xorig;
            }
        }



        private void SetCell(int i, int j, int xval, int yval)
        {
            var value = GetSlidesValue(xval, yval);
            Mask[i, j] = (int)value;
        }

        private double GetSlidesValue(int xval, int yval)
        {
            int rsquared = (xval * xval) + (yval * yval);
            var firstpart = ((rsquared - (Sigma * Sigma)) / (Sigma * Sigma * Sigma * Sigma));
            var power = -1 * (rsquared / (2 * Sigma * Sigma));
            var secondpart = Math.Pow(Math.E, power);
            var value = K * firstpart * secondpart;
            return value;
        }



        #endregion



        public int[,] Convolve(Bitmap original, int[,] padded)
        {
            int[,] arr = new int[original.Width, original.Height];
            int i = 0, j = 0;
            for (int x = M + 1; x < original.Width + M; x++)
            {
                for (int y = M + 1; y < original.Height + M; y++)
                {
                    arr[i, j] = ConvolveAtPoint(padded, x, y);
                    j++;
                }
                j = 0;
                i++;
            }
            return arr;
        }



        private int ConvolveAtPoint(int[,] padded, int x, int y)
        {

            var value = 0;
            for (int i = 0; i < Scale; i++)
            {
                for (int j = 0; j < Scale; j++)
                {
                    value += padded[x - M + i, y - M + j] * Mask[i, j];




                }
            }



            return value;
        }
    }
}