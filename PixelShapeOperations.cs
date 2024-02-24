using System.Drawing;
using System.Drawing.Drawing2D;

namespace stencil
{
    public partial class Program
    {
       
 
        public static void OutlineShapes()  // draws the outlines of all shapes in the image
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

        public static Point FindShapeStart()    //finds start of the next shape based on marked pixels
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
                            if (markedArea == false) { markedArea = true; break; }
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
 
    }
}