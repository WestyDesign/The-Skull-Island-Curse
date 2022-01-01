#region 'Using' information
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora; // An open-source package for a camera.
#endregion

namespace FGP
{
    enum Dir // Contains the directions that the player and enemies can move in - only four so that the game is a bit more challenging.
    { Down, Up, Left, Right }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Texture2D playerSprite;
        Texture2D walkUp;
        Texture2D walkDown;
        Texture2D walkLeft;
        Texture2D walkRight;

        Texture2D menuBackground; // Main menu's background
        Texture2D background; // Gameplay's background
        SpriteFont Font; // Used for the timer.

        private double timer = 90; // Gives the game a 90 second runtime, so that stronger enemies can come in at certain intervals.

        Texture2D ball;
        Texture2D skull;

        Player player = new Player();

        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            this.camera = new Camera(graphics.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerSprite = Content.Load<Texture2D>("Player/player");
            walkUp = Content.Load<Texture2D>("Player/walkUp");
            walkDown = Content.Load<Texture2D>("Player/walkDown");
            walkLeft = Content.Load<Texture2D>("Player/walkLeft");
            walkRight = Content.Load<Texture2D>("Player/walkRight");

            player.animations[0] = new SpriteAnimation(walkDown, 4, Color.White, 8); // Creates the player's walk down animation - applies the walkDown image, gives it 4 frames, and goes through 8 frames every second.     
            player.animations[1] = new SpriteAnimation(walkUp, 4, Color.White, 8); // Creates the player's walk up animation - applies the walkUp image, gives it 4 frames, and goes through 8 frames every second. 
            player.animations[2] = new SpriteAnimation(walkLeft, 4, Color.White, 8); // Creates the player's walk left animation - applies the walkLeft image, gives it 4 frames, and goes through 8 frames every second.
            player.animations[3] = new SpriteAnimation(walkRight, 4, Color.White, 8); // Creates the player's walk right animation - applies the walkRight image, gives it 4 frames, and goes through 8 frames every second.
            player.anim = player.animations[0];

            background = Content.Load<Texture2D>("Sprites/background");
            menuBackground = Content.Load<Texture2D>("Menus/Main Menu Image");
            Font = Content.Load<SpriteFont>("Fonts/timerFont"); // The font used to draw the timer on-screen.

            ball = Content.Load<Texture2D>("Sprites/ball"); // Projectile sprite.

            skull = Content.Load<Texture2D>("Enemies/skull"); // Enemy sprite
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            { Exit(); } // Close the game when the player presses the escape key.

            if (timer > 1) // Makes the timer gradually decrease in real time.
            { timer -= gameTime.ElapsedGameTime.TotalSeconds; }

            if(timer <= 1) // Ends the game when the timer reaches 0.
            { player.dead = true; }

            player.Update(gameTime); // Updates player movement using the switch statement in Player.cs.

            if(!player.dead)
            { Controller.Update(gameTime, skull); } // Uses Controller.cs to spawn enemies with the skull texture every few seconds, but only if the player's alive.

            camera.Position = player.Position; // Keeps the camera focused on the player at all times.
            camera.Update(gameTime); // Updates the camera's position.

            foreach (Projectile proj in Projectile.projectiles)
            { proj.Update(gameTime); } // Updates every projectile that's fired to control their direction and speed, using the update method in Projectile.cs.

            foreach (Enemy e in Enemy.enemies) // Updates every enemy that spawns to control their speed and the player's position, using the update method in Enemy.cs.
            { 
                e.Update(gameTime, player.Position, player.dead);
                int sum = 32 + e.Radius;
                if(Vector2.Distance(player.Position, e.Position) < sum)
                { player.dead = true; } // Kills the player when they're touched by an enemy.
            } 

            foreach (Projectile proj in Projectile.projectiles)
            { 
                foreach (Enemy enemy in Enemy.enemies)
                {
                    int sum = proj.radius + enemy.Radius;
                    if(Vector2.Distance(proj.Position, enemy.Position) < sum) // Checks for collisions between projectiles and enemies.
                    {
                        enemy.NumberOfHits--;
                        proj.Collided = true;

                        if(enemy.NumberOfHits == 0)
                            enemy.Dead = true;
                    }
                }
            }

            Projectile.projectiles.RemoveAll(p => p.Collided); // Removes all projectiles that have 'collided' with an enemy from the game.
            Enemy.enemies.RemoveAll(e => e.Dead); // Removes all enemies that have 'collided' with an projectile from the game, because they're dead.

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(this.camera);

            spriteBatch.Draw(background, new Vector2 (-500, -500), Color.White);

            spriteBatch.DrawString(Font, "Time Remaining: " + Math.Floor(timer).ToString(), new Vector2(2, 40), Color.Yellow); // Makes the timer visible and labels it as such. "Math.Floor" stops decimal places from appearing by rounding down.

            foreach (Projectile proj in Projectile.projectiles)
            { spriteBatch.Draw(ball, new Vector2(proj.Position.X - 48, proj.Position.Y - 48), Color.White); } // Draws a ball sprite onto every projectile that's fired, fires them from the player's center.

            foreach (Enemy e in Enemy.enemies)
            { e.anim.Draw(spriteBatch); } // Draws a skull sprite onto every enemy that' spawns.

            if (!player.dead) // Draws the player's animations, but only if they're alive.
            { player.anim.Draw(spriteBatch); } // Uses Player.cs's 'position' Vector2 to draw the player's sprite onto them.

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
