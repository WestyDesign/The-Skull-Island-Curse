#region 'Using' information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FGP.States
{
    public class GameState : State
    {
        Player player = new Player();

        #region Texture2Ds
        Texture2D walkUp;
        Texture2D walkDown;
        Texture2D walkLeft;
        Texture2D walkRight;

        Texture2D background; // Gameplay's background
        Texture2D skull;

        Texture2D ball;
        Texture2D slingshot;
        Texture2D boots;
        #endregion

        SpriteFont Font; // Used for the timer and buttons.
        private double timer = 80; // Counts down from 80; enemy spawns really ramp up in the last 20 seconds as a final challenge. Neat!

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base (game, graphicsDevice, content)
        {
            background = _content.Load<Texture2D>("Sprites/background");
            Font = _content.Load<SpriteFont>("Fonts/timerFont");

            ball = _content.Load<Texture2D>("Sprites/ball"); // Projectile sprite.
            skull = _content.Load<Texture2D>("Enemies/skull"); // Enemy sprite - variants with more health have different colours applied via code.
            slingshot = _content.Load<Texture2D>("Sprites/Slingshot"); // Fire rate pickup sprite
            boots = _content.Load<Texture2D>("Sprites/Boots"); // Speed boost pickup sprite

            #region Player sprites and animations.
            walkUp = _content.Load<Texture2D>("Player/walkUp");
            walkDown = _content.Load<Texture2D>("Player/walkDown");
            walkLeft = _content.Load<Texture2D>("Player/walkLeft");
            walkRight = _content.Load<Texture2D>("Player/walkRight");

            player.animations[0] = new SpriteAnimation(walkDown, 4, Color.White, 8); // Creates the player's walk down animation - applies the walkDown image, gives it 4 frames, and goes through 8 frames every second.     
            player.animations[1] = new SpriteAnimation(walkUp, 4, Color.White, 8); // Creates the player's walk up animation - applies the walkUp image, gives it 4 frames, and goes through 8 frames every second. 
            player.animations[2] = new SpriteAnimation(walkLeft, 4, Color.White, 8); // Creates the player's walk left animation - applies the walkLeft image, gives it 4 frames, and goes through 8 frames every second.
            player.animations[3] = new SpriteAnimation(walkRight, 4, Color.White, 8); // Creates the player's walk right animation - applies the walkRight image, gives it 4 frames, and goes through 8 frames every second.
            player.anim = player.animations[0];
            #endregion
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(-500, -500), Color.White);

            spriteBatch.DrawString(Font, "Time Remaining: " + Math.Floor(timer).ToString(), new Vector2(2, 40), Color.Yellow); // Makes the timer visible and labels it as such. "Math.Floor" stops decimal places from appearing by rounding down.

            foreach (Projectile proj in Projectile.projectiles)
            { spriteBatch.Draw(ball, new Vector2(proj.Position.X - 48, proj.Position.Y - 48), Color.White); } // Draws a ball sprite onto every projectile that's fired and fires them from the player's center.

            foreach (Enemy e in Enemy.enemies)
            { e.anim.Draw(spriteBatch); } // Draws a skull sprite onto every enemy that' spawns.

            if (!player.dead) // Draws the player's animations, but only if they're alive.
            { player.anim.Draw(spriteBatch); } // Uses Player.cs's 'position' Vector2 to draw the player's sprite onto them.

            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (timer > 1) // Makes the timer gradually decrease in real time.
            { timer -= gameTime.ElapsedGameTime.TotalSeconds; }

            if (timer <= 1) // Ends the game when the timer reaches 0.
            { _game.ChangeState(new Win(_game, _graphicsDevice, _content)); } // Sends the player to the 'win' screen when the timer reaches 0. They've done it!

            if(player.dead == true)
            { _game.ChangeState(new Fail(_game, _graphicsDevice, _content)); } // Sends the player to the 'lose' screen when they're hit by an enemy.

            player.Update(gameTime); // Constantly updates player movement using the switch statement in Player.cs.

            if (!player.dead) // Only runs this code if the player is ALIVE.
            { Controller.Update(gameTime, skull); } // Uses Controller.cs to spawn enemies every few seconds.

            // timer.Position = player.Position - new Vector2(100, 100); // Will make the timer follow the player, hopefully.

            foreach (Projectile proj in Projectile.projectiles)
            { proj.Update(gameTime); } // Updates every projectile that's fired to control their direction and speed, using the update method in Projectile.cs.

            foreach (Enemy e in Enemy.enemies) // Updates every enemy that spawns to control their speed and the player's position, using the update method in Enemy.cs.
            {
                e.Update(gameTime, player.Position, player.dead);
                int sum = 32 + e.Radius;
                if (Vector2.Distance(player.Position, e.Position) < sum)
                { player.dead = true; } // Kills the player when they're touched by an enemy.
            }

            foreach (Projectile proj in Projectile.projectiles)
            {
                foreach (Enemy enemy in Enemy.enemies)
                {
                    int sum = proj.radius + enemy.Radius;
                    if (Vector2.Distance(proj.Position, enemy.Position) < sum) // Checks for collisions between projectiles and enemies.
                    {
                        enemy.NumberOfHits--;
                        proj.Collided = true;

                        if (enemy.NumberOfHits == 0)
                            enemy.Dead = true;
                    }
                }
            }
            Projectile.projectiles.RemoveAll(p => p.Collided); // Removes all projectiles that have 'collided' with an enemy from the game.
            Enemy.enemies.RemoveAll(e => e.Dead); // Removes all enemies that have 'collided' with an projectile from the game, because they're dead.           
        }
    }
}