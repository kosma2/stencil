using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace stencil
{
    public partial class Program
    {
        //public static int granule = 1; //max4
        public static string sourceImage = @"C:\Users\User\Pictures\face3.jpg";
        public static string outputImage = "images/face3.jpg";
        public static List<Point> pointTrack = new List<Point>(); // tracks last 3 Points, preventing loops and backtracking
        public static Bitmap workImage;
        public static Bitmap outputImg;
        // set foreground and background colors
        public static Color foreCol = Color.FromArgb(255, 0, 0, 0);
        public static Color bakCol = Color.FromArgb(255, 255, 255, 255);
        // set shape outline color
        public static Color outlineCol = Color.FromArgb(255, 255, 0, 0);

        public static void Main()
        {
            int trgtHeight = 480;
            int trgtWidth = 640;

            Bitmap originalImage = new Bitmap(sourceImage);                 // image from file
            workImage = new Bitmap(originalImage, trgtWidth, trgtHeight);   // bitmap for pixel operations
            outputImg = new Bitmap(trgtWidth, trgtHeight);                 //blank Bitmap for drawing filled shapes


            picToPixelLockB(workImage);                                     // convert to black and white

            // deGranulize
            deGran(workImage, 3);
            deGran(workImage, 3, false);
            deGran(workImage, 2);
            deGran(workImage, 2, false);
            CreateShapes();

            outputImg.Save(outputImage);
        }
    }
}