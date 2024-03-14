The image is first converted to foreground and background colors based on pixel brightness, using LockBits method for performance.
Pixel noise is then cleaned up, leaving clean contours from which separate shapes are extracted and drawn as filled, closed curves.

"picToPixelLockB" finds the brightness of each pixel based on the average of the RGB values, then changes the pixel to black or white.

"deGran" reduces the granularity of an image by changing pixels near the boundary between foreground and background colors to match the background color, based on a specified granularity (2 through 4). The bool flag can be set to false to do the same with inverted colors.

"CreateShapes" first identifies outlines from a list of points, converting these outlines into arrays to define shapes, and then drawing these shapes as filled curves. The process iterates until there are no new starting points, thereby ensuring all identified shapes within the given outlines are created.
