using System;

namespace GameEngine
{
    
    public delegate void CollideWall(ref GameObject obj, WallCollision wall);

    public delegate void CollideObject(ref GameObject obj, ref GameObject obj2);

    public delegate void ItemComplete(ref GameObject obj);

    public struct Point3D
    {
        public int X;

        public int Y;

        public int Z;

        public Point3D(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }           
    }

    public struct DecimalPoint
    {
        public Decimal X;

        public Decimal Y;
    }

    public struct Size3D
    {
        public int Width;

        public int Height;

        public int Depth;
    }

    public enum CameraType
    {
        Standard,
        Left3D,
        Right3D
    }

    public enum GraphicType
    {
        Standard, ScaleX, ScaleY, ScaleZ, ScaleXY, ScaleXZ, ScaleYZ, ScaleXYZ
    }

    public enum FixedPosition
    {
        TopLeft, MiddleLeft, BottomLeft, TopCentre, MiddleCentre, BottomCentre, TopRight, MiddleRight, BottomRight
    }

    public enum GameObjectType
    {
        Mobile, Stationary
    }

    public enum WallCollision
    {
        Top, Left, Right, Bottom
    }
}
