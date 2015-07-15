using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace GameEngine
{
    public class GameObject
    {
        #region Properties
        public string ID = Guid.NewGuid().ToString();

        public string[,] BaseObject = new string[3, 3];

        public string CurrentObject = "";

        public string ObjectType = "";

        public Point3D Location;

        public Point3D Gravity = new Point3D(0, 0, 0);

        public Point3D Speed;

        public bool Ghost = false;

        public int CurrentFrame = 1;

        public int MaxFrame = 0;

        public Size ObjectSize = new Size(-1, -1);

        public bool Dead = false;

        public bool AutoChangeAnimation = true;

        public bool AutoSizeWithAnimation = true;

        public bool ReportEndAnimation = false;

        public bool AnimCompleted = false;

        public Point3D TargetLocation = new Point3D(-1, -1, -1);

        public bool ReachedTarget = false;

        public int Value1 = 1;

        public int Value2 = 0;

        //
        public int Angle = 0;

        public Size3D Scale;

        public bool Mirrored = false;

        public bool Flipped = false;

        public FixedPosition Position = FixedPosition.TopLeft;

        #endregion


        public GameObject(string objType, string animTopLeft, string animTop, string animTopRight, string animLeft, string animStay, string animRight, 
            string animBottomLeft, string animBottom, string animBottomRight, string x, string y, string z, string speedX, string speedY, string speedZ, string ghostYorN)
        {
            this.ObjectType = objType;

            BaseObject[0, 0] = animTopLeft;
            BaseObject[1, 0] = animTop;
            BaseObject[2, 0] = animTopRight;

            BaseObject[0, 1] = animLeft;
            BaseObject[1, 1] = animStay;
            BaseObject[2, 1] = animRight;


            BaseObject[0, 2] = animBottomLeft;
            BaseObject[1, 2] = animBottom;
            BaseObject[2, 2] = animBottomLeft;

            CurrentObject = animStay;

            Location = new Point3D(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(z));

            Speed = new Point3D(Convert.ToInt32(speedX), Convert.ToInt32(speedY), Convert.ToInt32(speedZ));

            this.Ghost = ghostYorN == "Y";
        }

        // Only for one animation
        public GameObject(string objType, string animStay, string x, string y, string z, string speedX, string speedY, string speedZ, string ghostYorN)
        {
            this.AutoChangeAnimation = false;
            this.ObjectType = objType;

            CurrentObject = animStay;

            Location = new Point3D(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(z));

            Speed = new Point3D(Convert.ToInt32(speedX), Convert.ToInt32(speedY), Convert.ToInt32(speedZ));

            this.Ghost = ghostYorN == "Y";
        }

        public void SetGravity (Point3D gravityLevel)
        {
            this.Gravity = gravityLevel;
        }

        public void SetLocationActual(Point3D location)
        {
            this.Location = location;
        }

        public void SetLocationRelative(Point3D offset)
        {
            Location.X += offset.X;
            Location.Y += offset.Y;
            Location.Z += offset.Z;
        }

        public void NextFrame()
        {

            if (CurrentFrame++ > MaxFrame) // Código não visível
            {
                if (ReportEndAnimation) 
                    AnimCompleted = true;
            }

            CurrentFrame = 1;
        }

        public void GravityMove()
        {
            Move(Gravity);            
        }

        public void Move(Point3D offset)
        {
            Location.X += offset.X;
            Location.Y += offset.Y;
            Location.Z += offset.Z;
        }


        public void Move()
        {
            if (TargetLocation.X == -1 && TargetLocation.Y == -1 && TargetLocation.Z == -1)
            {
                Move(Speed);
                MoveAutoChangeAnimation(Speed.X, Speed.Y);
            }
            else
            {
                int directionX = 0;
                int directionY = 0;

                // Move X
                if (Location.X == TargetLocation.X) // Código não visível
                {
                    directionX = Speed.X;
                    Location.X += directionX;

                    if (Location.X > TargetLocation.X)
                        Location.X = TargetLocation.X;

                }
                else if (Location.X > TargetLocation.X)
                {
                    directionX -= Speed.X;
                    Location.X -= directionX;

                    if (Location.X < TargetLocation.X)
                        Location.X = TargetLocation.X;
                }

                // Move Y
                if (Location.Y == TargetLocation.Y) // Código não visível
                {
                    directionY = Speed.Y;
                    Location.Y += directionY;

                    if (Location.Y > TargetLocation.Y)
                        Location.Y = TargetLocation.Y;

                }
                else if (Location.Y > TargetLocation.Y)
                {
                    directionY -= Speed.Y;
                    Location.Y -= directionY;

                    if (Location.X < TargetLocation.X)
                        Location.X = TargetLocation.X;
                }

                // Move Z
                if (Location.Z == TargetLocation.Z) // Código não visível
                {                    
                    Location.Z += Speed.Z;

                    if (Location.Z > TargetLocation.Z)
                        Location.Z = TargetLocation.Z;

                }
                else if (Location.Z > TargetLocation.Z)
                {
                    Location.Z -= Speed.Z;

                    if (Location.Z < TargetLocation.Z)
                        Location.Z = TargetLocation.Z;
                }

                // Reached Target                
                ReachedTarget = Location.X == TargetLocation.X && Location.Y == TargetLocation.Y && Location.Z == TargetLocation.Z;

                MoveAutoChangeAnimation(directionX, directionY);
            }
        }

        private void MoveAutoChangeAnimation(int directionX, int directionY)
        {
            if (AutoChangeAnimation)
            {
                int animX = 1;
                int animY = 1;

                if (directionX == 0) // Código não visível
                {
                    animX = 0;
                }
                else if (directionX > 0)
                    animX = 2;

                if (directionY == 0) // Código não visível
                {
                    animY = 0;
                }
                else if (directionY > 0)
                    animY = 2;

                if (CurrentObject != BaseObject[animX, animY])
                {
                    CurrentObject = BaseObject[animX, animY];

                    MaxFrame = 0;

                    CurrentFrame = 0;
                }
            }
        }

        public void Redraw(Camera cam, ref Hashtable graphicLibrary, ref Hashtable graphicObjects)
        {
            int xOffset = 0;

            var currentObject = graphicObjects[CurrentObject] as GraphicObject;

            if (MaxFrame == 0)
            {                
                MaxFrame = currentObject.NumberOfFrames;

                if (ObjectSize.Width == -1 || AutoSizeWithAnimation)
                {
                    ObjectSize = currentObject.FrameSize;
                }                
            }

            if (cam.CameraStyle == CameraType.Left3D) 
                xOffset += Location.Z;
            else if (cam.CameraStyle == CameraType.Right3D)
                xOffset -= Location.Z;
            else 
                xOffset = 0;

            var location = new Point3D(Location.X - cam.Offset.X + xOffset, Location.Y - cam.Offset.Y, Location.Z);
            currentObject.DrawGraphic(ref graphicLibrary, CurrentFrame, location, ObjectSize, ref cam.CameraScreen);
        }
    }
}
