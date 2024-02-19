using System.Drawing;
using System.Drawing.Drawing2D;

namespace stencil
{
    partial class Program
    {

        public static void deGran(Bitmap img, int granule, bool fore = true)// bool reverses foreground and background colors
        {
            Color tempForeCol = foreCol;
            Color tempBakCol = bakCol;
            if (!fore)
            {
                tempForeCol = bakCol;
                tempBakCol = foreCol;
            }
            for (int x = 1; x < img.Width - 1; x++)//1,-5
            {
                for (int y = 1; y < img.Height - 1; y++)//3,-5
                {
                    Point testPt = new(x,y);
                    if (img.GetPixel(x, y) == tempForeCol && img.GetPixel(x - 1, y) == tempBakCol)
                    {
                        for (int i = 0; i <= granule; i++)
                        {
                            if (x + i < img.Width && img.GetPixel(x + i, y) == tempBakCol)
                            {
                                for (int j = 0; j <= i; j++)
                                {
                                    img.SetPixel(x + j, y, tempBakCol);
                                }
                            }
                        }
                    }

                    if (img.GetPixel(x, y) == tempBakCol && img.GetPixel(x, y - 1) == tempBakCol)
                    {
                        for (int i = 0; i <= granule; i++)
                        {
                            if (y + i < img.Height && img.GetPixel(x, y + i) == tempBakCol)
                            {
                                for (int j = 0; j <= i; j++)
                                {
                                    img.SetPixel(x, y + j, tempBakCol);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}