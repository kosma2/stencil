using System.Drawing;

namespace stencil
{
    public partial class Program
    {
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


        public static void picToPixelLockB(Bitmap img)// converts image to black and white based on pixel brightness
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
                // get the brightness as the average of the RGB components
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
        public static Bitmap ConvertToBlackAndWhite(Bitmap originalImage, int threshold)
        {
            Bitmap blackAndWhiteImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    // Get the pixel's color.
                    Color originalColor = originalImage.GetPixel(i, j);

                    // Calculate the luminance.
                    int luminance = (int)(0.299 * originalColor.R + 0.587 * originalColor.G + 0.114 * originalColor.B);

                    // Determine if the pixel should be black or white.
                    Color bwColor = luminance < threshold ? Color.Black : Color.White;

                    // Set the new pixel color.
                    blackAndWhiteImage.SetPixel(i, j, bwColor);
                }
            }

            return blackAndWhiteImage;
        }

    }
}