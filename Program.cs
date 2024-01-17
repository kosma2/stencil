using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace stencil
{
    partial class Program
    {
        //public static int granule = 1; //max4
        public static string sourceImage = "C:/Users/User/Pictures/face3.jpg";
        public static string outputImage = "images/face3.jpg";
        public static List<Point> pointTrack = new List<Point>(); // tracks last 3 Points, preventing loops and backtracking
        public static Bitmap workImage;
        // set foreground and background colors
        public static Color foreCol = Color.FromArgb(255, 0, 0, 0);
        public static Color bakCol = Color.FromArgb(255, 255, 255, 255);
        // set shape outline color
        public static Color outlineCol = Color.FromArgb(255, 255, 0, 0);

        public static void Main()
        {
            try
            {
                // Retrieve the image.
                System.Drawing.Image img = System.Drawing.Image.FromFile(sourceImage);
                using Bitmap image = new Bitmap(sourceImage, true);
                int trgtHeight = (int)480;
                int trgtWidth = (int)640;

                workImage = new Bitmap(image, trgtWidth, trgtHeight);
                picToPixelLockB(workImage);

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
                OutlineShapes();

                workImage.Save(outputImage);
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }
        public static void CreateShapes()
        {
            List<Point> allOutlinePoints = new();
            Point startPt = new(0, 1);
            Point nullPt = new(0, 0);
            startPt = FindListShapeStart();
            System.Console.WriteLine("start is " + startPt);
            while (startPt != nullPt)
            {
                List<Point> pointsForShaping = ShapePointList(startPt); //get Point list of the first outline
                System.Console.WriteLine("list is " + pointsForShaping.Count());
                Point[] shapeArray = ListToCurveArray(pointsForShaping); //convert to Array
                System.Console.WriteLine("array  done");
                allOutlinePoints.AddRange(pointsForShaping); //add Points to the outlines of all shapes
                System.Console.WriteLine("points added to allPoint list");
                if (shapeArray.Length > 3) // need at least 3 Points to draw a closed curve
                {
                    CreateShape(shapeArray); //draw filled curve
                    System.Console.WriteLine("shape drawn"); 
                }
            }
        }
        public static void CreateShape(Point[] shapePoints)
        {
            using Graphics e = Graphics.FromImage(workImage);
            Pen pn = new Pen(Color.Blue);
            FillMode newFillMode = FillMode.Winding;
            float tension = .2F;

            e.DrawClosedCurve(pn, shapePoints, 0.2F, FillMode.Winding);
            e.FillClosedCurve(new SolidBrush(pn.Color), shapePoints, newFillMode, tension);

            //HatchBrush hBrush = new HatchBrush(HatchStyle.DiagonalCross,Color.Black,Color.White);
        }

        // converts a List of Points into Array of Points
        public static Point[] ListToCurveArray(List<Point> pointList)
        {
            int skippPoints = 15;
            List<Point> outlinePoints = new List<Point>();
            for (int i = 0; i < pointList.Count - 1; i += skippPoints)
            {
                outlinePoints.Add(pointList[i]);
            }

            Point[] pArray = new Point[outlinePoints.Count];
            for (int i = 0; i < outlinePoints.Count; i++)
            {
                pArray[i] = new Point(outlinePoints[i].X, outlinePoints[index: i].Y);
            }
            return pArray;
        }
        // draws the outlines of all shapes in the image
        public static void OutlineShapes()
        {
            //e.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Point startPt = new(0, 1);
            Point nullPt = new(0, 0);
            startPt = FindShapeStart();
            drawOutline(ShapePointList(startPt));
            while (startPt != nullPt)
            {
                System.Console.WriteLine("start is " + startPt);
                drawOutline(ShapePointList(startPt));
                //System.Console.WriteLine("shape has points: " + ShapePointList(startPt).Count());
                startPt = FindShapeStart();
            }
        }
        // draws all pixels in the list
        public static void drawOutline(List<Point> ptList)
        {
            foreach (var item in ptList)
            {
                workImage.SetPixel(item.X, item.Y, outlineCol);
            }
        }
        public static Point FindListShapeStart(List<Point> outlinePoints = null)
        {
            Point nullPt = new Point(0, 0);
            bool markedArea = false;
            bool wasLastPxMarked = false;
            for (int x = 5; x < workImage.Width - 5; x++)
            {
                for (int y = 29; y < workImage.Height - 1; y++)
                {
                    if (outlinePoints != null)
                    {
                        Point testPt = new(x, y);
                        if (outlinePoints.Contains(testPt))
                        {
                            if (!wasLastPxMarked)
                            {
                                if (markedArea == false) { markedArea = true;break; }
                                if (markedArea == true) { markedArea = false; }
                            }
                            wasLastPxMarked = true;
                        }
                        else { wasLastPxMarked = false; }
                    }
                    if (!markedArea)
                    {
                        // STARTING PIXEL - if there is a black pixel next to a white pixel
                        if (workImage.GetPixel(x, y) == bakCol && workImage.GetPixel(x + 1, y) == foreCol)
                        {
                            Point startPt = new(x + 1, y);
                            return startPt;
                        }
                    }
                }
            }
            System.Console.WriteLine("no more shapes");
            return nullPt;
        }
        public static Point FindShapeStart()
        {
            Point nullPt = new Point(0, 0);
            bool markedArea = false;
            bool wasLastPxMarked = false;
            for (int x = 5; x < workImage.Width - 5; x++)
            {
                for (int y = 29; y < workImage.Height - 1; y++)
                {
                    if (workImage.GetPixel(x, y) == outlineCol)
                    {
                        if (!wasLastPxMarked)
                        {
                            if (markedArea == false) { markedArea = true;break; }
                            if (markedArea == true) { markedArea = false; }
                        }
                        wasLastPxMarked = true;
                    }
                    else { wasLastPxMarked = false; }
                    if (!markedArea)
                    {
                        // STARTING PIXEL - if there is a black pixel next to a white pixel
                        if (workImage.GetPixel(x, y) == bakCol && workImage.GetPixel(x + 1, y) == foreCol)
                        {
                            Point startPt = new(x + 1, y);
                            return startPt;
                        }
                    }
                }
            }
            System.Console.WriteLine("no more shapes");
            return nullPt;
        }
        // creates a List of outline points for the startPoint
        public static List<Point> ShapePointList(Point startPoint)
        {
            //int skiPoints = 5; // number of points to skip for shape creation
            List<Point> drawOutln = new List<Point>();
            Point start = startPoint;
            Point cur = startPoint;
            Point next = new Point(startPoint.X - 1, startPoint.Y);
            drawOutln.Add(start);//START PIXEL
            pointTrack.Insert(0, start); // tracks last 3 Points, preventing loops and backtracking
            var outLnCount = 0;
            int skipPoints = 1;
            while (next != start)
            //for (int i = 0; i < 2500; i++)
            {
                next = NextPix(cur);  //FIND NEXT PIXEL
                if (pointTrack.Count > 3) { pointTrack.RemoveAt(3); }  // only need 3
                pointTrack.Insert(0, next); // adding to Point trail to detect loops
                ///OUTLINE
                outLnCount += 1;
                if (outLnCount % skipPoints == 0)
                {   // every n'th point gets added to outline
                    drawOutln.Add(next);
                }  //ADD IT TO drawing line
                cur = next;
                ///OUTLINE
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
            return !isOutBounds(pt) && !pointTrack.Contains(pt);
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

        public static bool isOutBounds(Point pt) // checks if Point is on the outside edge of image
        {
            if (pt.X < 0 || pt.X > workImage.Width - 1) { return true; }//-10
            if (pt.Y < 0 || pt.Y > workImage.Height - 1) { return true; }//-1
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
        // converts image to black and white based on pixel brightness

        public static void picToPixelLockB(Bitmap img)
        {
            // Lock the bitmap's bits
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            System.Drawing.Imaging.BitmapData bmpData = img.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);

            // Get the address of the first line
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap
            int bytes = Math.Abs(bmpData.Stride) * img.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Determine the number of bytes per pixel (assuming 32bpp format)
            int bpp = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat) / 8;

            for (int i = 0; i < rgbValues.Length; i += bpp)
            {
                // Compute the brightness as the average of the RGB components
                float brightness = (rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3f / 255f;
                byte colorValue = brightness >= 0.5 ? (byte)255 : (byte)0;

                // Set the pixel to black or white
                rgbValues[i] = colorValue;     // Blue component
                rgbValues[i + 1] = colorValue; // Green component
                rgbValues[i + 2] = colorValue; // Red component

                // If the image has an alpha component (32bpp), preserve it
                if (bpp == 4)
                {
                    rgbValues[i + 3] = 255; // Alpha component
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits
            img.UnlockBits(bmpData);
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


    }
}