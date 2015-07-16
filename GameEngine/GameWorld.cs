using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GameEngine
{
    public class GameWorld
    {
        #region Constants

        public const string ENDGRAPHICS = "ENDGRAPHICS";
        public const string ENDANIMATIONS = "ENDANIMATIONS";
        public const string ENDBACKGROUNDS = "ENDBACKGROUNDS";
        public const string ENDOBJECTS = "ENDOBJECTS";
        public const string ENDCAMERAS = "ENDCAMERAS";
               

        #endregion

        #region Events

        /// <summary>
        /// Fire if object hits the edge of the level, gives the object and the edge (top, bottom, right, left)
        /// </summary>
        public event CollideWall ItemWallCollision;

        /// <summary>
        /// Fires if two objects collide each other
        /// </summary>
        public event CollideObject ItemObjectCollision;

        /// <summary>
        /// Fires when an object is currently following a target and reach it.
        /// </summary>
        public event ItemComplete ReachedTarget;

        /// <summary>
        /// Fires when some object completed all frames
        /// </summary>
        public event ItemComplete AnimationComplete;

        /// <summary>
        /// Fires if an object hits the bump map.
        /// </summary>
        public event ItemComplete TouchedBumpMap;

        /// <summary>
        /// Fires if an object hits the bump map during a gravity fall.
        /// </summary>
        public event ItemComplete GravityTouchedBumpMap;

        #endregion

        #region Props

        /// <summary>
        /// Size of entire level in pixes
        /// </summary>
        public Size LevelSize = new Size(2048, 1456);

        /// <summary>
        /// Z-Depth
        /// </summary>
        public int LevelDepth = 1;

        /// <summary>
        /// The bump Map Array Data in blocks; 0 = can travel through this block, 1 = can travel
        /// </summary>
        public int[,] BumpMap;

        /// <summary>
        /// The size of each block at BumpData Array
        /// </summary>
        public Point BumpMapSize = new Point(32, 32);

        /// <summary>
        /// List of cameras
        /// </summary>
        public ArrayList WorldCameras = new ArrayList();

        /// <summary>
        /// A list of graphics used by the graphic objects in the game
        /// </summary>
        public Hashtable GraphicLibrary = new Hashtable();

        /// <summary>
        /// A list of all animations used in the game.
        /// </summary>
        public Hashtable GraphicObjects = new Hashtable();

        /// <summary>
        /// A list of all objects within the level file.
        /// </summary>
        public ArrayList WorldObjects = new ArrayList();

        /// <summary>
        /// The background object - a parallax scroller. Draws before all the objects in the environment.
        /// </summary>
        public Parallax Background = new Parallax();

        /// <summary>
        /// The foreground object - a parallax scroller. Draws after all the objects in the environment.
        /// </summary>
        public Parallax Foreground = new Parallax();

        #endregion

        #region Methods

        /// <summary>
        /// Load the level file into the game world
        /// </summary>
        /// <param name="levelFile"></param>
        public void LoadLevel(string levelFile)
        {
            using (var sr = new StreamReader(levelFile))
            {
                string line = sr.ReadLine();

                string[] levelSizeItems = line.Split(new char[] { ',' });

                //size of the level

                LevelSize.Width = Convert.ToInt32(levelSizeItems[0]);
                LevelSize.Height = Convert.ToInt32(levelSizeItems[1]);
                LevelDepth = Convert.ToInt32(levelSizeItems[0]);

                Bitmap temp;
                Color transparentColour;

                line = sr.ReadLine();
                while (line != ENDGRAPHICS)
                {
                    levelSizeItems = line.Split(new char[] { ',' });

                    temp = new Bitmap("images\\" + levelSizeItems[1]);

                    int r = Convert.ToInt32(levelSizeItems[2]);
                    int g = Convert.ToInt32(levelSizeItems[3]);
                    int b = Convert.ToInt32(levelSizeItems[4]);
                    transparentColour = Color.FromArgb(r, g, b);

                    temp.MakeTransparent(transparentColour);

                    //image name name, bitmap
                    GraphicLibrary.Add(levelSizeItems[0], temp);

                    line = sr.ReadLine();
                }

                bool vertical = false;
                line = sr.ReadLine();
                while (line != ENDANIMATIONS)
                {
                    levelSizeItems = line.Split(new char[] { ',' });

                    vertical = levelSizeItems[7] == "Y";

                    var oname = levelSizeItems[0];
                    var image = levelSizeItems[1];
                    var local = new Point(Convert.ToInt32(levelSizeItems[2]), Convert.ToInt32(levelSizeItems[3]));
                    var size = new Size(Convert.ToInt32(levelSizeItems[4]), Convert.ToInt32(levelSizeItems[5]));
                    int numFrames = Convert.ToInt32(levelSizeItems[6]);

                    var go = new GraphicObject(image, size, local, numFrames, vertical);

                    GraphicObjects.Add(oname, go);

                    line = sr.ReadLine();
                }

                Background tmpBgnd;
                line = sr.ReadLine();
                while (line != ENDBACKGROUNDS)
                {
                    string map = sr.ReadLine();

                    tmpBgnd = new Background(line, map);

                    Background.AddParallax(tmpBgnd);

                    line = sr.ReadLine();
                }

                // Next the bump map
                line = sr.ReadLine();

                levelSizeItems = line.Split(new char[] { ',' });

                BumpMapSize.X = Convert.ToInt32(levelSizeItems[0]);
                BumpMapSize.Y = Convert.ToInt32(levelSizeItems[1]);

                int width = LevelSize.Width / BumpMapSize.X;
                int height = LevelSize.Height / BumpMapSize.Y;
                BumpMap = new int[width, height];

                var gridTransfer = new Point(width, height);
                var gridPosition = new Point(0, 0);

                int bump = 0;
                while (gridPosition.Y < gridTransfer.Y) // intuitivo
                {
                    while (gridPosition.X < gridTransfer.X)
                    {
                        if (levelSizeItems[2][bump] == '0')
                            BumpMap[gridPosition.X, gridPosition.Y] = 0;
                        else
                            BumpMap[gridPosition.X, gridPosition.Y] = 1;

                        bump++;
                        gridPosition.X++;
                    }

                    gridPosition.X = 0;
                    gridPosition.Y++;
                }

                // Next the objects
                line = sr.ReadLine();

                while (line != ENDOBJECTS)
                {
                    levelSizeItems = line.Split(new char[] { ',' });

                    if (levelSizeItems.Length == 17)
                    {
                        var objType = levelSizeItems[0];

                        var animTopLeft = levelSizeItems[1];
                        var animTop = levelSizeItems[2];
                        var animTopRight = levelSizeItems[3];
                        var animLeft = levelSizeItems[4];
                        var animStay = levelSizeItems[5];
                        var animRight = levelSizeItems[6];

                        var animBottomLeft = levelSizeItems[7];
                        var animBottom = levelSizeItems[8];
                        var animBottomRight = levelSizeItems[9];

                        var x = levelSizeItems[10];
                        var y = levelSizeItems[11];
                        var z = levelSizeItems[12];
                        var spx = levelSizeItems[13];
                        var spy = levelSizeItems[14];
                        var spz = levelSizeItems[15];
                        var ghost = levelSizeItems[16];

                        var go = new GameObject(objType, animTopLeft, animTop, animTopRight, animLeft, animStay, animRight, animBottomLeft, animBottom, animBottomRight, x, y, z, spx, spy, spz, ghost);
                        WorldObjects.Add(go);
                    }
                    else
                    {
                        var objType = levelSizeItems[0];

                        var animStay = levelSizeItems[1];

                        var x = levelSizeItems[2];
                        var y = levelSizeItems[3];
                        var z = levelSizeItems[4];
                        var spx = levelSizeItems[5];
                        var spy = levelSizeItems[6];
                        var spz = levelSizeItems[7];
                        var ghost = levelSizeItems[8];

                        var go = new GameObject(objType, animStay, x, y, z, spx, spy, spz, ghost);
                        WorldObjects.Add(go);
                    }

                    line = sr.ReadLine();
                }


                // CAMERAS
                line = sr.ReadLine();
                while (line != ENDCAMERAS)
                {
                    levelSizeItems = line.Split(new char[] { ',' });

                    var camType = CameraType.Standard;

                    if (levelSizeItems[6] == "L")
                        camType = CameraType.Left3D;
                    else if (levelSizeItems[6] == "R")
                        camType = CameraType.Right3D;

                    var resolution = new Size(Convert.ToInt32(levelSizeItems[0]), Convert.ToInt32(levelSizeItems[1]));
                    var drawLocation = new Rectangle(Convert.ToInt32(levelSizeItems[2]), Convert.ToInt32(levelSizeItems[3]), Convert.ToInt32(levelSizeItems[4]), Convert.ToInt32(levelSizeItems[5]));

                    var camera = new Camera(resolution, drawLocation, camType);

                    WorldCameras.Add(camera);
                }
            }
        }


        public void SetGravity(string group, bool includeGhosts, Point3D gravityLevel)
        {
            foreach(GameObject item in WorldObjects)
            {
                if (includeGhosts || !item.Ghost)
                {
                    if (group == "All")
                    {
                        item.SetGravity(gravityLevel);
                    }
                    else
                    {
                        if (item.ObjectType == group)
                            item.SetGravity(gravityLevel);
                    }
                }                
            }
        }

        /// <summary>
        /// Draws the current environment into all cameras
        /// </summary>
        public void Frame()
        {
            foreach (Camera cam in WorldCameras)
            {
                // Check boundaries
                var offset = new Point3D(cam.Offset.X, cam.Offset.Y, cam.Offset.Z);

                if (offset.X > LevelSize.Width) // intuitivo
                {
                    offset.X = 0;
                } 
                else if (cam.Offset.X + cam.CameraScreen.Width > LevelSize.Width)
                {
                    offset.X = LevelSize.Width - cam.CameraScreen.Width;
                }

                if (offset.Y > LevelSize.Height) // intuitivo
                {
                    offset.Y = 0;
                }
                else if (cam.Offset.Y + cam.CameraScreen.Height > LevelSize.Height)
                {
                    offset.Y = LevelSize.Height - cam.CameraScreen.Height;
                }

                cam.MoveCameraActual(offset);

                if (cam.ShowBackground)
                    Background.Redraw(cam, LevelSize);

                // Draw items in z-order
                int currentZOrder = cam.StartZDraw;
                while (currentZOrder < cam.EndZDraw) // intuitivo
                {
                    foreach(GameObject item in WorldObjects)
                    {
                        if (item.Location.Z == currentZOrder)
                            item.Redraw(cam, ref GraphicLibrary, ref GraphicObjects);
                    }

                    currentZOrder++;
                }

                // Draw Foreground

                Foreground.Redraw(cam, LevelSize);
            }
        }

        /// <summary>
        /// Moves every item in the world objects one by one frame of animation. Fires AnimationCompleted event handler.
        /// </summary>
        public void AnimateItems()
        {
            int counter = 0;
                        

            while (counter < WorldObjects.Count)
            {
                GameObject go = WorldObjects[counter] as GameObject;
                go.NextFrame();

                if (go.AnimCompleted)
                {
                    if (AnimationComplete != null)
                        AnimationComplete(ref go);

                    WorldObjects[counter] = go;
                }

                counter++;
            }
        }


        /// <summary>
        /// Check to see if any part of an object  has toouched a '1' inside of bump map array. All four corners of the objet are tested.        
        /// 
        /// </summary>
        /// <param name="location">Object location</param>
        /// <param name="objSize">Object size</param>
        /// <returns>True if is has touched</returns>
        public bool CheckBumpMap(ref Point3D location, ref Size objSize)
        {
            if (BumpMapSize.X > 0) // intuitivo
            {
                if (location.X > 0 && location.Y > 0)
                {
                    // test two corners
                    int leftX = location.X / BumpMapSize.X;
                    int leftY = location.Y / BumpMapSize.Y;

                    int rightX = (location.X + objSize.Width) / BumpMapSize.X;
                    int rightY = (location.Y + objSize.Height) / BumpMapSize.Y;

                    if (BumpMap[leftX, leftY] == 0 && BumpMap[rightX, rightY] == 0)
                        return false;

                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}
