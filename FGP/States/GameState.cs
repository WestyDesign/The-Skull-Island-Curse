#region 'Using' information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Comora; // An open-source package for a camera.
#endregion

namespace FGP.States
{
    public class GameState : State
    {
        enum Dir // Contains the directions that the player and enemies can move in - only four so that the game is a bit more challenging.
        { Down, Up, Left, Right }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Player player = new Player();
        Camera camera;

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

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base (game, graphicsDevice, content)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(-500, -500), Color.White);

            spriteBatch.DrawString(Font, "Time Remaining: " + Math.Floor(timer).ToString(), new Vector2(2, 40), Color.Yellow); // Makes the timer visible and labels it as such. "Math.Floor" stops decimal places from appearing by rounding down.

            foreach (Projectile proj in Projectile.projectiles)
            { spriteBatch.Draw(ball, new Vector2(proj.Position.X - 48, proj.Position.Y - 48), Color.White); } // Draws a ball sprite onto every projectile that's fired, fires them from the player's center.

            foreach (Enemy e in Enemy.enemies)
            { e.anim.Draw(spriteBatch); } // Draws a skull sprite onto every enemy that' spawns.

            if (!player.dead) // Draws the player's animations, but only if they're alive.
            { player.anim.Draw(spriteBatch); } // Uses Player.cs's 'position' Vector2 to draw the player's sprite onto them.

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}