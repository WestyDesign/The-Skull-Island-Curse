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
    enum Dir // Contains the directions that the player and enemies can move in - only four so that the game is a bit more challenging.
    { Down, Up, Left, Right }

    class Projectile
    {
        #region Variables
        public static List<Projectile> projectiles = new List<Projectile>(); // Allows the projectile class to be passed into ForEach loops in Game1.cs.
        public int radius = 15; // The size / hitbox of projectiles.

        private Vector2 position;
        private int bulletSpeed = 400; // The speed at which projectiles travel.
        private int increasedBulletSpeed = 600; // A secondary speed for bullets to travel at - after reaching a high enough score.
        private Dir direction;
        private bool collided = false;
        private bool poweredUp = false;

        #endregion

        #region Properties

        public Vector2 Position
        { get { return position; } }

        public bool Collided
        { 
            get { return collided; }
            set { collided = value; }
        }

        public bool PoweredUp
        {
            get { return poweredUp; }
            set { poweredUp = value; }
        }

        #endregion

        #region Methods

        public Projectile(Vector2 newPos, Dir newDir)
        {
            position = newPos;
            direction = newDir;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(poweredUp == true)
                {
                    switch (direction) // Bullets will only move in the direction that they're fired from.
                    {
                        case Dir.Up:
                            position.Y -= increasedBulletSpeed * dt;
                            break;
                        case Dir.Down:
                            position.Y += increasedBulletSpeed * dt;
                            break;
                        case Dir.Left:
                            position.X -= increasedBulletSpeed * dt;
                            break;
                        case Dir.Right:
                            position.X += increasedBulletSpeed * dt;
                            break;

                    }
                }
                else
                {
                    switch (direction) // Bullets will only move in the direction that they're fired from.
                    {
                        case Dir.Up:
                            position.Y -= bulletSpeed * dt;
                            break;
                        case Dir.Down:
                            position.Y += bulletSpeed * dt;
                            break;
                        case Dir.Left:
                            position.X -= bulletSpeed * dt;
                            break;
                        case Dir.Right:
                            position.X += bulletSpeed * dt;
                            break;

                    }
                }
        }

        #endregion
    }
}
