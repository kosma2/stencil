using System;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Common;
using System.Drawing.Imaging;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Xml;

namespace stencil
{
    public class Program
    {
        //public static int granule = 1; //max4
        public static string sourceImage = "C:/Users/User/Pictures/face3.jpg";
        public static string outputImage = "images/face3.jpg";
        public static List<Point> ptTrack = new List<Point>(); // for keeping track a trail of 4 points to detect loops
        public static Bitmap workImage;
        public static Color foreCol = Color.FromArgb(255, 0, 0, 0);
        public static Color bakCol = Color.FromArgb(255, 255, 255, 255);

        public static void Main()
        {
            try
            {
                // Retrieve the image.
                System.Drawing.Image img = System.Drawing.Image.FromFile(sourceImage);
                using Bitmap image = new Bitmap(sourceImage, true);
                int trgtHeight = (int)480;
                int trgtWidth = (int)640;
                int lgHeight = 768;
                int lgWidth = 1024;

                workImage = new Bitmap(image, trgtWidth, trgtHeight);

                picToPixelBW(workImage);
                deGranB(workImage, 3);
                deGranW(workImage, 3);
                deGranB(workImage, 2);
                deGranW(workImage, 2);
                deGranB(workImage, 1);
                deGranW(workImage, 1);
                //corFilW(newImage);
                //corFilB(newImage);*/
                outlineStuff();

                workImage.Save(outputImage);
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }
        public static void outlineStuff()
        {
            /*for (int g = 0; g < 4; g++)
            {
                Point pt = new Point(g, g);
                ptTrack.Add(pt);
                //System.Console.WriteLine(ptTrack.Count);
            }*/
            Pen pn = new Pen(Color.Red);
            FillMode newFillMode = FillMode.Winding;
            float tension = .2F;

            using Graphics e = Graphics.FromImage(workImage);
            //e.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //Point[] outLine = LisToPoints(letsTryThis());
           // e.DrawClosedCurve(pn, outLine, 0.2F, FillMode.Winding);
            //HatchBrush hBrush = new HatchBrush(HatchStyle.DiagonalCross,Color.Black,Color.White);
            drawOutline(letsTryThis());
        }
        public static Point[] LisToPoints(List<Point> pList) // converts a List of Points into Array of Points
        {
            Point[] pArray = new Point[pList.Count];
            for (int i = 0; i < pList.Count; i++)
            {
                pArray[i] = new Point(pList[i].X, pList[index: i].Y);
            }
            return pArray;
        }
        public static void drawOutline(List<Point> ptList) // draws all pixels in the list
        {
            foreach (var item in ptList)
            {
                workImage.SetPixel(item.X, item.Y, Color.FromArgb(255, 255, 0, 0));
            }
        }
        
        public static List<Point> letsTryThis()
        {
            int skiPoints = 1;
            List<Point> drawOutln = new List<Point>();
            bool done = false;
            for (int x = 5; x < workImage.Width - 5; x++)
            {
                for (int y = 3; y < workImage.Height - 1; y++)
                {
                    // STARTING PIXEL - if there is a black pixel next to a white pixel
                    if (workImage.GetPixel(x, y) == bakCol
                     && workImage.GetPixel(x + 1, y) == foreCol)
                    {
                        Point start = new Point(x + 1, y);
                        Point cur = new Point(x + 1, y);
                        Point next = new Point(x, y);
                        Point prev = new Point(x, y);
                        drawOutln.Add(start);//START PIXEL
                        //System.Console.WriteLine("start is " + start);
                        var outLnCount = 0;
                        while(next != start)
                        //for (int i = 0; i < 2500; i++)
                        {
                            next = NextPix(cur);  //FIND NEXT PIXEL
                            //System.Console.WriteLine("cur is " + next);
                            if (ptTrack.Count > 3) { ptTrack.RemoveAt(3); }  // only need 
                            ptTrack.Insert(0, next); // adding to Pointtrail to detect loops
                            outLnCount += 1;
                            if (outLnCount % skiPoints == 0)   // every n'th point gets added to outline
                            { drawOutln.Add(next); }  //ADD IT TO drawing line
                            prev = cur;
                            cur = next;
                        }
                        //System.Console.WriteLine("done first");
                        return drawOutln;
                    }
                    if (done) { break; }
                }
                if (done) { break; }
            }
            return drawOutln;
        }
        public static Point NextPix(Point currentPoint)
        {
            // Iterate through 8 positions around the current point to find the next edge point
            for (int direction = 0; direction <= 7; direction++)
            {
                Point adjacentPoint = GetAdjacentPoint(currentPoint, direction);

                if (IsPointValid(adjacentPoint) && IsPointOnEdge(adjacentPoint, direction))
                {
                    return adjacentPoint;
                }
            }

            // Return the current point if no valid next point is found
            return currentPoint;
        }

        // Helper method to check if the point is valid (not out of bounds and not a repeated point)
        private static bool IsPointValid(Point pt)
        {
            return !isOutBounds(pt) && !ptTrack.Contains(pt);
        }

        // Helper method to check if the point is on the edge of the shape
        private static bool IsPointOnEdge(Point pt, int direction)
        {
            return workImage.GetPixel(pt.X, pt.Y) == foreCol && IsEdge(pt, direction);
        }

        // Helper method to get an adjacent point based on the current point and direction
        private static Point GetAdjacentPoint(Point currentPoint, int direction)
        {
            return ConvFromInt(currentPoint, direction);
        }

    
        public static bool isBounds(Point pt) // checks if Point is on the outside edge of image
        {
            if (pt.X == 0 || pt.X == workImage.Width - 10) { return true; }
            if (pt.Y == 0 || pt.Y == workImage.Height - 1) { return true; }
            return false;
        }
        public static bool isOutBounds(Point pt) // checks if Point is on the outside edge of image
        {
            if (pt.X < 0 || pt.X > workImage.Width - 10) { return true; }
            if (pt.Y < 0 || pt.Y > workImage.Height - 1) { return true; }
            return false;
        }
        public static bool IsEdge(Point test, int dir)
        {
            // Check if the point is at the boundary of the image
            if (IsAtImageBoundary(test))
            {
                return true;
            }

            // Define the directions to check based on the current direction
            int[] directionsToCheck = GetDirectionsToCheck(dir);

            // Check if any of the surrounding pixels in the specified directions are of background color
            foreach (var direction in directionsToCheck)
            {
                Point adjacentPoint = ConvFromInt(test, direction);
                if (workImage.GetPixel(adjacentPoint.X, adjacentPoint.Y) == bakCol)
                {
                    return true;
                }
            }

            return false;
        }

        // Helper method to check if a point is at the boundary of the image
        private static bool IsAtImageBoundary(Point pt)
        {
            return pt.X == 0 || pt.X == workImage.Width - 1 || pt.Y == 0 || pt.Y == workImage.Height - 1;
        }

        // Helper method to determine which directions to check based on the current direction
        private static int[] GetDirectionsToCheck(int dir)
        {
            switch (dir)
            {
                case 0: return new int[] { 2, 6 };
                case 1: return new int[] { 4, 6 };
                case 2: return new int[] { 0, 4 };
                case 3: return new int[] { 0, 6 };
                case 4: return new int[] { 2, 6 };
                case 5: return new int[] { 0, 2 };
                case 6: return new int[] { 0, 4 };
                case 7: return new int[] { 2, 4 };
                default: return new int[] { };
            }
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
                    if (pix == foreCol)
                    {
                        if (prev == bakCol
                        )
                        {
                            if (prox == foreCol)
                            {
                                if (proxx == bakCol
                                )
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
        {
            return true;
            if (img.GetPixel(x - 1, y - 1) == foreCol && img.GetPixel(x, y - 2) == foreCol)
            {
                if (img.GetPixel(x + 1, y) == foreCol && img.GetPixel(x + 1, y + 1) == foreCol)
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

        public static Point ConvFromInt(Point pt, int pos) //converts spider# into Point
        {
            switch (pos)
            {
                case 0:
                    return new Point(pt.X - 1, pt.Y);
                case 1:
                    return new Point(pt.X - 1, pt.Y - 1);
                case 2:
                    return new Point(pt.X, pt.Y - 1);
                case 3:
                    return new Point(pt.X + 1, pt.Y - 1);
                case 4:
                    return new Point(pt.X + 1, pt.Y);
                case 5:
                    return new Point(pt.X + 1, pt.Y + 1);
                case 6:
                    return new Point(pt.X, pt.Y + 1);
                case 7:
                    return new Point(pt.X - 1, pt.Y + 1);
                default:
                    return pt;
            }
        }

        public static void corFilB(Bitmap img)
        {
            for (int x = 1; x < img.Width - 5; x++)
            {
                for (int y = 3; y < img.Height - 5; y++)
                {
                    // black on x axis
                    if (img.GetPixel(x, y) == bakCol
                    )
                    {
                        if (img.GetPixel(x, y - 1) == foreCol && img.GetPixel(x - 1, y - 1) == foreCol)
                        {
                            if (img.GetPixel(x + 1, y) == foreCol && img.GetPixel(x + 1, y + 1) == foreCol)
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                    ///
                    // black on x axis
                    if (img.GetPixel(x, y) == bakCol
                    )
                    {
                        if (img.GetPixel(x - 1, y) == foreCol && img.GetPixel(x - 1, y - 1) == foreCol)
                        {
                            if (img.GetPixel(x, y + 1) == foreCol && img.GetPixel(x + 1, y + 1) == foreCol)
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == bakCol
                    )
                    {
                        if (img.GetPixel(x, y + 1) == foreCol && img.GetPixel(x - 1, y + 1) == foreCol)
                        {
                            if (img.GetPixel(x, y + 1) == foreCol && img.GetPixel(x + 1, y + 1) == foreCol)
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == bakCol
                    )
                    {
                        if (img.GetPixel(x - 1, y) == foreCol && img.GetPixel(x - 1, y + 1) == foreCol)
                        {
                            if (img.GetPixel(x, y - 1) == foreCol && img.GetPixel(x + 1, y - 1) == foreCol)
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                }
            }
        }

        public static void corFilW(Bitmap img)
        {
            for (int x = 1; x < img.Width - 5; x++)
            {
                for (int y = 3; y < img.Height - 5; y++)
                {
                    // black on x axis
                    if (img.GetPixel(x, y) == foreCol)
                    {
                        if (img.GetPixel(x, y - 1) == bakCol
                         && img.GetPixel(x - 1, y - 1) == bakCol
                        )
                        {
                            if (img.GetPixel(x + 1, y) == bakCol
                             && img.GetPixel(x + 1, y + 1) == bakCol
                            )
                            {
                                img.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                    ///
                    // black on x axis
                    if (img.GetPixel(x, y) == foreCol)
                    {
                        if (img.GetPixel(x - 1, y) == bakCol
                         && img.GetPixel(x - 1, y - 1) == bakCol
                        )
                        {
                            if (img.GetPixel(x, y + 1) == bakCol
                             && img.GetPixel(x + 1, y + 1) == bakCol
                            )
                            {
                                img.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == foreCol)
                    {
                        if (img.GetPixel(x, y + 1) == bakCol
                         && img.GetPixel(x - 1, y + 1) == bakCol
                        )
                        {
                            if (img.GetPixel(x, y + 1) == bakCol
                             && img.GetPixel(x + 1, y + 1) == bakCol
                            )
                            {
                                img.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == foreCol)
                    {
                        if (img.GetPixel(x - 1, y) == bakCol
                         && img.GetPixel(x - 1, y + 1) == bakCol
                        )
                        {
                            if (img.GetPixel(x, y - 1) == bakCol
                             && img.GetPixel(x + 1, y - 1) == bakCol
                            )
                            {
                                img.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                }
            }
        }
        public static void deGranB(Bitmap img, int granule)
        {
            for (int x = 1; x < img.Width - 5; x++)
            {
                for (int y = 3; y < img.Height - 5; y++)
                {
                    // black on x axis
                    if (img.GetPixel(x, y) == foreCol)
                    {

                        if (img.GetPixel(x - 1, y) == bakCol
                        )
                        {
                            if (img.GetPixel(x + 1, y) == bakCol
                            )
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
                                if (img.GetPixel(x + 2, y) == bakCol
                                )
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
                                if (img.GetPixel(x + 3, y) == bakCol
                                )
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
                                if (img.GetPixel(x + 4, y) == bakCol
                                )
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
                                if (img.GetPixel(x + 5, y) == bakCol
                                )
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
                    if (img.GetPixel(x, y) == bakCol
                    )
                    {
                        if (img.GetPixel(x, y - 1) == bakCol
                        )
                        {
                            if (img.GetPixel(x, y + 1) == bakCol
                            )
                            {
                                img.SetPixel(x, y, Color.White);
                            }

                            if (granule > 1)
                            {
                                if (img.GetPixel(x, y + 2) == bakCol
                                )
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x, y + 1, Color.White);
                                }
                            }
                            if (granule > 2)
                            {
                                if (img.GetPixel(x, y + 3) == bakCol
                                )
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x, y + 1, Color.White);
                                    img.SetPixel(x, y + 2, Color.White);
                                }
                            }
                            if (granule > 3)
                            {
                                if (img.GetPixel(x, y + 4) == bakCol
                                )
                                {
                                    img.SetPixel(x, y, Color.White);
                                    img.SetPixel(x, y + 1, Color.White);
                                    img.SetPixel(x, y + 2, Color.White);
                                    img.SetPixel(x, y + 3, Color.White);
                                }
                            }
                            if (granule > 4)
                            {
                                if (img.GetPixel(x, y + 5) == bakCol
                                )
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
                    if (img.GetPixel(x, y) == bakCol
                    )
                    {

                        if (img.GetPixel(x - 1, y) == foreCol)
                        {
                            if (img.GetPixel(x + 1, y) == foreCol)
                            {
                                img.SetPixel(x, y, Color.Black);
                            }

                            if (granule > 1)
                            {
                                if (img.GetPixel(x + 2, y) == foreCol)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x + 1, y, Color.Black);
                                }
                            }
                            if (granule > 2)
                            {
                                if (img.GetPixel(x + 3, y) == foreCol)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x + 1, y, Color.Black);
                                    img.SetPixel(x + 2, y, Color.Black);
                                }
                            }
                            if (granule > 3)
                            {
                                if (img.GetPixel(x + 4, y) == foreCol)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x + 1, y, Color.Black);
                                    img.SetPixel(x + 2, y, Color.Black);
                                    img.SetPixel(x + 3, y, Color.Black);
                                }
                            }
                        }
                        /// white on y axis
                        if (img.GetPixel(x, y - 1) == foreCol)
                        {
                            if (img.GetPixel(x, y + 1) == foreCol)
                            {
                                img.SetPixel(x, y, Color.Black);
                            }

                            if (granule > 1)
                            {
                                if (img.GetPixel(x, y + 2) == foreCol)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x, y + 1, Color.Black);
                                }
                            }
                            if (granule > 2)
                            {
                                if (img.GetPixel(x, y + 3) == foreCol)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                    img.SetPixel(x, y + 1, Color.Black);
                                    img.SetPixel(x, y + 2, Color.Black);
                                }
                            }
                            if (granule > 3)
                            {
                                if (img.GetPixel(x, y + 4) == foreCol)
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