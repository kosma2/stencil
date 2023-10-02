using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;
using System.Xml;

namespace stencil
{
    public class Program
    {
        //public static int granule = 1; //max4
        public static string sourceImage = "C:/Users/User/Pictures/szym1.jpg";
        public static string outputImage = "images/szym1BW321.jpg";
         public static void Main()
        {
            try
            {
                // Retrieve the image.
                System.Drawing.Image img = System.Drawing.Image.FromFile(sourceImage);
                using Bitmap image = new Bitmap(sourceImage, true);
                int trgtHeight = (int)640;
                int trgtWidth = (int)480;

                using Bitmap newImage = new Bitmap(image, trgtWidth, trgtHeight);

                picToPixelBW(newImage);
                deGranB(newImage,3);
                deGranW(newImage,3);
                deGranB(newImage,2);
                deGranW(newImage,2);
                deGranB(newImage,1);
                deGranW(newImage,1);
                newImage.Save(outputImage);
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }
        public static void picToPixelBW(Bitmap img)

        {
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color pixelColor = img.GetPixel(x, y);
                    float brig = pixelColor.GetBrightness();
                    if (brig >= .5) { pixelColor = Color.White; }
                    if (brig < .5) { pixelColor = Color.Black; }
                    img.SetPixel(x, y, pixelColor);
                }
            }
        }
        public static void deGranOld(Bitmap img)
        {
            for (int x = 1; x < img.Width - 2; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color prev = img.GetPixel(x - 1, y);
                    Color pix = img.GetPixel(x, y);
                    Color prox = img.GetPixel(x + 1, y);
                    Color proxx = img.GetPixel(x + 2, y);
                    if (pix == Color.FromArgb(255, 0, 0, 0))
                    {
                        if (prev == Color.FromArgb(255, 255, 255, 255))
                        {
                            if (prox == Color.FromArgb(255, 0, 0, 0))
                            {
                                if (proxx == Color.FromArgb(255, 255, 255, 255))
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x + 1, y, Color.White);
                                }
                            }
                        }
                    }

                }
            }
        }
        public static bool chekAng(Bitmap img, int x, int y)
        {return true;
            if (img.GetPixel(x - 1, y - 1) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x, y - 2) == Color.FromArgb(255, 0, 0, 0))
            {
                if (img.GetPixel(x + 1, y) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x + 1, y + 1) == Color.FromArgb(255, 0, 0, 0))
                {
                   
                   return false;

                    
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }
        public static void deGranB(Bitmap img,int granule)
        {
            for (int x = 1; x < img.Width - 5; x++)
            {
                for (int y = 3; y < img.Height - 5; y++)
                {
                    // black on x axis
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 0, 0, 0))
                    {

                        if (img.GetPixel(x - 1, y) == Color.FromArgb(255, 255, 255, 255))
                        {
                            if (img.GetPixel(x + 1, y) == Color.FromArgb(255, 255, 255, 255))
                            {
                                if (chekAng(img, x, y))//check if there is a sharp angle
                                {
                                    {
                                        img.SetPixel(x, y, Color.White);
                                    }
                                }
                            }

                            if (granule > 1)
                            {
                                if (img.GetPixel(x + 2, y) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    if (chekAng(img, x, y))
                                    {
                                        img.SetPixel(x, y, Color.White);
                                        img.SetPixel(x + 1, y, Color.White);
                                    }
                                }
                            }
                            if (granule > 2)
                            {
                                if (img.GetPixel(x + 3, y) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    if (chekAng(img, x, y))
                                    {
                                        img.SetPixel(x, y, Color.White);
                                        img.SetPixel(x + 1, y, Color.White);
                                        img.SetPixel(x + 2, y, Color.White);
                                    }
                                }
                            }
                            if (granule > 3)
                            {
                                if (img.GetPixel(x + 4, y) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    if (chekAng(img, x, y))
                                    {
                                        img.SetPixel(x, y, Color.White);
                                        img.SetPixel(x + 1, y, Color.White);
                                        img.SetPixel(x + 2, y, Color.White);
                                        img.SetPixel(x + 3, y, Color.White);
                                    }
                                }
                            }
                            if (granule > 4)
                            {
                                if (img.GetPixel(x + 5, y) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x + 1, y, Color.White);
                                    img.SetPixel(x + 2, y, Color.White);
                                    img.SetPixel(x + 3, y, Color.White);
                                    img.SetPixel(x + 4, y, Color.White);
                                }
                            }
                        }
                    }
                    // black on y axis
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        if (img.GetPixel(x, y - 1) == Color.FromArgb(255, 255, 255, 255))
                        {
                            if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 255, 255, 255))
                            {
                                img.SetPixel(x, y, Color.White);
                            }

                            if (granule > 1)
                            {
                                if (img.GetPixel(x, y + 2) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x, y + 1, Color.White);
                                }
                            }
                            if (granule > 2)
                            {
                                if (img.GetPixel(x, y + 3) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x, y + 1, Color.White);
                                    img.SetPixel(x, y + 2, Color.White);
                                }
                            }
                            if (granule > 3)
                            {
                                if (img.GetPixel(x, y + 4) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x, y + 1, Color.White);
                                    img.SetPixel(x, y + 2, Color.White);
                                    img.SetPixel(x, y + 3, Color.White);
                                }
                            }
                            if (granule > 4)
                            {
                                if (img.GetPixel(x, y + 5) == Color.FromArgb(255, 255, 255, 255))
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x, y + 1, Color.White);
                                    img.SetPixel(x, y + 2, Color.White);
                                    img.SetPixel(x, y + 3, Color.White);
                                    img.SetPixel(x, y + 4, Color.White);
                                }
                            }
                        }
                    }


                }
            }
        }
        // degranulate white
        public static void deGranW(Bitmap img, int granule)
        {
            for (int x = 1; x < img.Width - 4; x++)
            {
                for (int y = 1; y < img.Height - 4; y++)
                {
                    //white on x axis
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255))
                    {

                        if (img.GetPixel(x - 1, y) == Color.FromArgb(255, 0, 0, 0))
                        {
                            if (img.GetPixel(x + 1, y) == Color.FromArgb(255, 0, 0, 0))
                            {
                                img.SetPixel(x, y, Color.Black);
                            }

                            if (granule > 1)
                            {
                                if (img.GetPixel(x + 2, y) == Color.FromArgb(255, 0, 0, 0))
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x + 1, y, Color.Black);
                                }
                            }
                            if (granule > 2)
                            {
                                if (img.GetPixel(x + 3, y) == Color.FromArgb(255, 0, 0, 0))
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x + 1, y, Color.Black);
                                    img.SetPixel(x + 2, y, Color.Black);
                                }
                            }
                            if (granule > 3)
                            {
                                if (img.GetPixel(x + 4, y) == Color.FromArgb(255, 0, 0, 0))
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x + 1, y, Color.Black);
                                    img.SetPixel(x + 2, y, Color.Black);
                                    img.SetPixel(x + 3, y, Color.Black);
                                }
                            }
                        }
                        /// white on y axis
                        if (img.GetPixel(x, y - 1) == Color.FromArgb(255, 0, 0, 0))
                        {
                            if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 0, 0, 0))
                            {
                                img.SetPixel(x, y, Color.Black);
                            }

                            if (granule > 1)
                            {
                                if (img.GetPixel(x, y + 2) == Color.FromArgb(255, 0, 0, 0))
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x, y + 1, Color.Black);
                                }
                            }
                            if (granule > 2)
                            {
                                if (img.GetPixel(x, y + 3) == Color.FromArgb(255, 0, 0, 0))
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x, y + 1, Color.Black);
                                    img.SetPixel(x, y + 2, Color.Black);
                                }
                            }
                            if (granule > 3)
                            {
                                if (img.GetPixel(x, y + 4) == Color.FromArgb(255, 0, 0, 0))
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x, y + 1, Color.Black);
                                    img.SetPixel(x, y + 2, Color.Black);
                                    img.SetPixel(x, y + 3, Color.Black);

                                }
                            }
                        }
                    }

                }
            }
        }

       
    }
}