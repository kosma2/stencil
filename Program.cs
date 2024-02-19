using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace stencil
{
    public partial class Program
    {
        //public static int granule = 1; //max4
        public static string sourceImage = "C:/Users/User/Pictures/face3.jpg";
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
            // Retrieve the image.
            System.Drawing.Image img = System.Drawing.Image.FromFile(sourceImage);
            using Bitmap image = new Bitmap(sourceImage, true);
            int trgtHeight = (int)480;
            int trgtWidth = (int)640;
            //outputImg = new Bitmap( trgtWidth, trgtHeight);

            Bitmap bwImage = new Bitmap(image, trgtWidth, trgtHeight);
            //workImage = ConvertToBlackAndWhite(bwImage, 100);
            picToPixelLockB(workImage); // convert to black and white

            // deGranulize
            deGran(workImage, 3);
            deGran(workImage, 3, false);
            deGran(workImage, 2);
            deGran(workImage, 2, false);
            //deGran(workImage, 1);
            //deGran(workImage, 1,false);
            //OutlineShapes();
            //PointstartPt = FindShapeStart();
            //var pointsForShaping = ShapePointList(FindShapeStart());
            //CreateShape(ListToCurveArray(pointsForShaping));
            CreateShapes();

            //outputImg.Save(outputImage);

            //using Bitmap image1 = new Bitmap(sourceImage, true);
            //Bitmap originalImage = new Bitmap(image1,trgtWidth, trgtHeight);



//Bitmap bwImage2 = ConvertToBlackAndWhite(originalImage, 150);
//Bitmap bwImage3 = ConvertToBlackAndWhite(originalImage, 200);
// Save or display your images
workImage.Save("bw_image_100.jpg");
//bwImage2.Save("bw_image_150.jpg");
//bwImage3.Save("bw_image_200.jpg");


        }




    }
}