using System;
using System.Drawing;
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
        public static string sourceImage = "images/szym1.jpg";
        public static string outputImage = "images/szym.jpg";
        public static List<Point> ptTrack = new List<Point>(); // for keeping track a trail of 4 points to detect loops
        public static Bitmap workImage;
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

                workImage = new Bitmap(image, lgWidth, lgHeight);

                picToPixelBW(workImage);
                //deGranB(newImage,3);
                //deGranW(newImage,3);
                /*deGranB(newImage,2);
                deGranW(newImage,2);
                deGranB(newImage,1);
                deGranW(newImage,1);
                corFilW(newImage);
                corFilB(newImage);*/
                for (int g = 0; g < 4; g++)
                {
                    Point pt = new Point(g, g);
                    ptTrack.Add(pt);
                    //System.Console.WriteLine(ptTrack.Count);
                }
                drawOutline(letsTryThis());
                workImage.Save(outputImage);
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }
        public static void drawOutline(List<Point> ptList)
        {
            foreach (var item in ptList)
            {
                workImage.SetPixel(item.X, item.Y, Color.FromArgb(255, 255, 0, 0));
            }
        }
        public static List<Point> letsTryThis()
        {
            List<Point> drawOutln = new List<Point>();
            bool done = false;
            for (int x = 1; x < workImage.Width - 5; x++)
            {
                for (int y = 3; y < workImage.Height - 5; y++)
                {
                    // STARTING PIXEL - if there is a black pixel next to a white pixel
                    if (workImage.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255) && workImage.GetPixel(x + 1, y) == Color.FromArgb(255, 0, 0, 0))
                    {
                        Point start = new Point(x + 1, y);
                        Point cur = new Point(x + 1, y);
                        Point next = new Point(x, y);
                        Point prev = new Point(x, y);
                        drawOutln.Add(start);//START PIXEL
                        //System.Console.WriteLine("start is " + start);
                        //while(next != start)
                        for (int i = 0; i < 1420; i++)
                        {
                            next = NextPix(cur);  //FIND NEXT PIXEL
                            //System.Console.WriteLine("cur is " + next);

                            
                            if (ptTrack.Count > 4) { ptTrack.RemoveAt(4); }  // only need 3
                            /*System.Console.Write("ptTrack contains: ");
                            foreach (Point itm in ptTrack)
                            {
                                Console.Write(itm);
                            }
                            System.Console.WriteLine("");*/
                            ptTrack.Insert(0, next); // adding to Pointtrail to detect loops
                            drawOutln.Add(next);  //ADD IT TO drawing line

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
                foreach (Point item in ptTrack) // test if the point exists in trail and if its black
                {
                    if (item == test)
                    {
                        repeat = true;
                        //System.Console.WriteLine("repeat");
                        break;
                    }
                }
                if (repeat==false && workImage.GetPixel(test.X, test.Y) == Color.FromArgb(255, 0, 0, 0))
                {
                    if (IsEdge(test, i) == true) //is edge?
                    {
                        return test;
                    }
                }

            }
            return px;
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
        {
            return true;
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

        public static void deGranOld()
        {/*
            fing first
            dirCrawl(Image,)
            find next, store
            find*/
        }
        public static Point ConvFromInt(Point pt, int pos)
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

        public static bool IsEdge(Point test, int dir)
        {
            switch (dir)
            {
                case 0:
                    //2 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        return true;
                    }
                    return false;

                case 1:
                    //4 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 2:
                    //0 or 4
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 3:
                    //0 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 4:
                    // 2 or 6
                    if (workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 6).X, ConvFromInt(test, 6).Y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 5:
                    //0 or 2
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 6:
                    //0 or 4
                    if (workImage.GetPixel(ConvFromInt(test, 0).X, ConvFromInt(test, 0).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 7:
                    //2 or 4
                    if (workImage.GetPixel(ConvFromInt(test, 2).X, ConvFromInt(test, 2).Y) == Color.FromArgb(255, 255, 255, 255) || workImage.GetPixel(ConvFromInt(test, 4).X, ConvFromInt(test, 4).Y) == Color.FromArgb(255, 255, 255, 255))
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

        /*public static int dirCrawl(Bitmap img, Point pt, Color col)
        {
            Point cur = pt;

            //Point p0 = (Point) converter.ConvertTo(pt.X-1, pt.Y);
            Point p1 = (pt.X-1, pt.Y+1);
            Point p2 = (pt.X, pt.Y+1);
            Point p3 = (pt.X+1, pt.Y+1);
            Point p4 = (pt.X+1, pt.Y);
            Point p5 = (pt.X+1, pt.Y-1);
            Point p6 = (pt.X, pt.Y-1);
            Point p7 = (pt.X-1, pt.Y-1);

            if(img.GetPixel(p4.X, p4.Y) == Color.Black)//4
            {
                return 4;
            }
            if(img.GetPixel(p4.X, p4.Y)== Color.White && img.GetPixel(p5.X, p5.Y)== Color.Black)//5
            {
                return 5;
            }
            if(img.GetPixel(p4.X, p4.Y)== Color.White && img.GetPixel(p5.X, p5.Y)== Color.White)//6
            {
                if(img.GetPixel(p6.X, p6.Y)== Color.Black)
                {
                    return 6;
                }
            }
            if(img.GetPixel(p4.X, p4.Y)== Color.White && img.GetPixel(p5.X, p5.Y)== Color.White)//7
            {
                if(img.GetPixel(p6.X, p6.Y)== Color.White && img.GetPixel(p7.X, p7.Y)== Color.Black)
                {
                    return 7;
                }
            }
            if(img.GetPixel(p4.X, p4.Y)== Color.White && img.GetPixel(p5.X, p5.Y)== Color.White)//close
            {
                if(img.GetPixel(p6.X, p6.Y)== Color.White && img.GetPixel(p7.X, p7.Y)== Color.White)
                {
                    if(img.GetPixel(p0.X, p0.Y)== Color.Black)
                    {
                        return 0;
                    }
                }
            }
            if(img.GetPixel(p4.X, p4.Y)== Color.White && img.GetPixel(p5.X, p5.Y)== Color.White)//close
            {
                if(img.GetPixel(p6.X, p6.Y)== Color.White && img.GetPixel(p7.X, p7.Y)== Color.White)
                {
                    if(img.GetPixel(p0.X, p0.Y)== Color.White && img.GetPixel(p1.X, p1.Y)== Color.Black)
                    {
                        return 1;
                    }
                }
            }

                if(img.GetPixel(p5.X, p5.Y)== White)
                {
                    return 5;
                }
                if(img.GetPixel(p6.X, p6.Y)== White)
                {
                    return 6;
                }
                if(img.GetPixel(p7.X, p7.Y)== White)
                {
                    return 7;
                }
                if(img.GetPixel(p0.X, p0.Y)== White)
                {
                    return 0;
                }
            }
            if(img.GetPixel(p6.X, p6.Y) == white)
            {
                    return 6;
                    return 7;
                    return 0;
            }
            if(img.GetPixel(p6.X, p6.Y) == white)
            {
                
            }
        }
        public static Point crawler(Point pnt, int dir)
        {
            if(dir == 0)
            {
                return Point (pnt.X -1, pnt.Y);
            }
             if(dir == 1)
            {
                return Point (pnt.x -1, pnt.y+1);
            }
             if(dir == 2)
            {
                return Point (pnt.x , pnt.y+1);
            }
             if(dir == 3)
            {
                return Point (pnt.x + 1, pnt.y+1);
            }
             if(dir == 4)
            {
                return Point (pnt.x + 1, pnt.y);
            }
             if(dir == 5)
            {
                return Point (pnt.x + 1, pnt.y-1);
            }
             if(dir == 6)
            {
                return Point (pnt.x, pnt.y-1);
            }
             if(dir == 7)
            {
                return Point (pnt.x - 1, pnt.y-1);
            }
        }
        */
        public static void corFilB(Bitmap img)
        {
            for (int x = 1; x < img.Width - 5; x++)
            {
                for (int y = 3; y < img.Height - 5; y++)
                {
                    // black on x axis
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        if (img.GetPixel(x, y - 1) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x - 1, y - 1) == Color.FromArgb(255, 0, 0, 0))
                        {
                            if (img.GetPixel(x + 1, y) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x + 1, y + 1) == Color.FromArgb(255, 0, 0, 0))
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                    ///
                    // black on x axis
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        if (img.GetPixel(x - 1, y) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x - 1, y - 1) == Color.FromArgb(255, 0, 0, 0))
                        {
                            if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x + 1, y + 1) == Color.FromArgb(255, 0, 0, 0))
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x - 1, y + 1) == Color.FromArgb(255, 0, 0, 0))
                        {
                            if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x + 1, y + 1) == Color.FromArgb(255, 0, 0, 0))
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255))
                    {
                        if (img.GetPixel(x - 1, y) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x - 1, y + 1) == Color.FromArgb(255, 0, 0, 0))
                        {
                            if (img.GetPixel(x, y - 1) == Color.FromArgb(255, 0, 0, 0) && img.GetPixel(x + 1, y - 1) == Color.FromArgb(255, 0, 0, 0))
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
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 0, 0, 0))
                    {
                        if (img.GetPixel(x, y - 1) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x - 1, y - 1) == Color.FromArgb(255, 255, 255, 255))
                        {
                            if (img.GetPixel(x + 1, y) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x + 1, y + 1) == Color.FromArgb(255, 255, 255, 255))
                            {
                                img.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                    ///
                    // black on x axis
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 0, 0, 0))
                    {
                        if (img.GetPixel(x - 1, y) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x - 1, y - 1) == Color.FromArgb(255, 255, 255, 255))
                        {
                            if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x + 1, y + 1) == Color.FromArgb(255, 255, 255, 255))
                            {
                                img.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 0, 0, 0))
                    {
                        if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x - 1, y + 1) == Color.FromArgb(255, 255, 255, 255))
                        {
                            if (img.GetPixel(x, y + 1) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x + 1, y + 1) == Color.FromArgb(255, 255, 255, 255))
                            {
                                img.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                    if (img.GetPixel(x, y) == Color.FromArgb(255, 0, 0, 0))
                    {
                        if (img.GetPixel(x - 1, y) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x - 1, y + 1) == Color.FromArgb(255, 255, 255, 255))
                        {
                            if (img.GetPixel(x, y - 1) == Color.FromArgb(255, 255, 255, 255) && img.GetPixel(x + 1, y - 1) == Color.FromArgb(255, 255, 255, 255))
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