using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class Background
    {
        public String Name;

        // Actual Tile Map
        // LevelMap - X,Y,0 = MapX value to grab the image from the base image
        public int[, ,] LevelMap = new int[0, 0, 2];

        public int ZLevel = 0;

        // The base image to grab the tiles from
        public String BaseImageName;

        public Size ImageSize;

        // the size of each grid square in the base image of tiles.
        public Size GridSize;

        // color to be transparent
        public Color TransparentColour;

        public Bitmap Image;

        public int TileWidth;
        public int TileHeight;

        public char[] ConvertChar = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public Background(string backgroundItem, string map)
        {            
            BackgroundLevelSplit(backgroundItem);

            MakeMap(map);
        }

        public string BackgrondLevelString()
        {
            var sb = new StringBuilder(Name);
            sb.Append(",");

            sb.Append(BaseImageName);
            sb.Append(",");

            sb.Append(ImageSize.Width);
            sb.Append(",");

            sb.Append(ImageSize.Height);
            sb.Append(",");

            sb.Append(GridSize.Width);
            sb.Append(",");

            sb.Append(GridSize.Height);
            sb.Append(",");

            sb.Append(TransparentColour.R);
            sb.Append(",");

            sb.Append(TransparentColour.G);
            sb.Append(",");

            sb.Append(TransparentColour.B);
            sb.Append(",");

            sb.Append(ZLevel);


            return sb.ToString();
        }
       
        protected void BackgroundLevelSplit(string name)
        {
            string[] items = name.Split(new char[] { ',' });

            this.Name = items[0];

            this.BaseImageName = items[1];

            this.ImageSize.Width = Convert.ToInt32(items[2]);
            this.ImageSize.Height = Convert.ToInt32(items[3]);

            this.GridSize.Width = Convert.ToInt32(items[4]);
            this.GridSize.Height = Convert.ToInt32(items[5]);

            this.TransparentColour = Color.FromArgb(Convert.ToInt32(items[6]), Convert.ToInt32(items[7]), Convert.ToInt32(items[8]));

            this.ZLevel = Convert.ToInt32(items[9]);

            this.TileWidth = ImageSize.Width / GridSize.Width;
            this.TileHeight = ImageSize.Height / GridSize.Height;
        }

        protected void MakeMap(string map)
        {
            this.LevelMap = new int[TileWidth, TileHeight, 2];

            int characterNumber = 0;

            Point gridPosition = new Point(0, 0);
            Point gridTransfer = new Point(TileWidth, TileHeight);

            while (gridPosition.Y < gridTransfer.Y)
            {
                while (gridPosition.X < gridTransfer.X)
                {
                    LevelMap[gridPosition.X, gridPosition.Y, 0] = CalculateValue(map[characterNumber]);
                    LevelMap[gridPosition.X, gridPosition.Y, 1] = CalculateValue(map[characterNumber + 1]);

                    characterNumber += 2;
                    gridPosition.X += 1;
                }

                gridPosition.X = 0;
                gridPosition.Y += 1;
            }
        }

        public string GetMap()
        {
            var sb = new StringBuilder();
                        
            var gridTransfer = new Point(TileWidth, TileHeight);
            var gridPosition = new Point(0, 0);

            var gfx = Graphics.FromImage(Image);
            gfx.Clear(TransparentColour);

            while (gridPosition.Y < gridTransfer.Y)
            {
                while (gridPosition.X < gridTransfer.X)
                {
                    sb.Append(CalculateValue(LevelMap[gridPosition.X, gridPosition.Y, 0]) +
                              CalculateValue(LevelMap[gridPosition.X, gridPosition.Y, 1]));
                    
                    gridPosition.X += 1;
                }

                gridPosition.X = 0;
                gridPosition.Y += 1;
            }

            return sb.ToString();
        }

        protected int CalculateValue(char item)
        {
            for (int i = 0; i < ConvertChar.Length; i++)
                if (ConvertChar[i] == item)
                    return i;

            return 0;                
        }

        protected char CalculateValue(int item)
        {
            if (item < 0 || item > ConvertChar.Length)
                return '0';

            return ConvertChar[item];            
        }

        public void Draw()
        {
            this.Image = new Bitmap(ImageSize.Width, ImageSize.Height);

            Bitmap baseImage = new Bitmap(BaseImageName);

            Point gridTransfer = new Point(TileWidth, TileHeight);
            Point gridPosition = new Point(0, 0);

            Graphics gfx = Graphics.FromImage(Image);

            gfx.Clear(TransparentColour);

            int locationX = 0;
            int locationY = 0;

            int atLocationX = 0;
            int atLocationY = 0;

            while (gridPosition.Y < gridTransfer.Y)
            {
                while (gridPosition.X < gridTransfer.X)
                {
                    locationX = LevelMap[gridPosition.X, gridPosition.Y, 0] * GridSize.Width;
                    locationY = LevelMap[gridPosition.X, gridPosition.Y, 1] * GridSize.Height;

                    atLocationX = gridPosition.X * GridSize.Width;
                    atLocationY = gridPosition.Y * GridSize.Height;

                    gfx.DrawImage(baseImage,
                        new Rectangle(atLocationX, atLocationY, GridSize.Width, GridSize.Height),
                        new Rectangle(locationX, locationY, GridSize.Width, GridSize.Height),
                        GraphicsUnit.Pixel);                                                

                    gridPosition.X += 1;
                }

                gridPosition.X = 0;
                gridPosition.Y += 1;
            }

            Image.MakeTransparent(this.TransparentColour);
        }

        public Bitmap Draw(Rectangle drawRectangle)
        {
            if (Image == null)
                Draw();

            var bufferImage = new Bitmap(drawRectangle.Width, drawRectangle.Height);
            var gfx = Graphics.FromImage(bufferImage);

            gfx.Clear(TransparentColour);
            gfx.DrawImage(Image, drawRectangle);

            bufferImage.MakeTransparent(TransparentColour);

            return bufferImage;
        }

        public void Draw(Rectangle srcRectangle, ref Bitmap drawSurface)
        {               
            var destRect = new Rectangle(0, 0, drawSurface.Width, drawSurface.Height);

            Draw(srcRectangle, ref drawSurface, destRect);
        }

        public void Draw(Rectangle srcRectangle, ref Bitmap drawSurface, Rectangle destRectangle)
        {
            if (Image == null)
                Draw();
                        
            var gfx = Graphics.FromImage(drawSurface);

            gfx.Clear(TransparentColour);
            gfx.DrawImage(Image, destRectangle, srcRectangle, GraphicsUnit.Pixel);
        }        
    }
}
