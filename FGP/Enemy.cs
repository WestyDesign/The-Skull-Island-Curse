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
    class Enemy
    {
        public static List<Enemy> enemies = new List<Enemy>(); // Allows the enemy class to be passed into ForEach loops in Game1.cs.
        public int NumberOfHits { get; set; }
        public int Radius => radius;

        protected Vector2 position = new Vector2(0, 0);
        protected int speed = 100;

        protected int radius = 30;
        protected bool dead = false;
        protected Color color;

        public SpriteAnimation anim;

        public Enemy(Vector2 newPos, Texture2D spriteSheet, Color color)
        {
            NumberOfHits = 1; // Regular enemies can be killed in one hit.
            position = newPos;
            anim = new SpriteAnimation(spriteSheet, 10, color, 6);
        }

        public Vector2 Position
        { get { return position; } }

        public bool Dead
        { 
            get { return dead; }
            set { dead = value; }        
        }

        public void Update(GameTime gameTime, Vector2 playerPos, bool isPlayerDead)
        {
            anim.Position = new Vector2(position.X - 48, position.Y - 66); // Centres the enemy's animation and allows for multiple enemies on-screen.
            anim.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(!isPlayerDead) // Only moves enemies towards the player if the player is alive.
            {
                Vector2 moveDir = playerPos - position; // Moves the enemy towards the player.
                moveDir.Normalize();
                position += moveDir * speed * dt; // Makes the enemy move towards the player at the speed defined by the speed variable.
            }
        }
    }

    class StrongerEnemy : Enemy
    {
        public StrongerEnemy(Vector2 newPos, Texture2D spriteSheet, Color color) : base(newPos, spriteSheet, color)
        {
            speed = 80; 
            NumberOfHits = 2; // Slightly stronger enemies (red) can be killed in two hits.
        }        
    }

    class StrongestEnemy : Enemy
    {
        public StrongestEnemy(Vector2 newPos, Texture2D spriteSheet, Color color) : base(newPos, spriteSheet, color)
        {
            speed = 50;
            NumberOfHits = 3; // The strongest enemies (purple) must be killed in three hits.
        }
    }
}