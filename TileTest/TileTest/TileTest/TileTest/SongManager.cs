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
    class SongManager
    {
        //initialize sound files here.
        private static Song maze;
        private static Song desert;
        private static Song water;
        private static Song hell;
        private static Song spawnroom;



        public static void Initialize(ContentManager content)
        {
            try
            {
                // load song files here.
                spawnroom = content.Load<Song>(@"Songs\overworld");

                maze = content.Load<Song>(@"Songs\maze");
                desert = content.Load<Song>(@"Songs\sandsea");
                water = content.Load<Song>(@"Songs\waterTheme");
                hell = content.Load<Song>(@"Songs\hellTheme");
                MediaPlayer.IsRepeating = true;
            }
            catch
            {
                Debug.Write("SoundManager Initialization Failed");
            }
        }

        public static void playSpawnRoom()
        {
            try
            {
                MediaPlayer.Play(spawnroom);
            }

            catch
            {
                Debug.Write("spawnroom failed");
            }
        }

        public static void playMaze()
        {
            try
            {
                MediaPlayer.Play(maze);
            }

            catch
            {
                Debug.Write("maze failed");
            }
        }

        public static void playDesert()
        {
            try
            {
                MediaPlayer.Play(desert);
            }

            catch
            {
                Debug.Write("desert failed");
            }
        }

        public static void playWater()
        {
            try
            {
                MediaPlayer.Play(water);
            }

            catch
            {
                Debug.Write("water failed");
            }
        }

        public static void playHell()
        {
            try
            {
                MediaPlayer.Play(hell);
            }

            catch
            {
                Debug.Write("hell failed");
            }
        }

        public static void stopSongs()
        {
            MediaPlayer.Stop();
        }
    }
}
