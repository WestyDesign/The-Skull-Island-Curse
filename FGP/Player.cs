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
    class Player
    {
        private Vector2 position = new Vector2(625, 400);
        private int speed = 300;
        private Dir direction = Dir.Down; // Uses the enum in the Game1 class to give the player four directions to move in.
        private bool isMoving = false;
        private KeyboardState kStateOld = Keyboard.GetState();
        private bool mustRespawn = false; // Called when the player is touched by an enemy - triggers respawn.

        public bool dead = false;
        public SpriteAnimation anim;
        public SpriteAnimation[] animations = new SpriteAnimation[4]; // An array that will be responsible for showing the animation corresponding to the direction the player is moving in.

        public Vector2 Position // Public, because the draw method in the main class needs to know the player's position so it can attach e to it. 
        { get { return position; } }

        public void setX(float newX)
        { position.X = newX; }

        public void setY(float newY)
        { position.Y = newY; }

        public bool MustRespawn // Makes the player respawn when an enemy gets too close.
        {
            get { return mustRespawn; }
            set
            {
                mustRespawn = value;
                position = new Vector2(625, 400);
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds; // Provides movement that is independent from the game's framerate.
            isMoving = false; // Stops the player from moving when they aren't pressing the movement keys.

            if (kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.W))
            {
                direction = Dir.Up;
                isMoving = true;
            }

            if (kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S))
            {
                direction = Dir.Down;
                isMoving = true;
            }

            if (kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.A))
            {
                direction = Dir.Left;
                isMoving = true;
            }

            if (kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D))
            {
                direction = Dir.Right;
                isMoving = true;
            }

            if(kState.IsKeyDown(Keys.Space)) // Stops the player from moving whenever they shoot.
            { isMoving = false; }

            if (dead) // Stops the player from moving if they're dead.
            { isMoving = false; }

            if (isMoving) // Will only move the player if they're holding down a key.
            {
                switch (direction) // Actually makes the player move after the buttons have been pressed.
                {
                    case Dir.Up:
                        if (position.Y > 50) // Stops the player from leaving the northern level boundary.
                        { position.Y -= speed * dt; }
                        break;
                    case Dir.Down:
                        if (position.Y < 700) // Stops the player from leaving the southern level boundary.
                        { position.Y += speed * dt; }
                        break;
                    case Dir.Left:
                        if (position.X > 300) // Stops the player from leaving the western level boundary.
                        { position.X -= speed * dt; }
                        break;
                    case Dir.Right:
                        if (position.X < 950) // Stops the player from leaving the eastern level boundary.
                        { position.X += speed * dt; }
                        break;
                }
            }

            anim = animations[(int)direction]; // Applies the relevant animation based on the direction the player is moving in.
            anim.Position = new Vector2(position.X - 48, position.Y - 48); // Makes the player's animations match their position.

            if(kState.IsKeyDown(Keys.Space))
            { anim.setFrame(0); } // Plays a rudimentary 'shooting' animation whenever the player presses the 'shoot' button.

            else if (isMoving)
            { anim.Update(gameTime); } // Updates the player's animations in real time, and only plays them if they're moving.

            else
            { anim.setFrame(1); } // When the player isn't moving, regardless of the direction they're facing or animation they're using, they'll stand still.

            if(!dead)
            {
                if (kState.IsKeyDown(Keys.Space) && kStateOld.IsKeyUp(Keys.Space)) // When the player presses & releases space, a projectile is fired in the direction that they're facing / moving towards.
                { Projectile.projectiles.Add(new Projectile(position, direction)); }
            }

            kStateOld = kState; // Constantly updates the kState and kStateOld variables so that the current frame is kState, and the previous frame is kStateOld.
        }

        public void SpeedUp() // Speed boost - will be a reward for eaching a score of 10.
        { speed = 400; }

        public void SlowDown() // Speed boost removal - when the player's score resets to 0, their speed returns to normal too.
        { speed = 300; }
    }
}
