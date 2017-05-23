using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile;
using xTile.Display;
using xTile.Dimensions;
using xTile.Tiles;
using xTile.Layers;
using System.Diagnostics;

namespace TileTest
{
    public enum EnemyDirection { UP, DOWN, LEFT, RIGHT };

    class Enemy : Sprite 
    {
        private Map map;
        private List<int> wallTypes;
        private EnemyDirection dir;
        private Random rand;
        private bool switchDirections = false;
        private float duration = 0.4f;

        public Enemy(
            Vector2 location,
            Texture2D texture,
            Microsoft.Xna.Framework.Rectangle initialFrame,
            Vector2 velocity,
            Map map,
            List<int> wallTypes) : base(location, texture, initialFrame, velocity)
        {
            this.map = map;
            this.wallTypes = wallTypes;
            dir = EnemyDirection.DOWN;
            rand = Randnum.rand;
        }

        public bool isWall(Layer layer, int x, int y)
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

        protected bool canMove(Layer layer, Vector2 direction)
        {
            Vector2 location = this.Center + direction;

            if (!isWall(layer, (int)location.X, (int)location.Y))
            {
                return true;
            }
            return false;
        }

        private bool processMovement (Layer glayer)
        {
            

            if (dir == EnemyDirection.LEFT)
            {
                if (canMove(glayer, new Vector2(-32, 0)))
                    this.AnimateMove(this.Location + new Vector2(-32, 0), duration);
                else
                    return false;
            }
            else if (dir == EnemyDirection.RIGHT)
            {
                if (canMove(glayer, new Vector2(32, 0)))
                    this.AnimateMove(this.Location + new Vector2(32, 0), duration);
                else
                    return false;

            }
            else if (dir == EnemyDirection.UP)
            {
                if (canMove(glayer, new Vector2(0, -32)))
                    this.AnimateMove(this.Location + new Vector2(0, -32), duration);
                else
                    return false;

            }
            else if (dir == EnemyDirection.DOWN)
            {
                if (canMove(glayer, new Vector2(0, 32)))
                    this.AnimateMove(this.Location + new Vector2(0, 32), duration);
                else
                    return false;

            }

            return true;
        }

        public void Draw (SpriteBatch sb, xTile.Dimensions.Rectangle m_viewPort, Map currentMap)
        {
            if (this.map == currentMap)
                base.Draw(sb, m_viewPort);
        }

        public bool IsBoxColliding(Microsoft.Xna.Framework.Rectangle OtherBox, Map currentMap)
        {
            return this.map == currentMap && base.IsBoxColliding(OtherBox);
        }

        public override void Update(GameTime gameTime)
        {
            Layer glayer = map.GetLayer("Ground");

            int dirChangeProb = 70;

            if (state == SpriteStates.IDLE)
            {
                int randNum = rand.Next(0, 100);
                switch (dir)
                {
                    case EnemyDirection.UP:
                        if (randNum >= dirChangeProb || switchDirections)
                        {
                            // Change directions
                            while (dir == EnemyDirection.UP)
                            {
                                dir = (EnemyDirection)rand.Next(0, 4);
                            }
                        }
                        break;
                    case EnemyDirection.DOWN:
                        if (randNum >= dirChangeProb || switchDirections)
                        {
                            // Change directions
                            while (dir == EnemyDirection.DOWN)
                            {
                                dir = (EnemyDirection)rand.Next(0, 4);
                            }
                        }

                        break;
                    case EnemyDirection.LEFT:
                        if (randNum >= dirChangeProb || switchDirections)
                        {
                            // Change directions
                            while (dir == EnemyDirection.LEFT)
                            {
                                dir = (EnemyDirection)rand.Next(0, 4);
                            }
                        }

                        break;
                    case EnemyDirection.RIGHT:

                        if (randNum > dirChangeProb || switchDirections)
                        {
                            // Change directions
                            while (dir == EnemyDirection.RIGHT)
                            {
                                dir = (EnemyDirection)rand.Next(0, 4);
                            }
                        }

                        break;
                }

                switchDirections = !processMovement(glayer);
            }

            base.Update(gameTime);
        }
    }
}
