﻿#region 'Using' information
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
        #region Variables
        Player player = new Player();
        private Texture2D walkUp;
        private Texture2D walkDown;
        private Texture2D walkLeft;
        private Texture2D walkRight;
        private Texture2D gameBackground;
        private Texture2D skull;
        private Texture2D ball; // projectile
        private Texture2D boots; // powerup 1
        private Texture2D slingshot; // powerup 2
        private Texture2D clock; // powerup 3
        private SpriteFont Font; // Used for the timer and buttons.
        private double timer = 110; // Counts down from 110; enemy spawns really ramp up in the last 30 seconds as a final challenge. Neat!
        int Lives = 3; // The player has 3 lives to beat the game with - if all 3 are lost, it's game over.
        int Score = 0; // A score mechanic will encourage the player to defeat as many enemies as they can before time's up - and pickups will spawn when certain score values are reached.
        #endregion

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base (game, graphicsDevice, content)
        {
            gameBackground = _content.Load<Texture2D>("Sprites/background2");
            Font = _content.Load<SpriteFont>("Fonts/timerFont");

            ball = _content.Load<Texture2D>("Sprites/ball"); // Projectile sprite.
            skull = _content.Load<Texture2D>("Enemies/skull"); // Enemy sprite - variants with more health have different colours applied via code.
            clock = _content.Load<Texture2D>("Sprites/Clock"); // Fire rate pickup sprite
            boots = _content.Load<Texture2D>("Sprites/Boots"); // Speed boost pickup sprite
            slingshot = _content.Load<Texture2D>("Sprites/Slingshot"); // Speed boost pickup sprite

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

            spriteBatch.Draw(gameBackground, new Vector2(-125,-350), Color.White);

            spriteBatch.DrawString(Font, "Time Remaining: " + Math.Floor(timer).ToString(), new Vector2(2, 40), Color.Yellow); // Makes the timer visible and labels it as such. "Math.Floor" stops decimal places from appearing by rounding down.
            spriteBatch.DrawString(Font, "Lives Left: " + Lives.ToString(), new Vector2(2, 100), Color.Pink); // Makes the player's remaining lives visible and labels them as such.
            spriteBatch.DrawString(Font, "Score: " + Score.ToString(), new Vector2(2, 160), Color.LightGoldenrodYellow); // Makes the Score visible and labels it as such.          

            foreach (Projectile proj in Projectile.projectiles)
            { spriteBatch.Draw(ball, new Vector2(proj.Position.X - 48, proj.Position.Y - 48), Color.White); } // Draws a ball sprite onto every projectile that's fired and fires them from the player's center.

            foreach (Enemy e in Enemy.enemies)
            { e.anim.Draw(spriteBatch); } // Draws a skull sprite onto every enemy that spawns.

            if (!player.dead) // Draws the player's animations, but only if they're alive.
            { player.anim.Draw(spriteBatch); } // Uses Player.cs's 'position' Vector2 to draw the player's sprite onto them.

            if(Score >= 15)
            { spriteBatch.Draw(boots, new Vector2(2, 220), Color.White); } // Draws a boots sprite onto the player's UI once their scores reaches 15 - makes it clear they've been upgraded.

            if(Score >= 25)
            { spriteBatch.Draw(slingshot, new Vector2(2, 320), Color.White);  } // Draws a slingshot sprite onto the player's UI once their score reaches 25 - makes it clear they've been upgraded.

            if (Score >= 37)
            { spriteBatch.Draw(clock, new Vector2(2, 420), Color.White); } // Draws a clock sprite onto the player's UI once their score reaches 38 - makes it clear they're about to have some time removed from the counter.
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (timer > 1) // Makes the timer gradually decrease in real time.
            { timer -= gameTime.ElapsedGameTime.TotalSeconds; }

            if (timer <= 1) // Ends the game when the timer reaches 0.
            { _game.ChangeState(new WinState(_game, _graphicsDevice, _content)); } // Sends the player to the 'win' screen when the timer reaches 0. They've done it!

            if(player.dead == true)
            { _game.ChangeState(new FailState(_game, _graphicsDevice, _content)); } // Sends the player to the 'lose' screen when they're hit by an enemy.

            player.Update(gameTime); // Constantly updates player movement using the switch statement in Player.cs.

            if (!player.dead) // Only runs this code if the player is ALIVE.
            { EnemyController.Update(gameTime, skull); } // Uses Controller.cs to spawn enemies every few seconds.
         
            foreach (Projectile proj in Projectile.projectiles)
            { proj.Update(gameTime); } // Updates every projectile that's fired to control their direction and speed, using the update method in Projectile.cs.

            foreach (Enemy e in Enemy.enemies) // Updates every enemy that spawns to control their speed toward the player's position, using the update method in Enemy.cs.
            {
                e.Update(gameTime, player.Position, player.dead);
                int sum = 32 + e.Radius;

                if (Vector2.Distance(player.Position, e.Position) < sum) // If the enemy touches the player...
                {
                    if (Lives > 0) // And if the player has at least 1 life remaining...
                    {
                        player.MustRespawn = true; // Triggers the player's respawn.                    
                        Lives--; // Removes one life from their current total.
                        Score -= 5; // Subtracts 5 points from score.
                        e.Dead = true; // Removes the enemy that killed the player so that they don't spam-kill them.
                    }
                }
            }

            foreach (Projectile proj in Projectile.projectiles)
            {
                if(Score >= 25)
                { proj.PoweredUp = true; } // Increases bullet speed when the player's score has reached 25.

                foreach (Enemy enemy in Enemy.enemies)
                {
                    int sum = proj.radius + enemy.Radius;
                    if (Vector2.Distance(proj.Position, enemy.Position) < sum) // Checks for collisions between projectiles and enemies.
                    {
                        enemy.NumberOfHits--;
                        proj.Collided = true;

                        if (enemy.NumberOfHits == 0)
                        {
                            enemy.Dead = true;
                            Score += enemy.ScoreReward;
                        }                          
                    }
                }
            }

            if(Lives == 0) // If the player's out of lives,
            { player.dead = true; } // the game is over.

            if(Score >= 15)
            { player.SpeedUp(); } // Calls the SpeedUp method in the player class when the player's reached 15 points.

            /// When the player's score reaches 25, their bullet speed is increased - see the Projectile foreach loop for the code.

            if (Score == 40)
            { 
                timer -= 10; // Shortens the timer by 10 seconds when the player's reached 30 points. Encourages an active playstyle.
                Score = 0; // Sends score back to 0 - so this reward can be activated multiple times.
                player.SlowDown(); // Calls the SlowDown method in the player class - how's that for a skull island curse? ;)
            } 

            Projectile.projectiles.RemoveAll(p => p.Collided); // Removes all projectiles that have 'collided' with an enemy from the game.            
            Enemy.enemies.RemoveAll(e => e.Dead); // Removes all enemies that have 'collided' with an projectile from the game, because they're dead.           
        }
    }
}