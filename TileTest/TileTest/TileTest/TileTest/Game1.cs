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

        //xTile.Map map;
        Texture2D spriteSheet;
        Texture2D heartSprite;
        Texture2D link;

        private Vector2 scoreLocation = new Vector2(800, 10);
        private Vector2 healthLocation = new Vector2(20, 10);

        enum GameItems
        {
            CHEST = 2924,
            POTION = 300,// ?????
            STAIRS = 1001
        }

        public int score = 0;

        xTile.Dimensions.Rectangle m_viewPort;

        XnaDisplayDevice m_xnaDisplayDevice;
        Sprite hero;
        Sprite health;
        int healthNum = 6; //Health number variable.  For each damage affect, healthNum -=1.  if healthNum ==0; gameover.  
        SpriteFont pericles14;

        Dictionary<String, Map> maps;
        String currentMap = "level1";

        List<int> wallTypes;
        List<int> items;

        RenderTarget2D rt;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 960;//960
            graphics.PreferredBackBufferHeight = 540;//540
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
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

            items = new List<int>();
            items.Add(2924);//chest
            items.Add(1001);//stairs

            wallTypes = new List<int>();
            wallTypes.Add(821);// 821 is a wall
            wallTypes.Add(881);//881 - 884 is lava
            wallTypes.Add(882);
            wallTypes.Add(883);
            wallTypes.Add(884);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_viewPort = new xTile.Dimensions.Rectangle(0, 0,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight);

            m_xnaDisplayDevice = new XnaDisplayDevice(Content, GraphicsDevice);

            maps = new Dictionary<string, Map>();
            maps.Add("level1", Content.Load<Map>("DemoMap"));
            maps["level1"].LoadTileSheets(m_xnaDisplayDevice);

            maps.Add("level2", Content.Load<Map>("DemoMap2"));
            maps["level2"].LoadTileSheets(m_xnaDisplayDevice);

            //Debug.Assert (!Object.ReferenceEquals(maps["level1"], maps["level2"]));
            //map = Content.Load<Map>("Map1");
            //map.LoadTileSheets(m_xnaDisplayDevice);

            pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");
            heartSprite = Content.Load<Texture2D>(@"Textures\heartSprite");
            spriteSheet = Content.Load<Texture2D>(@"DungeonCrawl_ProjectUtumnoTileset");
            link = Content.Load<Texture2D>(@"Textures\spritesheet");

            
            hero = new Sprite(new Vector2(32, 32), link, new Microsoft.Xna.Framework.Rectangle(232, 0, 32, 15), Vector2.Zero);


            health = new Sprite(new Vector2(105, 10), heartSprite, new Microsoft.Xna.Framework.Rectangle(0, 0, 196, 28), Vector2.Zero);

            rt = new RenderTarget2D(this.GraphicsDevice, 1024, 768);
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

                if (t != null && (wallTypes.Contains(t.TileIndex) || (t.Properties.Count > 0 && t.Properties["type"] == "wall")))  // t.TileIndex == 822 // 813 or 822
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

        public void switchMap (String mapname)
        {

            currentMap = mapname;
            hero.Location = new Vector2(32, 32);
            hero.state = SpriteStates.IDLE;
            hero.Velocity = Vector2.Zero;
            m_viewPort.Location.X = 0;
            m_viewPort.Location.Y = 0;
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

            float duration = 0.2f;
            if (kb.IsKeyDown(Keys.A))
            {
                if (canMove(hero, glayer, new Vector2(-32, 0)))
                    hero.AnimateMove(hero.Location + new Vector2(-32, 0), duration);
            }
            else if (kb.IsKeyDown(Keys.D))
            {
                if (canMove(hero, glayer, new Vector2(32, 0)))
                    hero.AnimateMove(hero.Location + new Vector2(32, 0), duration);
            }
            else if (kb.IsKeyDown(Keys.W))
            {
                if (canMove(hero, glayer, new Vector2(0, -32)))
                    hero.AnimateMove(hero.Location + new Vector2(0, -32), duration);
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                if (canMove(hero, glayer, new Vector2(0, 32)))
                    hero.AnimateMove(hero.Location + new Vector2(0, 32), duration);
            }

            Tuple<bool, Tile> item = isItem(olayer, (int)hero.Center.X, (int)hero.Center.Y);
            
            if (item.Item1)  // If it is an item
            {
                Tile tile = item.Item2;

                GameItems type = (GameItems)tile.TileIndex;

                switch (type)
                {
                    case GameItems.CHEST:

                        if (!tile.Properties.ContainsKey("empty"))
                            score += 100;

                        tile.Properties["empty"] = true;
                        tile.TileIndex = 0;

                        

                        break;

                    case GameItems.POTION:

                        if(healthNum < 6 && healthNum > 0 && score >=100)
                        {
                            healthNum += 1;
                            score -= 100;
                        }
                        break;

                    case GameItems.STAIRS:
                        switchMap("level2");

                        break;
                }
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

            MouseState ms = Mouse.GetState();
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
            health.Draw(spriteBatch);

            spriteBatch.DrawString(
                pericles14, "Health:", healthLocation, Color.White);

            spriteBatch.DrawString(
                    pericles14,
                    "Score: " + score , scoreLocation, Color.White);


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
