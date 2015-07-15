using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace GameEngine
{
    public class GraphicObject
    {
        public string BaseImage { get; set; }

        public Size FrameSize { get; set; }

        public Point FrameInitialOffset { get; set; }

        public int NumberOfFrames { get; set; }

        public bool Vertical { get; set; }

        public GraphicObject(string baseImg, Size frameSize, Point frameInitialOffset, int fps, bool vertical)
        {
            this.BaseImage = baseImg;
            this.FrameSize = FrameSize;
            this.FrameInitialOffset = frameInitialOffset;
            this.Vertical = vertical;
        }

        public Bitmap GetGraphics(ref Hashtable graphicLibrary, int frameNumber)
        {
            var temp = new Bitmap(FrameSize.Width, FrameSize.Height);

            DrawGraphic(ref graphicLibrary, frameNumber, new Point3D(0,0,0), FrameSize, ref temp);

            return temp;
        }

        public void DrawGraphic(ref Hashtable graphicLibrary, int frameNumber, Point3D location, Size objSize, ref Bitmap image)
        {
            if (location.X >= 0 || location.Y >= 0 ||
                location.X + ((Bitmap)graphicLibrary[BaseImage]).Width >= 0 ||
                location.Y + ((Bitmap)graphicLibrary[BaseImage]).Height >= 0 &&
                location.X > NumberOfFrames)
                frameNumber -= NumberOfFrames;

            var gfx = Graphics.FromImage(image);

            var grab = FrameInitialOffset;
            if (Vertical)
            {
                grab.Y = FrameInitialOffset.Y + FrameSize.Height * (frameNumber - 1);
            }
            else
            {
                grab.X = FrameInitialOffset.X + FrameSize.Width * (frameNumber - 1);
            }

            var destRec = new Rectangle(location.X, location.Y, objSize.Width, objSize.Height);
            var srcRec = new Rectangle(grab, FrameSize);

            gfx.DrawImage(graphicLibrary[BaseImage] as Bitmap, destRec, srcRec, GraphicsUnit.Pixel);
        }
    }
}
