using System;
using System.Drawing;
using System.Collections.Generic;


namespace GameEngine
{
    public class Camera
    {
        public Point3D Offset;

        public Bitmap CameraScreen;

        public Rectangle DrawLocation;

        public int StartZDraw;
        public int EndZDraw;

        public CameraType CameraStyle;

        public bool ShowBackground;

        public Camera(Size resolution, Rectangle drawLocation, CameraType cameraStyle)
            : this(new Point3D(), resolution, drawLocation, cameraStyle)
        {            
        }

        public Camera(Point3D offset, Size resolution, Rectangle drawLocation, CameraType cameraStyle)
        {
            this.Offset = offset;

            this.CameraScreen = new Bitmap(resolution.Width, resolution.Height);

            this.DrawLocation = drawLocation;

            this.StartZDraw = 0;
            this.EndZDraw = 20;

            this.CameraStyle = cameraStyle;

            this.ShowBackground = true;
        }

        public void MoveCameraActual(Point3D newOffset)
        {
            this.Offset = newOffset;
        }

        public void MoveCameraRelative(Point3D newOffset)
        {
            this.Offset.X += newOffset.X;
            this.Offset.Y += newOffset.Y;
            this.Offset.Z += newOffset.Z;
        }

        public void ChangeCameraResolution(Size newResolution)
        {
            this.CameraScreen = new Bitmap(newResolution.Width, newResolution.Height);
        }
    }
}
