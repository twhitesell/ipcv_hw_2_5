using System;
using System.Drawing;

namespace IPCV_HW_2_5
{
    public class ZeroCrossingOperator
    {




        public ZeroCrossingOperator()
        {

        }

       

        public bool[,] OperateOverArrayWithSize(int[, ] array, int x, int y)
        {

            var bitmap = new Bitmap(x, y);
           // var bmp = BitmapOverArrayWithSize(array, x, y);
            bool[,] binary = new bool[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    bool value = CheckAtPosition(array, i, j, x, y);
                    if (value)
                    {
                        binary[i, j] = true;
                        bitmap.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }
            bitmap.Save("edgeImage.bmp");
            return binary;


        }

        private bool CheckAtPosition(int[,] array, int x, int y, int limitX, int limitY)
        {
            try
            {
                //check up and down
                if (y - 1 >= 0 && y + 1 < limitY)
                {
                    //do it
                    if (array[x, y - 1]*array[x, y + 1] < 0) return true;
                }

                //check left and right
                if (x - 1 >= 0 && x + 1 < limitX)
                {
                    if (array[x - 1, y]*array[x + 1, y] < 0) return true;
                }

                //check topleft and bottom right
                if (x - 1 >= 0 && x + 1 < limitX && y - 1 >= 0 && y + 1 < limitY)
                {
                    //do both
                    if (array[x - 1, y - 1]*array[x + 1, y + 1] < 0) return true;
                    if (array[x + 1, y - 1]*array[x - 1, y + 1] < 0) return true;
                }
                //check topright and bottom left
            }
            catch (Exception e)
            {
                var s = e.Message;
                
            }



            return false;
        }
    }
}