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

namespace TileTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        xTile.Map map;

        bool isAlGay = true;

        Texture2D spriteSheet;

        xTile.Dimensions.Rectangle m_viewPort;
        XnaDisplayDevice m_xnaDisplayDevice;
        Sprite hero;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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


            map = Content.Load<Map>("Map1");
            map.LoadTileSheets(m_xnaDisplayDevice);

            spriteSheet = Content.Load<Texture2D>(@"DungeonCrawl_ProjectUtumnoTileset");
            hero = new Sprite(new Vector2(32, 32), spriteSheet, new Microsoft.Xna.Framework.Rectangle(288, 161, 32, 32), Vector2.Zero);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            map.DisposeTileSheets(m_xnaDisplayDevice);
            map = null;
        }

        public bool isWall (Layer layer, int x, int y)
        {
            Location loc = new Location(x, y);

            Location q = layer.GetTileLocation(loc);
            if (layer.IsValidTileLocation(q))
            {
                if (layer.Tiles[q].TileIndex == 822)  // 813 or 822
                {
                    return true;
                }
            }

            return false;
        }

        protected bool canMove (Sprite sprite, Layer layer, Vector2 direction)
        {
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

            return false;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
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

            KeyboardState kb = Keyboard.GetState();
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


            Layer layer = map.GetLayer("Test");

            if (kb.IsKeyDown(Keys.A))
            {
                if (canMove(hero, layer, new Vector2(-2, 0)))
                    hero.Location += new Vector2(-2, 0);
            }
            else if (kb.IsKeyDown(Keys.D))
            {
                if (canMove(hero, layer, new Vector2(2, 0)))
                    hero.Location += new Vector2(2, 0);
            }
            else if (kb.IsKeyDown(Keys.W))
            {
                if (canMove(hero, layer, new Vector2(0, -2)))
                    hero.Location += new Vector2(0, -2);
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                if (canMove(hero, layer, new Vector2(0, 2)))
                    hero.Location += new Vector2(0, 2);
            }

            


            Tile p = layer.PickTile(new Location(10, 10), new Size(m_viewPort.Width,m_viewPort.Height));

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
                Location q = layer.GetTileLocation(pointer) ;
                if (layer.IsValidTileLocation(q))
                {
                    Tile pt = layer.Tiles[q.X, q.Y];
                    Location loc = layer.ConvertLayerToMapLocation(q, new Size(m_viewPort.Width, m_viewPort.Height));

                    Window.Title = (q == null ? "" : "" + q.X + " " + q.Y + " " + loc.X + " " + loc.Y + (pt.Properties.Count > 0 ? (String)pt.Properties["type"] : ""));
                }

                
                
                
                
            }

            // Limit ability to view offscreen
            m_viewPort.Location.X = Math.Max(0, m_viewPort.X);
            m_viewPort.Location.Y = Math.Max(0, m_viewPort.Y);
            m_viewPort.Location.X = Math.Min(map.DisplayWidth - m_viewPort.Width, m_viewPort.X);
            m_viewPort.Location.Y = Math.Min(map.DisplayHeight - m_viewPort.Height, m_viewPort.Y);

            map.Update(gameTime.ElapsedGameTime.Milliseconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            map.Draw(m_xnaDisplayDevice, m_viewPort, Location.Origin, false);
            spriteBatch.Begin();
            hero.Draw(spriteBatch, m_viewPort);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
