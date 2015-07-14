using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;


namespace GameEngine
{
    public class Parallax
    {
        public IList<Background> BackgroundList = new List<Background>();

        public void Redraw(Camera drawCamera, Size levelSize)
        {
            var gfx = Graphics.FromImage(drawCamera.CameraScreen);

            var parallaxOffset = new DecimalPoint();

            int xOffset = 0;

            foreach(var bgnd in BackgroundList)
            {
                if (drawCamera.CameraStyle == CameraType.Left3D)
                    xOffset += bgnd.ZLevel;
                else if (drawCamera.CameraStyle == CameraType.Right3D)
                    xOffset -= bgnd.ZLevel;
                else
                    xOffset = 0;

                if (levelSize.Width - drawCamera.CameraScreen.Width == 0)
                    parallaxOffset.X = 0;
                else
                    parallaxOffset.X = Convert.ToDecimal(bgnd.ImageSize.Width - drawCamera.CameraScreen.Width) /
                                       Convert.ToDecimal(levelSize.Width - drawCamera.CameraScreen.Width);

                if (levelSize.Height - drawCamera.CameraScreen.Height == 0)
                    parallaxOffset.Y = 0;
                else
                    parallaxOffset.Y = Convert.ToDecimal(bgnd.ImageSize.Height - drawCamera.CameraScreen.Height) /
                                      Convert.ToDecimal(levelSize.Height - drawCamera.CameraScreen.Height);

                var rec = new Rectangle(Convert.ToInt32(parallaxOffset.X * drawCamera.Offset.X) + xOffset,
                          Convert.ToInt32(parallaxOffset.Y * drawCamera.Offset.Y),
                          drawCamera.CameraScreen.Width,
                          drawCamera.CameraScreen.Height);

                bgnd.Draw(rec, ref drawCamera.CameraScreen);
            }
        }

        public void AddParallax(Background bgnd)
        {
            this.BackgroundList.Add(bgnd);
        }
    }
}
