#region 'Using' information
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora; // An open-source package for a camera.
using FGP.States;
#endregion

namespace FGP
{
    enum Dir // Contains the directions that the player and enemies can move in - only four so that the game is a bit more challenging.
    { Down, Up, Left, Right }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private State currentState;
        private State nextState;

        public void ChangeState(State state)
        { nextState = state; }

        #region Texture2Ds
        Texture2D walkUp;
        Texture2D walkDown;
        Texture2D walkLeft;
        Texture2D walkRight;

        Texture2D background; // Gameplay's background

        Texture2D ball;
        Texture2D skull;
        Texture2D slingshot;
        Texture2D boots;
        #endregion

        SpriteFont Font; // Used for the timer.

        private double timer = 90; // Gives the game a 90 second runtime, so that stronger enemies can come in at certain intervals.

        Player player = new Player();
        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            this.camera = new Camera(graphics.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("Fonts/timerFont"); // The font used to draw the timer on-screen.

            #region Player sprites and animations.
            walkUp = Content.Load<Texture2D>("Player/walkUp");
            walkDown = Content.Load<Texture2D>("Player/walkDown");
            walkLeft = Content.Load<Texture2D>("Player/walkLeft");
            walkRight = Content.Load<Texture2D>("Player/walkRight");

            player.animations[0] = new SpriteAnimation(walkDown, 4, Color.White, 8); // Creates the player's walk down animation - applies the walkDown image, gives it 4 frames, and goes through 8 frames every second.     
            player.animations[1] = new SpriteAnimation(walkUp, 4, Color.White, 8); // Creates the player's walk up animation - applies the walkUp image, gives it 4 frames, and goes through 8 frames every second. 
            player.animations[2] = new SpriteAnimation(walkLeft, 4, Color.White, 8); // Creates the player's walk left animation - applies the walkLeft image, gives it 4 frames, and goes through 8 frames every second.
            player.animations[3] = new SpriteAnimation(walkRight, 4, Color.White, 8); // Creates the player's walk right animation - applies the walkRight image, gives it 4 frames, and goes through 8 frames every second.
            player.anim = player.animations[0];
            #endregion

            currentState = new MenuState(this, graphics.GraphicsDevice, Content);
         
            background = Content.Load<Texture2D>("Sprites/background");
            
            ball = Content.Load<Texture2D>("Sprites/ball"); // Projectile sprite.
            skull = Content.Load<Texture2D>("Enemies/skull"); // Enemy sprite - variants with more health have different colours applied via code.
            slingshot = Content.Load<Texture2D>("Sprites/Slingshot"); // Fire rate pickup sprite
            boots = Content.Load<Texture2D>("Sprites/Boots"); // Speed boost pickup sprite
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            { Exit(); } // Close the game when the player presses the escape key.

            if (nextState != null)
            {
                currentState = nextState;
                nextState = null;
            }

            currentState.Update(gameTime);
            currentState.PostUpdate(gameTime);

            if (timer > 1) // Makes the timer gradually decrease in real time.
            { timer -= gameTime.ElapsedGameTime.TotalSeconds; }

            if(timer <= 1) // Ends the game when the timer reaches 0.
            { player.dead = true; } /// TODO: Send this to a 'win' screen, not kill the player

            player.Update(gameTime); // Constantly updates player movement using the switch statement in Player.cs.

            if(!player.dead) // Only runs this code if the player's alive.
            { Controller.Update(gameTime, skull); } // Uses Controller.cs to spawn enemies every few seconds.

            camera.Position = player.Position; // Keeps the camera focused on the player at all times.
            camera.Update(gameTime); // Updates the camera's position.

            // timer.Position = player.Position - new Vector2(100, 100);

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

            currentState.Draw(gameTime, spriteBatch);                   

            base.Draw(gameTime);
        }
    }
}
