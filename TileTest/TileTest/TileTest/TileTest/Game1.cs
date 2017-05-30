using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using xTile;
using xTile.Display;
using xTile.Dimensions;
using xTile.Tiles;
using xTile.Layers;
using System.Diagnostics;

namespace TileTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        bool isAlGay = true;
        bool isAlStupid = true;
     /*
       TO DO LIST

            2. Enable damage.
                a. Press space in front of enemies to damage them and reduce enemy health value
            4. Enable game states.
                a. Playing, title screen, game over.+
            5. Make a boss fight. Final boss.
                a. Make a bool for each dungeon.  isDungeon1Complete = false; For every dungeon, it starts false.  When finished it will go true.  If all 4 bool values
                = true, then a secret dungeon will appear with a final boss.
            6. WORK ON MAPS
                a. FINISH EM
            
      */








        //xTile.Map map;
        Texture2D spriteSheet;
        Texture2D heartSprite;
        Texture2D link;

        private Vector2 scoreLocation = new Vector2(800, 10);
        private Vector2 healthLocation = new Vector2(20, 10);

        enum GameItems
        {
            CHEST = 2924,
            POTION = 723,
            STAIRS = 1001,
            WATER = 977,
            MAZE_ENTER = 981,
            DESERT_ENTER = 998,
            DUNGEON_EXIT = 995,
            HELL_ENTER = 996,
            HELL_ENTER2 = 785
        }

        public int score = 0;

        xTile.Dimensions.Rectangle m_viewPort;

        XnaDisplayDevice m_xnaDisplayDevice;
        Sprite hero;
        Sprite heroWep;
        List<Sprite> enemies;
        Sprite mouse;
        Sprite health;
        int healthNum = 6; //Health number variable.  For each damage affect, healthNum -=1.  if healthNum ==0; gameover.  
        SpriteFont pericles14;

        Dictionary<String, Map> maps;
        Dictionary<String, bool> mapsMonstersLoaded;

        String currentMap = "";

        List<int> wallTypes;
        List<int> enemyWallTypes;
        List<int> items;

        enum Directions {  STATIONARY, LEFT, RIGHT, UP, DOWN }

        Directions lastDirection = Directions.STATIONARY;

        RenderTarget2D rt;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 960;//960
            graphics.PreferredBackBufferHeight = 540;//540
            graphics.ApplyChanges();
            //this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();


           
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SongManager.Initialize(this.Content);

            MouseState ms = new MouseState();

            items = new List<int>();
            items.Add(2924);//chest
            items.Add(1001);//stairs
            

            wallTypes = new List<int>();
            wallTypes.Add(821);// 821 is a wall
            wallTypes.AddRange(Enumerable.Range(881, 4));//881 - 884 is lava
            wallTypes.Add(1097);//water level wall
            wallTypes.AddRange(Enumerable.Range(904, 9));
            wallTypes.AddRange(Enumerable.Range(854, 7));
            wallTypes.Add(1092);

            enemyWallTypes = new List<int>();
            enemyWallTypes.AddRange(wallTypes);
            enemyWallTypes.AddRange(Enumerable.Range(891,4));//water grates

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_viewPort = new xTile.Dimensions.Rectangle(0, 0,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight);

            m_xnaDisplayDevice = new XnaDisplayDevice(Content, GraphicsDevice);

            mapsMonstersLoaded = new Dictionary<string, bool>();
            maps = new Dictionary<string, Map>();

            maps.Add("Spawn", Content.Load<Map>("SpawnRoom"));
            maps["Spawn"].LoadTileSheets(m_xnaDisplayDevice);

            maps.Add("maze", Content.Load<Map>("DemoMap"));
            maps["maze"].LoadTileSheets(m_xnaDisplayDevice);

            maps.Add("level2", Content.Load<Map>("Maze2"));
            maps["level2"].LoadTileSheets(m_xnaDisplayDevice);

            maps.Add("water", Content.Load<Map>("Water Level"));
            maps["water"].LoadTileSheets(m_xnaDisplayDevice);

            maps.Add("desert", Content.Load<Map>("DesertRoom"));
            maps["desert"].LoadTileSheets(m_xnaDisplayDevice);

            maps.Add("hell", Content.Load<Map>("HellRoom"));
            maps["hell"].LoadTileSheets(m_xnaDisplayDevice);

            maps.Add("hell2", Content.Load<Map>("HellRoom2"));
            maps["hell2"].LoadTileSheets(m_xnaDisplayDevice);



            //Debug.Assert (!Object.ReferenceEquals(maps["level1"], maps["level2"]));
            //map = Content.Load<Map>("Map1");
            //map.LoadTileSheets(m_xnaDisplayDevice);

            pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");
            heartSprite = Content.Load<Texture2D>(@"Textures\heartSprite");
            spriteSheet = Content.Load<Texture2D>(@"DungeonCrawl_ProjectUtumnoTileset");
            link = Content.Load<Texture2D>(@"Textures\spritesheet");

            
            hero = new Sprite(new Vector2(480, 384), link, new Microsoft.Xna.Framework.Rectangle(232, 0, 32, 15), Vector2.Zero);
            heroWep = new Sprite(new Vector2(480, 384), spriteSheet, new Microsoft.Xna.Framework.Rectangle(1824, 288, 32, 32), Vector2.Zero);
            enemies = new List<Sprite>();

            switchMap("Spawn", new Vector2(480, 384));

            health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 0, 196, 28), Vector2.Zero);

            mouse = new Sprite(new Vector2(ms.X, ms.Y), spriteSheet, new Microsoft.Xna.Framework.Rectangle(32, 0, 11, 11), Vector2.Zero);

            rt = new RenderTarget2D(this.GraphicsDevice, 1024, 768);
        }

        protected void LoadMonsters ()
        {
            //enemies.Clear();

            Layer mlayer = maps[currentMap].GetLayer("Monsters");

            Debug.Assert(mlayer != null, "YOU MUST ADD A Monsters LAYER TO THIS MAP.. Ugh.. noobs");

            mlayer.Visible = false;

            for (int x = 0; x < mlayer.LayerSize.Width; x++)
            {
                for (int y = 0; y < mlayer.LayerSize.Height; y++)
                {
                    Tile t = mlayer.Tiles[x, y];

                    if (t != null)
                    {
                        // Found a monster!
                        int mindex = t.TileIndex;
                        int tiley = t.TileIndex / 64;
                        int tilex = t.TileIndex % 64;
                        // 64 tiles wide

                        enemies.Add(new Enemy(new Vector2(x*32, y*32), spriteSheet, new Microsoft.Xna.Framework.Rectangle(tilex*32, tiley*32, 32, 32), Vector2.Zero, maps[currentMap], enemyWallTypes));
                    }
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (var name in maps.Keys)
            {
                maps[name].DisposeTileSheets(m_xnaDisplayDevice);              
            }
            maps.Clear();
        }

        public bool isWall (Layer layer, int x, int y)
        {
            Location loc = new Location(x, y);

            Location q = layer.GetTileLocation(loc);
            if (layer.IsValidTileLocation(q))
            {
                Tile t = layer.Tiles[q];

                if (t != null && (wallTypes.Contains(t.TileIndex) || (t.Properties.Count > 0 && t.Properties.ContainsKey("type") && t.Properties["type"] == "wall")))  // t.TileIndex == 822 // 813 or 822
                {
                    return true;
                }


            }
            return false;
        }

        public Tuple<bool, Tile> isItem(Layer layer, int x, int y)
        {
            Location loc = new Location(x, y);

            Location i = layer.GetTileLocation(loc);
            if (layer.IsValidTileLocation(i))
            {
                Tile d = layer.Tiles[i];

                

                if (d != null && (items.Contains(d.TileIndex) || (d.Properties.Count > 0 && d.Properties.ContainsKey("type") && d.Properties["type"] == "items")))
                {
                    return new Tuple<bool, Tile>(true, d);
                   
                }

            }
            return new Tuple<bool, Tile>(false, null);
        }
        protected bool canMove (Sprite sprite, Layer layer, Vector2 direction)
        {
            Vector2 location = sprite.Center + direction;
            /*
            Vector2 location = sprite.Location + direction;

            // Now check to see if location is moveable
            if (!isWall(layer, (int)location.X+1, (int)location.Y) && 
                !isWall(layer, (int)location.X-1+sprite.BoundingBoxRect.Width, (int)location.Y) && 
                !isWall(layer, (int)location.X+1, (int)location.Y - 1 + sprite.BoundingBoxRect.Height) && 
                !isWall(layer, (int)location.X-1 + sprite.BoundingBoxRect.Width, (int)location.Y - 1 + sprite.BoundingBoxRect.Height))
            {
                sprite.Location += direction;
                return true;
            }
            */
            if (!isWall(layer, (int)location.X, (int)location.Y))
            {
                return true;
            }
            return false;
        }

        public void switchMap (String mapname, Vector2 position)
        {
            bool newSong = (currentMap != mapname);

            currentMap = mapname;
            hero.Location = position;
            hero.state = SpriteStates.IDLE;
            hero.Velocity = Vector2.Zero;
            m_viewPort.Location.X = 0;
            m_viewPort.Location.Y = 0;

            if (!mapsMonstersLoaded.ContainsKey(currentMap))
            {
                LoadMonsters();
                mapsMonstersLoaded[currentMap] = true;
            }

            if (newSong)
            {
                SongManager.stopSongs();

                switch (currentMap)
                {
                    case "Spawn":
                        SongManager.playSpawnRoom();
                        break;
                    case "maze":
                        SongManager.playMaze();
                        break;
                    case "desert":
                        SongManager.playDesert();
                        break;
                    case "water":
                        SongManager.playWater();
                        break;
                    case "hell":
                        SongManager.playHell();
                        break;
                }
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();



            MouseState ms = Mouse.GetState();
            mouse.Location = new Vector2(ms.X - mouse.BoundingBoxRect.Width, ms.Y);

            if (healthNum == 6)
            {
                health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 0, 196, 28), Vector2.Zero);
            }

            if (healthNum == 5)
            {
                health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 30, 196, 28), Vector2.Zero);
            }

            if (healthNum == 4)
            {
                health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 60, 196, 28), Vector2.Zero);
            }

            if (healthNum == 3)
            {
                health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 90, 196, 28), Vector2.Zero);
            }

            if (healthNum == 2)
            {
                health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 120, 196, 28), Vector2.Zero);
            }

            if (healthNum == 1)
            {
                health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 150, 196, 28), Vector2.Zero);
            }

            if (healthNum == 0)
            {
                health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0), Vector2.Zero);
            }

            // TODO: Add your update logic here
            /*
             *  m_viewPort = new xTile.Dimensions.Rectangle(
                    new xTile.Dimensions.Location(
                        GraphicsDevice.Viewport.TitleSafeArea.Left,
                        GraphicsDevice.Viewport.TitleSafeArea.Top),
                    new xTile.Dimensions.Size(
                        GraphicsDevice.Viewport.TitleSafeArea.Width,
                        GraphicsDevice.Viewport.TitleSafeArea.Height)); 
                        
                         m_viewPort.Location.X += (int)(leftThumbStick.X * 4.0f);
            m_viewPort.Location.Y -= (int)(leftThumbStick.Y * 4.0f);
             */


            if (kb.IsKeyDown(Keys.Right))
            {
                m_viewPort.Location.X += 3;
            }
            else if (kb.IsKeyDown(Keys.Left))
            {
                m_viewPort.Location.X -= 3;
            }
            else if (kb.IsKeyDown(Keys.Down))
            {
                m_viewPort.Location.Y += 3;
            }
            else if (kb.IsKeyDown(Keys.Up))
            {
                m_viewPort.Location.Y -= 3;
            }



            Layer glayer = maps[currentMap].GetLayer("Ground");
            Layer olayer = maps[currentMap].GetLayer("objects");

            foreach (Enemy nme in enemies)
            {
                if (hero.IsBoxColliding(nme.BoundingBoxRect) && maps[currentMap] == nme.Map && hero.state == SpriteStates.IDLE)
                {
                    nme.Wait(1.0f);  // Make enemy wait 1.0 seconds before moving again
                    healthNum--;
                    if (lastDirection == Directions.STATIONARY)
                    {
                        if (canMove(hero, glayer, new Vector2(-64, 0)))
                            hero.AnimateMove(hero.Location + new Vector2(-64, 0), 0.2f);
                        else if (canMove(hero, glayer, new Vector2(64, 0)))
                            hero.AnimateMove(hero.Location + new Vector2(64, 0), 0.2f);
                        else if (canMove(hero, glayer, new Vector2(0, -64)))
                            hero.AnimateMove(hero.Location + new Vector2(0, -64), 0.2f);
                        else if (canMove(hero, glayer, new Vector2(0, 64)))
                            hero.AnimateMove(hero.Location + new Vector2(0, 64), 0.2f);
                    }
                    else
                    {
                        switch (lastDirection)
                        {
                            case Directions.LEFT:
                                if (canMove(hero, glayer, new Vector2(64, 0)))
                                    hero.AnimateMove(hero.Location + new Vector2(64, 0), 0.2f);
                                break;
                            case Directions.RIGHT:
                                if (canMove(hero, glayer, new Vector2(-64, 0)))
                                    hero.AnimateMove(hero.Location + new Vector2(-64, 0), 0.2f);
                                break;
                            case Directions.UP:
                                if (canMove(hero, glayer, new Vector2(0, 64)))
                                    hero.AnimateMove(hero.Location + new Vector2(0, 64), 0.2f);
                                break;
                            case Directions.DOWN:
                                if (canMove(hero, glayer, new Vector2(0, -64)))
                                    hero.AnimateMove(hero.Location + new Vector2(0, -64), 0.2f);
                                break;
                        }
                    }
                }
                nme.Update(gameTime);
            }

            float duration = 0.2f;
            if (hero.state == SpriteStates.IDLE)
            {
                if (kb.IsKeyDown(Keys.A))
                {
                    if (canMove(hero, glayer, new Vector2(-32, 0)))
                    {
                        hero = new Sprite(new Vector2(hero.Location.X, hero.Location.Y), link, new Microsoft.Xna.Framework.Rectangle(262, 0, 32, 15), Vector2.Zero);
                        hero.AnimateMove(hero.Location + new Vector2(-32, 0), duration);
                        lastDirection = Directions.LEFT;
                    }
                }
                else if (kb.IsKeyDown(Keys.D))
                {
                    if (canMove(hero, glayer, new Vector2(32, 0)))
                    {
                        hero = new Sprite(new Vector2(hero.Location.X, hero.Location.Y), link, new Microsoft.Xna.Framework.Rectangle(322, 0, 32, 15), Vector2.Zero);
                        hero.AnimateMove(hero.Location + new Vector2(32, 0), duration);
                        lastDirection = Directions.RIGHT;
                    }

                }
                else if (kb.IsKeyDown(Keys.W))
                {
                    if (canMove(hero, glayer, new Vector2(0, -32)))
                    {
                        hero = new Sprite(new Vector2(hero.Location.X, hero.Location.Y), link, new Microsoft.Xna.Framework.Rectangle(298, 0, 32, 15), Vector2.Zero);
                        hero.AnimateMove(hero.Location + new Vector2(0, -32), duration);
                        lastDirection = Directions.UP;
                    }
                }
                else if (kb.IsKeyDown(Keys.S))
                {
                    if (canMove(hero, glayer, new Vector2(0, 32)))
                    {
                        hero = new Sprite(new Vector2 (hero.Location.X,hero.Location.Y),link, new Microsoft.Xna.Framework.Rectangle(232, 0, 32, 15), Vector2.Zero);
                        hero.AnimateMove(hero.Location + new Vector2(0, 32), duration);
                        lastDirection = Directions.DOWN;
                    }
                }
            }

            

            Tuple<bool, Tile> item = isItem(olayer, (int)hero.Center.X, (int)hero.Center.Y);
            
            if (item.Item1)  // If it is an item
            {
                Tile tile = item.Item2;

                GameItems type = (GameItems)tile.TileIndex;

                switch (type)
                {
                    case GameItems.CHEST:

                        if (kb.IsKeyDown(Keys.F) && !tile.Properties.ContainsKey("empty"))
                        {
                            score += 100;
                            tile.Properties["empty"] = true;
                            tile.TileIndex = 2925;
                        }
                        

                        break;

                    case GameItems.POTION:
                        // For potion, if you land on it, it disappears right after.  You have to be holding F for the health gain to work.  It will disappear automatically no matter what.
                        if (kb.IsKeyDown(Keys.F) && !tile.Properties.ContainsKey("empty") && healthNum < 6 && healthNum > 0 && score >= 100)  //Pressing F to interact does not work.
                        {
                            score -= 100;
                            healthNum += 1;
                            tile.Properties["empty"] = true;
                            tile.TileIndex = 729;
                        }
                        
                        break;

                    case GameItems.WATER:
                        Vector2 dstination = new Vector2(32, 32);
                        if (tile.Properties.ContainsKey("jumpto"))
                        {
                            int[] parts = tile.Properties["jumpto"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            dstination.X = parts[0] * 32;
                            dstination.Y = parts[1] * 32;
                        }

                        if (tile.Properties.ContainsKey("map"))
                        {
                            switchMap(tile.Properties["map"], dstination);
                        }
                        else
                        {
                            switchMap(currentMap, dstination);
                        }
                        break;
                    case GameItems.STAIRS:

                        Vector2 destination = new Vector2(32, 32);
                        if (tile.Properties.ContainsKey("jumpto"))
                        {
                            int[] parts = tile.Properties["jumpto"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            destination.X = parts[0] * 32;
                            destination.Y = parts[1] * 32;
                        }

                        if (tile.Properties.ContainsKey("map"))
                        {
                            switchMap(tile.Properties["map"], destination);
                        }
                        else
                        {
                            switchMap(currentMap, destination);
                        }


                        break;

                    case GameItems.MAZE_ENTER:

                        Vector2 dest = new Vector2(32, 32);
                        if (tile.Properties.ContainsKey("jumpto"))
                        {
                            int[] parts = tile.Properties["jumpto"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            dest.X = parts[0] * 32;
                            dest.Y = parts[1] * 32;
                        }

                        if (tile.Properties.ContainsKey("map"))
                        {
                            switchMap(tile.Properties["map"], dest);
                        }
                        else
                        {
                            switchMap(currentMap, dest);
                        }

                        break;

                    case GameItems.DESERT_ENTER:

                        Vector2 dst = new Vector2(32, 32);
                        if (tile.Properties.ContainsKey("jumpto"))
                        {
                            int[] parts = tile.Properties["jumpto"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            dst.X = parts[0] * 32;
                            dst.Y = parts[1] * 32;
                        }

                        if (tile.Properties.ContainsKey("map"))
                        {
                            switchMap(tile.Properties["map"], dst);
                        }
                        else
                        {
                            switchMap(currentMap, dst);
                        }

                        break;

                    case GameItems.HELL_ENTER:

                        Vector2 dsti = new Vector2(32, 32);
                        if (tile.Properties.ContainsKey("jumpto"))
                        {
                            int[] parts = tile.Properties["jumpto"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            dst.X = parts[0] * 32;
                            dst.Y = parts[1] * 32;
                        }

                        if (tile.Properties.ContainsKey("map"))
                        {
                            switchMap(tile.Properties["map"], dsti);
                        }
                        else
                        {
                            switchMap(currentMap, dsti);
                        }

                        break;

                    case GameItems.HELL_ENTER2:

                        Vector2 dstin = new Vector2(32, 32);
                        if (tile.Properties.ContainsKey("jumpto"))
                        {
                            int[] parts = tile.Properties["jumpto"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            dst.X = parts[0] * 32;
                            dst.Y = parts[1] * 32;
                        }

                        if (tile.Properties.ContainsKey("map"))
                        {
                            switchMap(tile.Properties["map"], dstin);
                        }
                        else
                        {
                            switchMap(currentMap, dstin);
                        }

                        break;

                    case GameItems.DUNGEON_EXIT:

                        Vector2 dstnation = new Vector2(32, 32);
                        if (tile.Properties.ContainsKey("jumpto"))
                        {
                            int[] parts = tile.Properties["jumpto"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            destination.X = parts[0] * 32;
                            destination.Y = parts[1] * 32;
                        }

                        if (tile.Properties.ContainsKey("map"))
                        {
                            switchMap(tile.Properties["map"], dstnation);
                        }
                        else
                        {
                            switchMap(currentMap, dstnation);
                        }


                        break;


                }
            }


            item = isItem(glayer, (int)hero.Center.X, (int)hero.Center.Y);

            if (item.Item1)  // If it is an item
            {
                Tile tile = item.Item2;

                GameItems type = (GameItems)tile.TileIndex;

                
            }

            Vector2 viewOffs = new Vector2(m_viewPort.Location.X, m_viewPort.Location.Y);
            Vector2 screenPos = hero.Center - viewOffs;
            Microsoft.Xna.Framework.Rectangle moveBox = new Microsoft.Xna.Framework.Rectangle(this.Window.ClientBounds.Width / 3, this.Window.ClientBounds.Height / 3, this.Window.ClientBounds.Width / 3, this.Window.ClientBounds.Height / 3);
            //Microsoft.Xna.Framework.Rectangle moveBox = new Microsoft.Xna.Framework.Rectangle(300, 300, 300, 200);

            if (!moveBox.Contains((int)screenPos.X, (int)screenPos.Y))
            {
                if (screenPos.X < moveBox.Left)
                {
                    m_viewPort.Location.X -= 4;
                   
                }
                if (screenPos.X > moveBox.Right)
                {
                    m_viewPort.Location.X += 4;
                    
                }
                if (screenPos.Y > moveBox.Bottom)
                {
                    m_viewPort.Location.Y += 4;
                    
                }
                if (screenPos.Y < moveBox.Top)
                {
                    m_viewPort.Location.Y -= 4;
                    
                }
            }


            //Tile p = layer.PickTile(new Location(10, 10), new Size(m_viewPort.Width,m_viewPort.Height));


            Location pointer = new Location(ms.X + m_viewPort.X, ms.Y + m_viewPort.Y);

            if (ms.LeftButton == ButtonState.Pressed)
            {

                /*Tile pt = layer.PickTile(pointer, new Size(m_viewPort.Width, m_viewPort.Height));
                if (pt != null && pt.Properties.Count > 0)
                {
                    Window.Title = "w00t";
                }
                */
                //Window.Title = (q == null ? "" : "" + q.Properties.Count);
                Location q = glayer.GetTileLocation(pointer);
                if (glayer.IsValidTileLocation(q))
                {
                    Tile pt = glayer.Tiles[q.X, q.Y];
                    Location loc = glayer.ConvertLayerToMapLocation(q, new Size(m_viewPort.Width, m_viewPort.Height));

                    Window.Title = (q == null ? "" : "" + q.X + " " + q.Y + " " + loc.X + " " + loc.Y + (pt.Properties.Count > 0 ? (String)pt.Properties["type"] : ""));
                }

                
                
                
                
            }

            

                // Limit ability to view offscreen
                m_viewPort.Location.X = Math.Max(0, m_viewPort.X);
            m_viewPort.Location.Y = Math.Max(0, m_viewPort.Y);
            m_viewPort.Location.X = Math.Min(maps[currentMap].DisplayWidth - m_viewPort.Width, m_viewPort.X);
            m_viewPort.Location.Y = Math.Min(maps[currentMap].DisplayHeight - m_viewPort.Height, m_viewPort.Y);


            


            maps[currentMap].Update(gameTime.ElapsedGameTime.Milliseconds);
            hero.Update(gameTime);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.Begin();
            maps[currentMap].Draw(m_xnaDisplayDevice, m_viewPort, Location.Origin, false);
            hero.Draw(spriteBatch, m_viewPort);

            foreach (Enemy nme in enemies)
            {
                nme.Draw(spriteBatch, m_viewPort, maps[currentMap]);
            }

            health.Draw(spriteBatch);

            
                spriteBatch.DrawString(
                    pericles14, "Health:", healthLocation, Color.White);
            
            spriteBatch.DrawString(
                    pericles14,
                    "Score: " + score , scoreLocation, Color.White);

            mouse.Draw(spriteBatch);

            spriteBatch.End();


            /*
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(rt, new Microsoft.Xna.Framework.Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), new Microsoft.Xna.Framework.Rectangle(0, 0, 1024/2, 768/2), Color.White);
            spriteBatch.End();
            */
            base.Draw(gameTime);
        }
    }
}
