using System.Drawing;
using System.Drawing.Drawing2D;

namespace stencil
{
    partial class Program
    {
        public static void CreateShapes() // creates solid shapes
        {
            List<Point> allOutlinePoints = new();// list keeps track of all outlines for finding new startPoint
            Point startPt = new(0, 1);
            Point nullPt = new(0, 0);
            startPt = FindListShapeStart1(allOutlinePoints);
            System.Console.WriteLine(startPt);
            List<Point> pointsForShaping = ShapePointList(startPt); //get Point list of the first outline
            System.Console.WriteLine("list count is " + pointsForShaping.Count);
            allOutlinePoints.AddRange(pointsForShaping);//add points to overall list
            System.Console.WriteLine("list of all aoutlines is " + allOutlinePoints.Count);
            Point[] shapeArray = ListToCurveArray(pointsForShaping); //convert to array
            CreateShape(shapeArray); //draw filled curve
            System.Console.WriteLine("shwpe drawn");
            startPt = FindListShapeStart1(allOutlinePoints);

            while (startPt != nullPt)
            //for (int c = 0; c < 3; c++)
            {

                System.Console.WriteLine("loop started");
                pointsForShaping = ShapePointList(startPt); //get Point list of the outline
                System.Console.WriteLine("list is " + pointsForShaping.Count());

                allOutlinePoints.AddRange(pointsForShaping); //add Points to the outlines of all shapes
                System.Console.WriteLine("big list of outlines is " + allOutlinePoints.Count());
                //System.Console.WriteLine("points added to allPoint list");
               
                    Point[] shapeArray1 = ListToCurveArray(pointsForShaping); //convert to Array
                    CreateShape(shapeArray1); //draw filled curve
                    
                
                startPt = FindListShapeStart1(allOutlinePoints);
                System.Console.WriteLine(startPt);

            }

        }
       
        public static Point FindListShapeStart2(List<Point> outlinePoints) // flagging solid areas solid areas
        {
            Point nullPt = new Point(0, 0); // Indicates no more shapes were found.

            bool isWithinMarkedArea = false; // Tracks whether the current pixel is within a previously marked area.

            for (int x = 4; x < workImage.Width - 5; x++)
            {
                for (int y = 29; y < workImage.Height - 1; y++)
                {
                    Point currentPoint = new(x, y);

                    // Check if the current point is in the outlined area to skip it.
                    if (outlinePoints != null && outlinePoints.Contains(currentPoint))
                    {
                        isWithinMarkedArea = true; // Mark entering an outlined area.
                        continue; // Skip further processing for this point.
                    }
                    else
                    {
                        // If exiting a marked area, reset the flag and continue searching for a starting point.
                        if (isWithinMarkedArea)
                        {
                            isWithinMarkedArea = false;
                            continue;
                        }
                    }

                    // Looking for a starting point: a black pixel next to a white pixel indicates a potential outline start.
                    if (!isWithinMarkedArea && workImage.GetPixel(x, y) == bakCol && workImage.GetPixel(x + 1, y) == foreCol)
                    {
                        return new Point(x + 1, y); // Found a starting point outside any marked area.
                    }
                }
            }

            Console.WriteLine("No more shapes");
            return nullPt; // No starting point found outside marked areas.
        }

        public static Point FindListShapeStart1(List<Point> outlinePoints = null)
        {
            Point nullPt = new Point(0, 0); // Indicates no more shapes were found.

            for (int x = 6; x < workImage.Width - 5; x++)
            {
                for (int y = 29; y < workImage.Height - 1; y++)
                {
                    Point testPt = new(x, y);

                    // Skip points that are already part of an outline
                    if (outlinePoints != null && outlinePoints.Contains(testPt))
                    {
                        //System.Console.WriteLine(testPt + " contained in list");
                        continue; // Move to the next point if this one is already outlined
                    }

                    // Looking for a black pixel next to a white pixel to start
                    if (workImage.GetPixel(x, y) == bakCol && workImage.GetPixel(x + 1, y) == foreCol)
                    {
                        Point startPt = new(x + 1, y);
                        if (outlinePoints != null && outlinePoints.Contains(startPt))
                        {continue;}else{
                        return startPt;} // Found a starting point
                    }
                }
            }
            Console.WriteLine("No more shapes");
            return nullPt; // No starting point found
        }

        public static Point FindListShapeStart(List<Point> outlinePoints = null)  // finds a starting point within a list// ORIGINAL
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
                                if (markedArea == false) { markedArea = true; break; }
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

        public static List<Point> ShapePointList(Point startPoint)  // creates a List of outline points for the startPoint
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
            {
                next = NextPix(cur);  //FIND NEXT PIXEL
                                      //System.Console.WriteLine("checkin point " + next);
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
        public static void CreateShape(Point[] shapePoints)  // draws a filled closed curve with array Points
        {
            if (shapePoints.Length > 10)
            {
                using Graphics e = Graphics.FromImage(outputImg);
                Pen pn = new Pen(Color.Blue);
                FillMode newFillMode = FillMode.Winding;
                float tension = .2F;

                e.DrawClosedCurve(pn, shapePoints, 0.2F, FillMode.Winding);
                e.FillClosedCurve(new SolidBrush(pn.Color), shapePoints, newFillMode, tension);
                System.Console.WriteLine("shape drawn");
                //HatchBrush hBrush = new HatchBrush(HatchStyle.DiagonalCross,Color.Black,Color.White);
            }
            else
            {
                System.Console.WriteLine("too few points to draw curve");
            }
        }


        public static Point[] ListToCurveArray(List<Point> pointList)  // reduces number of points and converts a List of Points into Array of Points
        {
            int skippPoints = 10;
            List<Point> outlinePoints = new List<Point>();
            for (int i = 0; i < pointList.Count - 1; i += skippPoints)
            {
                outlinePoints.Add(pointList[i]);
            }
            System.Console.WriteLine("reduced list is " + outlinePoints.Count());

            Point[] pArray = new Point[outlinePoints.Count];
            for (int i = 0; i < outlinePoints.Count; i++)
            {
                pArray[i] = new Point(outlinePoints[i].X, outlinePoints[index: i].Y);
            }
            return pArray;
        }

    }
}