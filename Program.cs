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
        public static Color foreCol = Color.FromArgb(255,0,0,0);
        public static Color bakCol = Color.FromArgb(255,255,255,255);

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
                deGranB(workImage,3);
                deGranW(workImage,3);
                deGranB(workImage,2);
                deGranW(workImage,2);
                deGranB(workImage,1);
                deGranW(workImage,1);
                //corFilW(newImage);
                //corFilB(newImage);*/
                outlineStuff();
                
                workImage.Save(outputImage);
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }
        public static void outlineStuff()
        {
            for (int g = 0; g < 4; g++)
                {
                    Point pt = new Point(g, g);
                    ptTrack.Add(pt);
                    //System.Console.WriteLine(ptTrack.Count);
                }

                //drawOutline(letsTryThis());
                
                    SolidBrush redBrush = new SolidBrush(Color.Red);
                    Pen pn = new Pen(Color.Red);
             
                    // Set fill mode.
                    FillMode newFillMode = FillMode.Winding;
                            
                    // Set tension.
                    float tension = .2F;
                            
                    // Fill curve on screen.
                    using Graphics e = Graphics.FromImage(workImage);
                    e.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    Point[] outLine = LisToPoints(letsTryThis());
                    //drawOutline(LisToPoints(letsTryThis()));
                    //e.GraphicsPath(LisToPoints(letsTryThis()));
                    GraphicsPath pat = new GraphicsPath();
                    //e.DrawPath(pn, pat);
                    e.DrawClosedCurve(pn, outLine, 0.2F, FillMode.Winding);
                    HatchBrush hBrush = new HatchBrush(
        HatchStyle.DiagonalCross,
        Color.Black,
            Color.White);

                    //e.FillClosedCurve(hBrush, outLine, newFillMode, tension);
                    
        }
        public static Point[] LisToPoints(List<Point> pList) // converts a List of Points into Array of Points
        {   
            List<Point> smList = new();
            for(int i = 0; i < pList.Count; i += 20) // reduce pList using only every 20th Point
            {
                smList.Add(pList[i]);
            }
            Point[] pArray = new Point[smList.Count];
            for(int i=0; i < smList.Count; i++)
            {
                pArray[i] = new Point(smList[i].X, smList[index: i].Y);
            }
            return pArray;
            //return smList;
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
            List<Point> drawOutln = new List<Point>();
            int outLnCount = 0;
            bool done = false;
            for (int x = 1; x < workImage.Width - 5; x++)
            {
                for (int y = 3; y < workImage.Height - 5; y++)
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
                        //while(next != start)
                        for (int i = 0; i < 2800; i++)
                        {
                            next = NextPix(cur);  //FIND NEXT PIXEL
                            //System.Console.WriteLine("cur is " + next);
                            if (ptTrack.Count > 4) { ptTrack.RemoveAt(4); }  // only need 3
                            outLnCount += 20;
                            int h = outLnCount%20;
                            
                            ptTrack.Insert(0, next); // adding to Pointtrail to detect loops
                            if(h == 0){
                            drawOutln.Add(next);}  //ADD IT TO drawing line
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

        public static Point NextPix(Point px)
        {
            List<int> edges = new List<int>();
            bool repeat = false;
            for (int i = 0; i <= 7; i++)// testing 8 positions around current point
            {
                repeat=false;
                //System.Console.WriteLine("testing position " + i);
                Point test = ConvFromInt(px, i); // returns a Point based on Spider#
                if(ptTrack.Contains(test)){repeat = true;}
                if (!repeat && workImage.GetPixel(test.X, test.Y) == foreCol)
                {
                    if (IsEdge(test, i) || isBounds(test)) //is edge or image bounds?
                    {
                        return test;
                    }
                }
            }
            return px;
        }
        public static bool isBounds(Point pt) // checks if Point is on the outside edge of image
        {
            if(pt.X == 3 || pt.X == workImage.Width-3){return true;}
            if(pt.Y == 3 || pt.Y == workImage.Height-3){return true;}
            return false;
        }
         public static bool IsEdge(Point test, int dir)
        {
            switch (dir)
            {
                case 0:
                    //2 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == bakCol
                    )
                    {
                        return true;
                    }
                    return false;

                case 1:
                    //4 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == bakCol
                    )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 2:
                    //0 or 4
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == bakCol
                    )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 3:
                    //0 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == bakCol
                    )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 4:
                    // 2 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == bakCol
                    )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 5:
                    //0 or 2
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == bakCol
                    )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 6:
                    //0 or 4
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == bakCol)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 7:
                    //2 or 4
                    if (workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == bakCol
                     || workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == bakCol
                    )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
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