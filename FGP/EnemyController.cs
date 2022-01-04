#region 'Using' information
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace FGP
{
    class EnemyController
    {
        private static double timer = 2D; // The time between enemy spawns.
        private static double maxTime = 2D;
        private static Random rand = new Random();

        public static void Update(GameTime gameTime, Texture2D spriteSheet)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds; // Counts down in real time, and spawns an enemy when it reaches 0.

            if (timer <= 0)
            {
                int side = rand.Next(4); // Chooses a corner of the screen to spawn an enemy in.

                switch(side)
                {
                    case 0: // A potential spawn point for enemies - the top left corner of the screen.
                        Enemy.enemies.Add(new Enemy(new Vector2(-500, rand.Next(-500, 2000)), spriteSheet, Color.White)); 
                        break;

                    case 1: // A potential spawn point for enemies - the top right corner of the screen.
                        Enemy.enemies.Add(new StrongerEnemy(new Vector2(2000, rand.Next(-500, 2000)), spriteSheet, Color.Red));
                        break;

                    case 2: // A potential spawn point for enemies - the bottom left corner of the screen.
                        Enemy.enemies.Add(new StrongestEnemy(new Vector2(rand.Next(-500, 2000), -500), spriteSheet, Color.Purple));
                        break;

                    case 3: // A potential spawn point for enemies - the bottom right corner of the screen.
                        Enemy.enemies.Add(new Enemy(new Vector2(rand.Next(-500, 2000), 2000), spriteSheet, Color.White));
                        break;
                }

                timer = maxTime; // Resets the timer every time an enemy spawns, so that they can spawn endlessly.

                if (maxTime > 0.5)
                {
                    maxTime -= 0.05D; // Every time an enemy spawns, maxTime decreases by 0.05 seconds so that enemies will spawn more frequently.
                }
            }
        }
    }
}